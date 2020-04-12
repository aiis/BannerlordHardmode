using System;
using System.Windows.Forms;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using HarmonyLib;

namespace BannerlordHardmode
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage("Hardmode enabled", Color.White));
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                var harmony = new Harmony("mod.aiis.bannerlord.hardmode");
                harmony.PatchAll();
            } catch (Exception ex)
            {
                MessageBox.Show($"Error starting Bannerlord Hardmode:\n{ex.ToString()}");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            AddModels(gameStarterObject as CampaignGameStarter);
            AddBehaviors(gameStarterObject as CampaignGameStarter);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            CampaignEvents.RemoveListeners((object)Campaign.Current.GetCampaignBehavior<DesertionCampaignBehavior>());
        }

        private void AddBehaviors(CampaignGameStarter gameStarter)
        {
            gameStarter.AddBehavior(new HardmodeDesertionCampaignBehavior());
        }

        private void AddModels(CampaignGameStarter gameStarter)
        {
            if (gameStarter != null)
            {
                gameStarter.AddModel(new HardmodeDifficultyModel());
                gameStarter.AddModel(new HardmodePartyMoraleModel());
                gameStarter.AddModel(new HardmodeMobilePartyFoodConsumptionModel());
                gameStarter.AddModel(new HardmodeClanFinanceModel());
                gameStarter.AddModel(new HardmodePlayerCaptivityModel());
            }
        }
    }
}