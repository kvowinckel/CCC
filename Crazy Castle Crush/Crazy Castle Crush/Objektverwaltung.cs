using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Scenery;
using NOVA.Graphics;
using Microsoft.Xna.Framework;
using NOVA.UI;
using NOVA;

namespace Crazy_Castle_Crush
{
    class Objektverwaltung
    {
        public Objektverwaltung(Scene scene)
        {
            this.scene = scene;
        }

        private List<Objekte> objListe = new List<Objekte>();

        public Objekte createObj(int auswahl, Spieler spieler)
        {
            SceneObject newobj;
            Objekte dasobj;
            int lp;

            if (auswahl == 1)//Würfel
            {
                newobj = buildbox(new Vector3(0.5f, 0.5f, 0.5f));
                lp = 2;
                spieler.setMoney(spieler.getMoney() - 200); //Rohkosten abziehen
            }
            else if (auswahl == 2)
            {
                ModelObject l = new ModelObject(Vector3.Zero, Quaternion.CreateFromYawPitchRoll(1.57f, 0, 0), new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "L", 0f);
                newobj = l;
                lp = 2;
                spieler.setMoney(spieler.getMoney() - 250);
            }
            else if (auswahl == 3) // Latte
            {
                newobj = buildbox(new Vector3(2f, 0.2f, 0.5f));
                lp = 1;
                spieler.setMoney(spieler.getMoney() - 100); //Rohkosten abziehen
            }
            else if (auswahl == 4)
            {
                newobj = buildbox(new Vector3(0.5f, 1f, 0.5f));
                lp = 2;
                spieler.setMoney(spieler.getMoney() - 300); //Rohkosten abziehen
            }
            else
            {
                newobj = buildbox(new Vector3(0.1f, 0.1f, 0.1f));
                lp = 0;
            }

            scene.Add(newobj);
            dasobj = new Objekte(newobj, lp, "blank");
            objListe.Add(dasobj); //Liste hinzufügen
            return dasobj;
        }

        public void firstMaterial(Objekte first, int auswahl)
        {
            if (auswahl == 1)
            {
                first.setMaterial("MHolz");
                first.increaseLP(2); //Lebenspunkte um 2 erhöhen
                
            }
            else if (auswahl == 2)
            {
                first.setMaterial("MStein");
                first.increaseLP(1); //Lebenspunkte um 1 erhöhen
                
            }
            else if (auswahl == 3)
            {
                first.setMaterial("Papayrus");
                first.increaseLP(2); //Lebenspunkte um 2 erhöhen
                
            }
            else if (auswahl == 4)
            {
                first.setMaterial("himmel");
                first.increaseLP(0); //Lebenspunkte nicht erhöhen
                
            }
        }

        private BoxObject buildbox(Vector3 dimension)
        {
            BoxObject box = new BoxObject(new Vector3(0, 0, 0), //Position
                    dimension,               //Kantenlänge der Latte
                    0f);

            return box;
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

        private Scene scene;
    }
}
