using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ERM_Desktop.Services;

namespace ERM_Desktop.Models;

public class Punishment
{
    public string? Id { get; set; }
    public long Snowflake { get; set; }
    public string? Username { get; init; }
    public long UserID { get; set; }
    public string? Type { get; set; }
    public string? Reason { get; set; }
    public string? Moderator { get; init; }
    public long ModeratorID { get; init; }
    public long Guild { get; init; }
    public long Epoch { get; set; }
    public long UntilEpoch { get; set; } = 0;
}