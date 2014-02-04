using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Scenery;
using NOVA.Graphics;
using Microsoft.Xna.Framework;
using NOVA.UI;
using NOVA;
using BEPUphysics.Constraints.SolverGroups;
using NOVA.Scenery.ComplexObjects;
using Microsoft.Xna.Framework.Graphics;

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
        private static List<Waffen> waffenListe = new List<Waffen>();

        public static Objekte createObj(int auswahl, Spieler spieler, float xPos, Vector2 rHv2s)
        {
            idnummer++;
            Vector3 startort = new Vector3(xPos, 2, -5);
            SceneObject newobj;
            Objekte dasobj;

            if (auswahl == 1)//Würfel
            {
                newobj = buildbox(startort, new Vector3(0.5f, 0.5f, 0.5f));
                DrawHelper.setmoney(spieler, -150, rHv2s);
                /*spieler.setMoney(spieler.getMoney() - 150); //Rohkosten abziehen
                Gamestart.setShowGeld(-150, 100);           //Kosten visualisieren
                */
                newobj.Physics.Mass = 1f;
            }
            else if (auswahl == 2)
            {
                ModelObject l = new ModelObject(startort, Quaternion.CreateFromAxisAngle(new Vector3(1.2f,2.4f,0),(float)Math.PI), new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "L", 1f);
                l.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                l.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                l.Name = "L";
                newobj = l;
                DrawHelper.setmoney(spieler, -200, rHv2s);
            }
            else if (auswahl == 3) // Latte
            {
                newobj = buildbox(startort, new Vector3(2.0f, 0.15f, 0.5f));
                DrawHelper.setmoney(spieler, -200, rHv2s);
                newobj.Physics.Mass = 2f;
            }
            else if (auswahl == 4) // Quader       das kommentierte ist die //Pyramide
            {
                newobj = buildbox(startort, new Vector3(1.0f, 0.5f, 0.5f));
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

                ModelObject  Kanone = new ModelObject(startort, AP, new Vector3(1f, 1f, 1f), CollisionType.ExactMesh, " ", "Kanone_ganz", 1f);
                Kanone.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Kanone.SubModels[1].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Kanone.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Kanone.SubModels[1].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Kanone.PhysicsMaterial.KineticFriction = 1;
                Kanone.Collided += new EventHandler<CollisionArgs>(Kanone_Collided);
                Kanone.Physics.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
                
                Kanone.PhysicsMaterial.Bounciness = 0;
                Kanone.SubModels[0].Physics.Material.Bounciness = 0;
                Kanone.SubModels[1].Physics.Material.Bounciness = 0;
                Kanone.SubModels[0].PhysicsMaterial.Bounciness = 0;
                Kanone.SubModels[1].PhysicsMaterial.Bounciness = 0;
                

                scene.Add(Kanone);
                //TODO Kosten?    


                dasobj = new Waffen(Kanone, 1, (float)Math.PI / 4, 5f,"Kanone");

            }
            else if (auswahl == 2 ||auswahl == 3)
            {
                ModelObject Balliste = new ModelObject(startort, AP, new Vector3(1f, 1f, 1f), CollisionType.ExactMesh, " ", "Balliste", 0.1f);
                Balliste.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Balliste.SubModels[1].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Balliste.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Balliste.SubModels[1].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Balliste.SubModels[1].PhysicsMaterial.KineticFriction = 1;
                Balliste.SubModels[0].Physics.Mass = 0.001f;
                Balliste.SubModels[1].Physics.Mass = 0.001f;

                Balliste.Physics.Material.Bounciness = 0;
                scene.Add(Balliste);
                //TODO Kosten?    


                dasobj = new Waffen(Balliste, 1, (float)Math.PI / 4, 5f,"Balliste");

            }
            else
            {
                ModelObject Rakete = new ModelObject(startort, AP, new Vector3(1.5f, 1.5f, 1.5f), CollisionType.ExactMesh, " ", "Rakete", 0.1f);
                Rakete.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                Rakete.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
                Rakete.SubModels[0].Physics.Mass = 0.1f;


                Rakete.Physics.Material.Bounciness = 0;
                scene.Add(Rakete);
                //TODO Kosten?    

                dasobj = new Waffen(Rakete, 1, (float)Math.PI / 4, 5f, "Rakete");

            }
            spieler.setWaffen(dasobj);
            waffenListe.Add(dasobj);
            return dasobj;
            
        }

        static void Kanone_Collided(object sender, CollisionArgs e)
        {
            if (((ModelObject)sender).Physics.LinearVelocity.Length() < 0.01f && ((ModelObject)sender).Physics.AngularVelocity.Length() < 0.01f)
            {
                //((SceneObject)sender).Physics.BecomeKinematic();//Wird von der PhysicsEngine bewegt das Objekt nicht mehr hat masse 0
                ((SceneObject)sender).Physics.LinearVelocity = Vector3.Zero;
                ((ModelObject)sender).Collided -= new EventHandler<CollisionArgs>(Kanone_Collided);
            }

        }

        public static void createKing(Levels level)
        {
            ModelObject König1 = new ModelObject(new Vector3(level.getSpieler1Pos(), 1, -5), Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY(0)), new Vector3(1f, 1f, 1f), CollisionType.ConvexHull, " ", "König_Subdivision", 0.1f);
            König1.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            König1.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
            König1.Name = "König1";
            scene.Add(König1);

            /*RiggedModel König1 = new RiggedModel(new Vector3(level.getSpieler1Pos(), 1, -5), "König_Subdivision_joined", "König_Subdivision_joined_rigged", 1);
            König1.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            König1.RenderMaterial.Texture = Core.Content.Load<Texture2D>("textur_könig");
            König1.RenderMaterial.Specular = new Vector4(.1f, .1f, .1f, 1);
            König1.Position = new Vector3(level.getSpieler2Pos(), 1, -5);
            König1.Physics.Mass = 10;
            scene.Add(König1);
            König1.StartClip("Walk");
            */
            ModelObject König2 = new ModelObject(new Vector3(level.getSpieler2Pos(), 1, -5), Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY( (float) Math.PI)), new Vector3(1f, 1f, 1f), CollisionType.ConvexHull, " ", "König_Subdivision", 0.1f);
            König2.SubModels[0].RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            König2.SubModels[0].RenderMaterial.Specular = new Vector4(0.1f, 0.1f, 0.1f, 1);
            König2.Name = "König2";
            scene.Add(König2);
        }

        public static Waffen getWaffe(Spieler spieler, int firedwappons)
        {
            //firedwappons = 0 ==> erste Waffe
            if (spieler.getList().Count == 0 ||spieler.getList().Count <= firedwappons)
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
        
        /*
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
                first.increaseLP(1);
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

            List<Waffen> tempL = new List<Waffen>();
            foreach (Waffen temp in spieler1.getList())
            {
                if (temp.getLP() <= 0)
                {
                    if (scene.Contains(temp.getModelObject()))
                    {
                        scene.Remove(temp.getModelObject());
                    }
                    tempL.Add(temp);
                }
            }
            foreach (Waffen temp in tempL)
            {
                spieler1.resetWaffen(temp);
            }

            List<Waffen> tempL2 = new List<Waffen>();
            foreach (Waffen temp in spieler2.getList())
            {
                if (temp.getLP() <= 0)
                {
                    if (scene.Contains(temp.getModelObject()))
                    {
                        scene.Remove(temp.getModelObject());
                    }
                    tempL2.Add(temp);
                }
            }
            foreach (Waffen temp2 in tempL2)
            {
                spieler2.resetWaffen(temp2);
            }
           
        }

        public static void addToUmgebungsListe(SceneObject obj)
        {
            umgebungsListe.Add(obj);
        }

        public static Objekte getObj(SceneObject sobj)
        {
            foreach (Objekte obj in objListe)
            {
                if (obj.getSceneObject().Equals(sobj))
                {
                    return obj;
                }
            }

            return null;
        }

        public static Waffen getWaffe(SceneObject sobj)
        {
            foreach (Waffen obj in waffenListe)
            {
                if (obj.getModelObject().Equals(sobj))
                {
                    return obj;
                }
            }
            return null;
        }
    }
}
