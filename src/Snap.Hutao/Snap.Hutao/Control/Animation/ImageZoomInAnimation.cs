using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media.Animation;
using System.Numerics;

namespace Snap.Hutao.Control.Animation;

/// <summary>
/// 图片放大动画
/// </summary>
internal class ImageZoomInAnimation : ImplicitAnimation<string, Vector3>
{
    /// <summary>
    /// 构造一个新的图片放大动画
    /// </summary>
    public ImageZoomInAnimation()
    {
        EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut;
        EasingType = CommunityToolkit.WinUI.UI.Animations.EasingType.Circle;
        To = "1.1";
        Duration = TimeSpan.FromSeconds(0.5);
    }

    /// <inheritdoc/>
    protected override string ExplicitTarget => nameof(Visual.Scale);

    /// <inheritdoc/>
    protected override (Vector3?, Vector3?) GetParsedValues()
    {
        return (To?.ToVector3(), From?.ToVector3());
    }
}
