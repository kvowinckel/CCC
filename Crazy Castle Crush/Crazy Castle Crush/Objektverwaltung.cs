﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Scenery;
using NOVA.Graphics;
using Microsoft.Xna.Framework;
using NOVA.UI;
using NOVA;
using BEPUphysics.Constraints.SolverGroups;

namespace Crazy_Castle_Crush
{
    static public class Objektverwaltung
    {
        static private Scene scene;
        static private Levels level;
        static int idnummer = 0;

        public static void setObjektverwaltung(Scene scene0, Levels level0)
        {
            scene = scene0;
            level = level0;
        }

        private static List<Objekte> objListe = new List<Objekte>();
        private static List<SceneObject> umgebungsListe = new List<SceneObject>();
        //private static List<Waffen> waffenListe = new List<Waffen>();

        public static Objekte createObj(int auswahl, Spieler spieler, float xPos, Vector2 rHv2s)
        {
            idnummer++;
            Vector3 startort = new Vector3(xPos, 2, -5);
            SceneObject newobj;
            Objekte dasobj;

            if (auswahl == 1)//Würfel
            {
                newobj = buildbox(startort, new Vector3(0.4f, 0.4f, 0.4f));
                DrawHelper.setmoney(spieler, -150, rHv2s);
                /*spieler.setMoney(spieler.getMoney() - 150); //Rohkosten abziehen
                Gamestart.setShowGeld(-150, 100);           //Kosten visualisieren
                */
                newobj.Physics.Mass = 1f;
            }
            else if (auswahl == 2)
            {
                ModelObject l = new ModelObject(startort, Quaternion.CreateFromAxisAngle(new Vector3(1,2,0),(float)Math.PI), new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "L", 1f);
                l.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                l.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                newobj = l;
                DrawHelper.setmoney(spieler, -200, rHv2s);
            }
            else if (auswahl == 3) // Latte
            {
                newobj = buildbox(startort, new Vector3(1.2f, 0.1f, 0.4f));
                DrawHelper.setmoney(spieler, -200, rHv2s);
                newobj.Physics.Mass = 2f;
            }
            else if (auswahl == 4) // Quader       das kommentierte ist die //Pyramide
            {
                newobj = buildbox(startort, new Vector3(0.8f, 0.4f, 0.4f));
                newobj.Physics.Mass = 2f;
                DrawHelper.setmoney(spieler, -200, rHv2s);
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

        public static Waffen createWaffe(int auswahl, Spieler spieler, Vector2 posi)//TODO
        {
            Waffen dasobj;
            Vector3 startort = new Vector3(posi, -5f);
            Matrix rotationKanone;
            rotationKanone= Matrix.CreateRotationY( (float) Math.PI);
            Quaternion AP;
            if (posi.X > 0) //-->Spieler 2
            {
                AP = Quaternion.CreateFromRotationMatrix(rotationKanone);
            }
            else
            {
                AP = Quaternion.Identity;
            }
            
            if (auswahl == 1)
            {               

                ModelObject Kanone = new ModelObject(startort, AP, new Vector3(1f, 1f, 1f), CollisionType.ExactMesh, " ", "Kanone_ganz", 0.1f);
                Kanone.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Kanone.SubModels[1].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Kanone.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Kanone.SubModels[1].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);

                Kanone.SubModels[0].Physics.Mass = 1f;
                Kanone.SubModels[1].Physics.Mass = 1f;

                Kanone.Physics.Material.Bounciness = 0;
                scene.Add(Kanone);
                //TODO Kosten?    


                dasobj = new Waffen(Kanone, 1, (float)Math.PI / 4, 5f,"Kanone");
                spieler.setWaffen(dasobj);
                return dasobj;
            }
            if (auswahl == 2||auswahl==3)
            {

                ModelObject Balliste = new ModelObject(startort, AP, new Vector3(1f, 1f, 1f), CollisionType.ConvexHull, " ", "Balliste", 0.1f);
                Balliste.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Balliste.SubModels[1].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Balliste.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Balliste.SubModels[1].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);

                Balliste.SubModels[0].Physics.Mass = 1f;
                Balliste.SubModels[1].Physics.Mass = 1f;

                Balliste.Physics.Material.Bounciness = 0;
                scene.Add(Balliste);
                
                 
                
                //TODO Kosten?    


                dasobj = new Waffen(Balliste, 1, (float)Math.PI / 4, 5f,"Balliste");
                spieler.setWaffen(dasobj);
                return dasobj;
            }
            if (auswahl == 4)
            {

                ModelObject Rakete = new ModelObject(startort, AP, new Vector3(1f, 1f, 1f), CollisionType.ConvexHull, " ", "Rakete", 0.1f);
                Rakete.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                
                Rakete.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
          

                Rakete.SubModels[0].Physics.Mass = 1f;
                

                Rakete.Physics.Material.Bounciness = 0;
                scene.Add(Rakete);



                //TODO Kosten?    


                dasobj = new Waffen(Rakete, 1, (float)Math.PI / 4, 5f, "Rakete");
                spieler.setWaffen(dasobj);
                return dasobj;
            }
            else
            {
                ModelObject König = new ModelObject(startort, AP, new Vector3(1f, 1f, 1f), CollisionType.BoundingSphere, " ", "König_Subdivision", 0.1f);
                König.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                König.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);

                König.SubModels[0].Physics.Mass = 0.001f;
                König.SubModels[1].Physics.Mass = 0.001f;

                scene.Add(König);
                dasobj = new Waffen(König, 1, (float)Math.PI / 4, 5f,"König");
                spieler.setWaffen(dasobj);
                return dasobj;
            }
            
        }
        
