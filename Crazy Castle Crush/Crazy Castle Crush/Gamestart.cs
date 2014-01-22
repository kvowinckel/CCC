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
using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Controllers;
using Microsoft.Kinect.Toolkit.Interaction;




namespace Crazy_Castle_Crush
{
    public class Gamestart : GameplayScreen
    {
        public enum States //Verschiedene Spielzustände 
        {
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
        float Zeit2 =0;                                        //Zeit nach grab RH
        float Zeit3 =0;                                        //Zeit nach grab LH
        float zeit;                                         //vergangende Zeit seit letztem State
        float PosX1;                                        //X-Pos nach State
        bool schussphasenDurch;                             //TRUE wenn beide Spieler ihre Schussphase hatten
        int firedWaffen=0;                                  //Anzahl der abgefeuerten Waffen in einer Schussphase
        bool detecting = false;                             //Kinect benötigt
        Vector2 rHv2s;                                      //rechte Hand als Vector 2 in ScreenPos
        Vector2 rHv2w;                                      //rechte Hand als Vector 2 in WorldPos
        Vector2 rHv2n;                                      //rechte Hand als Vector 2 in normScreenPos
        Vector2 lHv2s;                                      //linke Hand als Vector 2 in ScreenPos
        Vector2 lHv2w;                                      //linke Hand als Vector 2 in WorldPos
        Vector2 lHv2n;                                      //linke Hand als Vector 2 in normScreenPos
        Vector2 screenDim;                                  //Screen Dimension
        bool showWaffe;                                     //Gibt an ob gerade die Waffen angezeigt werden sollen
        int auswahl;                                        //je nach Position der linken Hand erhält die Auswahl ihre Werte (für Objekt und Texturauswahl)
        static int[] showGeld = new int[2];                 //Wenn geld hinzugefügt oder abgezogen wird                             

        bool klickRH;                                       //klick der rechten Hand
        bool klickLH;                                       //klick der linken Hand
        
        bool shoot;                                         //Wenn Spieler schießt (noch S)
        
        bool objInHand;                                     //solange das Objekt an der Hand ist
        Objekte aktuellesObj;                               //Objekt das gerade bearbeitet wird
        Waffen aktuelleWaffe;                                //Waffe die gerade bedient wird
        CameraObject cam;                                   //camera
        SphereObject bullet;                                  //Geschoss  
        bool bulletInAir;



        //Benötigt für die einblendung von Auswahlmenu
        States objState = States.Start;
        Auswahl Auswahl = new Auswahl();

        
        //Erstellt zwei Spieler und das erste Level
        Spieler spieler1 = new Spieler();
        Spieler spieler2 = new Spieler();
        Spieler gamer;                                      //Zum zwischenspeichern des aktuellen Spielers
        Levels level = new Levels();

        //Initiallisiert die Klassen
        CameraMovement cameraMovement;
        StartObjects startObjects;
        

        #endregion

        public override void Initialize()
        {
            base.Initialize();
            
            //Kinect initialisieren
            Scene.InitKinect();

            Scene.Physics.ForceUpdater.Gravity = new Vector3(0,-5.0f,0);            //Definierte Schwerkraft

            //Kamera
            cam = new CameraObject(new Vector3(0,0,0),                 //Position
                                                new Vector3(0,-1,-5));              //Blickpunkt
            Scene.Add(cam);
            Scene.Camera=cam;
            cameraMovement = new CameraMovement(cam);


            this.Scene.ShowCameraImage = true;
            this.Scene.Kinect.ShowCameraImage = NOVA.Components.Kinect.Kinect.KinectCameraImage.ReducedRGB;
            

            //Objecte
            startObjects = new StartObjects(Scene, level);
            Objektverwaltung.setObjektverwaltung(Scene,level);

            currentState = States.Start;                                            //Anfangszustand
        }
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            #region Kinect

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
                            rHv2s = skeleton.Joints[JointType.HandRight].ScreenPosition;
                            rHv2s -= new Vector2(0, 10);
                            rHv2n.X = rHv2s.X / screenDim.X;
                            rHv2n.Y = rHv2s.Y / screenDim.Y;

