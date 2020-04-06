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
