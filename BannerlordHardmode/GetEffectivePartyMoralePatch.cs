using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Localization;
using HarmonyLib;

namespace BannerlordHardmode
{
    // Purpose: reduce the player's base party morale from 50 to 30
    [HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
    class GetEffectivePartyMoralePatch
    {
        static bool Prefix(ref DefaultPartyMoraleModel __instance, MobileParty mobileParty, StatExplainer explanation, ref float __result)
        {
            bool patched = false;
            try
            {
                if (mobileParty.IsMainParty)
                {
                    MethodInfo mGetMoraleEffectsFromSkill = typeof(DefaultPartyMoraleModel).GetMethod("GetMoraleEffectsFromSkill", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo mGetStarvationMoralePenalty = typeof(DefaultPartyMoraleModel).GetMethod("GetStarvationMoralePenalty", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo mGetNoWageMoralePenalty = typeof(DefaultPartyMoraleModel).GetMethod("GetNoWageMoralePenalty", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo mCalculateFoodVarietyMoraleBonus = typeof(DefaultPartyMoraleModel).GetMethod("CalculateFoodVarietyMoraleBonus", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo mGetPartySizeMoraleEffect = typeof(DefaultPartyMoraleModel).GetMethod("CalculateFoodVarietyMoraleBonus", BindingFlags.NonPublic | BindingFlags.Instance);

                    FieldInfo fRecentEventText = typeof(DefaultPartyMoraleModel).GetField("_recentEventsText", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo fStarvationText = typeof(DefaultPartyMoraleModel).GetField("_starvationMoraleText", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo fNoWageText = typeof(DefaultPartyMoraleModel).GetField("_noWageMoraleText", BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    ExplainedNumber explainedNumber = new ExplainedNumber(30f, explanation, (TextObject)null); // THIS IS THE ONLY CHANGE TO ORIGINAL LOGIC 50f to 30f
                    explainedNumber.Add(mobileParty.RecentEventsMorale, (TextObject)fRecentEventText.GetValue(__instance));
                    
                    mGetMoraleEffectsFromSkill.Invoke(__instance, new object[2] { mobileParty, explainedNumber});
                    if (mobileParty.Party.IsStarving)
                        explainedNumber.Add(Convert.ToSingle(mGetStarvationMoralePenalty.Invoke(__instance, new object[1] { mobileParty })), (TextObject)fStarvationText.GetValue(__instance));
                    if ((double)mobileParty.HasUnpaidWages > 0.0)
                        explainedNumber.Add(mobileParty.HasUnpaidWages * (float)mGetNoWageMoralePenalty.Invoke(__instance, new object[1] { mobileParty }), (TextObject)fNoWageText.GetValue(__instance));
                    mCalculateFoodVarietyMoraleBonus.Invoke(__instance, new object[2] { mobileParty, explainedNumber});
                    
                    mGetPartySizeMoraleEffect.Invoke(__instance, new object[2] { mobileParty, explainedNumber });
                    __result = explainedNumber.ResultNumber;
                    patched = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during GetEffectivePartyMoralePatch\n\nException:\n{ex.ToString()}\n\n{ex.InnerException?.Message}\n\n{ex.InnerException?.InnerException?.Message}");
            }
            return !patched;
        }
    }
}
