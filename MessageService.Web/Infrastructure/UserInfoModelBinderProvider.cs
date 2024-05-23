using System;
using MessageService.Core.Security;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace MessageService.Web.Infrastructure;

public class UserInfoModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(UserInfoModel))
        {
            return new BinderTypeModelBinder(typeof(UserInfoModelBinder));
        }

        return null;
    }
}