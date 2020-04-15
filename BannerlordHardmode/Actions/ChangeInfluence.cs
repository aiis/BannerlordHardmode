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
                GUI.Notifications.ClanStatChanged.Show("influence", delta);
            }
        }
    }
}
