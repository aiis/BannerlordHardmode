using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem;

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

        public override float GetPlayerMapMovementSpeedBonusMultiplier()
        {
            switch (CampaignOptions.PlayerMapMovementSpeed)
            {
                case CampaignOptions.Difficulty.VeryEasy:
                    return 1f;
                case CampaignOptions.Difficulty.Easy:
                    return .5f;
                case CampaignOptions.Difficulty.Realistic:
                    return 0.0f;
                default:
                    return 0.0f;
            }
        }
    }

    public enum HardmodeDifficulty
    {
        VeryEasy,
        Easy,
        Realistic,
        Hardmode,
    }
}
