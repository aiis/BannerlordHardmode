using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace BannerlordHardmode
{
    [HarmonyPatch(typeof(DefaultDifficultyModel))]
    [HarmonyPatch("GetDamageToPlayerMultiplier")]
    class PlayerDamagePatch
    {
        static void Postfix(ref float __result)
        {
            __result = 2f;
        }
    }
}
