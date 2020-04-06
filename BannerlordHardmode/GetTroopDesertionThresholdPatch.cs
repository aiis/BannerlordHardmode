using System;
using System.Windows.Forms;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace BannerlordHardmode
{
    // Purpose: troops begin to desert at 25 morale
    [HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetTroopDesertionThreshold")]
    class GetTroopDesertionThresholdPatch
    {
        static bool Prefix(MobileParty party, ref int __result)
        {
            bool patched = false;
            try
            {
                if (party.IsMainParty)
                {
                    __result = 25;
                    patched = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during GetTroopDesertionThresholdPatch\n\nException:\n{ex.ToString()}\n\n{ex.InnerException?.Message}\n\n{ex.InnerException?.InnerException?.Message}");
            }
            return !patched;
        }
    }
}
