using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using BannerlordHardmode.Actions;

namespace BannerlordHardmode.patches
{
    [HarmonyPatch(typeof(MobilePartyHelper), "DesertTroopsFromParty")]
    class DesertTroopsFromPartyPatch
    {
        static void Postfix(MobileParty party, int stackNo, int numberOfDeserters, int numberOfWoundedDeserters, ref TroopRoster desertedTroopList)
        {
            if (party.IsMainParty) {
                Hero hero = Hero.MainHero;
                if (hero != null)
                {
                    ChangeRenown.Apply(hero, -1f);
                }
            }
        }
    }
}
