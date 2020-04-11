using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.Core;

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
                        if (delta > 0)
                        {
                            TextObject text = new TextObject($"{hero.Name} has gained {Convert.ToInt32(delta)} renown");
                            InformationManager.AddQuickInformation(text);
                        } else
                        {
                            TextObject text = new TextObject($"{hero.Name} has lost {Math.Abs(Convert.ToInt32(delta))} renown");
                            InformationManager.AddQuickInformation(text);
                        }
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
