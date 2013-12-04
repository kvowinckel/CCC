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

        public SceneObject createObj(int auswahl)
        {
            if (auswahl == 1)// Würfel
            {
                return buildbox(new Vector3(0.5f, 0.5f, 0.5f));
            }

            else if (auswahl == 3) // Latte
            {
                return buildlatte();
            }
            else if (auswahl == 4)
            {
                return buildquader();
            }
            else return buildquader(); 
            
        }
        

        private BoxObject buildbox(Vector3 dimension) //Würfel
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box = new BoxObject(new Vector3(0f,0f,0f),        //Position
                               dimension,                               //Kantenlängen
                               1f);                                     //Masse
            scene.Add(box);

            return box;
        }
        private BoxObject buildlatte() //Latte
        {
            BoxObject latte = new BoxObject(new Vector3(0, 0, 0), //Position
                              new Vector3(4, 1, 1),               //Kantenlänge der Latte
                              1f);                                //Masse
            scene.Add(latte);   
            return latte;                                       
        }
        private BoxObject buildquader()
        {
            BoxObject quader = new BoxObject(new Vector3(0, 0, 0),
                               new Vector3(1f, 2f, 1f),
                               1f);
            scene.Add(quader);
            return quader;
        }

        private Scene scene;
    }
}
