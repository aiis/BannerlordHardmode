using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace BannerlordHardmode.Patches.Objects
{
    [HarmonyPatch(typeof(MobileParty), "DailyTick")]
    class MobilePartyPatch
    {
        static void Postfix(MobileParty __instance)
        {
            if (__instance.CurrentSettlement != null && __instance.IsActive && !__instance.Party.IsStarving)
            {
                if (__instance.RecentEventsMorale < 0)
                {
                    // if party morale is below base, double recovery (normally recovers 10%)
                    __instance.RecentEventsMorale -= (__instance.RecentEventsMorale / 0.9f) * 0.2f;
                } else if (__instance.RecentEventsMorale > 0)
                {
                    // if party morale is above base, half the amount the game normally takes (normally takes 10%)
                    __instance.RecentEventsMorale = __instance.RecentEventsMorale / 0.95f;
                }
                
            } else if (__instance.IsMainParty && __instance.Party.IsStarving)
            {
                __instance.RecentEventsMorale = __instance.RecentEventsMorale / 0.9f;
            }
        }
    }
}
