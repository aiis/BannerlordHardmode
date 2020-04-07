using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace BannerlordHardmode
{
    class HardmodeMobilePartyFoodConsumptionModel : DefaultMobilePartyFoodConsumptionModel
    {
        private static readonly TextObject _partyConsumption = new TextObject("{=UrFzdy4z}Daily Consumption", (Dictionary<string, TextObject>)null);
        public override float CalculateDailyFoodConsumptionf(MobileParty party, StatExplainer explainer = null)
        {
            float menFedPerFood = (party.IsMainParty ? 8.0f : 20f);
            if (party.CurrentSettlement != null)
            {
                menFedPerFood *= 2;
            }

            int eaters = party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners / 2;
            float foodConsumed = (float)(-(eaters < 1 ? 1.0 : (double)eaters) / menFedPerFood);
            ExplainedNumber explainedNumber = new ExplainedNumber(0.0f, explainer, (TextObject)null);
            explainedNumber.Add(foodConsumed, HardmodeMobilePartyFoodConsumptionModel._partyConsumption);
            return explainedNumber.ResultNumber;
        }
    }
}
