// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.OrTools.LinearSolver;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Runtime.InteropServices;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MinimalPromotionDelta
{
    private const string CacheKey = $"{nameof(MinimalPromotionDelta)}.Cache";

    private readonly IMetadataService metadataService;
    private readonly IMemoryCache memoryCache;

    public async ValueTask<List<AvatarPromotionDelta>> GetAsync()
    {
        if (memoryCache.TryGetRequiredValue(CacheKey, out List<AvatarPromotionDelta>? cache))
        {
            return cache;
        }

        List<ICultivationItemsAccess> cultivationItemsEntryList =
        [
            .. await metadataService.GetAvatarListAsync().ConfigureAwait(false),
            .. (await metadataService.GetWeaponListAsync().ConfigureAwait(false)).Where(w => w.Quality >= Model.Intrinsic.QualityType.QUALITY_BLUE),
        ];
        return memoryCache.Set(CacheKey, GeneratePromotionDeltas(Minimize(cultivationItemsEntryList)));
    }

    private static List<ICultivationItemsAccess> Minimize(List<ICultivationItemsAccess> cultivationItems)
    {
        using (Solver? solver = Solver.CreateSolver("SCIP"))
        {
            ArgumentNullException.ThrowIfNull(solver);

            Objective objective = solver.Objective();
            objective.SetMinimization();

            Dictionary<ICultivationItemsAccess, Variable> itemVariableMap = [];
            foreach (ref readonly ICultivationItemsAccess item in CollectionsMarshal.AsSpan(cultivationItems))
            {
                Variable variable = solver.MakeBoolVar(item.Name);
                itemVariableMap[item] = variable;
                objective.SetCoefficient(variable, 1);
            }

            Dictionary<MaterialId, Constraint> materialConstraintMap = [];
            foreach (ref readonly ICultivationItemsAccess item in CollectionsMarshal.AsSpan(cultivationItems))
            {
                foreach (ref readonly MaterialId materialId in CollectionsMarshal.AsSpan(item.CultivationItems))
                {
                    ref Constraint? constraint = ref CollectionsMarshal.GetValueRefOrAddDefault(materialConstraintMap, materialId, out _);
                    constraint ??= solver.MakeConstraint(1, double.PositiveInfinity, $"{materialId}");
                    constraint.SetCoefficient(itemVariableMap[item], 1);
                }
            }

            Solver.ResultStatus status = solver.Solve();
            HutaoException.ThrowIf(status != Solver.ResultStatus.OPTIMAL, "Unable to solve minimal item set");

            List<ICultivationItemsAccess> results = [];
            foreach ((ICultivationItemsAccess item, Variable variable) in itemVariableMap)
            {
                if (variable.SolutionValue() > 0.5)
                {
                    results.Add(item);
                }
            }

            return results;
        }
    }

    private static List<AvatarPromotionDelta> GeneratePromotionDeltas(List<ICultivationItemsAccess> cultivationItems)
    {
        List<AvatarPromotionDelta> deltas = [];

        foreach (ref readonly ICultivationItemsAccess item in CollectionsMarshal.AsSpan(cultivationItems))
        {
            switch (item)
            {
                case MetadataAvatar avatar:
                    deltas.Add(new()
                    {
                        AvatarId = avatar.Id,
                        AvatarLevelCurrent = 1,
                        AvatarLevelTarget = 90,
                        SkillList = avatar.SkillDepot.CompositeSkillsNoInherents().SelectList(skill => new PromotionDelta()
                        {
                            Id = skill.GroupId,
                            LevelCurrent = 1,
                            LevelTarget = 10,
                        }),
                    });
                    break;
                case MetadataWeapon weapon:
                    if (deltas.FirstOrDefault(d => d.Weapon is null) is { } delta)
                    {
                        delta.Weapon = new()
                        {
                            Id = weapon.Id,
                            LevelCurrent = 1,
                            LevelTarget = 90,
                        };

                        break;
                    }

                    deltas.Add(new()
                    {
                        Weapon = new()
                        {
                            Id = weapon.Id,
                            LevelCurrent = 1,
                            LevelTarget = 90,
                        },
                    });

                    break;
            }
        }

        return deltas;
    }
}