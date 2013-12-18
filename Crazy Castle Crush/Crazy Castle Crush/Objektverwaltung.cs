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
    static public class Objektverwaltung
    {
        static private Scene scene;
        static private Levels level;
        static int idnummer = 0;
        static int waffenid = 0;
        static int anzWaffen = 0;

        public static void setObjektverwaltung(Scene scene0, Levels level0)
        {
            scene = scene0;
            level = level0;
        }

        private static List<Objekte> objListe = new List<Objekte>();
        private static List<SceneObject> umgebungsListe = new List<SceneObject>();
        private static List<Waffen> waffenListe = new List<Waffen>();

        public static Objekte createObj(int auswahl, Spieler spieler, float xPos)
        {
            idnummer++;
            Vector3 startort = new Vector3(xPos, 2, -5);
            SceneObject newobj;
            Objekte dasobj;

            if (auswahl == 1)//Würfel
            {
                newobj = buildbox(startort, new Vector3(0.5f, 0.5f, 0.5f));
                spieler.setMoney(spieler.getMoney() - 150); //Rohkosten abziehen
                newobj.Physics.Mass = 1f;
            }
            else if (auswahl == 2)
            {
                ModelObject l = new ModelObject(startort, Quaternion.CreateFromAxisAngle(new Vector3(1,2,0),(float)Math.PI), new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "L", 1f);
                newobj = l;
                spieler.setMoney(spieler.getMoney() - 200);
            }
            else if (auswahl == 3) // Latte
            {
                newobj = buildbox(startort, new Vector3(2f, 0.2f, 0.5f));
                spieler.setMoney(spieler.getMoney() - 200); //Rohkosten abziehen
                newobj.Physics.Mass = 1f;
            }
            else if (auswahl == 4) //Pyramide
            {
                ModelObject p = new ModelObject(startort, Quaternion.CreateFromAxisAngle(new Vector3(1, 2, 0), (float)Math.PI), new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "Pyramide", 1f);
                newobj = p;
                spieler.setMoney(spieler.getMoney() - 200);
            }
            else
            {
                newobj = buildbox(startort, new Vector3(0.1f, 0.1f, 0.1f));
            }

            //TODO z-Achse sperren
            newobj.Physics.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
            newobj.Tag = idnummer;
            newobj.PhysicsMaterial.Bounciness = 0.2f;
            scene.Add(newobj);
            dasobj = new Objekte(newobj, 1, "blank");
            objListe.Add(dasobj); //Liste hinzufügen
            return dasobj;
        }

        public static Waffen createWaffe(int auswahl, Spieler spieler, float xPos)//TODO
        {
            waffenid++;
            anzWaffen++;
            spieler.setWaffen(waffenid);    //Waffe wird Spieler zugeordnet
            SceneObject newobj;
            Waffen dasobj;

            //if(auswahl==1){ TEMPORÄR BOX ALS WAFFE
            newobj = buildbox(new Vector3(xPos, 2, -5f), Vector3.One);

            //TODO z-Achse sperren
            newobj.Tag = waffenid;
            scene.Add(newobj);
            dasobj = new Waffen(newobj, 1, "MStein", (float)Math.PI / 4, 5f);
            waffenListe.Add(dasobj);
            return dasobj;

        }

        public static Waffen getWaffe(Spieler spieler, int firedwappons)
        {
            //firedwappons = 0 ==> erste Waffe
            int[] waffenids = spieler.getList().ToArray();
            int Waffenid = waffenids[firedwappons];

            foreach (Waffen w in waffenListe)
            {
                if (w.getSceneObject().Tag.Equals(Waffenid))
                {
                    return w;
                }
            }

            return null;
        }

        public static Objekte projektil(int id, Vector3 startpos, float winkel) //TODO
        {
            SceneObject newobj;
            Objekte dasobj;

            if (id == 1) 
            {
                newobj = buildbox(startpos, Vector3.One);
            }
            else
            {
                newobj = buildbox(startpos, Vector3.One);
            }

            dasobj = new Objekte(newobj, 0, "blank");

            return dasobj;
        }

        public static void orientObj(Objekte obj, float linkeHX, float linkeHY)
        {
            
            Vector2 rechteH = new Vector2(obj.getPosition().X, obj.getPosition().Y);
            Vector2 zero = new Vector2(0, 1);
            Vector2 ausrichtung = new Vector2(rechteH.X - linkeHX, rechteH.Y - linkeHY);
            ausrichtung.Normalize();

            double radian = Math.Atan2(ausrichtung.Y, ausrichtung.X);


            obj.getSceneObject().Orientation = Quaternion.CreateFromAxisAngle(new Vector3(0,0,1),(float)radian);
        }

        public static void firstMaterial(Objekte first, int auswahl)
        {
            first.getSceneObject().Physics.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Discrete;
            
            if (auswahl == 1)
            {
                first.setMaterial("MHolz");                
            }
            else if (auswahl == 2)
            {
                first.setMaterial("MStein");
                first.increaseLP(1); 
                
            }
            else if (auswahl == 3)
            {
                first.setMaterial("Papayrus");
                first.increaseLP(2); 
                
            }
            else if (auswahl == 4)
            {
                first.setMaterial("himmel");
            }
        }

        private static BoxObject buildbox(Vector3 startort, Vector3 dimension)
        {
            BoxObject box = new BoxObject(startort, //Position
                    dimension,               //Kantenlänge der Latte
                    1f);

            return box;
        }

        public static void Geldanzeige(Spieler spieler)
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

        public static void refreshObj(Spieler spieler1, Spieler spieler2)
        {
            foreach (Objekte temp in objListe)
            {
                if (temp.getLP() <= 0)
                {
                    scene.Remove(temp.getSceneObject()); //Löscht Objekte aus Scene
                }
            }

            objListe.RemoveAll(x => x.getLP() <= 0); //Löscht Objekte aus Liste

            foreach (Waffen temp in waffenListe)
            {
                if (temp.getLP() <= 0)
                {
                    scene.Remove(temp.getSceneObject());
                    anzWaffen--;
                    foreach (int ids in spieler1.getList())
                    {
                        if (temp.getSceneObject().Tag.Equals(ids))
                        {
                            spieler1.resetWaffen(ids);
                        }
                    }
                    foreach (int ids in spieler2.getList())
                    {
                        if (temp.getSceneObject().Tag.Equals(ids))
                        {
                            spieler2.resetWaffen(ids);
                        }
                    }

                }
            }
            waffenListe.RemoveAll(x => x.getLP() <= 0);
        }

        public static void addToUmgebungsListe(SceneObject obj)
        {
            umgebungsListe.Add(obj);
        }

    }
}
