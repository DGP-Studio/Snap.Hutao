// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
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
            ScriptContext context = new();
            object? result = await CSharpScript.EvaluateAsync(InputScript, ScriptOptions.Default, context).ConfigureAwait(false);
            resultOrError = result?.ToString();
        }
        catch (Exception ex)
        {
            resultOrError = ex.ToString();
        }

        await taskContext.SwitchToMainThreadAsync();
        OutputResult = resultOrError;
    }
}