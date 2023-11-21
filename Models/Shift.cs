using System;
using System.Collections.Generic;

namespace ERM_Desktop.Models;

public class Shift
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Nickname { get; set; }
    public long UserID { get; set; }
    public string? Type { get; set; }
    public double StartEpoch { get; set; }
    public List<Break> Breaks { get; set; } = new();
    public long Guild { get; set; }
    public List<ObjectIdRaw> Moderations { get; set; } = new();
    public int AddedTime { get; set; }
    public int RemovedTime { get; set; }
    public double EndEpoch { get; set; }
}

public class Break
{
    public double StartEpoch { get; set; }
    public double EndEpoch { get; set; }
}

public class ObjectIdRaw
{
    public int timestamp { get; set; }
    public int machine { get; set; }
    public int pid { get; set; }
    public int increment { get; set; }
    public DateTime creationTime { get; set; }
}