using System;
using System.Windows.Forms;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordHardmode
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleName = "BannerlordHardmode";
        private bool _isLoaded;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                var harmony = new Harmony("mod.bannerlord.aiis");
                harmony.PatchAll();
                // MessageBox.Show("Patched succesfully");
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
            InformationManager.DisplayMessage(new InformationMessage("BannerlordHardmode loaded", Color.White));
            this._isLoaded = true;
        }
    }
}