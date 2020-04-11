using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace BannerlordHardmode
{
    class HardmodeDifficultyModel : DefaultDifficultyModel
    {
        public override float GetDamageToPlayerMultiplier()
        {
            return 2f;
        }

        public override float GetDamageToFriendsMultiplier()
        {
            return 1f;
        }

        public override float GetPlayerTroopsReceivedDamageMultiplier()
        {
            return 1f;
        }
    }
}
