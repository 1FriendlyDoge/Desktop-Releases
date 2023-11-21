using System;
using System.Collections.Generic;

namespace ERM_Desktop.Models;

public class SearchResult
{
    public object? previousPageCursor { get; set; } = null;
    public object? nextPageCursor { get; set; } = null;
    public List<UserSearch> data { get; set; } = new List<UserSearch>();
}

public class UserSearch
{
    public List<string> previousUsernames { get; set; } = new List<string>();
    public bool hasVerifiedBadge { get; set; }
    public long id { get; set; }
    public string name { get; set; } = String.Empty;
    public string displayName { get; set; } = String.Empty;
}