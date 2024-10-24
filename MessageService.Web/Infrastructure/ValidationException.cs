﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Results;

namespace MessageService.Web.Infrastructure;

[Serializable]
public class ValidationException : Exception
{
    public ValidationException() 
        : base("One or more validation failures have occurred.")
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(List<ValidationFailure> failures)
        : this()
    {
        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var propertyName in propertyNames)
        {
            var propertyFailures = failures
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();

            Failures.Add(propertyName, propertyFailures);
        }
    }

    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        // IDictionary is not serializable
        Failures = (Dictionary<string, string[]>) info.GetValue(nameof(Failures),
            typeof(Dictionary<string, string[]>));
    }

    public IDictionary<string, string[]> Failures { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // IDictionary is not serializable
        info.AddValue(nameof(Failures), new Dictionary<string, string[]>(Failures),
            typeof(Dictionary<string, string[]>));
        base.GetObjectData(info, context);
    }
}
