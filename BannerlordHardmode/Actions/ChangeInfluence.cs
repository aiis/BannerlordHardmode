using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace BannerlordHardmode.Actions
{
    class ChangeInfluence
    {
        public static void Apply(Hero hero, float delta)
        {
            StatisticsDataLogHelper.AddLog(StatisticsDataLogHelper.LogAction.GainKingdomInfluenceAction);
            float oldInfluence = hero.Clan.Influence;
            hero.Clan.Influence += delta;
            if (hero == Hero.MainHero)
            {
                // notify user
                MethodInfo mAddInformationData = typeof(CampaignInformationManager).GetMethod("AddInformationData", BindingFlags.NonPublic | BindingFlags.Instance);
                mAddInformationData.Invoke(Campaign.Current.CampaignInformationManager, new object[1] { new LogEntries.PlayerClanStatChangeLogEntry("influence", delta) });
            }
        }
    }
}
