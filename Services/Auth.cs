using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ERM_Desktop.Models;
using Newtonsoft.Json;

namespace ERM_Desktop.Services;

public class Auth
{
    public string OAuth2URL = string.Empty;
    
    public async Task<string> GetIdentifier()
    {
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync("api/Auth/GetIdentifier");
        if(resp.IsSuccessStatusCode)
        {
            Storage.Identifier = await resp.Content.ReadAsStringAsync();
            Storage.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Storage.Identifier);
            return Storage.Identifier;
        }

        throw new HttpRequestException("Failed to get identifier.");
    }
    

    public async Task<string> GetOAuth2URL()
    {
        if(String.IsNullOrEmpty(Storage.Identifier))
        {
            await GetIdentifier();
        }
        
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync("api/Auth/GetOAuth2URL");
        if(resp.IsSuccessStatusCode)
        {
            OAuth2URL = await resp.Content.ReadAsStringAsync();
            return OAuth2URL;
        }

        throw new HttpRequestException("Failed to get OAuth2 URL.");
    }
    
    
    public async Task<bool> ResolveIdentifier()
    {
        if(string.IsNullOrEmpty(Storage.Identifier))
        {
            throw new NullReferenceException("Identifier is null.");
        }
        
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Auth/ResolveIdentifier");
        if(resp.IsSuccessStatusCode)
        {
            Storage.UserInstance = JsonConvert.DeserializeObject<UserInstance>(await resp.Content.ReadAsStringAsync());

            return true;
        }
        
        return false;
    }


    public async Task<List<GuildsPermissions>> GetServers()
    {
        if(string.IsNullOrEmpty(Storage.Identifier))
        {
            throw new NullReferenceException("Identifier is null.");
        }
        
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Auth/GetServers");
        if(resp.IsSuccessStatusCode)
        {
            List<GuildsPermissions> guildsPermissions = JsonConvert.DeserializeObject<List<GuildsPermissions>>(await resp.Content.ReadAsStringAsync()) ?? new List<GuildsPermissions>();

            List<DiscordServer>? discordServers = Storage.UserInstance!.DiscordServers;
            if (discordServers == null)
            {
                throw new ApplicationException("Invalid local data.");
            }
            
            
            List<DiscordServer> updatedDiscordServers = new List<DiscordServer>();
            if (Storage.UserInstance.DiscordServers != null)
            {
                foreach (DiscordServer server in Storage.UserInstance.DiscordServers)
                {
                    GuildsPermissions? permissions =
                        guildsPermissions.FirstOrDefault(x => x.id == server.ID);

                    if (permissions != null)
                    {
                        updatedDiscordServers.Add(new DiscordServer(server.IconURL, server.Name, server.ID,
                            int.Parse(permissions.member_count), permissions.permission_level));
                    }
                }
                
                Storage.UserInstance.DiscordServers = updatedDiscordServers;
            }

            return guildsPermissions;
        }
        
        throw new HttpRequestException("Failed to load servers.");
    }
}