using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.ScreenManagement.BaseScreens;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.Events;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NOVA.Components.Kinect;
using NOVA.Scenery;
using NOVA.UI;
using NOVA.ScreenManagement;
using NOVA.Physics;
using NOVA.Utilities;
using NOVA;
using NOVA.Graphics;


namespace Crazy_Castle_Crush
{
    public class Gamestart : GameplayScreen
    {
        public enum States //Verschiedene Spielzustände 
        {
            Menu,           //Menu am Anfang des Spiels
            Start,          //Feste Objekte werden eingefügt
            Camto1,         //Kamera fährt an Spielerposition 1
            Bauphase1O,     //Bauphase Spieler 1: Objekte
            Bauphase1T,     //Bauphase Spieler 1: Texturen
            Camto2,         //Kamera fährt an Spielerposition 2
            Bauphase2O,     //Bauphase Spieler 2: Objekte
            Bauphase2T,     //Bauphase Spieler 2: Texturen,
            Schussphase1,
            Schussphase2,
            End
        }

        #region Variablen (Deklarationen)

        States currentState;                                //aktueller Zustand
        States prewState;                                   //vorheriger Zustand
        float Zeit1;                                        //Zeit nach State
        float PosX1;                                        //X-Pos nach State
        Matrix cameraRotation;                              //Benötigt für die Kamerabewegung
        BoxObject hintergrund;                              //Benötigt für den Hintergrund
        bool schussphasenDurch;                             //TRUE wenn beide Spieler ihre Schussphase hatten
        int firedWaffen;                                    //Anzahl der abgefeuerten Waffen in einer Schussphase

        RenderMaterial hintergrundsbild = new RenderMaterial();

        CylinderObject coint = new CylinderObject(Vector3.Zero, 0.3f, 0.01f, 8, 0f);
        
        //Erstellt zwei Spieler und das erste Level
        Spieler spieler1 = new Spieler();
        Spieler spieler2 = new Spieler();
        Levels level = new Levels();

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            
            //Kinect initialisieren
            Scene.InitKinect();

            //RGB-Kamerabild als Szenenhintergrund verwenden
            //Scene.Kinect.ShowCameraImage = Kinect.KinectCameraImage.RGB;

            Scene.Physics.ForceUpdater.Gravity = new Vector3(0,-9.81f,0);            //Definierte Schwerkraft

            //Kamera
            CameraObject cam = new CameraObject(new Vector3(0,0,0),                 //Position
                                                new Vector3(0,-1,-5));              //Blickpunkt
            Scene.Add(cam);
            Scene.Camera=cam;


            currentState = States.Menu;                                            //Anfangszustand
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            UpdateCoint();

