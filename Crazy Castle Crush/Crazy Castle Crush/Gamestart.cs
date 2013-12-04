using System;
using System.Collections.Generic;
using NOVA.ScreenManagement.BaseScreens;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NOVA.Scenery;
using NOVA.UI;
using NOVA.ScreenManagement;
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
        float zeit;                                         //vergangende Zeit seit letztem State
        float PosX1;                                        //X-Pos nach State
        bool schussphasenDurch;                             //TRUE wenn beide Spieler ihre Schussphase hatten
        int firedWaffen;                                    //Anzahl der abgefeuerten Waffen in einer Schussphase
        bool detecting = false;                             //Kinect benötigt
        BoxObject rightHand;
        BoxObject leftHand;
        BoxObject auswahlanzeigeO;                          //zur Auswahl von Objekten 
        BoxObject auswahlanzeigeT;                          //zur Auswahl von Texturen 
        int auswahl;                                        //je nach Position der linken Hand erhält die Auswahl ihre Werte (für Objekt und Texturauswahl)

        //Benötigt für die einblendung von Auswahlmenu
        States objState = States.Start;
        Auswahl Auswahl = new Auswahl();

        
        //Erstellt zwei Spieler und das erste Level
        Spieler spieler1 = new Spieler();
        Spieler spieler2 = new Spieler();
        Levels level = new Levels();

        //Initiallisiert die Klassen
        CameraMovement cameraMovement;
        StartObjects startObjects;
        Objekte objekte;

        #endregion

        public override void Initialize()
        {
            base.Initialize();
            
            //Kinect initialisieren
            Scene.InitKinect();

            Scene.Physics.ForceUpdater.Gravity = new Vector3(0,-9.81f,0);            //Definierte Schwerkraft

            //Kamera
            CameraObject cam = new CameraObject(new Vector3(0,0,0),                 //Position
                                                new Vector3(0,-1,-5));              //Blickpunkt
            Scene.Add(cam);
            Scene.Camera=cam;
            cameraMovement = new CameraMovement(cam);

            //Objecte
            startObjects = new StartObjects(Scene, level);
            objekte = new Objekte(Scene);



            //Hände bekommen transparente Box
            rightHand = startObjects.RightHand();
            leftHand = startObjects.LeftHand();

            currentState = States.Menu;                                            //Anfangszustand
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            
            
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

                    startObjects.LoadStartObjects(level.getLevel());

                    //Zeigt das Baumenü mit den Objekten und Texturen die der Spieler wählen kann, benötigt Name des Bildes
                    auswahlanzeigeO = startObjects.showObjects("Bau");
                    auswahlanzeigeT = startObjects.showObjects("hpic3");

                    //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt
                    PosX1 = Scene.Camera.Position.X;
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                    aktuallisiereZeit(gameTime);

                    //danach Kamera an Spielerposition 1 bewegen
                    prewState = States.Start;
                    currentState = States.Camto1;

                    break;

                #endregion

                #region Camto1
                //Camto1: Kamera wird an die Linke Position bewegt
                case States.Camto1:
                    aktuallisiereZeit(gameTime);

                    detecting = false;  //Kinect deaktiviert

                    //Variable wird für nächste Schussphasen zurückgesetzt
                    firedWaffen = 0;                        
                    
                    //Kamera wird bewegt
                    cameraMovement.move(zeit,3000,PosX1, level.getSpieler1Pos());

                #region Übergangsbedingungen
                    //Wenn die Spielerposition 1 erreicht wurde startet die Bauphase/Schussphase
                    if (Scene.Camera.Position.X == level.getSpieler1Pos())
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

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
                    aktuallisiereZeit(gameTime);
                    detecting = true;               //Kinect aktiv

                    
                    #region Übergangsbedingungen
                    //wenn Spieler1 nicht mehr ausreichend Geld hat oder auf weiter geklickt hat...
                    //Klick auf weiter handelt die HandleInput Fkt ab
                    if (spieler1.getMoney() <= level.getMinMoney()/* || Klick auf weiter*/)
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

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
                    aktuallisiereZeit(gameTime);

                
                
                    //noch leer




                    //Übergang wird mit neuer Texture erzeugt
                    break;

                #endregion

                #region Camto2
                //Kamera wird an die Rechte Positon bewegt
                case States.Camto2:
                    aktuallisiereZeit(gameTime);
                    detecting = false;               //Kinect deaktiviert

                    //Variable wird für nächste Schussphasen zurückgesetzt
                    firedWaffen = 0; 

                    //Kamera wird bewegt
                    cameraMovement.move(zeit, 3000, PosX1, level.getSpieler2Pos());

                #region Übergangsbedingungen
                    //Wenn die Spielerposition 2 erreicht wurde startet die Bauphase/Schussphase
                    if (Scene.Camera.Position.X == level.getSpieler2Pos())
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

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
                    aktuallisiereZeit(gameTime);
                    detecting = true;                       //Kinect aktiv



                    //noch leer




                    #region Übergangsbedingungen
                    //wenn Spieler2 nicht mehr ausreichend Geld hat oder auf weiter geklickt hat...
                    //Klick auf weiter handelt die HandleInput Fkt ab
                    if (spieler1.getMoney() <= level.getMinMoney()/* || Klick auf weiter*/)
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

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
                    aktuallisiereZeit(gameTime);
                    //noch leer




                    //Übergang wird mit neuer Texture erzeugt



                    break;

                #endregion

                #region Schussphase1
                //Schussphase des ersten Spielers
                case States.Schussphase1:
                    aktuallisiereZeit(gameTime);
                    detecting = true;               //Kinect aktiv

                    //noch leer
                    

                    //firedWaffen += 1;                       //es wurde eine Weitere Waffe abgefeuert



                    #region Übergangsbedingungen
                    //Wenn alle Waffen abgefeuert wurden...
                    if (firedWaffen == spieler2.getWaffen())
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

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
                    aktuallisiereZeit(gameTime);
                    detecting = true;               //Kinect aktiv
                    //noch leer


                    //firedWaffen += 1;                       //es wurde eine Weitere Waffe abgefeuert



                    #region Übergangsbedingungen
                    //Wenn alle Waffen abgefeuert wurden...
                    if (firedWaffen == spieler2.getWaffen())
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

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



            #region Objekt-/ Texturauswahl ein-/ausblenden
            if (currentState != States.Menu && currentState != States.Start && currentState != States.End)
            {
                if (currentState == States.Bauphase1O)
                {
                    startObjects.einausblender(auswahlanzeigeO, auswahlanzeigeT, 1, zeit);
                }
                if (currentState == States.Bauphase1T)
                {
                    startObjects.einausblender(auswahlanzeigeO, auswahlanzeigeT, 11, zeit);
                }
                if (currentState == States.Bauphase2O)
                {
                    startObjects.einausblender(auswahlanzeigeO, auswahlanzeigeT, 2, zeit);
                }
                if (currentState == States.Bauphase2T)
                {
                    startObjects.einausblender(auswahlanzeigeO, auswahlanzeigeT, 22, zeit);
                }
                else
                {
                    startObjects.einausblender(auswahlanzeigeO, auswahlanzeigeT, 0, zeit);
                }
            }



            #endregion

            #region Kinect
            if (detecting)
            {
                if (Scene.Kinect.SkeletonDataReady)
                {
                    List<NOVA.Components.Kinect.Skeleton> skeletons = new List<NOVA.Components.Kinect.Skeleton>(Scene.Kinect.Skeletons);

                    //Aktives Skelett finden
                    foreach (NOVA.Components.Kinect.Skeleton skeleton in skeletons)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.Joints.Count != 0)
                        {
                            //Box auf Hand, Klick auf Weiter
                            #region Detektion der rechten Hand

                            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                            {
                                //Position der rechten Hand des Spielers in Bildschirmkoodinaten
                                Vector2 screenPos = skeleton.Joints[JointType.HandRight].ScreenPosition;
                                Vector2 normScreenPos = new Vector2(screenPos.X, screenPos.Y);
                                screenPos.X = screenPos.X * Scene.Game.Window.ClientBounds.Width;
                                screenPos.Y *= Scene.Game.Window.ClientBounds.Height;

                                //parallele Ebene zum Bildschirm erzeugen in der die Kugel transformiert wird
                                Plane plane2 = new Plane(Vector3.Forward, -4f);

                                //Weltkoordinatenpunk finden
                                Vector3 worldPos2 = Helpers.Unproject(screenPos, plane2);

                                #region Box auf Hand
                                //Position der Kugel setzen
                                rightHand.Position = worldPos2;
                                #endregion

                                #region WEITER klick
                                //Wenn sich die rechte Hand in der oberen, rechten Ecke befindet -> Klick auf WEITER
                                if (normScreenPos.X >= 0.9f && normScreenPos.Y >= 0.9f)
                                {
                                    //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                                    PosX1 = Scene.Camera.Position.X;
                                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                                    aktuallisiereZeit(gameTime);

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
                            #endregion

                            //Box auf Hand, Auswahl Textur/ Objekt
                            #region Detektion der linken Hand
                            if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                            {
                                //Position der linken Hand des Spielers in Bildschirmkoodinaten
                                Vector2 screenPos = skeleton.Joints[JointType.HandLeft].ScreenPosition;
                                Vector2 normScreenPos = new Vector2(screenPos.X, screenPos.Y);
                                screenPos.X = screenPos.X * Scene.Game.Window.ClientBounds.Width;
                                screenPos.Y *= Scene.Game.Window.ClientBounds.Height;

                                //parallele Ebene zum Bildschirm erzeugen in der die Kugel transformiert wird
                                Plane plane2 = new Plane(Vector3.Forward, -4f);

                                //Weltkoordinatenpunk finden
                                Vector3 worldPos2 = Helpers.Unproject(screenPos, plane2);

                                #region Box auf Hand
                                //Position der Kugel setzen
                                leftHand.Position = worldPos2;
                                #endregion

                                #region Auswahl Textur/ Objekt
                                auswahl = Auswahl.auswahl(normScreenPos);

                                #endregion
                            }
                            #endregion

                            //Hintergrundsbild verschieben
                            #region Detektion des Kopfes

                            if (skeleton.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
                            {
                                //Position des Kopfes des Spielers in Bildschirmkoodinaten
                                Vector2 screenPos = skeleton.Joints[JointType.Head].ScreenPosition;
                                Vector2 normScreenPos = new Vector2(screenPos.X, screenPos.Y);

                                startObjects.MoveBackground(normScreenPos.X - 0.5f, normScreenPos.Y - 0.5f);
                            }
                            #endregion
                        }
                    }
                    
                }
            }
            #endregion


            objState = currentState; //Am Ende jenden Updates wird der State angeglichen

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }




        public override void HandleInput(InputState input)
        {

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

        public override void Draw(GameTime gameTime)
        {
            #region State Anzeige
            string wobinich = "";
            if (currentState == States.Bauphase1O)
            {
                wobinich = "Bau1 Obj"+ auswahl;
            }
            else if (currentState == States.Bauphase1T)
            {
                wobinich = "Bau1 Tex"+ auswahl;
            }
            else if (currentState == States.Bauphase2O)
            {
                wobinich = "Bau2 Obj"+ auswahl;
            }
            else if (currentState == States.Bauphase2T)
            {
                wobinich = "Bau2 Tex" + auswahl;
            }
            else if (currentState == States.Camto1)
            {
                wobinich = "Cam1";
            }
            else if (currentState == States.Camto2)
            {
                wobinich = "Cam2";
            }
            else if (currentState == States.End)
            {
                wobinich = "End";
            }
            else if (currentState == States.Menu)
            {
                wobinich = "Menu";
            }
            else if (currentState == States.Schussphase1)
            {
                wobinich = "Schussphase1" + firedWaffen;
            }
            else if (currentState == States.Schussphase2)
            {
                wobinich = "Schussphase2" + firedWaffen;
            }
            else if (currentState == States.Start)
            {
                wobinich = "Start";
            }

            Textanzeiger(wobinich);
            #endregion 

            #region Geldanzeige
            if (currentState == States.Bauphase1O || currentState == States.Bauphase1T || currentState == States.Schussphase1)
            {
                objekte.Geldanzeige(spieler1);  //Blendet die Geldbetrag des Spielers ein
            }
            if (currentState == States.Bauphase2O || currentState == States.Bauphase2T || currentState == States.Schussphase2)
            {
                objekte.Geldanzeige(spieler2); //Blendet die Geldbetrag des Spielers ein
            }
            #endregion

            base.Draw(gameTime);
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

        private void aktuallisiereZeit(GameTime gameTime)
        {
            zeit = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000 - Zeit1;
        }


    }


} 
