﻿namespace UrlShortener.Common.Exceptions;

public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException()
    {
    }

    public AuthenticationFailedException(string? message) : base(message)
    {
    }

    public AuthenticationFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
