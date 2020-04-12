using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace BannerlordHardmode.Actions
{
    public static class ChangeRenown
    {
        public static void Apply(Hero hero, float delta, float floor)
        {
            StatisticsDataLogHelper.AddLog(StatisticsDataLogHelper.LogAction.GainRenownAction);

            // TODO do qualifying check
            if (delta >= 0.1f | delta <= -0.1f)
                if (hero.Clan.Renown+delta > 0)
                {
                    if (hero.Clan.Renown + delta > floor)
                    {
                        hero.Clan.Renown += delta;
                    } else
                    {
                        delta = hero.Clan.Renown - floor;
                        hero.Clan.Renown = floor;
                    }

                    if (hero == Hero.MainHero)
                    {
                        // notify user of renown change
                        MethodInfo mAddInformationData = typeof(CampaignInformationManager).GetMethod("AddInformationData", BindingFlags.NonPublic | BindingFlags.Instance);
                        mAddInformationData.Invoke(Campaign.Current.CampaignInformationManager, new object[1] { new LogEntries.PlayerClanStatChangeLogEntry("renown", delta) });
                    }

                    // Change clan tier if needed
                    int tier = Campaign.Current.Models.ClanTierModel.CalculateTier(hero.Clan);
                    if (tier != hero.Clan.Tier)
                    {
                        ChangeClanTier.Apply(hero.Clan, tier);
                    }
                }
        }
    }
}
