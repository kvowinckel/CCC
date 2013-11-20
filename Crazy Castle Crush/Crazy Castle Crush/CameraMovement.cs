using System;
using NOVA.Scenery;
using Microsoft.Xna.Framework;


namespace Crazy_Castle_Crush
{
    class CameraMovement
    {
        public CameraMovement(CameraObject camera)
        {
            this.camera = camera;
        }

        public void move(float zeit, int dauer, float Ausgangsposition, float Zielposition)
        {
            camera.Position = new Vector3(
                getXMovement(zeit, dauer, Ausgangsposition, Zielposition),
                0,
                getZMovement(zeit, dauer) * 0.8f);

            Matrix cameraRotation = Matrix.CreateRotationX(0) * Matrix.CreateRotationY(0);
            camera.Orientation = Quaternion.CreateFromRotationMatrix(cameraRotation);
        }

        //Erzeut eine Bewegung von Start zum Zielpunkt. Benötigt runTime und exeTime, Zeit in der die Bewegung ausgeführt werden soll
        //Das ganze mit einer Sin-Funktion versehen, damit eine Beschleunigung und abbremsung der Kamera realisiert wird
        static private float getXMovement(float runTime, int exeTime, float startXPos, float zielXPos)
        {
            return startXPos + (zielXPos-startXPos) * (float)Math.Sin(Math.PI * runTime / (2*exeTime));
        }

        static private float getZMovement(float runTime, int exeTime)
        {
            return (float)Math.Sin(runTime * Math.PI / exeTime);
        }

        private CameraObject camera;
    }
}
