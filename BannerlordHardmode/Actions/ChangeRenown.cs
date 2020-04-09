using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace BannerlordHardmode.Actions
{
    public static class ChangeRenown
    {
        public static void Apply(Hero hero, float delta)
        {
            StatisticsDataLogHelper.AddLog(StatisticsDataLogHelper.LogAction.GainRenownAction);

            // TODO do qualifying check
            if (delta >= 0.1f | delta <= -0.1f)
                if (hero.Clan.Renown+delta > 0)
                {
                    hero.Clan.Renown += delta;
                    // notify user of renown change
                    MethodInfo mOnRenownGained = typeof(DefaultNotificationsCampaignBehavior).GetMethod("OnRenownGained", BindingFlags.NonPublic | BindingFlags.Instance);
                    mOnRenownGained.Invoke(Campaign.Current.GetCampaignBehavior<DefaultNotificationsCampaignBehavior>(), new object[3] { hero, Convert.ToInt32(delta), false });

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
