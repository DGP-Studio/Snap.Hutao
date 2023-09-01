// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 集合部分
/// </summary>
internal sealed partial class CultivationService
{
    private ObservableCollection<CultivateProject>? projects;

    /// <inheritdoc/>
    public CultivateProject? Current
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<CultivateProject> ProjectCollection
    {
        get
        {
            if (projects is null)
            {
                projects = cultivationDbService.GetCultivateProjectCollection();
                Current ??= projects.SelectedOrDefault();
            }

            return projects;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ProjectAddResult> TryAddProjectAsync(CultivateProject project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            return ProjectAddResult.InvalidName;
        }

        ArgumentNullException.ThrowIfNull(projects);

        if (projects.Any(a => a.Name == project.Name))
        {
            return ProjectAddResult.AlreadyExists;
        }

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        projects.Add(project);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await cultivationDbService.AddCultivateProjectAsync(project).ConfigureAwait(false);

        return ProjectAddResult.Added;
    }

    /// <inheritdoc/>
    public async ValueTask RemoveProjectAsync(CultivateProject project)
    {
        ArgumentNullException.ThrowIfNull(projects);

        // Sync cache
        // Keep this on main thread.
        await taskContext.SwitchToMainThreadAsync();
        projects.Remove(project);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await cultivationDbService.DeleteCultivateProjectByIdAsync(project.InnerId).ConfigureAwait(false);
    }
}
