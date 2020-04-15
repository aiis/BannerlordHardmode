using System.Reflection;
using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Library;

namespace BannerlordHardmode
{
    class HardmodePartyHealingModel : DefaultPartyHealingModel
    {
        private static float _maxHealingRate = 1f;
        public override float GetDailyHealingHpForHeroes(MobileParty party, StatExplainer explanation = null)
        {
            float stat = base.GetDailyHealingHpForHeroes(party, explanation);
            if (party.IsMainParty)
            {
                if (party.IsMoving | party.Party.IsStarving)
                {
                    return 0.0f;
                } else if (party.CurrentSettlement != null | party.LastVisitedSettlement.GetTrackDistanceToMainAgent() <= 2.0f)
                {
                    // Don't like getting a stacktrace here but it's a hacky fix for now to keep from healing when tooltip calls this method
                    MethodBase mth = new StackTrace().GetFrame(1).GetMethod();
                    string mName = mth.Name;
                    if (mName == "ChangeHp")
                    {
                        // MobileParty.ChangeHP() which calls this fxn every in game hour, limits HP gain. So I'm manually calling a healing fxn too
                        MethodInfo mHealHeroes = typeof(MobileParty).GetMethod("HealHeroes", BindingFlags.NonPublic | BindingFlags.Instance);
                        mHealHeroes.Invoke(party, new object[1] { _maxHealingRate });
                    }
                    return _maxHealingRate;
                } else
                {
                    if (MathF.Floor(CampaignTime.Now.CurrentHourInDay) % 2 == 0)
                        return stat*0.2f;
                    return 0.0f;
                }
            }
            return stat;
        }

        public override float GetDailyHealingForRegulars(MobileParty party, StatExplainer explanation = null)
        {
            float stat = base.GetDailyHealingForRegulars(party, explanation);
            if (party.IsMainParty)
            {
                if (party.IsMoving | party.Party.IsStarving)
                {
                    return 0.0f;
                }
                else if (party.CurrentSettlement != null)
                {
                    // MobileParty.ChangeHP() which calls this fxn every in game hour, limits HP gain. So I'm manually calling a healing fxn too
                    MethodInfo mHealHeroes = typeof(MobileParty).GetMethod("HealRegulars", BindingFlags.NonPublic | BindingFlags.Instance);
                    mHealHeroes.Invoke(party, new object[1] { _maxHealingRate });
                    return _maxHealingRate;
                }
                else
                {
                    if (MathF.Floor(CampaignTime.Now.CurrentHourInDay) % 2 == 0)
                        return stat * 0.2f;
                    return 0.0f;
                }
            }
            return stat;
        }
    }
}