            switch (currentState)
            {
                    
                #region Menu
		 
	            case States.Menu: 

                    //Platz für ein Menu
                    //Menu();
                    prewState = States.Menu;
                    currentState = States.Start;
                    break;

                    #endregion

                #region Start
                //Start: Objekte werden geladen, Kamera wird erstellt, danach Camto1
                case States.Start:

         
                    
                    //Lädt Spieluntergrund
                    LoadBox(new Vector3(0, -1.5f, -5), new Vector3(46, 0.2f, 1), 0f);

                    //Zusätzlicher Baustein in der Mitte
                    LoadBox(new Vector3(0, 2, -5), new Vector3(1, 1, 1), 22f); ///???Will nicht Fallen???
                                                                             
                    //Lädt Spielhintergrund
                    LoadBackground();

                    //Lädt Coint
                    LoadCoint();

                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds*1000;
                    PosX1 = Scene.Camera.Position.X;

                    //danach Kamera an Spielerposition 1 bewegen
                    prewState = States.Start;
                    currentState = States.Camto1;

                    break;

                #endregion

                #region Camto1
                //Camto1: Kamera wird an die Linke Position bewegt
                case States.Camto1:
                    Textanzeiger("Camto1");
                                        
                    //zeit ist die Zeit (in ms) die Seit State Start vergangen ist
                    float zeit = (gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds*1000) - Zeit1;

                    //Variable wird für nächste Schussphasen zurückgesetzt
                    firedWaffen = 0;                        
                    
                    //Kamera wird bewegt
                    CamMovement(zeit,3000,PosX1, level.getSpieler1Pos());

                #region Übergangsbedingungen
                    //Wenn die Spielerposition 1 erreicht wurde startet die Bauphase/Schussphase
                    if (Scene.Camera.Position.X == level.getSpieler1Pos())
                    {
                        //Wenn wir aus der Startphase kommen, -> Bauphase 1
                        if (prewState == States.Start)
                        {
                            prewState = States.Camto1;
                            currentState = States.Bauphase1O;
                        }

                        //Wenn wir aus der Bauphase von Spieler2 kommen, -> Schussphase 1
                        else if (prewState == States.Bauphase2O)
                        {
                            prewState = States.Camto1;
                            currentState = States.Schussphase1;
                        }

                        //Wenn wir aus der Schussphase von Spieler2 kommen
                        else if (prewState == States.Schussphase2)
                        {
                            //Ist die Schussphase durch -> Bauphase 1
                            if (schussphasenDurch)
                            {
                                prewState = States.Camto1;
                                currentState = States.Schussphase1;
                                /* schussphasenDurch wird auf true gesetzt, damit nach der nächsten Schussphase wieder in die Bauphase gewechselt wird.
                                 * Schussphase2 sagt also schussphaseDurch= true, will aber erst noch Schussphase 1
                                 */

                            }
                            //sonst Schussphase 1
                            else
                            {
                                prewState = States.Camto1;
                                currentState = States.Bauphase1O;
                            }
                        }
                    } 
                #endregion

                    break;

                #endregion

                #region Bauphase1 Objekte
                //Bauphase, Spiele 1, Objekte erstellen
                case States.Bauphase1O:
                    Textanzeiger("Bauphase 1 Obj");
                    Geldanzeige(spieler1);          //Blendet die Geldbetrag des Spielers ein
                    //noch leer



                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt 
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000;
                    PosX1 = Scene.Camera.Position.X;

                    #region Übergangsbedingungen
                    //wenn Spieler1 nicht mehr ausreichend Geld hat oder auf weiter geklickt hat...
                    //Klick auf weiter handelt die HandleInput Fkt ab
                    if (spieler1.getMoney() <= level.getMinMoney()/* || Klick auf weiter*/)
                    {
                        prewState = States.Bauphase1O;

                        //wenn Spieler2 über genügend Geld zum bauen verfügt, Bauphase Spieler 2
                        //Wenn Spieler2 mehr Geld besitzt fängt er die Schussphase2 an
                        if (spieler2.getMoney() >= level.getMinMoney() || spieler2.getMoney() > spieler1.getMoney())
                        {
                            currentState = States.Camto2;
                        }
                        //wenn Spieler2 nicht über genügend Geld zum bauen verfügt, und Spieler1 mehr Geld hat beginnt Schussphase1
                        else
                        {
                            currentState = States.Schussphase1;
                        }
                    }
                    //Wird ein Objekt erstellt, wird in den Bauphase1T State gewechselt: Eventhandler wickelt dies ab!
                    #endregion

                    break;

                #endregion

                #region Bauphase1 Texturen
                //Bauphase, Spiele 1, Objekte erstellen
                case States.Bauphase1T:
                    Textanzeiger("Bauphase 1 Tex");
                    Geldanzeige(spieler1);          //Blendet die Geldbetrag des Spielers ein
                    //noch leer


                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt 
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000;
                    PosX1 = Scene.Camera.Position.X;

                    //Übergang wird mit neuer Texture erzeugt
                    break;

                #endregion

                #region Camto2
                //Kamera wird an die Rechte Positon bewegt
                case States.Camto2:
                    Textanzeiger("Camto2");

                    //zeit ist die Zeit (in ms) die Seit State Start vergangen ist
                    zeit = (gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000) - Zeit1;

                    //Variable wird für nächste Schussphasen zurückgesetzt
                    firedWaffen = 0; 

                    //Kamera wird bewegt
                    CamMovement(zeit, 3000, PosX1, level.getSpieler2Pos());

                #region Übergangsbedingungen
                    //Wenn die Spielerposition 2 erreicht wurde startet die Bauphase/Schussphase
                    if (Scene.Camera.Position.X == level.getSpieler2Pos())
                    {
                        //Wenn wir aus der Bauphase1 kommen -> Bauphase 2 (ohne Geld, aber mehr Geld als Sp1 Schussphase2)
                        if (prewState == States.Bauphase1O)
                        {
                            //Spieler2 hat genug Geld zum Bauen
                            if (spieler2.getMoney() >= level.getMinMoney())
                            {
                                prewState = States.Camto2;
                                currentState = States.Bauphase2O;
                            }
                            //Spieler zwei hat nicht genug Geld aber mehr als Spieler1 -> Schussphase 2
                            else if (spieler2.getMoney() > spieler1.getMoney())
                            {
                                prewState = States.Camto2;
                                currentState = States.Bauphase2O;
                            }
                        }
                        //Wenn wir aus der Schussphase1 kommen, muss Schussphase 2 starten 
                        else if (prewState == States.Schussphase1)
                        {
                            prewState = States.Camto2;
                            currentState = States.Schussphase2;
                        }

                    }
                #endregion

                    break;

                #endregion

                #region Bauphase2 Objekte
                //Bauphase, Spiele 1, Objekte erstellen
                case States.Bauphase2O:
                    Textanzeiger("Bauphase 2 Obj");
                    Geldanzeige(spieler2);          //Blendet die Geldbetrag des Spielers ein
                    //noch leer


                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt 
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000;
                    PosX1 = Scene.Camera.Position.X;

                    #region Übergangsbedingungen
                    //wenn Spieler2 nicht mehr ausreichend Geld hat oder auf weiter geklickt hat...
                    //Klick auf weiter handelt die HandleInput Fkt ab
                    if (spieler1.getMoney() <= level.getMinMoney()/* || Klick auf weiter*/)
                    {
                        prewState = States.Bauphase2O;

                        //Wenn Spieler2 mehr Geld besitzt fängt er die Schussphase2 an
                        if (spieler2.getMoney() > spieler1.getMoney())
                        {
                            currentState = States.Schussphase2;
                        }
                        //sonst Spieler 1
                        else
                        {
                            currentState = States.Camto1;
                        }
                    }
                    //Wird ein Objekt erstellt, wird in den Bauphase2T State gewechselt: Eventhandler wickelt dies ab!
                    #endregion


                    break;

                #endregion

                #region Bauphase2 Texturen
                //Bauphase, Spiele 1, Objekte erstellen
                case States.Bauphase2T:
                    Textanzeiger("Bauphase 2 Tex");
                    Geldanzeige(spieler2);          //Blendet die Geldbetrag des Spielers ein
                    //noch leer


                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt 
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000;
                    PosX1 = Scene.Camera.Position.X;

                    //Übergang wird mit neuer Texture erzeugt
                    break;

                #endregion

                #region Schussphase1
                //Schussphase des ersten Spielers
                case States.Schussphase1:
                    string fuertextanz = "Schussphase 1, " + firedWaffen;
                    Textanzeiger(fuertextanz);
                    Geldanzeige(spieler1);          //Blendet die Geldbetrag des Spielers ein

                    //noch leer
                    

                    //firedWaffen += 1;                       //es wurde eine Weitere Waffe abgefeuert

                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000;
                    PosX1 = Scene.Camera.Position.X;

                    #region Übergangsbedingungen
                    //Wenn alle Waffen abgefeuert wurden...
                    if (firedWaffen == spieler2.getWaffen())
                    {
                        prewState = States.Schussphase1;

                        //Wenn die Schussphase durch ist, beginnt die Bauphase
                        if (schussphasenDurch)
                        {
                            schussphasenDurch = false;
                            currentState = States.Bauphase1O;
                        }
                        //Ist die Schussphase nicht durch, gehen wir in Schussphase 2
                        else
                        {
                            schussphasenDurch = true;           //nach der Schussphase2 ist die Schussphase beendet
                            currentState = States.Camto2;
                        }
                    }

                    #endregion
                
                    break;

                #endregion

                #region Schussphase2
                //Schussphase des zweiten Spielers
                case States.Schussphase2:
                    string fuertext = "Schussphase 2, " + firedWaffen;
                    Textanzeiger(fuertext);
                    Geldanzeige(spieler2);          //Blendet die Geldbetrag des Spielers ein
                    //noch leer


                    //firedWaffen += 1;                       //es wurde eine Weitere Waffe abgefeuert

                    //setzt die Variable Zeit1 auf die Zeit und PosX1 auf die Position bevor er in den nächsten State wechselt
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000;
                    PosX1 = Scene.Camera.Position.X;

                    #region Übergangsbedingungen
                    //Wenn alle Waffen abgefeuert wurden...
                    if (firedWaffen == spieler2.getWaffen())
                    {
                        prewState = States.Schussphase2;

                        //Wenn die Schussphase durch ist, beginnt die Bauphase
                        if (schussphasenDurch)
                        {
                            schussphasenDurch = false;
                            currentState = States.Camto1;
                        }
                        //Ist die Schussphase nicht durch, gehen wir in Schussphase 1
                        else
                        {
                            schussphasenDurch = true;           //nach der Schussphase1 ist die Schussphase beendet
                            currentState = States.Camto1;
                        }
                    }

                    #endregion
                    break;

                #endregion

                #region End
                //Ende des Spiels
                case States.End:
                    //noch leer



                    //neues Spiel, alle vorherigen Objekte werden gelöscht
                    Scene.RemoveAllSceneObjects();  
                    break;

                #endregion

            }

