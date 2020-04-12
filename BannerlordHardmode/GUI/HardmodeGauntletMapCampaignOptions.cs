using System;
using TaleWorlds.MountAndBlade.View.Missions;
using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.InputSystem;
using TaleWorlds.Engine.Screens;

namespace BannerlordHardmode.GUI
{
    [OverrideView(typeof(MapCampaignOptions))]
    public class HardmodeGauntletMapCampaignOptions : MapView
    {
        private HardmodeOptionsVM _dataSource;
        private GauntletLayer _layer;

        protected override void CreateLayout()
        {
            base.CreateLayout();
            this._dataSource = new HardmodeOptionsVM(new Action(this.OnClose));
            GauntletLayer gauntletLayer = new GauntletLayer(4401, "GauntletLayer");
            gauntletLayer.IsFocusLayer = true;
            this._layer = gauntletLayer;
            this._layer.LoadMovie("CampaignOptions", (ViewModel)this._dataSource);
            this._layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            this._layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            this.MapScreen.AddLayer((ScreenLayer)this._layer);
            this.MapScreen.PauseAmbientSounds();
            ScreenManager.TrySetFocus((ScreenLayer)this._layer);
        }

        private void OnClose()
        {
            MapScreen.Instance.CloseCampaignOptions();
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            if (this._layer.Input.IsHotKeyReleased("Exit"))
                this.OnClose();
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            this._layer.InputRestrictions.ResetInputRestrictions();
            this.MapScreen.RemoveLayer((ScreenLayer)this._layer);
            this.MapScreen.RestartAmbientSounds();
            ScreenManager.TryLoseFocus((ScreenLayer)this._layer);
            this._layer = (GauntletLayer)null;
            this._dataSource = (HardmodeOptionsVM)null;
        }
    }
}
