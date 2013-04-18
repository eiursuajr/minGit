using System;
using System.Collections.Generic;

[Serializable]
public class LoadBalanceStatusResponse : LoadBalanceResponse
{
    /// <summary>
    /// 
    /// </summary>
    public LoadBalanceStatusResponse()
    {
        IsComplete = false;
        Message = string.Empty;
        Entries = new List<ForceLoadBalanceStatusEntry>();
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<ForceLoadBalanceStatusEntry> Entries { get; set; }
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class ForceLoadBalanceStatusEntry
{
    /// <summary>
    /// 
    /// </summary>
    public ForceLoadBalanceStatusEntry()
    {
        Code = string.Empty;
        Message = string.Empty;
        Error = string.Empty;
        Date = string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Date { get; set; }
}
