// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.UI.Text;
using System.Numerics;
using Windows.UI;
using Windows.UI.Text;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control.Effect;

[DependencyProperty<EasingMode>("Easing", NotNull = true, DefaultValue = EasingMode.EaseInOut, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<double>("TimeLineFrom", NotNull = true, DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<double>("TimeLineTo", NotNull = true, DefaultValue = 1D, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<double>("Duration", NotNull = true, DefaultValue = 1D, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<TimeSpan>("BeginTime", NotNull = true, CreateDefaultValueCallbackName = nameof(CreateBeginTimeDefaultValue))]
[DependencyProperty<double>("EffectFontSize", NotNull = true, DefaultValue = 100D, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<FontWeight>("EffectFontWeight", NotNull = true, CreateDefaultValueCallbackName = nameof(CreateFontWeightDefaultValue), PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<string>("Text", PropertyChangedCallbackName = nameof(OnTextChanged))]
[DependencyProperty<string>("Delimiter", DefaultValue = ",", PropertyChangedCallbackName = nameof(OnTextChanged))]
[DependencyProperty<CanvasTextDirection>("Direction", NotNull = true, DefaultValue = CanvasTextDirection.LeftToRightThenTopToBottom, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<CanvasVerticalAlignment>("EffectVerticalAlignment", NotNull = true, DefaultValue = CanvasVerticalAlignment.Center, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<CanvasHorizontalAlignment>("EffectHorizontalAlignment", NotNull = true, DefaultValue = CanvasHorizontalAlignment.Center, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<bool>("AutoReverse", NotNull = true, DefaultValue = true, PropertyChangedCallbackName = nameof(OnAnimationChanged))]
[DependencyProperty<double>("BlurAmount", NotNull = true, DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnResourcePropertyValueChanged))]
[DependencyProperty<Color>("ColorBrush", NotNull = true, PropertyChangedCallbackName = nameof(OnPropertyChanged))]
[DependencyProperty<int>("MorphSpeed", NotNull = true, DefaultValue = 2000, PropertyChangedCallbackName = nameof(OnPropertyChanged))]
[TemplatePart(Name = nameof(PartCanvas), Type = typeof(CanvasControl))]
internal partial class TextMorphEffect : Microsoft.UI.Xaml.Controls.Control
{
    private const string PartCanvas = "PART_Canvas";

    private GaussianBlurEffect? blurEffect;
    private Vector2 centerPoint;
    private ColorMatrixEffect? colorMatrixEffect;
    private List<TextMorphItem>? morphItems;

    private CanvasTextFormat? textFormat;
    private string[]? texts;

    private CanvasControl? Canvas { get; set; }

    private static object CreateBeginTimeDefaultValue()
    {
        return TimeSpan.FromSeconds(0);
    }

    private static object CreateFontWeightDefaultValue()
    {
        return FontWeights.Bold;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextMorphEffect control = d.As<TextMorphEffect>();
        if (control is { Canvas: not null })
        {
            if (double.IsNaN(control.BlurAmount))
            {
                control.BlurAmount = 0f;
            }

            control.Canvas.Invalidate();
        }
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextMorphEffect control = d.As<TextMorphEffect>();
        if (control is { Canvas: not null })
        {
            control.UpdateTextMorph();
            control.Canvas.Invalidate();
        }
    }

    private static void OnResourcePropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextMorphEffect control = d.As<TextMorphEffect>();
        if (control is { Canvas: not null })
        {
            control.Canvas.CreateResources -= control.OnCreateResource;
            control.Canvas.CreateResources += control.OnCreateResource;
            control.Canvas.Invalidate();
        }
    }

    private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextMorphEffect control = d.As<TextMorphEffect>();
        if (control is { Canvas: not null })
        {
            control.UpdateTextMorph();
            control.Canvas.Invalidate();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (Canvas is not null)
        {
            Canvas.Draw -= OnDraw;
            Canvas.CreateResources -= OnCreateResource;
            Canvas.SizeChanged -= OnCanvasSizeChanged;
        }

        Canvas = GetTemplateChild(PartCanvas) as CanvasControl;
        if (Canvas is not null)
        {
            Canvas.Draw += OnDraw;
            Canvas.CreateResources += OnCreateResource;
            Canvas.SizeChanged += OnCanvasSizeChanged;
        }

        UpdateTextMorph();
    }

    private void UpdateTextMorph()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        texts = Text.Split(Delimiter);
        CircleEase easingFunction = new() { EasingMode = Easing };
        double morphSpeedInSeconds = TimeSpan.FromMilliseconds(MorphSpeed).TotalSeconds;
        morphItems = [];
        for (int i = 0; i < texts.Length; i++)
        {
            morphItems.Add(new()
            {
                Text = texts[i],
                Timeline = new(
                    TimeLineFrom,
                    TimeLineTo,
                    Duration,
                    TimeSpan.FromSeconds(BeginTime.TotalSeconds + i * morphSpeedInSeconds),
                    AutoReverse,
                    false,
                    easingFunction)
            });
        }

        morphItems.Reverse();

        textFormat = new()
        {
            FontSize = Convert.ToSingle(EffectFontSize),
            Direction = Direction,
            VerticalAlignment = EffectVerticalAlignment,
            HorizontalAlignment = EffectHorizontalAlignment,
            FontWeight = EffectFontWeight,
            FontFamily = FontFamily.Source
        };
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Canvas is null)
        {
            return;
        }

        centerPoint = Canvas.ActualSize / 2;
    }

    private void OnCreateResource(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        if (double.IsNaN(BlurAmount))
        {
            BlurAmount = 0f;
        }

        blurEffect = new()
        {
            BlurAmount = Convert.ToSingle(BlurAmount)
        };

        // result.R = (src.R * matrix.M11) + (src.G * matrix.M21) + (src.B * matrix.M31) + (src.A * matrix.M41) + matrix.M51
        // result.G = (src.R * matrix.M12) + (src.G * matrix.M22) + (src.B * matrix.M32) + (src.A * matrix.M42) + matrix.M52
        // result.B = (src.R * matrix.M13) + (src.G * matrix.M23) + (src.B * matrix.M33) + (src.A * matrix.M43) + matrix.M53
        // result.A = (src.R * matrix.M14) + (src.G * matrix.M24) + (src.B * matrix.M34) + (src.A * matrix.M44) + matrix.M54
        colorMatrixEffect = new()
        {
            ColorMatrix = new()
            {
                M11 = 1, M21 = 0, M31 = 0, M41 = 0, M51 = 0,
                M12 = 0, M22 = 1, M32 = 0, M42 = 0, M52 = 0,
                M13 = 0, M23 = 0, M33 = 1, M43 = 0, M53 = 0,
                M14 = 0, M24 = 0, M34 = 0, M44 = 10, M54 = -4.5f,
            },
            Source = blurEffect,
            ClampOutput = true,
        };
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(morphItems);

        CanvasCommandList source = new(sender);
        double totalSeconds = TimeSpan.FromMilliseconds(MorphSpeed).TotalSeconds * morphItems.Count;
        TimeSpan totalTime = TimeSpan.FromSeconds((double)Environment.TickCount / MorphSpeed % totalSeconds);

        double maxProgress = 0;
        using (CanvasDrawingSession drawingSession = source.CreateDrawingSession())
        {
            foreach (TextMorphItem item in morphItems)
            {
                double progress = item.Timeline.GetCurrentProgress(totalTime);
                maxProgress = Math.Max(maxProgress, progress);
                drawingSession.DrawText(
                    item.Text,
                    centerPoint,
                    new CanvasSolidColorBrush(sender, ColorBrush)
                    {
                        Opacity = Convert.ToSingle(progress),
                    },
                    textFormat);
            }
        }

        ArgumentNullException.ThrowIfNull(blurEffect);
        blurEffect.BlurAmount = Convert.ToSingle(20 * (1 - maxProgress));
        blurEffect.Source = source;

        args.DrawingSession.DrawImage(colorMatrixEffect);
        Canvas?.Invalidate();
    }
}