using System.Collections.Generic;
using System.Text;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;
using TaleWorlds.Library;
using TaleWorlds.Core;
using Helpers;

namespace BannerlordHardmode
{
    class HardmodeClanFinanceModel : DefaultClanFinanceModel
    {
        private static TextObject _mercnaryStr = new TextObject("Mercenary Contract", (Dictionary<string, TextObject>)null);

        public override void CalculateClanIncome(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false)
        {
            // Gives (Clan not mercenary, is in faction, faction they're in is a kingdom, clan is not a ruling clan, and they have no fortifications) 100 gold per commander heroes
            if (!clan.IsUnderMercenaryService && clan.MapFaction != null && (clan.MapFaction.IsKingdomFaction && clan.Leader != clan.MapFaction.Leader) && clan.Fortifications.Count == 0)
            {
                TextObject desription = new TextObject("{=*} King's support", (Dictionary<string, TextObject>)null);
                int welfare = ((clan == Clan.PlayerClan ? 1 : 0) + clan.CommanderHeroes.Count) * 100;
                goldChange.Add((float)welfare, desription);
            }

            // INCOME FROM TOWN & CASTLE TAXES
            float totalTownTaxes = 0.0f;
            foreach (Town fortification in (IEnumerable<Town>)clan.Fortifications)
            {
                TextObject desription = new TextObject("{=YnvU7tWg} {PARTY_NAME} taxes", (Dictionary<string, TextObject>)null);
                desription.SetTextVariable("PARTY_NAME", fortification.Name);
                float townTax = (float)Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(fortification, (StatExplainer)null)*2f;
                if (!fortification.IsCastle)
                {
                    townTax *= 1.25f; // end result is 2.5x for towns but only 2x for castles
                }
                goldChange.Add(MathF.Round((townTax)), desription);
                totalTownTaxes += townTax;
            }
            if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan && clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.WarTax))
                goldChange.Add(totalTownTaxes * 0.05f, DefaultPolicies.WarTax.Name);

            // INCOME FROM MERCENARY SERVICE
            if (clan.IsUnderMercenaryService && clan.Leader != null && (clan.Leader.MapFaction.IsKingdomFaction && clan.Leader.MapFaction.Leader != null))
            {
                int num2 = (int)((double)clan.Influence * 0.100000001490116);
                int income = num2 * clan.MercenaryAwardMultiplier;
                goldChange.Add((float)income, HardmodeClanFinanceModel._mercnaryStr);
                if (applyWithdrawals)
                    clan.Influence -= (float)num2;
            }

            // INCOME FROM VILLAGES
            foreach (Village village in clan.Villages)
            {
                int accumulatedTradeTaxPayout = village.VillageState == Village.VillageStates.Looted || village.VillageState == Village.VillageStates.BeingRaided ? 0 : (int)((double)village.TradeTaxAccumulated / (double)this.RevenueSmoothenFraction());
                if (clan.Kingdom != null && clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LandTax))
                    goldChange.Add((float)-accumulatedTradeTaxPayout * 0.05f, DefaultPolicies.LandTax.Name);
                TextObject desription = new TextObject("{=YnvU7tWg} {PARTY_NAME} taxes", (Dictionary<string, TextObject>)null);
                desription.SetTextVariable("PARTY_NAME", village.Name);
                
                if (clan == Clan.PlayerClan)
                {
                    goldChange.Add(MathF.Round((float)accumulatedTradeTaxPayout * 0.7f), desription); // <-- 70% village income
                }
                else
                {
                    goldChange.Add((float)accumulatedTradeTaxPayout, desription);
                }

