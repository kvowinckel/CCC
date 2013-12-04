using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Scenery;
using NOVA.UI;
using NOVA.Graphics;
using Microsoft.Xna.Framework;

namespace Crazy_Castle_Crush
{
    class Objekte
    {
        public Objekte(Scene scene)
        {
            this.scene = scene;
        }

        public void Geldanzeige(Spieler spieler)
        {
            
            string aktuellerText = "$" + spieler.getMoney();
            UI2DRenderer.WriteText(new Vector2(scene.Camera.Position.X + 2, scene.Camera.Position.Y),           //Position
                        aktuellerText,                                                                          //Anzuzeigender Text
                        Color.Red,                                                                              //Textfarbe
                        null,                                                                                   //Interne Schriftart verwenden
                        Vector2.One,                                                                            //Textskallierung
                        UI2DRenderer.HorizontalAlignment.Left,                                                  //Horizontal zentriert
                        UI2DRenderer.VerticalAlignment.Top);                                                    //am unteren Bildschirmrand ausrichten

        }

        public BoxObject createObj(int auswahl)
        {
            if (auswahl == 1)
            {
                return buildbox(new Vector3(0.5f, 0.5f, 0.5f));
            }
            else
            {
                return buildbox(new Vector3(5f, 0.5f, 0.5f));
            }
        }

        private BoxObject buildbox(Vector3 dimension)
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box = new BoxObject(new Vector3(0,-10,-6f),             //Position
                               dimension,                          //Kantenlängen
                               0f);
            scene.Add(box);

            return box;
        }

        private Scene scene;
    }
}
