using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Model.Intrinsic;
internal static class FightPropertyExtensions
{
    public static bool IsDoubleCritical(this FightProperty fightProperty)
    {
        return fightProperty == FightProperty.FIGHT_PROP_CRITICAL || fightProperty == FightProperty.FIGHT_PROP_CRITICAL_HURT;
    }
}
