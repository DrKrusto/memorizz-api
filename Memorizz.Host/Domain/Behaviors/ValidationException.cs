﻿namespace Memorizz.Host.Domain.Behaviors;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred")
        => Errors = errors;
}