// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using System.Numerics;

namespace Snap.Hutao.Extension;

/// <summary>
/// 合成扩展
/// </summary>
internal static class CompositionExtensions
{
    /// <summary>
    /// 创建拼合图视觉对象
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="brush">画刷</param>
    /// <returns>拼合图视觉对象</returns>
    public static SpriteVisual CompositeSpriteVisual(this Compositor compositor, CompositionBrush brush)
    {
        SpriteVisual spriteVisual = compositor.CreateSpriteVisual();
        spriteVisual.Brush = brush;
        return spriteVisual;
    }

    /// <summary>
    /// 创建混合效果画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="background">前景</param>
    /// <param name="foreground">背景</param>
    /// <param name="blendEffectMode">混合模式</param>
    /// <returns>合成效果画刷</returns>
    public static CompositionEffectBrush CompositeBlendEffectBrush(
        this Compositor compositor,
        CompositionBrush background,
        CompositionBrush foreground,
        BlendEffectMode blendEffectMode = BlendEffectMode.Multiply)
    {
        BlendEffect effect = new()
        {
            Background = new CompositionEffectSourceParameter("Background"),
            Foreground = new CompositionEffectSourceParameter("Foreground"),
            Mode = blendEffectMode,
        };

        CompositionEffectBrush brush = compositor.CreateEffectFactory(effect).CreateBrush();

        brush.SetSourceParameter("Background", background);
        brush.SetSourceParameter("Foreground", foreground);

        return brush;
    }

    /// <summary>
    /// 创建灰阶效果画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="source">源</param>
    /// <returns>合成效果画刷</returns>
    public static CompositionEffectBrush CompositeGrayScaleEffectBrush(
        this Compositor compositor,
        CompositionBrush source)
    {
        GrayscaleEffect effect = new()
        {
            Source = new CompositionEffectSourceParameter("Source"),
        };

        CompositionEffectBrush brush = compositor.CreateEffectFactory(effect).CreateBrush();

        brush.SetSourceParameter("Source", source);

        return brush;
    }

    /// <summary>
    /// 创建亮度转不透明度效果画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="sourceBrush">源</param>
    /// <returns>合成效果画刷</returns>
    public static CompositionEffectBrush CompositeLuminanceToAlphaEffectBrush(
        this Compositor compositor,
        CompositionBrush sourceBrush)
    {
        LuminanceToAlphaEffect effect = new()
        {
            Source = new CompositionEffectSourceParameter("Source"),
        };

        CompositionEffectBrush brush = compositor.CreateEffectFactory(effect).CreateBrush();

        brush.SetSourceParameter("Source", sourceBrush);

        return brush;
    }

    /// <summary>
    /// 创建不透明度蒙版效果画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="sourceBrush">源</param>
    /// <param name="alphaMask">不透明度蒙版</param>
    /// <returns>合成效果画刷</returns>
    public static CompositionEffectBrush CompositeAlphaMaskEffectBrush(
        this Compositor compositor,
        CompositionBrush sourceBrush,
        CompositionBrush alphaMask)
    {
        AlphaMaskEffect maskEffect = new()
        {
            AlphaMask = new CompositionEffectSourceParameter("AlphaMask"),
            Source = new CompositionEffectSourceParameter("Source"),
        };

        CompositionEffectBrush brush = compositor.CreateEffectFactory(maskEffect).CreateBrush();

        brush.SetSourceParameter("AlphaMask", alphaMask);
        brush.SetSourceParameter("Source", sourceBrush);

        return brush;
    }

    /// <summary>
    /// 创建一个表面画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="surface">合成表面</param>
    /// <param name="stretch">拉伸方法</param>
    /// <param name="hRatio">水平对齐比</param>
    /// <param name="vRatio">垂直对齐比</param>
    /// <returns>合成表面画刷</returns>
    public static CompositionSurfaceBrush CompositeSurfaceBrush(
        this Compositor compositor,
        ICompositionSurface surface,
        CompositionStretch stretch = CompositionStretch.None,
        float hRatio = 0.5f,
        float vRatio = 0.5f)
    {
        CompositionSurfaceBrush brush = compositor.CreateSurfaceBrush(surface);
        brush.Stretch = stretch;
        brush.VerticalAlignmentRatio = vRatio;
        brush.HorizontalAlignmentRatio = hRatio;

        return brush;
    }

    /// <summary>
    /// 创建一个线性渐变画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="stops">锚点</param>
    /// <returns>线性渐变画刷</returns>
    public static CompositionLinearGradientBrush CompositeLinearGradientBrush(
        this Compositor compositor,
        Vector2 start,
        Vector2 end,
        params GradientStop[] stops)
    {
        CompositionLinearGradientBrush brush = compositor.CreateLinearGradientBrush();
        brush.StartPoint = start;
        brush.EndPoint = end;

        foreach (GradientStop stop in stops)
        {
            brush.ColorStops.Add(compositor.CreateColorGradientStop(stop.Offset, stop.Color));
        }

        return brush;
    }

    /// <summary>
    /// 创建一个新的蒙版画刷
    /// </summary>
    /// <param name="compositor">合成器</param>
    /// <param name="source">源</param>
    /// <param name="mask">蒙版</param>
    /// <returns>蒙版画刷</returns>
    public static CompositionMaskBrush CompositeMaskBrush(
        this Compositor compositor,
        CompositionBrush source,
        CompositionBrush mask)
    {
        CompositionMaskBrush brush = compositor.CreateMaskBrush();
        brush.Source = source;
        brush.Mask = mask;

        return brush;
    }

    public record struct GradientStop(float Offset, Windows.UI.Color Color);
}
