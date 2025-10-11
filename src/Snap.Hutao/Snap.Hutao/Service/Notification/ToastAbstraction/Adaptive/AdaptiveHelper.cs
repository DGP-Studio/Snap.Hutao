// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

internal static class AdaptiveHelper
{
    internal static IElementToastBindingChild ConvertToElement(IAdaptiveChild obj)
    {
        return obj switch
        {
            AdaptiveText text => text.ConvertToElement(),
            AdaptiveImage image => image.ConvertToElement(),
            AdaptiveGroup group => group.ConvertToElement(),
            // AdaptiveSubgroup subgroup => subgroup.ConvertToElement(),
            _ => throw new NotImplementedException($"Unknown object: {obj.GetType()}")
        };
    }
}