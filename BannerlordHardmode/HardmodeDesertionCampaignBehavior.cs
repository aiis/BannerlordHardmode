using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using Helpers;

namespace BannerlordHardmode
{
    class HardmodeDesertionCampaignBehavior : DesertionCampaignBehavior
    {
        private int _numberOfDesertersFromLordParty;
        public new void DailyTickParty(MobileParty mobileParty)
        {
            if (!Campaign.Current.DesertionEnabled || !mobileParty.IsActive || (mobileParty.IsDisbanding || mobileParty.Party.MapEvent != null) || !mobileParty.IsLordParty && (!mobileParty.IsGarrison || mobileParty.CurrentSettlement == null) && !mobileParty.IsCaravan)
                return;
            TroopRoster desertedTroopList = (TroopRoster)null;
            if (mobileParty.IsLordParty || mobileParty.IsCaravan)
                this.PartiesCheckDesertionDueToMorale(mobileParty, ref desertedTroopList);
            this.PartiesCheckDesertionDueToPartySizeExceedsPaymentRatio(mobileParty, ref desertedTroopList);
            if (desertedTroopList != (TroopRoster)null && desertedTroopList.Count > 0)
            {
                MethodInfo mOnTroopsDeserted = typeof(DefaultNotificationsCampaignBehavior).GetMethod("OnTroopsDeserted", BindingFlags.NonPublic | BindingFlags.Instance);
                mOnTroopsDeserted.Invoke(CampaignEventDispatcher.Instance, new object[2] { mobileParty, desertedTroopList });
            }
            
            if (mobileParty.Party.NumberOfAllMembers > 0)
                return;
            mobileParty.RemoveParty();
        }

        private void PartiesCheckDesertionDueToPartySizeExceedsPaymentRatio(MobileParty mobileParty, ref TroopRoster desertedTroopList)
        {
            int partySizeLimit = mobileParty.Party.PartySizeLimit;
            if ((double)mobileParty.Party.NumberOfAllMembers / (double)partySizeLimit <= (double)mobileParty.PaymentRatio)
                return;
            int paymentRatio = Campaign.Current.Models.PartyMoraleModel.NumberOfDesertersDueToPaymentRatio(mobileParty);
            for (int index1 = 0; index1 < paymentRatio; ++index1)
            {
                float num = (float)mobileParty.Party.NumberOfRegularMembers * MBRandom.RandomFloat;
                for (int index2 = mobileParty.MemberRoster.Count - 1; index2 >= 0; --index2)
                {
                    if (!mobileParty.MemberRoster.GetCharacterAtIndex(index2).IsHero)
                    {
                        num -= (float)mobileParty.MemberRoster.GetElementNumber(index2);
                        if ((double)num < 0.0)
                        {
                            MobilePartyHelper.DesertTroopsFromParty(mobileParty, index2, 1, 0, ref desertedTroopList);
                            break;
                        }
                    }
                }
            }
        }

        private bool PartiesCheckDesertionDueToMorale(MobileParty party, ref TroopRoster desertedTroopList)
        {
            int desertionThreshold = Campaign.Current.Models.PartyMoraleModel.GetTroopDesertionThreshold(party);
            bool flag = false;
            if ((double)party.Morale < (double)desertionThreshold && party.Leader != null)
            {
                for (int index = party.MemberRoster.Count - 1; index >= 0; --index)
                {
                    if (!party.MemberRoster.GetCharacterAtIndex(index).IsHero)
                    {
                        int numberOfDeserters = 0;
                        int numberOfWoundedDeserters = 0;
                        this.PartiesCheckForTroopDesertionEffectiveMorale(party, index, desertionThreshold, out numberOfDeserters, out numberOfWoundedDeserters);
                        if (numberOfDeserters + numberOfWoundedDeserters > 0)
                        {
                            if (party.IsLordParty && party.MapFaction.IsKingdomFaction)
                                ++this._numberOfDesertersFromLordParty;
                            flag = true;
                            MobilePartyHelper.DesertTroopsFromParty(party, index, numberOfDeserters, numberOfWoundedDeserters, ref desertedTroopList);
                            if (party.IsMainParty)
                            {
                                Actions.ChangeRenown.Apply(Hero.MainHero, -5f);
                            }
                        }
                    }
                }
            }
            return flag;
        }

        private void PartiesCheckForTroopDesertionEffectiveMorale(MobileParty party, int stackNo, int desertIfMoraleIsLessThanValue, out int numberOfDeserters, out int numberOfWoundedDeserters)
        {
            int num1 = 0;
            int num2 = 0;
            float morale = party.Morale;
            if (party.IsActive && party.MemberRoster.Count > 0)
            {
                TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(stackNo);
                double num3 = Math.Pow((double)elementCopyAtIndex.Character.Level / 100.0, 0.100000001490116 * (((double)desertIfMoraleIsLessThanValue - (double)morale) / (double)desertIfMoraleIsLessThanValue));
                for (int index = 0; index < elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber; ++index)
                {
                    if (num3 < (double)MBRandom.RandomFloat)
                        ++num1;
                }
                for (int index = 0; index < elementCopyAtIndex.WoundedNumber; ++index)
                {
                    if (num3 < (double)MBRandom.RandomFloat)
                        ++num2;
                }
            }
            numberOfDeserters = num1;
            numberOfWoundedDeserters = num2;
        }
    }
}