                            Plane plane2 = new Plane(Vector3.Forward, -4f);

                            //Weltkoordinatenpunk finden
                            Vector3 worldPos2R = Helpers.Unproject(rHv2s, plane2, false);
                            rHv2w = new Vector2(worldPos2R.X, worldPos2R.Y);

                            #region Auswahl Textur/ Objekt
                            auswahl = Auswahl.auswahl(rHv2n);

                            #endregion

                            if (skeleton.HandPointers[1].HandEventType == InteractionHandEventType.GripRelease)
                            {
                                klickRH = true;
                            }
                            else
                            {
                                klickRH = false;
                            }

                        }
                        #endregion

                        //Box auf Hand, Auswahl Textur/ Objekt
                        #region Detektion der linken Hand
                        if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                        {
                            //Position der linken Hand des Spielers in Bildschirmkoodinaten
                            lHv2s = skeleton.Joints[JointType.HandLeft].ScreenPosition;
                            lHv2n.X = lHv2s.X / screenDim.X;
                            lHv2n.Y = lHv2s.Y / screenDim.Y;

                            //parallele Ebene zum Bildschirm erzeugen in der die Kugel transformiert wird
                            Plane plane2 = new Plane(Vector3.Forward, -4f);

                            //Weltkoordinatenpunk finden
                            Vector3 worldPos2L = Helpers.Unproject(lHv2s, plane2, false);
                            lHv2w = new Vector2(worldPos2L.X, worldPos2L.Y);


                            klickLH = (skeleton.HandPointers[1].HandEventType == InteractionHandEventType.Grip);
                        }

                        #endregion

