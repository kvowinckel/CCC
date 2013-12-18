using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crazy_Castle_Crush
{
    public class Spieler
    {
        private int geld = 1000;    //Anfangsgehlt
        private int anzWaffen=1;

        List<int> waffenIDs = new List<int>();


        //Gibt an, wie viel Geld der Spieler hat
        public int getMoney()
        {
            return geld;
        }

        //Verändert den Wert des Geldkontos des Spielers
        public void setMoney(int money)
        {
            geld = money;
        }

        //Funktion fügt einer weitere Waffe hinzu
        public void setWaffen(int waffenid)
        {
            anzWaffen += 1;
            waffenIDs.Add(waffenid);
        }

        //Funktion zieht eine Waffe ab
        public void resetWaffen(int waffenid)
        {
            anzWaffen -= 1;
            waffenIDs.Remove(waffenid);
        }

        public List<int> getList()
        {
            return waffenIDs;
        }

        //Gibt die Anzahl der Waffen des Spielers an
        public int getWaffen()
        {
            return anzWaffen;
        }



    }
}
