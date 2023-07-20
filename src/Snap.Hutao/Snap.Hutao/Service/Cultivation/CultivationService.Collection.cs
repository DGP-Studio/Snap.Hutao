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
            if (projects == null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    projects = appDbContext.CultivateProjects.ToObservableCollection();
                }

                Current ??= projects.SelectedOrDefault();
            }

            return projects;
        }
    }

    /// <inheritdoc/>
    public async Task<ProjectAddResult> TryAddProjectAsync(CultivateProject project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            return ProjectAddResult.InvalidName;
        }

        if (projects!.SingleOrDefault(a => a.Name == project.Name) != null)
        {
            return ProjectAddResult.AlreadyExists;
        }
        else
        {
            // Sync cache
            await taskContext.SwitchToMainThreadAsync();
            projects!.Add(project);

            // Sync database
            await taskContext.SwitchToBackgroundAsync();
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await appDbContext.CultivateProjects.AddAndSaveAsync(project).ConfigureAwait(false);
            }

            return ProjectAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveProjectAsync(CultivateProject project)
    {
        // Sync cache
        // Keep this on main thread.
        await taskContext.SwitchToMainThreadAsync();
        projects!.Remove(project);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.CultivateProjects
                .ExecuteDeleteWhereAsync(p => p.InnerId == project.InnerId)
                .ConfigureAwait(false);
        }
    }
}