                        //Hintergrundsbild verschieben
                        #region Detektion des Kopfes
                        if (skeleton.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
                        {
                            //Position des Kopfes des Spielers in Bildschirmkoodinaten
                            Vector2 screenPos = skeleton.Joints[JointType.Head].ScreenPosition;
                            Vector2 normScreenPos = new Vector2(screenPos.X / screenDim.X, screenPos.Y / screenDim.Y);

                            Vector3 realPos = skeleton.Joints[JointType.Head].WorldPosition;
                            //Hintergrund bewegen
                            startObjects.MoveBackground(normScreenPos.X - 0.5f, normScreenPos.Y - 0.5f);

                            //Kamera auf z-Achse bewegen
                            float zoom;
                            zoom = realPos.Z;
                            if (zoom >= 1.5 && zoom <= 4)
                            {
                                zoom -= 1.5f;

                                cam.Position = new Vector3(cam.Position.X, cam.Position.Y, zoom * 5);
                            }

                        }

                        #endregion

                    }
                }

            }
            #endregion

            switch (currentState)
            {
                #region Start
                //Start: Objekte werden geladen, Kamera wird erstellt, danach Camto1
                case States.Start:

                    startObjects.LoadStartObjects(level.getLevel());

                    //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt
                    PosX1 = Scene.Camera.Position.X;
                    Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                    aktuallisiereZeit(gameTime);

                    showGeld[1] = 0;

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

                #region Objektemenüs
                case States.Bauphase2O:

                case States.Bauphase1O:
                    aktuallisiereZeit(gameTime);
                    float pos;

                    #region Spieler &  Spielerposition
                    if (currentState == States.Bauphase1O)
                    {
                        gamer = spieler1;
                        pos = level.getSpieler1Pos();
                    }
                    else
                    {
                        gamer = spieler2;
                        pos = level.getSpieler2Pos();
                    }
                    #endregion

                    #region Objekt erzeugen und mit Hand positionieren
                    if (!showWaffe)
                    {
                        if (klickRH && !objInHand && auswahl != 0 &&auswahl < 5)    //"klick" und das Objekt wurde noch nicht erstellt und linke hand befindet sich auf auswahlfeld
                        {
                            klickRH = false;
                            objInHand = true;                                               //soll jetzt der Hand folgen
                            aktuellesObj = Objektverwaltung.createObj(auswahl, gamer, pos); //aktuelles Objekt wird erzeugt
                        }

                        if (objInHand)//Ausrichten des Obj
                        {
                            Vector3 rH = new Vector3(rHv2w.X, rHv2w.Y, -5f); //Handvektor ohne Tiefenveränderung
                            aktuellesObj.setPosition(rH);                 //Objektposition wird auf Handgelegt

                            Objektverwaltung.orientObj(aktuellesObj, lHv2w.X, lHv2w.Y);
                        }

                        if (klickRH && objInHand)                //wenn sich ein Objekt in der Hand befindet und erneut geklickt wird
                        {
                            klickRH = false;
                            objInHand = false;                          //Bekommt nicht mehr die Posiotion der hand -> fällt

                            if (currentState == States.Bauphase1O)
                            {
                                prewState = States.Bauphase1O;              //Statewechsel
                                currentState = States.Bauphase1T;
                            }
                            else
                            {
                                prewState = States.Bauphase2O;
                                currentState = States.Bauphase2T;
                            }
                        }
                    }
                    #endregion

                    #region Waffe erzeugen und mit Hand positionieren
                    if (showWaffe)
                    {
                        if (klickRH && !objInHand && auswahl != 0 && auswahl < 5)    //"klick" und die Waffe wurde noch nicht erstellt und linke hand befindet sich auf auswahlfeld
                        {
                            klickRH = false;
                            objInHand = true;                                                   //soll jetzt der Hand folgen

                            aktuelleWaffe = Objektverwaltung.createWaffe(auswahl, gamer, rHv2w);  //aktuelles Objekt wird erzeugt
                            if (spieler1 == gamer)
                            {
                                spieler1.setWaffen(aktuelleWaffe);                                          //Waffe der Waffenliste des Spieler hinzufügen
                            }
                            else
                            {
                                spieler2.setWaffen(aktuelleWaffe);
                            }
                        }

                        
                        if (objInHand && showWaffe == true)                                              //Ausrichten der Waffe
                        {
                            Vector3 rH = new Vector3(rHv2w, -5f);                                       //Handvektor ohne Tiefenveränderung
                            aktuelleWaffe.getModelObject().Position = rH;                               //Waffenposition wird auf Handgelegt
                        }

                        if (klickRH && objInHand)                                                         //wenn sich ein Objekt in der Hand befindet und erneut geklickt wird
                        {
                            klickRH = false;
                            objInHand = false;
                        }
                    }
                    
                    #endregion

                    #region Wechsel von der Objekt zur Waffenauswahl
                    if (klickRH && objInHand == false && auswahl == 5 && showWaffe == false) //"klick" und das Objekt wurde noch nicht erstellt und linke hand befindet sich auf auswahlfeld
                    {
                        klickRH = false;
                        showWaffe = true;
                    }
                    else if (klickRH && objInHand == false &&  auswahl == 5 && showWaffe)
                    {
                        klickRH = false;
                        showWaffe = false;
                    }
                    #endregion
                    
                    #region Übergangsbedingungen
                    //Wenn Spieler nicht ausreichend Geld hat (oder auf weiter Klickt => in Kinect realisiert)
                    if (gamer.getMoney() < level.getMinMoney() && objInHand == false)
                    {
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

                        if (currentState == States.Bauphase1O)
                        {
                            prewState = States.Bauphase1O;
                            if (spieler2.getMoney() < level.getMinMoney() && spieler2.getMoney() < spieler1.getMoney())
                            {
                                currentState = States.Schussphase1;
                            }
                            else
                            {
                                currentState = States.Camto2;
                            }
                        }
                        else //Bauphase2O
                        {
                            prewState = States.Bauphase2O;
                            if (spieler1.getMoney() < spieler2.getMoney())
                            {
                                currentState = States.Schussphase2;
                            }
                            else
                            {
                                currentState = States.Camto1;
                            }
                        }
                    }
                    #endregion
                    
                    break;

                #endregion

                #region Texturenmenüs
                //Bauphase, Spiele 1, Objekte erstellen
                case States.Bauphase1T:
                case States.Bauphase2T:
                    aktuallisiereZeit(gameTime);
                    Objektverwaltung.firstMaterial(aktuellesObj, auswahl);

                    if (currentState == States.Bauphase1T)
                    {
                        gamer = spieler1;
                    }
                    else 
                    {
                        gamer = spieler2;
                    }

                    if (klickRH) //Übergang wird mit klick erzeugt
                    {
                        klickRH = false;
                        #region Kosten dem Spieler abziehen
                        if (aktuellesObj.getMaterial() == "MHolz")
                        { } //kostenlos
                        else if (aktuellesObj.getMaterial() == "MStein")
                        {
                            gamer.setMoney(spieler1.getMoney() - 50);
                            setShowGeld(-50, 100);           //Kosten visualisieren
                        }
                        else if (auswahl == 3)
                        {
                            gamer.setMoney(spieler1.getMoney() - 100);
                            setShowGeld(-100, 100);           //Kosten visualisieren
                        }
                        else if (auswahl == 4)
                        {
                            gamer.setMoney(spieler1.getMoney() - 200);
                            setShowGeld(-200, 100);           //Kosten visualisieren
                        }
                        #endregion

                        if (currentState == States.Bauphase1T)
                        {
                            prewState = States.Bauphase1T;
                            currentState = States.Bauphase1O;
                        }
                        else
                        {
                            prewState = States.Bauphase2T;
                            currentState = States.Bauphase2O;
                        }
                    }
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
                                currentState = States.Schussphase2;
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

                #region Schussphasen
                //Schussphasen
                case States.Schussphase2:
                case States.Schussphase1:

                    
                    aktuallisiereZeit(gameTime);
                    detecting = true;               //Kinect aktiv
                    int xR;

                    if (currentState == States.Schussphase1)
                    {
                        gamer = spieler1;
                        xR = 1;
                    }
                    else
                    {
                        gamer = spieler2;
                        xR = -1;
                    }
                    #region Schussfunktion //shoot Funktion TODO: "auslagern"
                    if (gamer.getWaffen() != 0)//Wenn der Spieler Waffen hat
                    {   
                        aktuelleWaffe = Objektverwaltung.getWaffe(gamer, firedWaffen);
                        aktuelleWaffe.setWinkel(rHv2n.Y);//Setzt Winkel der Kanone in Waffen

                        if (klickRH)
                        {
                            klickRH = false;
                            float schusswinkel;
                            float x;
                            float y;
                            float velocity;
                            
                            bullet = new SphereObject(new Vector3(aktuelleWaffe.getPosition().X, aktuelleWaffe.getPosition().Y,aktuelleWaffe.getPosition().Z), 0.1f, 10, 10, 0.05f);
                            Vector3 shootdirection = new Vector3();
                            Scene.Add(bullet);
                            
                            schusswinkel = aktuelleWaffe.getWinkel();
                            x=(float)Math.Cos(schusswinkel);
                            y=(float)Math.Sin(schusswinkel);
                            shootdirection = new Vector3(x,y,0);

                            velocity = -lHv2n.Y * 10f;
                            bullet.Physics.LinearVelocity = shootdirection * velocity * xR;

                            firedWaffen++;
                            bulletInAir = true;
                        }
                    }

                    if (bulletInAir)
                    {
                       
                        cameraMovement.chaseBullet(bullet.Position, cam.Position);
                        
                        bullet.Collided +=new EventHandler<CollisionArgs>(bulletCollidedHandler);

                        //Partikel Effekte FUNKTIONIERT NOCH NICHT
                        ParticleEffect effect = new ParticleEffect()
                        {
                            Emitters = new EmitterCollection()
                                {   
                                    new SphereEmitter
                                    {
                                        Name="Flame",
                                        Budget = 100,
                                        Term = 0.5f,
                                        ReleaseQuantity = 8,
                                        Enabled = true,
                                        ReleaseSpeed = new Range(5f,5f),
                                        ReleaseColour = new ColourRange
                                        {
                                            Red = new Range(0.9f,1f),
                                            Green = new Range(0.5f,0.5f),
                                            Blue = new Range(0f,0f),
                                        },
                                        ReleaseOpacity = new Range(1f,1f),
                                        ReleaseScale = new Range(2f,2f),
                                        ReleaseRotation = new RotationRange
                                        {
                                            Pitch = new Range(0f,0f),
                                            Yaw = new Range(0f,0f),
                                            Roll = new Range(-3.14f,3.14f),
                                        },
                                        ParticleTexture = Core.Content.Load<Texture2D>("Flames"),
                                        BlendMode = EmitterBlendMode.Add,
                                        Radius = 3f,
                                        Shell = true,
                                        Radiate = true,                        
                                        BillboardStyle = ProjectMercury.BillboardStyle.Spherical,
                                        Modifiers = new ModifierCollection
                                        {
                                            new OpacityInterpolator2
                                            {
                                                InitialOpacity = 0.5f,                                
                                                FinalOpacity = 0f,
                                            },
                                            new RotationModifier
                                            {
                                                RotationRate = new Vector3(0,0,1)
                                            }
                                        },
                                        Controllers = new ControllerPipeline
                                        {
                                            new CooldownController
                                            {
                                                CooldownPeriod = 0.02f,
                                            },
                                        }
                                    }
                                }
                        };

                        ParticleObject particle = new ParticleObject(bullet.Position, effect);
                        
                       


                    }
                    
                    #endregion

                    #region Übergangsbedingungen
                    //Wenn alle Waffen abgefeuert wurden...
                    if (firedWaffen == gamer.getWaffen())
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

                        //Wenn die Schussphase durch ist, beginnt die Bauphase
                        if (schussphasenDurch)
                        {
                            if (currentState == States.Schussphase1)
                            {
                                prewState = States.Schussphase1;
                                schussphasenDurch = false;
                                currentState = States.Bauphase1O;
                            }
                            else
                            {
                                prewState = States.Schussphase2;
                                schussphasenDurch = false;
                                currentState = States.Camto1;
                            }
                        }

                        //Sonst in die andere Schussphase wechseln
                        else
                        {
                            if (currentState == States.Schussphase1)
                            {
                                prewState = States.Schussphase1;
                                schussphasenDurch = true;           //nach der Schussphase2 ist die Schussphase beendet
                                currentState = States.Camto2;
                            }
                            else
                            {
                                prewState = States.Schussphase2;
                                schussphasenDurch = true;           //nach der Schussphase1 ist die Schussphase beendet
                                currentState = States.Camto1;
                            }
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


            #region WEITER
            //Wenn sich die rechte Hand in der oberen, rechten Ecke befindet & KLICK -> Klick auf WEITER
            if (rHv2n.X >= 0.9f && rHv2n.Y >= 0.9f && klickRH)
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

            #region Update Ende

            /*
            if(auswahl==0)
            {
                klickLH = klickRH = false;
            }
            */

            objState = currentState; //Am Ende jenden Updates wird der State angeglichen

            screenDim = new Vector2(Scene.Game.Window.ClientBounds.Width, Scene.Game.Window.ClientBounds.Height);

            Objektverwaltung.refreshObj(spieler1,spieler2); //Entfernt Objekte ohne LP
         
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            #endregion

        }

        private void bulletCollidedHandler(object sender, CollisionArgs e)
        {
            bullet.Collided -= new EventHandler<CollisionArgs>(bulletCollidedHandler);

            object scn = this.Scene.Find("Welt" + e.Collider.ID);
            if (e.Collider == scn)//level.GetType() == e.Collider.GetType())
            {
                if (this.Scene.SceneObjects.Contains(bullet))
                {

                    bulletInAir = false;
                    Scene.Remove(bullet);
                    cameraMovement.move(zeit, 3000, PosX1, level.getSpieler1Pos()); //TODO Kamera fahrt noch ändern
                }
            }
            //TODO: Objektkollision

            /*
            if (e.Collider is Objekte)
            {
                
                e.Collider.decreaseLP();
                Scene.Remove(bullet);
                cameraMovement.move(zeit, 3000, PosX1, level.getSpieler1Pos());//TODO Kamera fahr noch ändern
                bulletInAir = false;
            }
             * */
        }
      
        public override void HandleInput(InputState input)
        {
            #region Wenn Spieler eine Waffe abgefeuert hat (Hier noch mit S realisiert)
            if (input.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.S, PlayerIndex.One))
            {
                shoot=true;
            }
            else
            {
                shoot = false;
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
                wobinich = "Bau1 Obj"+ rHv2n + rHv2s + rHv2w;
            }
            else if (currentState == States.Bauphase1T)
            {
                wobinich = "Bau1 Tex" + auswahl + aktuellesObj.getMaterial();
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
                Objektverwaltung.Geldanzeige(spieler1);  //Blendet die Geldbetrag des Spielers ein
            }
            if (currentState == States.Bauphase2O || currentState == States.Bauphase2T || currentState == States.Schussphase2)
            {
                Objektverwaltung.Geldanzeige(spieler2); //Blendet die Geldbetrag des Spielers ein
            }
            #endregion

            #region Handpos
            if (currentState == States.Bauphase1O || currentState == States.Bauphase1T || currentState == States.Bauphase2O || currentState == States.Bauphase2T)
            {
                Handkreise(rHv2s, lHv2s);
            }
            #endregion

            #region Weiterbutton
            if (currentState == States.Bauphase1O || currentState == States.Bauphase2O)
            {
                Vector2 dim = new Vector2((screenDim.X * 0.09f), (screenDim.Y * 0.09f));
                Vector2 pos = new Vector2(screenDim.X - dim.X - screenDim.X * 0.01f, screenDim.Y - dim.Y - screenDim.Y * 0.01f);
                drawBox(pos, dim, "weiter2");
            }
            #endregion

            #region Bau/Tex/Waf - Auswahl
            if (currentState == States.Bauphase1O || currentState == States.Bauphase1T || currentState == States.Bauphase2O || currentState == States.Bauphase2T)
            {
                String bild;
                if (currentState == States.Bauphase1T || currentState == States.Bauphase2T)
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

                Vector2 dim = new Vector2(screenDim.X * 0.5f, screenDim.Y * 0.15f);
                Vector2 pos = new Vector2(dim.X * 0.5f, 5);
                drawBox(pos, dim, bild);

            }
            #endregion

            #region Obj-Waffen Switch
            if (currentState == States.Bauphase1O || currentState == States.Bauphase2O)
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

                Vector2 dim = new Vector2(screenDim.X * 0.125f, screenDim.Y * 0.15f);
                Vector2 pos = new Vector2((screenDim.X * 0.875f) - 5, 5);
                drawBox(pos, dim, bild);
            }
            #endregion

            #region showGeld
            if (showGeld[1] > 0)
            {
                geldFliegt(showGeld[0],new Vector2(showGeld[1],showGeld[1]),showGeld[1]);
                showGeld[1]--;
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

        private void Handkreise(Vector2 posL, Vector2 posR)
        {
            Point centerL = new Point((int)posL.X,(int)posL.Y);
            UI2DRenderer.DrawCircle(centerL, 20, Color.Green);

            Point centerR = new Point((int)posR.X, (int)posR.Y);
            UI2DRenderer.DrawCircle(centerR, 20, Color.Red);
        }

        private void drawBox(Vector2 pos, Vector2 dim, String bild)
        {
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)dim.X, (int)dim.Y);
            Texture2D texture = Core.Content.Load<Texture2D>(bild);
            UI2DRenderer.FillRectangle(rect, texture, Color.White);
        }

        private void aktuallisiereZeit(GameTime gameTime)
        {
            zeit = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000 - Zeit1;
        }

        private void geldFliegt(int betrag, Vector2 pos, int prozent)
        {
            Color farbe = (betrag > 0) ? Color.Green : Color.Red;
            String text = (betrag > 0) ? "+" + betrag.ToString() : betrag.ToString();
            UI2DRenderer.WriteText(new Vector2(pos.X, pos.Y),
                text, farbe, null, new Vector2(0.01f*prozent, 0.01f*prozent));
        }

        public static void setShowGeld(int betrag, int prozent)
        {
            showGeld = new int[2] { betrag, prozent };
        }

    }


} 
