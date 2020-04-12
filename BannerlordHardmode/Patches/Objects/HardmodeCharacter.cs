using TaleWorlds.CampaignSystem;
using HarmonyLib;

namespace BannerlordHardmode.Patches.Objects
{
    [HarmonyPatch(typeof(CharacterObject))]
    [HarmonyPatch("TroopWage", MethodType.Getter)]
    class HardmodeCharacter
    {
        static void Postfix(ref int __result)
        {
            __result *= 4;
        }
    }
}
