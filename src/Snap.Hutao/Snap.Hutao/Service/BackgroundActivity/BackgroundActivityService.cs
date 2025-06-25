// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.BackgroundActivity;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class BackgroundActivityOptions
{
    private readonly ITaskContext taskContext;

    public ObservableCollection<BackgroundActivity> Activities { get; } = [];

    public async ValueTask<bool> AddActivityAsync(string id, string name, string description, bool isIndeterminate, double progressValue = 0)
    {
        if (Activities.Any(a => a.Id == id))
        {
            return false;
        }

        BackgroundActivity activity = new(id)
        {
            Name = name,
            Description = description,
            IsIndeterminate = isIndeterminate,
            ProgressValue = progressValue,
        };

        await taskContext.SwitchToMainThreadAsync();
        Activities.Add(activity);
        return true;
    }

    public async ValueTask<bool> UpdateActivityAsync(string id, Action<BackgroundActivity> updateAction)
    {
        if (Activities.SingleOrDefault(a => a.Id == id) is not { } activity)
        {
            return false;
        }

        await taskContext.SwitchToMainThreadAsync();
        updateAction(activity);
        return true;
    }

    public async ValueTask<bool> DeleteActivityAsync(string id)
    {
        await taskContext.SwitchToMainThreadAsync();
        BackgroundActivity? activity = Activities.SingleOrDefault(a => a.Id == id);
        if (activity is null)
        {
            return false;
        }

        Activities.Remove(activity);
        return true;
    }
}