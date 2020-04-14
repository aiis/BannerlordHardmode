using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BannerlordHardmode.LogEntries
{
    class PlayerClanStatChangeLogEntry : LogEntry, IChatNotification
    {
        private String _name;
        private int _delta;
        public PlayerClanStatChangeLogEntry(String name, float delta)
        {
            this._name = name;
            this._delta = Convert.ToInt32(delta);
        }

        public PlayerClanStatChangeLogEntry(String name, int delta)
        {
            this._name = name;
            this._delta = delta;
        }

        public bool IsVisibleNotification
        {
            get
            {
                return true;
            }
        }


        public override ChatNotificationType NotificationType
        {
            get
            {
                return this._delta > 0 ? ChatNotificationType.PlayerClanPositive : ChatNotificationType.PlayerClanNegative;
            }
        }

        public TextObject GetNotificationText()
        {
            String verb = this._delta > 0 ? "gained" : "lost";
            TextObject text = new TextObject($"Your clan has {verb} {Math.Abs(this._delta).ToString()} {this._name}");
            return text;
        }
    }
}
