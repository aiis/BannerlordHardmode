using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;


namespace BannerlordHardmode.LogEntries
{
    class DesertionLogEntry : LogEntry, IChatNotification
    {
        Hero hero;
        int numberOfDeserters;
        public DesertionLogEntry(Hero hero, int numberOfDeserters)
        {
            this.hero = hero;
            this.numberOfDeserters = numberOfDeserters;
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
                return ChatNotificationType.Civilian;
            }
        }

        public TextObject GetNotificationText()
        {
            String grammaticalNumber = this.numberOfDeserters == 1 ? "soldier has" : "soldiers have";
            TextObject parent = new TextObject($"{this.numberOfDeserters.ToString()} {grammaticalNumber} deserted from {this.hero.Name}'s party. Word of this will surely spread.");
            return parent;
        }
    }
}
