using System.Linq;
using MessageService.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace MessageService.Core.Requests;

public static class IdentityExtensions
{
    public static void CheckIfSucceeded(this IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errorDescription = result.Errors.Aggregate("",
            (current, error) => current + error.Description + " \r\n ");
        throw new ServiceException($"{errorDescription}");
    }
}