using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crazy_Castle_Crush
{
    public class Spieler
    {
        private int geld = 1000;    //Anfangsgeld
        private int anzWaffen=1;

        List<Waffen> waffen = new List<Waffen>();


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
        public void setWaffen(Waffen waffe)
        {
            anzWaffen += 1;
            waffen.Add(waffe);
        }

        //Funktion zieht eine Waffe ab
        public void resetWaffen(Waffen waffe)
        {
            anzWaffen -= 1;
            waffen.Remove(waffe);
        }

        public List<Waffen> getList()
        {
            return waffen;
        }

        //Gibt die Anzahl der Waffen des Spielers an
        public int getWaffen()
        {
            return anzWaffen;
        }



    }
}
