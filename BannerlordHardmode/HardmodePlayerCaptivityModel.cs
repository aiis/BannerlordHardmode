using System;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.CampaignSystem;

namespace BannerlordHardmode
{
    class HardmodePlayerCaptivityModel : DefaultPlayerCaptivityModel
    {
        private float lastApplied = 0;
        public override string CheckCaptivityChange(float dt)
        {
            if (Campaign.CurrentTime - lastApplied == 24f | lastApplied == 0)
            {
                Actions.ChangeInfluence.Apply(Hero.MainHero, -Math.Abs(Hero.MainHero.Clan.Influence * 0.25f));
                Actions.ChangeRenown.Apply(Hero.MainHero, -(Hero.MainHero.Clan.Renown * 0.15f), Convert.ToSingle(Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(Hero.MainHero.Clan.Tier)));
                lastApplied = Campaign.CurrentTime;
            }
            return base.CheckCaptivityChange(dt);
        }
    }
}
