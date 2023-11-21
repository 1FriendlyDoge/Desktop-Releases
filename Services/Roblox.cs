using System.Collections.Generic;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ERM_Desktop.Models;
using Newtonsoft.Json;

namespace ERM_Desktop.Services;

public class Roblox
{
    public static readonly HttpClient client = new HttpClient();
    
    public static List<RobloxUser> SearchUsers(string query, int size = 50, int limit = 10)
    {
        
        HttpResponseMessage resp = client.GetAsync($"https://users.roblox.com/v1/users/search?keyword={query}&limit={limit}").Result;
        
        if(!resp.IsSuccessStatusCode)
        {
            return new List<RobloxUser>();
        }
        
        SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(resp.Content.ReadAsStringAsync().Result) ?? new SearchResult();
        
        List<RobloxUser> users = new List<RobloxUser>();
        
        string idsConcat = String.Join(',', searchResult.data.Select(user => user.id.ToString()).ToArray());
        foreach (UserSearch user in searchResult.data)
        {
            users.Add(new RobloxUser(user.name, user.id));
        }
        
        HttpResponseMessage thumbnailResp = client.GetAsync($"https://thumbnails.roblox.com/v1/users/avatar?userIds={idsConcat}&size={size}x{size}&format=Png").Result;
        
        if(!thumbnailResp.IsSuccessStatusCode)
        {
            return users;
        }
        
        AvatarThumbnails thumbnails = JsonConvert.DeserializeObject<AvatarThumbnails>(thumbnailResp.Content.ReadAsStringAsync().Result) ?? new AvatarThumbnails();
        
        foreach (AvatarThumbnail thumbnail in thumbnails.data)
        {
            RobloxUser? user = users.Find(user => user.UserID == thumbnail.targetId);
            if(user != null)
            {
                user.AvatarURL = thumbnail.imageUrl;
            }
        }

        return users;
    }
    
    public static async Task<List<DisplayPunishment>?> ResolveAvatars(List<Punishment> data, int size = 48)
    {
        string ids = string.Join(",", data.Select(x => x.UserID.ToString()).ToArray());

        HttpResponseMessage thumbnailResp = await client.GetAsync($"https://thumbnails.roblox.com/v1/users/avatar?userIds={ids}&size={size}x{size}&format=Png");
        if(!thumbnailResp.IsSuccessStatusCode)
        {
            return null;
        }

        AvatarThumbnails? thumbnails = JsonConvert.DeserializeObject<AvatarThumbnails>(await thumbnailResp.Content.ReadAsStringAsync());
        if(thumbnails == null)
        {
            return null;
        }
        
        List<DisplayPunishment> punishments = new List<DisplayPunishment>();
        foreach (Punishment t in data)
        {
            AvatarThumbnail? imageData = thumbnails.data.FirstOrDefault(x => x.targetId == t.UserID);
            if(imageData == null)
            {
                continue;
            }
            
            punishments.Add(new DisplayPunishment(t, imageData.imageUrl));
        }

        return punishments;
    }
    
    public static async Task<string> ResolveAvatar(string id, int size = 48)
    {
        HttpResponseMessage thumbnailResp = await client.GetAsync($"https://thumbnails.roblox.com/v1/users/avatar?userIds={id}&size={size}x{size}&format=Png");
        if(!thumbnailResp.IsSuccessStatusCode)
        {
            return String.Empty;
        }

        AvatarThumbnails? thumbnails = JsonConvert.DeserializeObject<AvatarThumbnails>(await thumbnailResp.Content.ReadAsStringAsync());
        if(thumbnails == null || thumbnails.data.Count < 1)
        {
            return String.Empty;
        }

        return thumbnails.data[0].imageUrl;
    }

    public static async Task<UserSearch?> GetUser(string username)
    {
        StringContent content = new StringContent(JsonConvert.SerializeObject(new UsernameResolvePost()
        {
            usernames = new List<string>()
            {
                username
            },
            excludeBannedUsers = false
        }), Encoding.UTF8, "application/json");
        
        HttpResponseMessage response = await client.PostAsync($"https://users.roblox.com/v1/usernames/users", content);
        
        if(!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        SearchResult result = JsonConvert.DeserializeObject<SearchResult>(await response.Content.ReadAsStringAsync()) ?? new SearchResult();
        
        if(result.data.Count < 1)
        {
            return null;
        }

        return result.data[0];
    }

    public static async Task<List<string>> SearchUsernames(string query, int limit = 10)
    {
        HttpResponseMessage resp = await client.GetAsync($"https://users.roblox.com/v1/users/search?keyword={query}&limit={limit}");
        
        if(!resp.IsSuccessStatusCode)
        {
            return new List<string>();
        }
        
        SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(resp.Content.ReadAsStringAsync().Result) ?? new SearchResult();
        
        return searchResult.data.Select(user => user.name).ToList();
    }
    
    public static async Task<List<string>?> AutocompleteUsername(string username)
    {
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Moderation/AutocompleteUsername/{Storage.DiscordServer?.ID}/{username}");
        
        if(!resp.IsSuccessStatusCode)
        {
            return null;
        }
        
        return JsonConvert.DeserializeObject<List<string>>(await resp.Content.ReadAsStringAsync());
    }
    
    public static async Task<string> PredictPunishment(string reason)
    {
        string reasonEscaped = reason.Replace("\"", "\\\"").Replace("\'", "\\\'");
        reasonEscaped = "\"" + reasonEscaped + "\"";
        
        StringContent content = new StringContent(reasonEscaped, Encoding.UTF8, "application/json");
        HttpResponseMessage resp = await Storage.HttpClient.PostAsync("api/Moderation/PredictPunishment", content);

        return resp.Content.ReadAsStringAsync().Result;
    }
}