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
        //States prewState;                                   //vorheriger Zustand
        float Zeit1;                                        //Zeit nach State
        float zeit;                                         //vergangende Zeit seit letztem State
        float PosX1;                                        //X-Pos nach State
        int firedWaffen=0;                                  //Anzahl der abgefeuerten Waffen in einer Schussphase
        Vector2 rHv2s;                                      //rechte Hand als Vector 2 in ScreenPos
        Vector2 rHv2w;                                      //rechte Hand als Vector 2 in WorldPos
        Vector2 rHv2n;                                      //rechte Hand als Vector 2 in normScreenPos
        Vector2 lHv2s;                                      //linke Hand als Vector 2 in ScreenPos
        Vector2 lHv2w;                                      //linke Hand als Vector 2 in WorldPos
        Vector2 lHv2n;                                      //linke Hand als Vector 2 in normScreenPos
        Vector2 screenDim;                                  //Screen Dimension
        bool showWaffe;                                     //Gibt an ob gerade die Waffen angezeigt werden sollen
        bool objInHand;                                     //solange das Objekt an der Hand ist
        int auswahl;                                        //je nach Position der linken Hand erhält die Auswahl ihre Werte (für Objekt und Texturauswahl)  
        bool klickRH;                                       //klick der rechten Hand
        bool klickLH;                                       //klick der linken Hand
        Objekte aktuellesObj;                               //Objekt das gerade bearbeitet wird
        Waffen aktuelleWaffe;                                //Waffe die gerade bedient wird
        CameraObject cam;                                   //camera
        SphereObject bullet;                                  //Geschoss  
        ModelObject bolzen;
        string Munition;
        bool bulletInAir;
        int klickCounter =0;
       



        //Benötigt für die einblendung von Auswahlmenu
        States objState = States.Start;
        Auswahl Auswahl = new Auswahl();

        
        //Erstellt zwei Spieler und das erste Level
        Spieler spieler1 = new Spieler();
        Spieler spieler2 = new Spieler();
        Spieler gamer;                                      //Zum zwischenspeichern des aktuellen Spielers
        Levels level = new Levels();
        Logik logik = new Logik();

        //Initiallisiert die Klassen
        CameraMovement cameraMovement;
        StartObjects startObjects;
        

        #endregion

        public override void Initialize()
        {
            base.Initialize();
            Scene.ShowFPS = true;
            Scene.ShowObjectOrigin = false;
            Scene.ShowCollisionMeshes = false;
            //Kinect initialisieren
            Scene.InitKinect();

            Scene.Physics.ForceUpdater.Gravity = new Vector3(0,-2.5f,0);            //Definierte Schwerkraft

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
                            
                            if (klickCounter<100)
                            {
                                klickCounter++;
                            }

                            try
                            {

                                if (skeleton.HandPointers[1].IsTracked == true)
                                {
                                    if (skeleton.HandPointers[1].HandEventType == InteractionHandEventType.GripRelease && klickCounter >= 100)
                                    {
                                        klickRH = true;
                                        klickCounter = 0;
                                    }
                                    else
                                    {
                                        klickRH = false;
                                    }

                                }
                            }
                            catch { };

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
                          /*  try
                            {
                               // klickLH = (skeleton.HandPointers[1].HandEventType == InteractionHandEventType.Grip);
                            }
                            catch { };*/
                        }

                        #endregion

                        //Hintergrundsbild verschieben
                        if (bulletInAir == false)
                        {
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
                                realPos = skeleton.Joints[JointType.Head].WorldPosition;

                                #region Zoom Funktionen
                                //ZOOM Funktionen
                                if (currentState == States.Schussphase1 || currentState == States.Schussphase2)
                                {
                                    if (gamer == spieler1)
                                    {
                                        cameraMovement.zoom(realPos.Z, 1, new Vector3(10f, 2f, 15f));
                                    }
                                    if (gamer == spieler2)
                                    {
                                        cameraMovement.zoom(realPos.Z, -1, new Vector3(10f, 2f, 15f));
                                    }
                                }
                                if (currentState == States.Bauphase1O || currentState == States.Bauphase1T || currentState == States.Bauphase2O || currentState == States.Bauphase2T)
                                {
                                    if (gamer == spieler1)
                                    {
                                        cameraMovement.zoom(realPos.Z, 1, new Vector3(1.5f, 0f, 4f));
                                    }
                                    if (gamer == spieler2)
                                    {
                                        cameraMovement.zoom(realPos.Z, -1, new Vector3(1.5f, 0f, 4f));
                                    }
                                }


                                #endregion


                            }
                            #endregion
                        }


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


                    //danach Kamera an Spielerposition 1 bewegen
                    currentState = States.Camto1;

                    break;

                #endregion

                #region Camto1
                //Camto1: Kamera wird an die Linke Position bewegt
                case States.Camto1:
                    aktuallisiereZeit(gameTime);

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

                        currentState = logik.uebergang(currentState, spieler1, spieler2, level);
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
                            aktuellesObj = Objektverwaltung.createObj(auswahl, gamer, pos, rHv2s); //aktuelles Objekt wird erzeugt
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
                                currentState = States.Bauphase1T;
                            }
                            else
                            {
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
                    if (gamer.getMoney() < level.getMinMoney() && objInHand == false && (currentState == States.Bauphase1O ||currentState == States.Bauphase2O))
                    {
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);
                        showWaffe = false; 

                        currentState = logik.uebergang(currentState, spieler1, spieler2, level);
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
                            DrawHelper.setmoney(gamer, -50, rHv2s);
                        }
                        else if (auswahl == 3)
                        {
                            DrawHelper.setmoney(gamer, -100, rHv2s);
                        }
                        else if (auswahl == 4)
                        {
                            DrawHelper.setmoney(gamer, -200, rHv2s);
                        }
                        #endregion

                        if (currentState == States.Bauphase1T)
                        {
                            currentState = States.Bauphase1O;
                        }
                        else
                        {
                            currentState = States.Bauphase2O;
                        }
                    }
                    break;

                #endregion

                #region Camto2
                //Kamera wird an die Rechte Positon bewegt
                case States.Camto2:
                    aktuallisiereZeit(gameTime);

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

                        currentState = logik.uebergang(currentState, spieler1, spieler2, level);
                            }
                #endregion

                    break;

                #endregion

                #region Schussphasen
                //Schussphasen
                case States.Schussphase2:
                case States.Schussphase1:

                    aktuallisiereZeit(gameTime);
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
                    if (gamer.getWaffen() != 0 && !bulletInAir)//Wenn der Spieler Waffen hat
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
                     
                       
                            if (aktuelleWaffe.getType()=="Balliste")
                            {
                                Munition="Bolzen";
                            }
                            else if (aktuelleWaffe.getType() == "Kanone")
                            { 
                                Munition = "Kugel"; 
                            }
                            else { 
                                Munition = "Crap";     
                            }
                            if (Munition =="Kugel")
                            { bullet = new SphereObject(new Vector3(aktuelleWaffe.getPosition().X, aktuelleWaffe.getPosition().Y+2.5f,aktuelleWaffe.getPosition().Z), 0.1f, 10, 10, 0.05f);
                              Scene.Add(bullet);
                            }
                            else if (Munition == "Bolzen")
                            {
                                bolzen = new ModelObject(new Vector3(aktuelleWaffe.getPosition().X, aktuelleWaffe.getPosition().Y + 0.5f, aktuelleWaffe.getPosition().Z), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "Bolzen", 0.05f);
                                Scene.Add(bolzen);
                            }
                            
                            schusswinkel = aktuelleWaffe.getWinkel();
                            x=(float)Math.Cos(schusswinkel);
                            y=(float)Math.Sin(schusswinkel);
                            Vector3 shootdirection = new Vector3(x,y,0);

                            velocity = (1-lHv2n.Y) * 10f;
                            if (Munition == "Kugel")
                            {
                                bullet.Physics.LinearVelocity = shootdirection * velocity * xR;
                            }
                            else if (Munition == "Bolzen")
                            {
                                bolzen.Physics.LinearVelocity = shootdirection * velocity * xR;
                            }
                            

                            firedWaffen++;
                            bulletInAir = true;
                        }
                    }

                    if (bulletInAir)
                    {
                        if (Munition == "Kugel")
                        {
                            cameraMovement.chaseBullet(bullet.Position, cam.Position);
                            bullet.Collided += new EventHandler<CollisionArgs>(bulletCollidedHandler);
                        }
                        else if (Munition == "Bolzen")
                        {
                            cameraMovement.chaseBullet(bolzen.Position, cam.Position);
                            bolzen.Collided += new EventHandler<CollisionArgs>(bulletCollidedHandler);
                        }
                        
                        
                        
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

                        currentState = logik.uebergang(currentState, spieler1, spieler2, level);
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
            if (rHv2n.X >= 0.9f && rHv2n.Y >= 0.4f && rHv2n.Y <= 0.6f && klickRH)
            {
                klickRH = false;
                showWaffe = false;
                //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                PosX1 = Scene.Camera.Position.X;
                Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                aktuallisiereZeit(gameTime);

                currentState = logik.uebergang(currentState, spieler1, spieler2, level);
                    }
            #endregion

            #region Update Ende

            objState = currentState; //Am Ende jenden Updates wird der State angeglichen

            screenDim = new Vector2(Scene.Game.Window.ClientBounds.Width, Scene.Game.Window.ClientBounds.Height);

            Objektverwaltung.refreshObj(spieler1,spieler2); //Entfernt Objekte ohne LP
         
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            #endregion

        }

        private void bulletCollidedHandler(object sender, CollisionArgs e)
        {
            bulletInAir = false;
           

            object scn = this.Scene.Find("Welt" + e.Collider.ID);
            if (e.Collider == scn)//level.GetType() == e.Collider.GetType())
            {
                if (this.Scene.SceneObjects.Contains(bullet))
                {
                    Scene.Remove(bullet);
                    bullet.Collided -= new EventHandler<CollisionArgs>(bulletCollidedHandler);
                    
                    cameraMovement.move(zeit, 3000, PosX1, level.getSpieler1Pos()); //TODO Kamera fahrt noch ändern
                }
            }
            //TODO: Objektkollision

            
            if (e.Collider != scn)
            {
                Objektverwaltung.removeLP(e.Collider);
                
                Scene.Remove(bullet);
                cameraMovement.move(zeit, 3000, PosX1, level.getSpieler1Pos());//TODO Kamera fahr noch ändern
                bulletInAir = false;
            }
             
        }
      
        public override void HandleInput(InputState input)
        {
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

            DrawHelper.run(currentState, rHv2s, lHv2s,screenDim,showWaffe,spieler1,spieler2,bulletInAir);

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
