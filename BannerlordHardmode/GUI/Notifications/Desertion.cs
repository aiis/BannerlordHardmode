using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordHardmode.GUI.Notifications
{
    class Desertion
    {
        public static void Show(Hero hero, int numberOfDeserters)
        {
            String grammaticalNumber = numberOfDeserters == 1 ? "soldier has" : "soldiers have";
            String msg = $"{numberOfDeserters.ToString()} {grammaticalNumber} deserted from {hero.Name}'s party. Word of this will surely spread.";
            InformationManager.DisplayMessage(new InformationMessage(msg, Colors.Red));
        }
    }
}
