using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace BannerlordHardmode
{
    class HardmodeDifficultyModel : DefaultDifficultyModel
    {
        public override float GetDamageToPlayerMultiplier()
        {
            return 2f;
        }
    }
}
