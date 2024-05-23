﻿using System;
using System.Threading.Tasks;
using Aeb.DigitalPlatform.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MessageService.Web.Infrastructure;

public class UserInfoModelBinder : IModelBinder
{
    private readonly IUserInfoProvider _userInfoProvider;

    public UserInfoModelBinder(IUserInfoProvider userInfoProvider)
    {
        _userInfoProvider = userInfoProvider;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.Model != null)
        {
            throw new InvalidOperationException("Cannot update instances");
        }

        bindingContext.Result = ModelBindingResult.Success(await _userInfoProvider.GetUserInfoAsync());
    }
}