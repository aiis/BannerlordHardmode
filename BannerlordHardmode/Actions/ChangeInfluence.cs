using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.Core;

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
                TextObject text = new TextObject($"{hero.Name}'s influence has changed from {Convert.ToInt32(oldInfluence)} to {Convert.ToInt32(hero.Clan.Influence)}");
                InformationManager.AddQuickInformation(text);
            }
        }
    }
}
