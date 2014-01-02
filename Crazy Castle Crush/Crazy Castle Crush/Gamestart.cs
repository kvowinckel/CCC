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
        int firedWaffen=0;                                    //Anzahl der abgefeuerten Waffen in einer Schussphase
        bool detecting = false;                             //Kinect benötigt
        BoxObject rightHand;
        BoxObject leftHand;
        BoxObject weiterSym;
        BoxObject auswahlanzeige;                           //zur Auswahl von Objekten 
        BoxObject objWafC;                                  //Objekt zum wechseln zwischen Waffen und Objekten
        bool showWaffe;                                  //Gibt an ob gerade die Waffen angezeigt werden sollen
        int auswahl;                                        //je nach Position der linken Hand erhält die Auswahl ihre Werte (für Objekt und Texturauswahl)
        bool klick;                                         //Wenn Spieler quasi klickt (noch Leertaste)
        bool shoot;                                         //Wenn Spieler schießt (noch S)
        bool getObj;                                        //True wenn die Objektauswahl bestätigt wird
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

            //Objecte
            startObjects = new StartObjects(Scene, level);
            Objektverwaltung.setObjektverwaltung(Scene,level);



            //Hände bekommen transparente Box
            rightHand = startObjects.RightHand();
            leftHand = startObjects.LeftHand();

            weiterSym = startObjects.Weiter();

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
                    auswahlanzeige = startObjects.showObjects("Bau");
                    objWafC = startObjects.LoadObjWafC();

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
                    weiterSym.Visible = false;
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
                    detecting = true;               //Kinect aktiv
                    if (!weiterSym.Visible)
                    {
                        weiterSym.Visible = true;
                    }
                    float pos;

                    #region Spieler &  Spielerposition
                    if (currentState == States.Bauphase1O)
                    {
                        gamer = spieler1;
                        pos = level.getSpieler1Pos();
                        weiterSym.Position = new Vector3(pos+1.13f, -0.7f, -2f);
                    }
                    else
                    {
                        gamer = spieler2;
                        pos = level.getSpieler2Pos();
                        weiterSym.Position = new Vector3(pos + 1.13f, -0.7f, -2f);
                    }
                    #endregion

                    #region Objekt erzeugen und mit Hand positionieren
                    if (!showWaffe)
                    {
                        if (getObj && objInHand == false && auswahl != 0 && auswahl < 5)    //"klick" und das Objekt wurde noch nicht erstellt und linke hand befindet sich auf auswahlfeld
                        {
                            objInHand = true;                                               //soll jetzt der Hand folgen
                            aktuellesObj = Objektverwaltung.createObj(auswahl, gamer, pos); //aktuelles Objekt wird erzeugt
                        }

                        if (objInHand)//Ausrichten des Obj
                        {
                            Vector3 rH = new Vector3(rightHand.Position.X, rightHand.Position.Y, -5f); //Handvektor ohne Tiefenveränderung
                            aktuellesObj.setPosition(rH);                 //Objektposition wird auf Handgelegt

                            Objektverwaltung.orientObj(aktuellesObj, leftHand.Position.X, leftHand.Position.Y);

                            rightHand.Visible = false;                  //Anzeige der rechten Hand deaktiviert
                        }

                        if (klick && objInHand == true)                //wenn sich ein Objekt in der Hand befindet und erneut geklickt wird
                        {
                            rightHand.Visible = true;                   //Rechte Hand wird wieder angezeigt
                            klick = false;
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


                        if (getObj && objInHand == false && auswahl != 0 && auswahl < 5)    //"klick" und die Waffe wurde noch nicht erstellt und linke hand befindet sich auf auswahlfeld
                        {
                            objInHand = true;                                                   //soll jetzt der Hand folgen

                            aktuelleWaffe = Objektverwaltung.createWaffe(auswahl, gamer, rightHand.Position);  //aktuelles Objekt wird erzeugt
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
                            Vector3 rH = new Vector3(rightHand.Position.X, rightHand.Position.Y, -5f);  //Handvektor ohne Tiefenveränderung
                            aktuelleWaffe.setPosition(rH);                                              //Waffenposition wird auf Handgelegt
                            
                            rightHand.Visible = false;                                                  //Anzeige der rechten Hand deaktiviert
                            
                        }

                        if (klick && objInHand)                                                         //wenn sich ein Objekt in der Hand befindet und erneut geklickt wird
                        {
                            rightHand.Visible = true;                                                   //Rechte Hand wird wieder angezeigt
                            klick = false;
                            objInHand = false;
                            
                        }

                    }
                    
                    #endregion

                    #region Wechsel von der Objekt zur Waffenauswahl
                    if (klick&&objInHand == false && auswahl == 5 && showWaffe == false) //"klick" und das Objekt wurde noch nicht erstellt und linke hand befindet sich auf auswahlfeld
                    {
                        showWaffe = true;
                    }
                    else if (klick && objInHand == false && auswahl == 5 && showWaffe)
                    {
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
                    weiterSym.Visible = false;

                    if (currentState == States.Bauphase1T)
                    {
                        gamer = spieler1;
                    }
                    else 
                    {
                        gamer = spieler2;
                    }

                    if (klick) //Übergang wird mit klick erzeugt
                    {
                        #region Kosten dem Spieler abziehen
                        if (aktuellesObj.getMaterial() == "MHolz")
                        { } //kostenlos
                        else if (aktuellesObj.getMaterial() == "MStein")
                        {
                            gamer.setMoney(spieler1.getMoney() - 50);
                        }
                        else if (auswahl == 3)
                        {
                            gamer.setMoney(spieler1.getMoney() - 100);
                        }
                        else if (auswahl == 4)
                        {
                            gamer.setMoney(spieler1.getMoney() - 200);
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
                    weiterSym.Visible = false;

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
                    weiterSym.Visible = false;
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
                    if (gamer.getWaffen() != 0)
                    {   
                        aktuelleWaffe = Objektverwaltung.getWaffe(gamer, firedWaffen);
                        aktuelleWaffe.setWinkel(rightHand.Position.Y);

                        if (klick==true)
                        {
                           float schusswinkel,x,y,velocity;
                           Vector3 spawnpoint = new Vector3 ( rightHand.Position.X,rightHand.Position.Y, rightHand.Position.Z); //Spawnposition nur Vorübergehend sollte am Objekt sein!
                           bullet = new SphereObject(spawnpoint, 0.1f, 10, 10, 0.05f);
                           Vector3 shootdirection = new Vector3();
                           Scene.Add(bullet);
                           
                           schusswinkel = aktuelleWaffe.getWinkel();
                           x=(float)Math.Cos(schusswinkel);
                           y=(float)Math.Sin(schusswinkel);
                           shootdirection = new Vector3(x,y,0);
                           velocity = leftHand.Position.Y * 10f;
                           bullet.Physics.LinearVelocity = shootdirection * velocity;
                           firedWaffen++;
                           bulletInAir = true;
                           
                           
                        }
        
                        
                        
                    }
                    if (bulletInAir)
                    {
                        cameraMovement.chaseBullet(bullet.Position, cam.Position);

                        


                        


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




            #region Objekt-/ Texturauswahl ein-/ausblenden
            if (currentState != States.Menu && currentState != States.Start && currentState != States.End)
            {
                if (currentState == States.Bauphase1O)
                {
                    if (showWaffe)
                    {
                        startObjects.einausblender(auswahlanzeige, objWafC, 11, zeit);
                    }
                    else
                    {
                        startObjects.einausblender(auswahlanzeige, objWafC, 1, zeit);
                    }
                }
                else if (currentState == States.Bauphase1T)
                {
                    showWaffe = false;
                    startObjects.einausblender(auswahlanzeige, objWafC, 12, zeit);
                }
                else if (currentState == States.Bauphase2O)
                {
                    if (showWaffe)
                    {
                        startObjects.einausblender(auswahlanzeige, objWafC, 21, zeit);
                    }
                    else
                    {
                        startObjects.einausblender(auswahlanzeige, objWafC, 2, zeit);
                    }
                }
                else if (currentState == States.Bauphase2T)
                {
                    showWaffe = false;
                    startObjects.einausblender(auswahlanzeige, objWafC, 22, zeit);
                }
                else
                {
                    startObjects.einausblender(auswahlanzeige, objWafC, 0, zeit);
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

                                #region getObj
                                if (normScreenPos.X >= 0.2f && normScreenPos.X <= 0.8f && normScreenPos.Y <= 0.1f)
                                {
                                    getObj = true;
                                }
                                else {getObj = false; }
                                #endregion

                                #region WEITER klick
                                //Wenn sich die rechte Hand in der oberen, rechten Ecke befindet & KLICK -> Klick auf WEITER
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
                                Plane plane2 = new Plane(Vector3.Forward, -1f);

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
                                Vector2 normScreenPos = new Vector2(screenPos.X / Scene.Game.Window.ClientBounds.Width, screenPos.Y / Scene.Game.Window.ClientBounds.Height);

                                //Hintergrund bewegen
                                startObjects.MoveBackground(normScreenPos.X - 0.5f, normScreenPos.Y - 0.5f);
                                


                            }
                            
                            #endregion

                        }
                    }

                }
            }
            else
            {
                
            }
            #endregion

            

            objState = currentState; //Am Ende jenden Updates wird der State angeglichen

            Objektverwaltung.refreshObj(spieler1,spieler2); //Entfernt Objekte ohne LP
         
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      
        }




        public override void HandleInput(InputState input)
        {

            #region Wenn Spieler Auswählt (Hier Leertaste)
            if (input.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space, PlayerIndex.One))
            {
                klick = true;
            }
            else
            {
                klick = false;
            }
            #endregion

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
                wobinich = "Bau1 Obj"+ auswahl + "    " + weiterSym.Visible + weiterSym.Position;
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
                Objektverwaltung.Geldanzeige(spieler1);  //Blendet die Geldbetrag des Spielers ein
            }
            if (currentState == States.Bauphase2O || currentState == States.Bauphase2T || currentState == States.Schussphase2)
            {
                Objektverwaltung.Geldanzeige(spieler2); //Blendet die Geldbetrag des Spielers ein
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
