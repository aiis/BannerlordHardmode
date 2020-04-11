using System;
using TaleWorlds.CampaignSystem;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Localization;
using TaleWorlds.Core;

namespace BannerlordHardmode
{
    class HardmodePartyMoraleModel : DefaultPartyMoraleModel
    {
        private readonly TextObject _recentEventsText = GameTexts.FindText("str_recent_events", (string)null);
        private readonly TextObject _starvationMoraleText = GameTexts.FindText("str_starvation_morale", (string)null);
        private readonly TextObject _noWageMoraleText = GameTexts.FindText("str_no_wage_morale", (string)null);
        private readonly TextObject _foodBonusMoraleText = GameTexts.FindText("str_food_bonus_morale", (string)null);
        private readonly TextObject _partySizeMoraleText = GameTexts.FindText("str_party_size_morale", (string)null);

        private void GetMoraleEffectsFromSkill(MobileParty party, ref ExplainedNumber bonus)
        {
            if (party.LeaderHero == null || party.Leader.GetSkillValue(DefaultSkills.Leadership) <= 0)
                return;
            SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Leadership, DefaultSkillEffects.LeadershipMoraleBonus, party.Leader, ref bonus, true);
        }

        private int GetStarvationMoralePenalty(MobileParty party)
        {
            return -30;
        }

        private void CalculateFoodVarietyMoraleBonus(MobileParty party, ref ExplainedNumber result)
        {
            float num;
            switch (party.ItemRoster.FoodVariety)
            {
                case 0:
                case 1:
                    num = -2f;
                    break;
                case 2:
                    num = -1f;
                    break;
                case 3:
                    num = 0.0f;
                    break;
                case 4:
                    num = 1f;
                    break;
                case 5:
                    num = 2f;
                    break;
                case 6:
                    num = 3f;
                    break;
                case 7:
                    num = 5f;
                    break;
                case 8:
                    num = 6f;
                    break;
                case 9:
                    num = 7f;
                    break;
                case 10:
                    num = 8f;
                    break;
                default:
                    num = 2f;
                    break;
            }
            if ((double)num == 0.0)
                return;
            result.Add(num, this._foodBonusMoraleText);
        }

        private int GetNoWageMoralePenalty(MobileParty party)
        {
            return -20;
        }

        private void GetPartySizeMoraleEffect(MobileParty party, ref ExplainedNumber result)
        {
            int num = party.Party.NumberOfAllMembers - party.Party.PartySizeLimit;
            if (num <= 0)
                return;
            result.Add(-1f * (float)Math.Sqrt((double)num), this._partySizeMoraleText);
        }

        public override int GetTroopDesertionThreshold(MobileParty party)
        {
            return party.IsMainParty ? 25 : 10;
        }

        public override float GetDefeatMoraleChange(PartyBase party)
        {
            if (party.MobileParty.IsMainParty)
            {
                return -30f;
            }
            return base.GetDefeatMoraleChange(party);
        }

        public override float GetEffectivePartyMorale(MobileParty mobileParty, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber;
            if (mobileParty.IsMainParty)
            {
                explainedNumber = new ExplainedNumber(30f, explanation, (TextObject)null); // Changed from 50f to 30f
            }
            else
            {
                explainedNumber = new ExplainedNumber(50f, explanation, (TextObject)null);
            }
            
            explainedNumber.Add(mobileParty.RecentEventsMorale, this._recentEventsText);
            this.GetMoraleEffectsFromSkill(mobileParty, ref explainedNumber);
            if (mobileParty.Party.IsStarving)
                explainedNumber.Add((float)this.GetStarvationMoralePenalty(mobileParty), this._starvationMoraleText);
            if ((double)mobileParty.HasUnpaidWages > 0.0)
                explainedNumber.Add(mobileParty.HasUnpaidWages * (float)this.GetNoWageMoralePenalty(mobileParty), this._noWageMoraleText);
            this.CalculateFoodVarietyMoraleBonus(mobileParty, ref explainedNumber);
            this.GetPartySizeMoraleEffect(mobileParty, ref explainedNumber);
            return explainedNumber.ResultNumber;
        }
    }
}
