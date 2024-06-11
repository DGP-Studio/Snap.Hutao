// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.OrTools.LinearSolver;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Primitive;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Inventory;

internal static class MinimalPromotionDelta
{
    public static List<ICultivationItemsAccess> Find(List<ICultivationItemsAccess> cultivationItems)
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
}