using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;


namespace BannerlordHardmode
{
    // Purpose: control rewards
    // [HarmonyPatch(typeof(MapEventSide), "ApplyRewardsAndChanges")]
    class ApplyRewardsAndChangesPatch
    {
        static bool Prefix(ref MapEventSide __instance)
        {
            FieldInfo fBattleParties = typeof(MapEventSide).GetField("_battleparties", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MapEventParty mapEventParty in ((Dictionary<PartyBase, MapEventParty>)fBattleParties.GetValue(__instance)).Values)
            {
                PartyBase party = mapEventParty.Party;
                Hero hero = party == PartyBase.MainParty ? Hero.MainHero : party.LeaderHero;
                if (party.MobileParty != null)
                    party.MobileParty.RecentEventsMorale += mapEventParty.MoraleChange;
                if (hero != null)
                {
                    if ((double)mapEventParty.GainedRenown > 1.0 / 1000.0)
                        GainRenownAction.Apply(hero, mapEventParty.GainedRenown, true);
                    if ((double)mapEventParty.GainedInfluence > 1.0 / 1000.0)
                        GainKingdomInfluenceAction.ApplyForBattle(hero, mapEventParty.GainedInfluence);
                }
                if (hero != null)
                {
                    if ((double)mapEventParty.PlunderedGold > 1.0 / 1000.0)
                    {
                        if (hero == Hero.MainHero)
                        {
                            MBTextManager.SetTextVariable("GOLD", mapEventParty.PlunderedGold, false);
                            InformationManager.AddQuickInformation(GameTexts.FindText("str_plunder_gain_message", (string)null), 0, (BasicCharacterObject)null, "");
                        }
                        GiveGoldAction.ApplyBetweenCharacters((Hero)null, hero, mapEventParty.PlunderedGold, true);
                    }
                    if ((double)mapEventParty.GoldLost > 1.0 / 1000.0)
                        GiveGoldAction.ApplyBetweenCharacters(hero, (Hero)null, mapEventParty.GoldLost, true);
                }
                else if (mapEventParty.Party.IsMobile && mapEventParty.Party.MobileParty.IsPartyTradeActive)
                {
                    mapEventParty.Party.MobileParty.PartyTradeGold -= mapEventParty.GoldLost;
                    mapEventParty.Party.MobileParty.PartyTradeGold += mapEventParty.PlunderedGold;
                }
            }
            return true;
        }
    }
}
