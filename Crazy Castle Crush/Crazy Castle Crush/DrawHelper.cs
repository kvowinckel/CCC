using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NOVA.UI;

namespace Crazy_Castle_Crush
{
    class DrawHelper
    {
        private static int change;
        private static Vector2 from;
        private static int prozent;

        static public void run()
        {
            if (prozent>0)
            {
                Color farbe = (change > 0) ? Color.Green : Color.Red;
                String text = (change > 0) ? "+" + change.ToString() : change.ToString();
                Vector2 pos = from * prozent / 100;
                Vector2 dim = new Vector2(0.05f * prozent, 0.05f * prozent);
                drawText(text, farbe, pos, dim);
                prozent--;
            }

        }

        public static void setmoney(Spieler spieler, int money, Vector2 pos)
        {
            spieler.setMoney(spieler.getMoney() + money);
            change = money;
            from = pos;
            prozent = 100;
        }

        private static void drawText(String text, Color farbe, Vector2 position, Vector2 dimension)
        {
            UI2DRenderer.WriteText(position, text, farbe, null, dimension);
        }
    }
}
