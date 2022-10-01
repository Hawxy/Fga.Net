namespace Fga.Net.AspNetCore.Exceptions;

/// <summary>
/// Custom exception for FGA middleware internals.
/// </summary>
public sealed class FgaMiddlewareException : Exception
{
    /// <summary>
    /// Custom exception for FGA middleware internals.
    /// </summary>
    public FgaMiddlewareException() { }

    /// <summary>
    /// Custom exception for FGA middleware internals.
    /// </summary>
    /// <param name="message"></param>
    public FgaMiddlewareException(string message) 
        : base(message) { }

    /// <summary>
    /// Custom exception for FGA middleware internals.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public FgaMiddlewareException(string message, Exception inner)
        : base(message, inner) { }

}