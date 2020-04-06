using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace BannerlordHardmode
{
    [HarmonyPatch(typeof(DefaultPartyWageModel))]
    [HarmonyPatch("GetTotalWage")]
    public class PlayerWagePatch
    {
        static void Postfix(DefaultPartyWageModel __instance, MobileParty mobileParty, StatExplainer explanation, ref int __result)
        {
            __result *= (mobileParty.IsMainParty ? 5 : 1);
        }
    }
}
