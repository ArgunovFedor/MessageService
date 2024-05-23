using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MessageService.Core.Security;

public class UserInfoModel
{
    public UserInfoModel(Guid? userId, IEnumerable<string> roles, IEnumerable<Claim> claims)
    {
        UserId = userId;
        Roles = roles;
        Claims = claims;
    }

    public IEnumerable<string> Roles { get; set; }
    public Guid? UserId { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
}