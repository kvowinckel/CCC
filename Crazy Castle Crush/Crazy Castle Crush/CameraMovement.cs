using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Crazy_Castle_Crush
{
    class CameraMovement
    {
        //Erzeut eine Bewegung von Start zum Zielpunkt. Benötigt runTime und exeTime, Zeit in der die Bewegung ausgeführt werden soll
        //Das ganze mit einer Sin-Funktion versehen, damit eine Beschleunigung und abbremsung der Kamera realisiert wird
        static public float getXMovement(float runTime, int exeTime, float startXPos, float zielXPos)
        {
            return startXPos + (zielXPos-startXPos) * (float)Math.Sin(Math.PI * runTime / (2*exeTime));
        }

        static public float getZMovement(float runTime, int exeTime)
        {
            return (float)Math.Sin(runTime * Math.PI / exeTime);
        }
    }
}
