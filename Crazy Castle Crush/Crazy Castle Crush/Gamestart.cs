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
using BEPUphysics.Constraints.TwoEntity.Joints;




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
            Wackel,
            End
        }

        #region Variablen (Deklarationen)

        States currentState;                                //aktueller Zustand
        States prewState;                                   //vorheriger Zustand
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
        Vector3 rocketDirection = new Vector3(0, 1, 0);
        bool showWaffe;                                     //Gibt an ob gerade die Waffen angezeigt werden sollen
        bool objInHand;                                     //solange das Objekt an der Hand ist
        int auswahl;                                        //je nach Position der linken Hand erhält die Auswahl ihre Werte (für Objekt und Texturauswahl)  
        bool klickRH;                                       //klick der rechten Hand
        Objekte aktuellesObj;                               //Objekt das gerade bearbeitet wird
        Waffen aktuelleWaffe;                               //Waffe die gerade bedient wird
        CameraObject cam;                                   //camera
        SceneObject bullet;                                 //Geschoss  
        bool bulletInAir;
        int klickCounter = 0;
        float shootTimer = 0;
        public string winner;
        GameTime GameT;
        string hilfsstring;
        



        //Benötigt für die einblendung von Auswahlmenu
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
            BEPUphysics.Settings.CollisionDetectionSettings.AllowedPenetration = 0.05f;
            BEPUphysics.Settings.CollisionDetectionSettings.DefaultMargin = 0.05f;
            BEPUphysics.Settings.CollisionResponseSettings.MaximumPenetrationCorrectionSpeed /= 2; 
            Scene.ShowCollisionMeshes = true;
            base.Initialize();
            Scene.ShowFPS = true;
            Scene.ShowObjectOrigin = true;
            Scene.ShowGrid = true;
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
            
            if (Scene.Kinect.SkeletonDataReady && bulletInAir == false)
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

                            #region Klick
                            if (klickCounter<60)
                            {
                                klickCounter++;
                            }

                            try
                            {
                                if (skeleton.HandPointers[1].IsTracked == true)
                                {
                                    if (skeleton.HandPointers[1].HandEventType == InteractionHandEventType.GripRelease && klickCounter >= 60)
                                    {
                                        klickRH = true;
                                        klickCounter = 0;
                                    }
                                    else
                                    {
                                        klickRH = false;
                                    }
                                }
                            } catch { };
                            #endregion
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

                            //klickLH = (skeleton.HandPointers[1].HandEventType == InteractionHandEventType.Grip);
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
                    Objektverwaltung.createKing(level);

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
                    //firedWaffen = 0;                        
                    
                    //Kamera wird bewegt
                    cameraMovement.move(zeit,3000,PosX1, level.getSpieler1Pos());

                #region Übergangsbedingungen
                    //Wenn die Spielerposition 1 erreicht wurde startet die Bauphase/Schussphase
                    if (Scene.Camera.Position.X < level.getSpieler1Pos()+0.5f)
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

                            BEPUphysics.Constraints.TwoEntity.Joints.PointOnPlaneJoint Objektwird2D = new BEPUphysics.Constraints.TwoEntity.Joints.PointOnPlaneJoint(null, aktuellesObj.getSceneObject().Physics, new Vector3(0, 0, -5f), Vector3.Forward, aktuellesObj.getPosition()); 
                            Scene.Physics.Add(Objektwird2D);

                            RevoluteAngularJoint objRotiertNicht = new RevoluteAngularJoint(null, aktuellesObj.getSceneObject().Physics, new Vector3(0, 0, 1)); 
                            Scene.Add(objRotiertNicht);


                            aktuellesObj.getSceneObject().Collided +=new EventHandler<CollisionArgs>(Box_Collided);
                            aktuellesObj.getSceneObject().Physics.LinearVelocity = Vector3.Zero;

                            aktuellesObj.getSceneObject().Physics.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;

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

                        
                        if (objInHand)                                              //Ausrichten der Waffe
                        {
                            Vector3 rH = new Vector3(rHv2w, -5f);                                       //Handvektor ohne Tiefenveränderung
                            aktuelleWaffe.setPosition(rH);                                              //Waffenposition wird auf Handgelegt
                        }

                        if (klickRH && objInHand)                                                         //wenn sich ein Objekt in der Hand befindet und erneut geklickt wird
                        {
                            aktuelleWaffe.getModelObject().Physics.LinearVelocity = Vector3.Zero;
                            aktuelleWaffe.getModelObject().PhysicsMaterial.KineticFriction = 0;
                            aktuelleWaffe.getModelObject().Physics.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Discrete;

                            BEPUphysics.Constraints.TwoEntity.Joints.PointOnPlaneJoint Objektwird2D = new BEPUphysics.Constraints.TwoEntity.Joints.PointOnPlaneJoint(null, aktuelleWaffe.getModelObject().Physics, new Vector3(0, 0, -5f), Vector3.Forward, aktuelleWaffe.getPosition());
                            Scene.Physics.Add(Objektwird2D);

                            RevoluteAngularJoint objRotiertNicht2 = new RevoluteAngularJoint(null, aktuelleWaffe.getModelObject().Physics, new Vector3(0, 0, 1)); 
                            Scene.Add(objRotiertNicht2);

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
                    //firedWaffen = 0; 

                    //Kamera wird bewegt
                    cameraMovement.move(zeit, 3000, PosX1, level.getSpieler2Pos());

                #region Übergangsbedingungen
                    //Wenn die Spielerposition 2 erreicht wurde startet die Bauphase/Schussphase
                    if (Scene.Camera.Position.X > level.getSpieler2Pos()-0.5f)
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

                    #region Spieler und Richtung
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
                    #endregion
                    hilfsstring = gamer.getWaffen().ToString();

                    #region Schussfunktion //shoot Funktion TODO: "auslagern"
                    #region Abschießen
                    if (gamer.getWaffen() != 0 && !bulletInAir && gamer.getWaffen() > firedWaffen)//Wenn der Spieler Waffen hat
                    {
                        aktuelleWaffe = Objektverwaltung.getWaffe(gamer, firedWaffen);
                        aktuelleWaffe.setWinkel(rHv2n.Y);//Setzt Winkel der Kanone in Waffen

                        if (klickRH)
                        {
                            klickRH = false;

                            bullet = aktuelleWaffe.shoot(Scene, lHv2n.Y,xR);    //bullet wir 0.5f über die Waffe gesetzt
                            aktuelleWaffe.UpdatePhysics();

                            shootTimer = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000;
                            bullet.Collided += new EventHandler<CollisionArgs>(bulletCollidedHandler);

                            firedWaffen++;
                            bulletInAir = true;
                        }
                        
                    }
                    #endregion
                    #region bullet in Air
                    if (bulletInAir)
                    {
                        float aktTime = (float)gameTime.TotalGameTime.TotalMilliseconds - shootTimer;
                        if (aktTime < 10000)
                        {
                            if (bullet.Position.Y > -2)
                            {
                                if ((gamer == spieler1 && bullet.Position.X < level.getSpieler2Pos()+3) || (gamer == spieler2 && bullet.Position.X > level.getSpieler1Pos()-3))
                                {
                                    cameraMovement.chaseBullet(bullet.Position, cam.Position);
                                    if (bullet.Name.Contains("Rakete"))
                                    {
                                        rocketDirection.X += (rHv2n.Y - 0.5f) * (-0.2f); 
                                        rocketDirection.Normalize();
                                        bullet.Orientation = Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.Atan(rocketDirection.X / rocketDirection.Y));
                                        bullet.Physics.LinearVelocity = rocketDirection * 5;


                                        //Vector3 rocketDirection = new Vector3(0,1,0)
                                    }
                                }
                                else
                                {
                                    AfterBulletHit();
                                }
                            }
                            else
                            {
                                AfterBulletHit();
                            }
                        }
                        else
                        {
                            AfterBulletHit();
                        }
                        
                        


                        #region Partikel
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
                        #endregion Partikel
                    #endregion
                    }
                    
                    #endregion

                    #region Übergangsbedingungen
                    //Wenn alle Waffen abgefeuert wurden...
                    if (firedWaffen >= gamer.getWaffen() && !bulletInAir)
                    {
                        //setzt die Variable PosX1 auf die Position bevor er in den nächsten State wechselt 
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Minutes * 60 * 1000; //Zeit zwischenspeichern
                        aktuallisiereZeit(gameTime);

                        firedWaffen = 0;

                        currentState = logik.uebergang(currentState, spieler1, spieler2, level);
                    }

                    #endregion

                    break;

                #endregion

                #region Wackel
                case States.Wackel:
                    aktuallisiereZeit(gameTime);

                    if (zeit > 950 && zeit < 1050)
                    {
                        Objektverwaltung.refreshObj(spieler1, spieler2); //Entfernt Objekte ohne LP
                    }

                    cameraMovement.wackel(zeit, 2000);
                    
                    if (zeit > 2000)
                    {
                        PosX1 = Scene.Camera.Position.X;
                        Zeit1 = (float)GameT.TotalGameTime.TotalMilliseconds;
                        aktuallisiereZeit(GameT);

                        if (prewState == States.Schussphase1)
                        {
                            logik.setZielStatebyHand(States.Schussphase1);
                            currentState = States.Camto1;
                        }
                        else
                        {
                            logik.setZielStatebyHand(States.Schussphase2);
                            currentState = States.Camto2;
                        }
                    }
                    break;
                #endregion

                #region End
                //Ende des Spiels
                case States.End:
                    string text = winner + " hat gewonnen!";
                    Textanzeiger(text);
                    
                    if (klickRH)
                    {
                        LoadingScreen.Load(Core.ScreenManager, true, PlayerIndex.One, new Menu()); //erstes Level wird geladen
                    }

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

            GameT = gameTime;

            screenDim = new Vector2(Scene.Game.Window.ClientBounds.Width, Scene.Game.Window.ClientBounds.Height);
         
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            #endregion

        }

        private void bulletCollidedHandler(object sender, CollisionArgs e)
        {
            bullet.Collided -= new EventHandler<CollisionArgs>(bulletCollidedHandler);
            Spieler spieler;
            if(currentState== States.Schussphase1)
            {
                spieler = spieler1;
            }
            else
            {
                spieler = spieler2;
            }
            winner = Collided.Zerstören(sender, e, Scene, currentState, spieler);
            if (!winner.Equals(""))
            {
                currentState = States.End;
            }
            AfterBulletHit();
        }
        private void Box_Collided(object sender, CollisionArgs e)
        {
            ((SceneObject)sender).Physics.AngularVelocity = Vector3.Zero;
        }

        public void AfterBulletHit()
        {
            bulletInAir = false;
            if (Scene.Contains(bullet))
            {
                Scene.Remove(bullet);
            }

            PosX1 = Scene.Camera.Position.X;
            Zeit1 = (float)GameT.TotalGameTime.TotalMilliseconds;
            aktuallisiereZeit(GameT);

            prewState = currentState;
            currentState = States.Wackel;
        }

        public override void Draw(GameTime gameTime)
        {
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
            zeit = (float)gameTime.TotalGameTime.TotalMilliseconds - Zeit1;
        }



    }


} 
