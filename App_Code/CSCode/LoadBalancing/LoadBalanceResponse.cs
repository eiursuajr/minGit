using System;

/// <summary>
/// Response data structure for the load balance request.
/// </summary>
[Serializable]
public class LoadBalanceResponse
{
    public LoadBalanceResponse()
    {
        Success = true;
        IsAuthorized = true;
        Message = string.Empty;
    }

    /// <summary>
    /// Gets or sets a flag indicating if the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if the user was authorized to make the request.
    /// </summary>
    public bool IsAuthorized { get; set; }

    /// <summary>
    /// Gets or sets a message associated with the request.
    /// </summary>
    public string Message { get; set; }
}