using System;
using System.Windows.Forms;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace BannerlordHardmode
{
    // Purpose: losing a battle costs 30 morale for the player instead of typical 20
    [HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetDefeatMoraleChange")]
    class GetDefeatMoraleChangePatch
    {
        static bool Prefix(PartyBase party, ref float __result)
        {
            bool patched = false;
            try
            {
                if (party.MobileParty.IsMainParty)
                {
                    __result = -30f;
                    patched = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"FUCKAn error occurred during GetDefeatMoraleChangePatch\n\nException:\n{ex.ToString()}\n\n{ex.InnerException?.Message}\n\n{ex.InnerException?.InnerException?.Message}");
            }
            return !patched;
        }
    }
}
