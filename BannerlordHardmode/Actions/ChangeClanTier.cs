using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace BannerlordHardmode.Actions
{
    public static class ChangeClanTier
    {
        public static void Apply(Clan clan, int tier)
        {
            FieldInfo fTier = typeof(Clan).GetField("_tier", BindingFlags.NonPublic | BindingFlags.Instance);
            int minClanTier = Campaign.Current.Models.ClanTierModel.MinClanTier;
            int maxClanTier = Campaign.Current.Models.ClanTierModel.MaxClanTier;
            if (tier > maxClanTier)
                tier = maxClanTier;
            else if (tier < minClanTier)
                tier = minClanTier;

            fTier.SetValue(clan, tier);

            // TODO notify user of clan tier change (if player's clan)
        }
    }
}
