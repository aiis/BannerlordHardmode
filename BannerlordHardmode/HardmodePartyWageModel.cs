using System.Text;
using Helpers;
using TaleWorlds.CampaignSystem;
using StoryMode.GameModels;
using TaleWorlds.Library;

namespace BannerlordHardmode
{
    class HardmodePartyWageModel : StoryModePartyWageModel
    {
        public override int GetTotalWage(MobileParty mobileParty, StatExplainer explanation = null)
        {
            int lowtierTroops = 0;
            int midtierTroops = 0;
            int hightierTroops = 0;
            for (int index = 0; index < mobileParty.MemberRoster.Count; ++index)
            {
                TroopRosterElement partyMember = mobileParty.MemberRoster.GetElementCopyAtIndex(index);
                if (partyMember.Character != mobileParty.Party.Owner.CharacterObject)
                {
                    if (partyMember.Character.Tier < 4)
                        lowtierTroops += partyMember.Character.TroopWage * partyMember.Number;
                    else if (partyMember.Character.Tier == 4)
                        midtierTroops += partyMember.Character.TroopWage * partyMember.Number;
                    else if (partyMember.Character.Tier > 4)
                        hightierTroops += partyMember.Character.TroopWage * partyMember.Number;
                }
            }
            if (mobileParty.HasPerk(DefaultPerks.Leadership.LevySergeant))
            {
                ExplainedNumber stat = new ExplainedNumber(1f, (StringBuilder)null);
                explanation?.AddLine(DefaultPerks.Leadership.LevySergeant.Name.ToString(), (float)(lowtierTroops + midtierTroops) - (float)(lowtierTroops + midtierTroops) * stat.ResultNumber, StatExplainer.OperationType.Add);
                PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.LevySergeant, mobileParty, ref stat);
                lowtierTroops = MathF.Round(stat.ResultNumber * (float)lowtierTroops);
                midtierTroops = MathF.Round(stat.ResultNumber * (float)midtierTroops);
            }
            if (mobileParty.HasPerk(DefaultPerks.Leadership.VeteransRespect))
            {
                ExplainedNumber stat = new ExplainedNumber(1f, (StringBuilder)null);
                explanation?.AddLine(DefaultPerks.Leadership.VeteransRespect.Name.ToString(), (float)(midtierTroops + hightierTroops) - (float)(midtierTroops + hightierTroops) * stat.ResultNumber, StatExplainer.OperationType.Add);
                PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.VeteransRespect, mobileParty, ref stat);
                midtierTroops = MathF.Round(stat.ResultNumber * (float)midtierTroops);
                hightierTroops = MathF.Round((float)hightierTroops * stat.ResultNumber);
            }

            if (mobileParty.IsMainParty)
            {
                // main party wages x 5
                lowtierTroops *= 5;
                midtierTroops *= 5;
                hightierTroops *= 5;
            }
            return (int)((double)(lowtierTroops + midtierTroops + hightierTroops) * (mobileParty.LeaderHero == null || mobileParty.LeaderHero.Clan.Kingdom == null || (mobileParty.LeaderHero.Clan.IsUnderMercenaryService || !mobileParty.LeaderHero.Clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.MilitaryCoronae)) ? 1.0 : 1.10000002384186));
        }
    }
}
