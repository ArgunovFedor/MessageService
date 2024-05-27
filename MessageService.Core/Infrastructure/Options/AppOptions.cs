using System.Collections.Generic;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace MessageService.Core.Infrastructure.Options;

public class AppOptions
{
    public AppDatabaseOptions Database { get; set; }
    public bool IsSut { get; set; }
}