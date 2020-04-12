using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;

namespace BannerlordHardmode
{
    class HardmodeDesertionCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener((object)this, new Action<MobileParty>(this.DailyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            return;
        }

        public void DailyTick(MobileParty mobileParty)
        {   
            if (!Campaign.Current.DesertionEnabled || !mobileParty.IsActive || (mobileParty.IsDisbanding || mobileParty.Party.MapEvent != null) || !mobileParty.IsLordParty && (!mobileParty.IsGarrison || mobileParty.CurrentSettlement == null) && !mobileParty.IsCaravan)
                return;
            
            // This party can have desertions, so figure out if it needs them and apply
            TroopRoster desertedTroopList = (TroopRoster)null;
            if (mobileParty.IsLordParty || mobileParty.IsCaravan)
                this.CheckDesertionDueToMorale(mobileParty, ref desertedTroopList);
            this.CheckDesertionDueToPartySizeExceedsPaymentRatio(mobileParty, ref desertedTroopList);

            if (desertedTroopList != (TroopRoster)null && desertedTroopList.Count > 0 && mobileParty.IsMainParty)
            {
                // Show desertions in log
                MethodInfo mAddInformationData = typeof(CampaignInformationManager).GetMethod("AddInformationData", BindingFlags.NonPublic | BindingFlags.Instance);
                mAddInformationData.Invoke(Campaign.Current.CampaignInformationManager, new object[1] { new LogEntries.DesertionLogEntry(Hero.MainHero, desertedTroopList.TotalManCount) });
                
                // Apply penalties to clan (notification handled on action level)
                Actions.ChangeRenown.Apply(mobileParty.LeaderHero, -Convert.ToSingle(desertedTroopList.TotalManCount)*0.8f, Convert.ToSingle(Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(mobileParty.LeaderHero.Clan.Tier))); // TODO test effects if applying to all parties
                Actions.ChangeInfluence.Apply(mobileParty.LeaderHero, -Convert.ToSingle(desertedTroopList.TotalManCount * 2f));
            }

            // Remove party if all members have deserted
            if (mobileParty.Party.NumberOfAllMembers > 0)
                return;
            mobileParty.RemoveParty();
        }

        public static void DesertTroopsFromParty(MobileParty party, int stackNo, int numberOfDeserters, ref TroopRoster desertedTroopList)
        {
            TroopRosterElement rosterSlot = party.MemberRoster.GetElementCopyAtIndex(stackNo);
            party.MemberRoster.AddToCounts(rosterSlot.Character, -numberOfDeserters, false, 0, 0, true, -1);
            if (desertedTroopList == (TroopRoster)null)
                desertedTroopList = new TroopRoster();
            desertedTroopList.AddToCounts(rosterSlot.Character, numberOfDeserters, false, 0, 0, true, -1);
        }

        private void CheckForTroopDesertionEffectiveMorale(MobileParty party, int stackNo, int desertIfMoraleIsLessThanValue, out int numberOfDeserters)
        {
            int regularDeserters = 0;
            float morale = party.Morale;
            if (party.IsActive && party.MemberRoster.Count > 0)
            {
                TroopRosterElement troopStack = party.MemberRoster.GetElementCopyAtIndex(stackNo);
                double desertChance = Math.Pow((double)troopStack.Character.Level / 100.0, 0.100000001490116 * (((double)desertIfMoraleIsLessThanValue - (double)morale) / (double)desertIfMoraleIsLessThanValue));
                for (int index = 0; index < troopStack.Number; ++index)
                {
                    if (desertChance < (double)MBRandom.RandomFloat)
                        ++regularDeserters;
                }
            }
            numberOfDeserters = regularDeserters;
        }

        private void CheckDesertionDueToPartySizeExceedsPaymentRatio(MobileParty mobileParty, ref TroopRoster desertedTroopList)
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
                            DesertTroopsFromParty(mobileParty, index2, 1, ref desertedTroopList);
                            break;
                        }
                    }
                }
            }
        }

        private void CheckDesertionDueToMorale(MobileParty party, ref TroopRoster desertedTroopList)
        {
            int desertionThreshold = Campaign.Current.Models.PartyMoraleModel.GetTroopDesertionThreshold(party);
            if ((double)party.Morale < (double)desertionThreshold && party.Leader != null)
            {
                for (int stackNo = 0; stackNo < party.MemberRoster.Count; stackNo++)
                {
                    if (!party.MemberRoster.GetCharacterAtIndex(stackNo).IsHero)
                    {
                        int numberOfDeserters = 0;
                        this.CheckForTroopDesertionEffectiveMorale(party, stackNo, desertionThreshold, out numberOfDeserters);
                        if (numberOfDeserters > 0)
                        {
                            DesertTroopsFromParty(party, stackNo, numberOfDeserters, ref desertedTroopList);
                        }
                    }
                }
            }
        }
    }
}
