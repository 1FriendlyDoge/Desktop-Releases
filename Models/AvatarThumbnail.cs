using System;
using System.Collections.Generic;

namespace ERM_Desktop.Models;

public class AvatarThumbnail
{
    public long targetId { get; set; }
    public string state { get; set; } = String.Empty;
    public string imageUrl { get; set; } = String.Empty;
}

public class AvatarThumbnails
{
    public List<AvatarThumbnail> data { get; set; } = new List<AvatarThumbnail>();
}