        public static Waffen getWaffe(Spieler spieler, int firedwappons)
        {
            //firedwappons = 0 ==> erste Waffe
            if (spieler.getList().Count == 0)
            {
                return null;
            }
            else
            {
                return spieler.getList()[firedwappons];
            }
            // TODO darf nur zurück geben wenn es noch unabgefeuerte Waffen gibt sonst -> exception!           
            
            // TODO darf nur zurück geben wenn es noch unabgefeuerte Waffen gibt sonst -> exception!           
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
        /*public void bulletCollided(SphereObject bullet,CollisionArgs e)
        {



        }*/

        public static void orientObj(Objekte obj, float linkeHX, float linkeHY)
        {
            
            Vector2 rechteH = new Vector2(obj.getPosition().X, obj.getPosition().Y);
            Vector2 zero = new Vector2(0, 1);
            Vector2 ausrichtung = new Vector2(rechteH.X - linkeHX, rechteH.Y - linkeHY);
            ausrichtung.Normalize();
            //Diskrete Drehung von Objekten ( wird nur durch Höhe der linken Hand bestimmt)
            double radian = 0;
            if (linkeHY <= 0.9)
            {
                radian = Math.PI/4;
            }
            if (1.1 >= linkeHY && linkeHY > 0.9)
            {
                radian = 0;
            }
            if (1.25 >= linkeHY && linkeHY > 1.1)
            {
                radian = 3 * Math.PI / 4;
            }
            if (linkeHY > 1.25)
            {
                radian = Math.PI / 2;

            }



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
                first.setMaterial("Metal");
                first.increaseLP(2); 
                
            }
            else if (auswahl == 4)
            {
                first.setMaterial("Rubber");
                first.setBounciness(1);
                
            }
        }

        private static BoxObject buildbox(Vector3 startort, Vector3 dimension)
        {
            BoxObject box = new BoxObject(startort, //Position
                    dimension,               //Kantenlänge der Latte
                    1f);

            return box;
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

            foreach (Waffen temp in spieler1.getList())
            {
                if (temp.getLP() <= 0)
                {
                    scene.Remove(temp.getModelObject());
                    spieler1.resetWaffen(temp);
                }
            }

            foreach (Waffen temp in spieler2.getList())
            {
                if (temp.getLP() <= 0)
                {
                    scene.Remove(temp.getModelObject());
                    spieler2.resetWaffen(temp);
                }
            }
           
        }

        public static void addToUmgebungsListe(SceneObject obj)
        {
            umgebungsListe.Add(obj);
        }

        public static void removeLP(SceneObject obj)
        {
            foreach (Objekte akt in objListe)
            {
                if(akt.getSceneObject().Equals(obj))
                {
                    akt.decreaseLP();
                }
            }
        }

    }
}
