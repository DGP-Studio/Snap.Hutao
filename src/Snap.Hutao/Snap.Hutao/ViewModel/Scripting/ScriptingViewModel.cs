// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Scripting;

namespace Snap.Hutao.ViewModel.Scripting;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class ScriptingViewModel : Abstraction.ViewModel
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial ScriptingViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial string? InputScript { get; set; }

    [ObservableProperty]
    public partial string? OutputResult { get; set; }

    [Command("ExecuteScriptCommand")]
    private async Task ExecuteScriptAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Execute script", "ScriptingViewModel.Command"));

        string? resultOrError;
        try
        {
            using (InteractiveAssemblyLoader loader = new())
            {
                ScriptOptions options = ScriptOptions.Default.WithAllowUnsafe(true);
                Script<object> script = CSharpScript.Create(InputScript, options, typeof(ScriptContext), assemblyLoader: loader);
                ScriptState<object> state = await script.RunAsync(globals: new ScriptContext()).ConfigureAwait(false);
                object? result = state.ReturnValue;
                resultOrError = result?.ToString();
            }
        }
        catch (Exception ex)
        {
            resultOrError = ex.ToString();
        }

        await taskContext.SwitchToMainThreadAsync();
        OutputResult = resultOrError;
    }
}