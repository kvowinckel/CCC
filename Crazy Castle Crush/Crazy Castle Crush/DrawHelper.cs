using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NOVA.UI;
using Microsoft.Xna.Framework.Graphics;
using NOVA;

namespace Crazy_Castle_Crush
{
    class DrawHelper
    {
        private static int change;
        private static Vector2 from;
        private static int prozent;

        static public void run(Gamestart.States state, Vector2 rHv2s, Vector2 lHv2s, Vector2 screenDim, bool showWaffe, Spieler spieler1, Spieler spieler2, bool bulletInAir)
        {
            #region Bau/Tex/Waf - Auswahl
            if (state == Gamestart.States.Bauphase1O || state == Gamestart.States.Bauphase1T || state == Gamestart.States.Bauphase2O || state == Gamestart.States.Bauphase2T)
            {
                String bild;
                if (state == Gamestart.States.Bauphase1T || state == Gamestart.States.Bauphase2T)
                {
                    bild = "Material";
                }
                else
                {
                    if (showWaffe)
                    {
                        bild = "WaffenMenüDummy";
                    }
                    else
                    {
                        bild = "Bau";
                    }
                }
                float rny = rHv2s.Y / screenDim.Y;
                float rnx = rHv2s.X / screenDim.X;
                double yrun;
                if (rnx > 0.25f && rnx < 0.75f)
                {
                    if (rny > 0.3f)
                    {
                        yrun = 0;
                    }
                    else if (rny < 0.2f)
                    {
                        yrun = 0.105f * screenDim.Y;
                    }
                    else
                    {
                        yrun = (0.3f - rny) * 1.05f * screenDim.Y;
                    }
                }
                else
                {
                    yrun = 0;
                }
                Vector2 dim = new Vector2(screenDim.X * 0.5f, screenDim.Y * 0.15f);
                Vector2 pos = new Vector2(dim.X * 0.5f, (float)yrun);
                drawBox(pos, dim, bild);

            }
            #endregion

            #region Geldanzeige
            if (state == Gamestart.States.Bauphase1O || state == Gamestart.States.Bauphase1T || state == Gamestart.States.Schussphase1)
            {
                Vector2 pos = new Vector2(1, 1);
                Vector2 dim = screenDim / 800;
                string aktuellerText = "$" + spieler1.getMoney();
                drawText(aktuellerText, Color.Gold, pos, dim);
            }
            if (state == Gamestart.States.Bauphase2O || state == Gamestart.States.Bauphase2T || state == Gamestart.States.Schussphase2)
            {
                Vector2 pos = new Vector2(1, 1);
                Vector2 dim = screenDim / 800;
                string aktuellerText = "$" + spieler2.getMoney();
                drawText(aktuellerText, Color.Gold, pos, dim);
            }
            #endregion

            #region Obj-Waffen Switch
            if (state == Gamestart.States.Bauphase1O || state == Gamestart.States.Bauphase2O)
            {
                String bild;
                if (showWaffe)
                {
                    bild = "pist";
                }
                else
                {
                    bild = "ChangerDummy";
                }

                float rny = rHv2s.Y / screenDim.Y;
                float rnx = rHv2s.X / screenDim.X;
                double yrun;
                if (rnx > 0.75f)
                {
                    if (rny > 0.3f)
                    {
                        yrun = 0;
                    }
                    else if (rny < 0.2f)
                    {
                        yrun = 0.105f * screenDim.Y;
                    }
                    else
                    {
                        yrun = (0.3f - rny) * 1.05f * screenDim.Y;
                    }
                }
                else
                {
                    yrun = 0;
                }

                Vector2 dim = new Vector2(screenDim.X * 0.125f, screenDim.Y * 0.15f);
                Vector2 pos = new Vector2((screenDim.X * 0.875f) - 5, (float)yrun);
                drawBox(pos, dim, bild);
            }
            #endregion

            #region Weiterbutton
            if (state == Gamestart.States.Bauphase1O || state == Gamestart.States.Bauphase2O)
            {
                Vector2 dim = new Vector2((screenDim.X * 0.09f), (screenDim.Y * 0.09f));
                Vector2 pos = new Vector2(screenDim.X - dim.X - screenDim.X * 0.01f, screenDim.Y * 0.45f);
                drawBox(pos, dim, "weiter2");
            }
            #endregion

            #region Poweranzeige
            if ((state == Gamestart.States.Schussphase1 || state == Gamestart.States.Schussphase2) && !bulletInAir)
            {
                float pro;
                if (lHv2s.Y < 0.15f * screenDim.Y)
                {
                    pro = 0.15f * screenDim.Y;
                }
                else if (lHv2s.Y > 0.95f * screenDim.Y)
                {
                    pro = 0.95f * screenDim.Y;
                }
                else
                {
                    pro = lHv2s.Y;
                }

                Vector2 pos1 = new Vector2(screenDim.X * 0.01f, screenDim.Y * 0.15f);
                Vector2 pos2 = new Vector2(screenDim.X * 0.01f, pro);
                Vector2 dim1 = new Vector2(screenDim.X * 0.05f, pro);
                Vector2 dim2 = new Vector2(screenDim.X * 0.05f, screenDim.Y * 0.95f - pro);

                drawBox(pos1, dim1, Color.Green);
                drawBox(pos2, dim2, Color.Red);
            }
            #endregion

            #region Geldfliegen
            if (prozent > 0)
            {
                Color farbe = (change > 0) ? Color.Green : Color.Red;
                String text = (change > 0) ? "+" + change.ToString() : change.ToString();
                Vector2 pos = from * prozent / 100;
                Vector2 dim = new Vector2(0.05f * prozent, 0.05f * prozent);
                drawText(text, farbe, pos, dim);
                prozent--;
            }
            #endregion

            #region Handpos
            if (state == Gamestart.States.Bauphase1O || state == Gamestart.States.Bauphase1T || state == Gamestart.States.Bauphase2O || state == Gamestart.States.Bauphase2T)
            {
                Handkreise(rHv2s, lHv2s);
            }
            #endregion

        }

        public static void setmoney(Spieler spieler, int money, Vector2 pos)
        {
            spieler.setMoney(spieler.getMoney() + money);
            change = money;
            from = pos;
            prozent = 100;
        }

        private static void Handkreise(Vector2 posL, Vector2 posR)
        {
            Texture2D HandsmallL = Core.Content.Load<Texture2D>("HandCursorR");
            UI2DRenderer.DrawTexture(HandsmallL, new Vector2((int)posL.X, (int)posL.Y), 30, 30);

            Texture2D HandsmallR = Core.Content.Load<Texture2D>("HandCursorL");
            UI2DRenderer.DrawTexture(HandsmallR, new Vector2((int)posR.X, (int)posR.Y), 30, 30);
        }

        private static void drawBox(Vector2 pos, Vector2 dim, String bild)
        {
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)dim.X, (int)dim.Y);
            Texture2D texture = Core.Content.Load<Texture2D>(bild);
            UI2DRenderer.FillRectangle(rect, texture, Color.White);
        }

        private static void drawBox(Vector2 pos, Vector2 dim, Color color)
        {
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)dim.X, (int)dim.Y);
            UI2DRenderer.FillRectangle(rect, null, color);
        }

        private static void drawText(String text, Color farbe, Vector2 position, Vector2 dimension)
        {
            UI2DRenderer.WriteText(position, text, farbe, null, dimension);
        }
    }
}