                if (applyWithdrawals)
                    village.TradeTaxAccumulated -= accumulatedTradeTaxPayout;
            }

            // INCOME FROM TOWN TARIFFS
            foreach (Town fortification in (IEnumerable<Town>)clan.Fortifications)
            {
                TextObject desription = new TextObject("{=oICap7jL} {SETTLEMENT_NAME} tariff", (Dictionary<string, TextObject>)null);
                desription.SetTextVariable("SETTLEMENT_NAME", fortification.Name);
                int townTradeTax = (int)((double)fortification.TradeTaxAccumulated / (double)this.RevenueSmoothenFraction());
                if (fortification.Governor != null && fortification.Governor.GetPerkValue(DefaultPerks.Trade.ContentTrades))
                {
                    ExplainedNumber bonuses = new ExplainedNumber((float)townTradeTax, (StringBuilder)null);
                    PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.ContentTrades, fortification, ref bonuses);
                    townTradeTax = MathF.Round(bonuses.ResultNumber);
                }
                goldChange.Add((float)townTradeTax, desription);
                if (applyWithdrawals)
                    fortification.TradeTaxAccumulated -= townTradeTax;
            }

            // INCOME FROM WORKSHOPS
            this.CalculateHeroIncomeFromWorkshops(clan.Leader, ref goldChange, applyWithdrawals);

            // INCOME FROM CARAVANS
            foreach (MobileParty party in clan.Parties)
            {
                if (party.IsActive && (party.IsLordParty || party.IsGarrison || party.IsCaravan))
                {
                    int amount = party.LeaderHero == null || party.LeaderHero == clan.Leader || (party.IsCaravan || party.LeaderHero.Gold < 10000) ? 0 : (int)((double)(party.LeaderHero.Gold - 10000) / 10.0);
                    if (party.IsCaravan)
                        amount = (int)((double)(party.PartyTradeGold - 10000) / 10.0);
                    if (amount > 0)
                    {
                        TextObject desription = new TextObject("{=uyvxafSw} {PARTY_NAME} exceed gold", (Dictionary<string, TextObject>)null);
                        desription.SetTextVariable("PARTY_NAME", party.Name);
                        goldChange.Add((float)amount, desription);
                        if (applyWithdrawals)
                        {
                            if (party.IsCaravan)
                            {
                                if (((party.LeaderHero == null || !party.LeaderHero.Clan.Leader.GetPerkValue(DefaultPerks.Trade.GreatInvestor) ? (party.Party.Owner?.Clan == null || party.Party.Owner.Clan == CampaignData.NeutralFaction || party.Party.Owner.Clan.Leader == null ? 0 : (party.Party.Owner.Clan.Leader.GetPerkValue(DefaultPerks.Trade.GreatInvestor) ? 1 : 0)) : 1) & (applyWithdrawals ? 1 : 0)) != 0)
                                    party.LeaderHero.Clan.AddRenown(DefaultPerks.Trade.GreatInvestor.PrimaryBonus, true);
                                party.PartyTradeGold -= amount;
                            }
                            else
                                GiveGoldAction.ApplyBetweenCharacters(party.LeaderHero, (Hero)null, amount, true);
                        }
                    }
                }
            }
        }

        private void CalculateHeroIncomeFromWorkshops(Hero hero, ref ExplainedNumber goldChange, bool applyWithdrawals)
        {
            int num1 = 0;
            foreach (Workshop ownedWorkshop in (IEnumerable<Workshop>)hero.OwnedWorkshops)
            {
                int incomeFromWorkshop = Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromWorkshop(ownedWorkshop);
                int num2 = incomeFromWorkshop;
                if (applyWithdrawals && incomeFromWorkshop > 0)
                    ownedWorkshop.ChangeGold(-incomeFromWorkshop);
                if (incomeFromWorkshop > 0)
                {
                    TextObject desription = new TextObject("{=Vg7glbwp} {WORKSHOP_NAME} at {SETTLEMENT_NAME}", (Dictionary<string, TextObject>)null);
                    desription.SetTextVariable("SETTLEMENT_NAME", ownedWorkshop.Settlement.Name);
                    desription.SetTextVariable("WORKSHOP_NAME", ownedWorkshop.Name);
                    if (hero.Clan == Clan.PlayerClan && num2 > 0)
                    {
                        goldChange.Add(MathF.Round((float)num2 * 0.6f), desription); // 60% workshop income
                    }
                    else
                    {
                        goldChange.Add((float)num2, desription);
                    }
                    if (hero.Clan.Leader.GetPerkValue(DefaultPerks.Trade.ArtisanCommunity) & applyWithdrawals)
                        ++num1;
                }
            }
            if (!(hero.Clan.Leader.GetPerkValue(DefaultPerks.Trade.ArtisanCommunity) & applyWithdrawals))
                return;
            hero.Clan.AddRenown((float)num1 * DefaultPerks.Trade.ArtisanCommunity.PrimaryBonus, true);
        }

        private static void ApplyMoraleEffect(MobileParty mobileParty, int wage, int paymentAmount)
        {
            if (paymentAmount < wage)
            {
                float num1 = (float)(1.0 - (double)paymentAmount / (double)wage);
                float num2 = (float)Campaign.Current.Models.PartyMoraleModel.GetDailyNoWageMoralePenalty(mobileParty) * num1;
                if ((double)mobileParty.HasUnpaidWages < (double)num1)
                    num2 += (float)Campaign.Current.Models.PartyMoraleModel.GetDailyNoWageMoralePenalty(mobileParty) * (num1 - mobileParty.HasUnpaidWages);
                mobileParty.RecentEventsMorale += num2;
                mobileParty.HasUnpaidWages = num1;
                MBTextManager.SetTextVariable("reg1", (float)Math.Round((double)Math.Abs(num2), 1), false);
                if (mobileParty != MobileParty.MainParty)
                    return;
                InformationManager.AddQuickInformation(GameTexts.FindText("str_party_loses_moral_due_to_insufficent_funds", (string)null), 0, (BasicCharacterObject)null, "");
            }
            else
                mobileParty.HasUnpaidWages = 0.0f;
        }

        public override void CalculateClanExpenses(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false)
        {
            foreach (MobileParty party in clan.Parties)
            {
                if (party.IsActive && (party.IsLordParty || party.IsGarrison || party.IsCaravan))
                {
                    int partyWage = this.CalculatePartyWage(party, applyWithdrawals);
                    int amount = party.LeaderHero == null || party.LeaderHero == clan.Leader || (party.IsCaravan || party.LeaderHero.Gold > 10000) ? 0 : (party.LeaderHero.Gold < 5000 ? (int)((double)(5000 - party.LeaderHero.Gold) / 10.0) : 0);
                    if (applyWithdrawals)
                    {
                        float num = (float)clan.Gold + MathF.Abs(goldChange.ResultNumber);
                        int paymentAmount;
                        if ((double)num > (double)(partyWage + amount))
                        {
                            paymentAmount = partyWage;
                            if (amount > 0)
                                GiveGoldAction.ApplyBetweenCharacters((Hero)null, party.LeaderHero, amount, true);
                        }
                        else
                        {
                            paymentAmount = (int)num > 0 ? (int)num : 0;
                            if (paymentAmount > partyWage)
                                paymentAmount = partyWage;
                        }
                        HardmodeClanFinanceModel.ApplyMoraleEffect(party, partyWage, paymentAmount);
                    }
                    TextObject desription1 = new TextObject("{=rhKxsdtz} {PARTY_NAME} wages", (Dictionary<string, TextObject>)null);
                    desription1.SetTextVariable("PARTY_NAME", party.Name);
                    goldChange.Add((float)-partyWage, desription1);
                    if (party.LeaderHero != null && party.LeaderHero != clan.Leader && amount > 0)
                    {
                        TextObject desription2 = new TextObject("{=tetGlwTx} {PARTY_NAME} finance help", (Dictionary<string, TextObject>)null);
                        desription2.SetTextVariable("PARTY_NAME", party.Name);
                        goldChange.Add((float)-amount, desription2);
                    }
                }
            }
            if (clan.MapFaction == null || !clan.MapFaction.IsKingdomFaction || clan.Leader != clan.MapFaction.Leader)
                return;
            foreach (Clan clan1 in ((Kingdom)clan.MapFaction).Clans)
            {
                if (!clan1.IsUnderMercenaryService && clan1 != clan && clan1.Fortifications.Count == 0)
                {
                    TextObject desription = new TextObject("{=*} King's support", (Dictionary<string, TextObject>)null);
                    int num1 = ((clan1 == Clan.PlayerClan ? 1 : 0) + clan1.CommanderHeroes.Count) * 100;
                    int num2 = clan.Leader.Gold > num1 ? num1 : clan.Leader.Gold;
                    goldChange.Add((float)-num2, desription);
                }
            }
        }

        public override int CalculatePartyWage(MobileParty mobileParty, bool applyWithdrawals)
        {
            if (!mobileParty.IsActive || mobileParty.IsMilitia)
                return 0;
            int wages = mobileParty.GetTotalWage(1f, (StatExplainer)null);
            if (mobileParty.IsMainParty && (mobileParty.CurrentSettlement != null | mobileParty.LastVisitedSettlement.GetTrackDistanceToMainAgent() <= 2.0f))
            {
                // player in settlement should have 2x wages so bring it half down
                wages /= 2;
            } else if (mobileParty.Party.Owner.Clan == Clan.PlayerClan && mobileParty.IsGarrison)
            {
                // player clan garrison has 2x wages so bring it half down
                wages /= 2;
            } else if (mobileParty.Party.Owner.Clan != Clan.PlayerClan)
            {
                // AI shouldn't have 4x wages so bring it back down
                wages /= 4;
            }
            if (mobileParty.IsGarrison)
                wages = (int)((double)wages * 0.75);
            
            ExplainedNumber bonuses = new ExplainedNumber((float)wages, (StringBuilder)null);
            if (mobileParty.CurrentSettlement != null && mobileParty.CurrentSettlement.IsTown)
                PerkHelper.AddPerkBonusForTown(DefaultPerks.TwoHanded.ReducedWage, mobileParty.CurrentSettlement.Town, ref bonuses);
            if (mobileParty.IsCaravan && mobileParty.LeaderHero != null && (mobileParty.Party.Owner.Clan.Leader != null && mobileParty.Party.Owner.Clan.Leader.GetPerkValue(DefaultPerks.Trade.CaravanMaster)))
                PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Trade.CaravanMaster, mobileParty.Party.Owner.Clan.Leader.CharacterObject, ref bonuses);
            return (int)bonuses.ResultNumber;
        }
    }
}