            if (Scene.Kinect.SkeletonDataReady)
            {
                List<NOVA.Components.Kinect.Skeleton> skeletons = new List<NOVA.Components.Kinect.Skeleton>(Scene.Kinect.Skeletons);

                //Aktives Skelett finden
                foreach (NOVA.Components.Kinect.Skeleton skeleton in skeletons)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.Joints.Count != 0 &&
                        skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        //if JointType.HandRight == 
                        //Position der rechten Hand des Spielers in Bildschirmkoodinaten
                        Vector2 screenPos = skeleton.Joints[JointType.HandRight].ScreenPosition;
                        screenPos.X = screenPos.X * Scene.Game.Window.ClientBounds.Width;
                        screenPos.Y *= Scene.Game.Window.ClientBounds.Height;

                        //parallele Ebene zum Bildschirm erzeugen in der die Kugel transformiert wird
                        Plane plane = new Plane(Vector3.Forward, -10f);

                        //Weltkoordinatenpunk finden
                        Vector3 worldPos = Helpers.Unproject(screenPos, plane);

                    }

                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.Joints.Count != 0 &&
                        skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                    {
                        //Position der rechten Hand des Spielers in Bildschirmkoodinaten
                        Vector2 screenPos = skeleton.Joints[JointType.HandRight].ScreenPosition;
                        screenPos.X = screenPos.X * Scene.Game.Window.ClientBounds.Width;
                        screenPos.Y *= Scene.Game.Window.ClientBounds.Height;

                        #region WEITER klick
                        //Wenn sich die rechte Hand in der unteren, rechten Ecke befindet -> Klick auf WEITER
                        if (screenPos.X >= 0.8f && screenPos.Y <= 0.2f)
                        {
                            if (currentState == States.Bauphase1O)
                            {
                                prewState = States.Bauphase1O;

                                //wenn Spieler2 über genügend Geld zum bauen verfügt, Bauphase Spieler 2
                                //Wenn Spieler2 mehr Geld besitzt fängt er die Schussphase2 an
                                if (spieler2.getMoney() >= level.getMinMoney() || spieler2.getMoney() > spieler1.getMoney())
                                {
                                    currentState = States.Camto2;
                                }
                                //wenn Spieler2 nicht über genügend Geld zum bauen verfügt, und Spieler1 mehr Geld hat beginnt Schussphase1
                                else
                                {
                                    currentState = States.Schussphase1;
                                }
                            }
                            else if (currentState == States.Bauphase2O)
                            {
                                prewState = States.Bauphase2O;

                                //Wenn Spieler2 mehr Geld besitzt fängt er die Schussphase2 an
                                if (spieler2.getMoney() > spieler1.getMoney())
                                {
                                    currentState = States.Schussphase2;
                                }
                                //sonst Spieler 1
                                else
                                {
                                    currentState = States.Camto1;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        #endregion


                    }
                }
            }

        }




