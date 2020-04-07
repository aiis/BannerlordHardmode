using System;
using System.Windows.Forms;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;

namespace BannerlordHardmode
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleName = "zBannerlordHardmode";
        private bool _isLoaded;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                var harmony = new Harmony("mod.bannerlord.aiis.hardmode");
                harmony.PatchAll();
                //MessageBox.Show("Patched succesfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting BannerlordHardmode:\n\n{ex.ToString()}");
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (this._isLoaded)
                return;
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage("Hardmode enabled", Color.White));
            this._isLoaded = true;
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            try
            {
                AddModels(gameStarterObject as CampaignGameStarter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading models for BannerlordHardmode:\n\n{ex.ToString()}");
            }
        }

        private void AddModels(CampaignGameStarter gameStarter)
        {
            if (gameStarter != null)
            {
                gameStarter.AddModel(new HardmodeDifficultyModel());
                gameStarter.AddModel(new HardmodePartyMoraleModel());
                gameStarter.AddModel(new HardmodeMobilePartyFoodConsumptionModel());
                //gameStarter.AddModel(new HardmodePartyWageModel());
                gameStarter.AddModel(new HardmodeClanFinanceModel());
            }
        }
    }
}