using System;

namespace ERM_Desktop.Models;

public class GuildsPermissions
{
    public string id { get; set; } = String.Empty;
    public string name { get; set; } = String.Empty;
    public string icon_url { get; set; } = String.Empty;
    public string member_count { get; set; } = String.Empty;
    public int permission_level { get; set; }
}