        public override void HandleInput(InputState input)
        {

            #region Wenn Spieler auf Weiter Klickt (Hier noch mit Leertaste realisiert)
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space, PlayerIndex.One))
            {
                if (currentState == States.Bauphase1O)
                {
                    prewState = States.Bauphase1O;

                    //wenn Spieler2 über genügend Geld zum bauen verfügt, Bauphase Spieler 2
                    //Wenn Spieler2 mehr Geld besitzt fängt er die Schussphase2 an
                    if (spieler2.getMoney() >= level.getMinMoney() || spieler2.getMoney() > spieler1.getMoney())
                    {
                        currentState = States.Camto2;
                    }
                    //wenn Spieler2 nicht über genügend Geld zum bauen verfügt, und Spieler1 mehr Geld hat beginnt Schussphase1
                    else
                    {
                        currentState = States.Schussphase1;
                    }
                }
                else if (currentState == States.Bauphase2O)
                {
                    prewState = States.Bauphase2O;

                    //Wenn Spieler2 mehr Geld besitzt fängt er die Schussphase2 an
                    if (spieler2.getMoney() > spieler1.getMoney())
                    {
                        currentState = States.Schussphase2;
                    }
                    //sonst Spieler 1
                    else
                    {
                        currentState = States.Camto1;
                    }
                }
                else
                {
                    return;
                }

            }
            #endregion

