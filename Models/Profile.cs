using Avalonia;

namespace ERM_Desktop.Models;

public class Profile
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public long UserID { get; set; }
    public long Epoch { get; set; }
    
    public string IdentifierTooltip => $"Identifier: {Id}";

    public PixelPoint ModerationPosition { get; set; } = new PixelPoint(0, 0);
    public PixelPoint RecentLogsPosition { get; set; } = new PixelPoint(0, 0);
    public PixelPoint ShiftPosition { get; set; } = new PixelPoint(0, 0);
    public PixelPoint ActiveStaffPosition { get; set; } = new PixelPoint(0, 0);
    public PixelPoint BolosPosition { get; set; } = new PixelPoint(0, 0);
    public PixelPoint SearchPosition { get; set; } = new PixelPoint(0, 0);
    
    public WidgetState ModerationState { get; set; }
    public WidgetState RecentLogsState { get; set; }
    public WidgetState ShiftState { get; set; }
    public WidgetState ActiveStaffState { get; set; }
    public WidgetState BolosState { get; set; }
    public WidgetState SearchState { get; set; }

    public struct WidgetState
    {
        public bool Enabled { get; init; }
        public bool Pinned { get; init; }
        public bool Locked { get; init; }
    }
    
    public Profile(string name = "Default")
    {
        Name = name;
    }
    
    public void OverwriteSettings(Profile ov)
    {
        ModerationPosition = ov.ModerationPosition;
        RecentLogsPosition = ov.RecentLogsPosition;
        ShiftPosition = ov.ShiftPosition;
        ActiveStaffPosition = ov.ActiveStaffPosition;
        BolosPosition = ov.BolosPosition;
        SearchPosition = ov.SearchPosition;
        
        ModerationState = ov.ModerationState;
        RecentLogsState = ov.RecentLogsState;
        ShiftState = ov.ShiftState;
        ActiveStaffState = ov.ActiveStaffState;
        BolosState = ov.BolosState;
        SearchState = ov.SearchState;
    }
}
