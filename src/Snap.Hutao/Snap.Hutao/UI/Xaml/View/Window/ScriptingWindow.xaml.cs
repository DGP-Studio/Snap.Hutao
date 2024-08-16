// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Scripting;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class ScriptingWindow : Microsoft.UI.Xaml.Window, IXamlWindowExtendContentIntoTitleBar, IXamlWindowHasInitSize
{
    private const int MinWidth = 600;
    private const int MinHeight = 200;

#pragma warning disable SA1310
    private static readonly SolidColorBrush Brush_Interface = new(ColorHelper.ToColor(0xFFB8D7A3));
    private static readonly SolidColorBrush Brush_Struct = new(ColorHelper.ToColor(0xFF86C691));
    private static readonly SolidColorBrush Brush_Keyword = new(ColorHelper.ToColor(0xFF569CD6));
    private static readonly SolidColorBrush Brush_Bracket = new(ColorHelper.ToColor(0xFF65ADE5));
    private static readonly SolidColorBrush Brush_Method = new(ColorHelper.ToColor(0xFFDCDCAA));
    private static readonly SolidColorBrush Brush_Argument = new(ColorHelper.ToColor(0xFF9CDCFE));
#pragma warning restore SA1310

    public ScriptingWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Write XAML is not fesible, so we use code
        {
            InlineCollection inlines;

            // System.IServiceProvider ServiceProvider { get; }
            inlines = TextBlock_ServiceProvider.Inlines;
            inlines.Add(new Run() { Text = "System." });
            inlines.Add(new Run() { Text = "IServiceProvider", Foreground = Brush_Interface });
            inlines.Add(new Run() { Text = " ServiceProvider " });
            inlines.Add(new Run() { Text = "{ ", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "get", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "; " });
            inlines.Add(new Run() { Text = "}", Foreground = Brush_Bracket });

            // static string FormatJson(string input);
            inlines = TextBlock_FormatJson.Inlines;
            inlines.Add(new Run() { Text = "static string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = " FormatJson", Foreground = Brush_Method });
            inlines.Add(new Run() { Text = "(", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = " input", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = ")", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = ";" });

            // System.Threading.Tasks.ValueTask<string> RequestAsync(string method, string url, string[] headers, string? body)
            inlines = TextBlock_RequestAsync.Inlines;
            inlines.Add(new Run() { Text = "System.Threading.Tasks." });
            inlines.Add(new Run() { Text = "ValueTask", Foreground = Brush_Struct });
            inlines.Add(new Run() { Text = "<", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "> ", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "RequestAsync", Foreground = Brush_Method });
            inlines.Add(new Run() { Text = "(", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "string ", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "method, ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "string ", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "url, ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "[] ", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "headers, ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "? ", });
            inlines.Add(new Run() { Text = "body ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "= " });
            inlines.Add(new Run() { Text = "default", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = ")", Foreground = Brush_Bracket });

            // System.Threading.Tasks.ValueTask<string> RequestWithCurrentUserAndUidAsync(string method, string url, string[] headers, string? body, string? ds = default)
            inlines = TextBlock_RequestWithCurrentUserAndUidAsync.Inlines;
            inlines.Add(new Run() { Text = "System.Threading.Tasks." });
            inlines.Add(new Run() { Text = "ValueTask", Foreground = Brush_Struct });
            inlines.Add(new Run() { Text = "<", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "> ", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "RequestWithCurrentUserAndUidAsync", Foreground = Brush_Method });
            inlines.Add(new Run() { Text = "(", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "string ", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "method, ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "string ", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "url, ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "[] ", Foreground = Brush_Bracket });
            inlines.Add(new Run() { Text = "headers, ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "? ", });
            inlines.Add(new Run() { Text = "body ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "= " });
            inlines.Add(new Run() { Text = "default, ", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "string", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = "? ", });
            inlines.Add(new Run() { Text = "ds ", Foreground = Brush_Argument });
            inlines.Add(new Run() { Text = "= " });
            inlines.Add(new Run() { Text = "default", Foreground = Brush_Keyword });
            inlines.Add(new Run() { Text = ")", Foreground = Brush_Bracket });
        }

        RootGrid.InitializeDataContext<ScriptingViewModel>(serviceProvider);
        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public SizeInt32 InitSize { get; } = new(800, 500);

    public SizeInt32 MinSize { get; } = new(MinWidth, MinHeight);
}