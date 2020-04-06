using System;
using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BannerlordHardmode
{
    [HarmonyPatch(typeof(DefaultMobilePartyFoodConsumptionModel))]
    [HarmonyPatch("CalculateDailyFoodConsumptionf")]
    class MainPartyFoodConsumptionPatch
    {
        static bool Prefix(ref DefaultMobilePartyFoodConsumptionModel __instance, MobileParty party, StatExplainer explainer, ref float __result)
        {
            bool patched = false;
            try
            {
                if (party.IsMainParty)
                {
                    float menFedPerFood = 8.0f;
                    FieldInfo fPartyConsumption = typeof(DefaultMobilePartyFoodConsumptionModel).GetField("_partyConsumption", BindingFlags.NonPublic | BindingFlags.Static);

                    if (party.CurrentSettlement != null)
                    {
                        menFedPerFood *= 2;
                    }
                    int eaters = party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners / 2;
                    float foodConsumed = (float)(-(eaters < 1 ? 1.0 : (double)eaters) / menFedPerFood);
                    ExplainedNumber explainedNumber = new ExplainedNumber(0.0f, explainer, (TextObject)null);
                    explainedNumber.Add(foodConsumed, (TextObject)fPartyConsumption.GetValue(__instance));
                    __result = explainedNumber.ResultNumber;
                    patched = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during CalculateDailyFoodConsumption\n\nException:\n{ex.ToString()}\n\n{ex.InnerException?.Message}\n\n{ex.InnerException?.InnerException?.Message}");
            }

            return !patched;
        }
    }
}
