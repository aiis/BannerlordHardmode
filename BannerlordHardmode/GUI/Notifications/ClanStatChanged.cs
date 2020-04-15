using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordHardmode.GUI.Notifications
{
    class ClanStatChanged
    {
        public static void Show(String name, int delta)
        {
            String verb;
            Color color;

            if (delta > 0)
            {
                verb = "gained";
                color = Colors.Green;
            }
            else
            {
                verb = "lost";
                color = Colors.Red;
            }
            String msg = $"Your clan has {verb} {Math.Abs(delta).ToString()} {name}";
            InformationManager.DisplayMessage(new InformationMessage(msg, color));
        }

        public static void Show(String name, float delta)
        {
            Show(name, Convert.ToInt32(delta));
        }
    }
}
