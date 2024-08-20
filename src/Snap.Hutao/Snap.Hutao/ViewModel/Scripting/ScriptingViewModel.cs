// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Snap.Hutao.Core.Scripting;

namespace Snap.Hutao.ViewModel.Scripting;

[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class ScriptingViewModel : Abstraction.ViewModel
{
    private readonly ITaskContext taskContext;

    private string? inputScript;
    private string? outputResult;

    public string? InputScript { get => inputScript; set => SetProperty(ref inputScript, value); }

    public string? OutputResult { get => outputResult; set => SetProperty(ref outputResult, value); }

    [Command("ExecuteScriptCommand")]
    private async Task ExecuteScriptAsync()
    {
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