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
        public bool Enabled { get; set; }
        public bool Pinned { get; set; }
        public bool Locked { get; set; }
    }
    
    public Profile(string name = "Default")
    {
        Name = name;
    }
}