            #region Wenn Spieler ein Objekt erzeugt hat (Hier noch mit O realisiert)
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.O, PlayerIndex.One))
            {
                if (currentState == States.Bauphase1O)
                {
                    prewState = States.Bauphase1O;
                    currentState = States.Bauphase1T;
                }
                if (currentState == States.Bauphase2O)
                {
                    prewState = States.Bauphase2O;
                    currentState = States.Bauphase2T;
                }
            }
            #endregion

            #region Wenn Spieler eine Texture erzeugt hat (Hier noch mit T realisiert)
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T, PlayerIndex.One))
            {
                if (currentState == States.Bauphase1T)
                {
                    prewState = States.Bauphase1T;
                    currentState = States.Bauphase1O;
                }
                if (currentState == States.Bauphase2T)
                {
                    prewState = States.Bauphase2T;
                    currentState = States.Bauphase2O;
                }
            }
            #endregion

            #region Wenn Spieler eine Waffe abgefeuert hat (Hier noch mit S realisiert)
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.S, PlayerIndex.One))
            {
                firedWaffen = 1;
            }
            #endregion


            #region Spiel Beenden (Esc)
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape, PlayerIndex.One))
            {
                Environment.Exit(0);
            }
            #endregion

            base.HandleInput(input);
        }



        private void LoadBox(Vector3 position, Vector3 dimension, float masse)
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box = new BoxObject(position,             //Position
                               dimension,                          //Kantenlängen
                               masse);
            Scene.Add(box);
            
        }

        private void LoadBackground()
        {
            Vector3 pos = new Vector3(0, 0, -11);
            hintergrund = new BoxObject(pos,
                                        new Vector3(56, 10, 0),
                                        0f);


            hintergrundsbild.Texture = Core.Content.Load<Texture2D>("himmel");
            hintergrundsbild.Diffuse = Color.White.ToVector4();
            hintergrundsbild.EnableGlow = true;
            hintergrund.RenderMaterial = hintergrundsbild;

            Scene.Add(hintergrund);
        }

        private void LoadCoint()
        {
            RenderMaterial gold = new RenderMaterial();
            gold.Diffuse = Color.Gold.ToVector4();
            coint.RenderMaterial = gold;
            Scene.Add(coint);
        }

        private void UpdateCoint()
        {
            
            coint.Position = new Vector3(Scene.Camera.Position.X, Scene.Camera.Position.Y, -5);
        }


        private void CamMovement(float zeit, int dauer, float Ausgangsposition, float Zielposition)
        {

            Scene.Camera.Position = new Vector3(CameraMovement.getXMovement(zeit, dauer, Ausgangsposition, Zielposition),
                                                0,
                                                CameraMovement.getZMovement(zeit, dauer) * 0.8f);
            cameraRotation = Matrix.CreateRotationX(0) *
                             Matrix.CreateRotationY(0);
            Scene.Camera.Orientation = Quaternion.CreateFromRotationMatrix(cameraRotation);
            Scene.Camera.Orientation = Quaternion.CreateFromRotationMatrix(cameraRotation);

        }

        private void Textanzeiger(string aktuellerText)
        {
            UI2DRenderer.WriteText(new Vector2(Scene.Camera.Position.X,Scene.Camera.Position.Y),            //Position
                                  aktuellerText,                    //Anzuzeigender Text
                                  Color.Black,                   //Textfarbe
                                  null,                    //Interne Schriftart verwenden
                                  Vector2.One,             //Textskallierung
                                  UI2DRenderer.HorizontalAlignment.Center, //Horizontal zentriert
                                  UI2DRenderer.VerticalAlignment.Bottom);  //am unteren Bildschirmrand ausrichten
        }

        private void Geldanzeige(Spieler spieler)
        {
            string aktuellerText = "$" + spieler.getMoney() ;
            UI2DRenderer.WriteText(new Vector2(Scene.Camera.Position.X+2, Scene.Camera.Position.Y),            //Position
                        aktuellerText,                    //Anzuzeigender Text
                        Color.Red,                   //Textfarbe
                        null,                    //Interne Schriftart verwenden
                        Vector2.One,             //Textskallierung
                        UI2DRenderer.HorizontalAlignment.Left, //Horizontal zentriert
                        UI2DRenderer.VerticalAlignment.Top);  //am unteren Bildschirmrand ausrichten

        }
    }


} 
