using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace BannerlordHardmode.GUI
{
    class HardmodeOptionsVM : ViewModel
    {
        private readonly Action _onClose;
        private MBBindingList<CampaignOptionItemVM> _options;
        private string _titleText;

        public HardmodeOptionsVM(Action onClose)
        {
            this._onClose = onClose;
            MBBindingList<CampaignOptionItemVM> mbBindingList = new MBBindingList<CampaignOptionItemVM>();
            mbBindingList.Add(new CampaignOptionItemVM("MaximumIndexPlayerCanRecruit"));
            mbBindingList.Add(new CampaignOptionItemVM("PlayerMapMovementSpeed"));
            mbBindingList.Add(new CampaignOptionItemVM("AutoAllocateClanMemberPerks"));
            this.Options = mbBindingList;
            this.RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            this.TitleText = new TextObject("Hardmode Options", (Dictionary<string, TextObject>)null).ToString();
            this.Options.ApplyActionOnAllItems((Action<CampaignOptionItemVM>)(o => o.RefreshValues()));
        }

        private void ExecuteDone()
        {
            Action onClose = this._onClose;
            if (onClose == null)
                return;
            onClose();
        }

        [DataSourceProperty]
        public MBBindingList<CampaignOptionItemVM> Options
        {
            get
            {
                return this._options;
            }
            set
            {
                if (value == this._options)
                    return;
                this._options = value;
                this.OnPropertyChanged(nameof(Options));
            }
        }

        [DataSourceProperty]
        public string TitleText
        {
            get
            {
                return this._titleText;
            }
            set
            {
                if (!(value != this._titleText))
                    return;
                this._titleText = value;
                this.OnPropertyChanged(nameof(TitleText));
            }
        }

    }
}
