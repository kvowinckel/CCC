using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crazy_Castle_Crush
{
    class Levels
    {
        private int levelnr = 1;            //Das erste Level
        private float Sp1Pos = -20;         //Position Sp1 im ersten Level
        private float Sp2Pos = 20;          //Position Sp2 im ersten Level
        private int minMoney = 200;         //mindestGeld im ersten Level

        //Nächsten Level
        public void nextLevel()
        {
            levelnr += 1;
            #region Settings für das zweite Level
            //Hier können alle Voreinstellungen für das zweite Level geändert werden
            if (levelnr == 2)
            {
                //Beispiele
                Sp1Pos = -30;
                Sp2Pos = 30;
                minMoney = 150;
            }
            #endregion

        }
        
        //In welchem Level befindet man sich
        public int getLevel()
        {
           return levelnr;
        }
        
        //Wo sich die Spielerposition 1 befindet
        public float getSpieler1Pos()
        {
            return Sp1Pos;
        }

        //Wo sich die Spielerposition 2 befindet
        public float getSpieler2Pos()
        {
            return Sp2Pos;
        }

        //Das Geld was man im Level mindestens braucht, um etwas zu bauen
        public float getMinMoney()
        {
            return minMoney;
        }
    }
}
