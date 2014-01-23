using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crazy_Castle_Crush
{
    class Logik
    {
        bool schussphase1done;
        bool schussphase2done;
        Gamestart.States zielstate;

        public Logik()
        {
            schussphase1done = schussphase2done = false;
            zielstate = Gamestart.States.Bauphase1O;
        }

        public Gamestart.States uebergang(Gamestart.States actuellState, Spieler spieler1, Spieler spieler2, Levels level)
        {
            #region Bauphase 1
            if (actuellState == Gamestart.States.Bauphase1O)
            {
                if (spieler2.getMoney() < level.getMinMoney())                  //Spieler2 hat nicht genug Geld
                {
                    if (spieler1.getMoney() > spieler2.getMoney())                      //Spieler1 ist reicher
                    {
                        if (spieler1.getList().Count > 0)                                       //Spieler1 hat Waffen
                        {
                            schussphase1done = true;
                            return Gamestart.States.Schussphase1;
                        }
                        else if (spieler2.getList().Count > 0)                                  //nein, aber Spieler2 hat Waffen
                        {
                            schussphase1done = true; /*Da Spieler 1 eh nicht Schießen kann*/
                            zielstate = Gamestart.States.Schussphase2;
                            return Gamestart.States.Camto2;
                        }
                        else                                                                    //beide haben keine Waffen 
                        {
                            newround(spieler1, spieler2, level);
                            return Gamestart.States.Bauphase1O;
                        }
                    }
                    else                                                                //Spieler2 ist reicher
                    {
                        if (spieler2.getList().Count > 0)                                       //Spieler2 hat Waffen
                        {
                            zielstate = Gamestart.States.Schussphase2;
                            return Gamestart.States.Camto2;
                        }
                        else if (spieler1.getList().Count > 0)                                  //nein, aber Spieler1 hat Waffen
                        {
                            schussphase2done = true; /*Da Spieler 2 eh nicht Schießen kann*/
                            schussphase1done = true;
                            return Gamestart.States.Schussphase1;
                        }
                        else                                                                    //beide haben keine Waffen 
                        {
                            newround(spieler1, spieler2, level);
                            return Gamestart.States.Bauphase1O;
                        }                                
                    }
                }
                else                                                            //Spieler2 hat genug Geld
                {
                    zielstate = Gamestart.States.Bauphase2O;
                    return Gamestart.States.Camto2;
                }
            }
            #endregion
            #region Bauphase 2
            else if(actuellState == Gamestart.States.Bauphase2O)
            {
                if (spieler1.getMoney() > spieler2.getMoney())                  //Spieler1 hat mehr Geld als Spieler2
                {
                    if (spieler1.getList().Count > 0)                                       //Spieler1 hat Waffen
                    {
                        zielstate = Gamestart.States.Schussphase1;
                        return Gamestart.States.Camto1;
                    }
                    else if (spieler2.getList().Count > 0)                                  //nein, aber Spieler2 hat Waffen
                    {
                        schussphase1done = true; /*Da Spieler 1 eh nicht Schießen kann*/
                        schussphase2done = true;
                        return Gamestart.States.Schussphase2;
                    }
                    else                                                                    //beide haben keine Waffen 
                    {
                        newround(spieler1, spieler2, level);
                        zielstate = Gamestart.States.Bauphase1O;
                        return Gamestart.States.Camto1;
                    }
                }
                else                                                           //Spieler2 ist reicher
                {
                    if (spieler2.getList().Count > 0)                                       //Spieler2 hat Waffen
                    {
                        schussphase2done = true;
                        return Gamestart.States.Schussphase2;
                    }
                    else if (spieler1.getList().Count > 0)                                  //nein, aber Spieler1 hat Waffen
                    {
                        schussphase2done = true; /*Da Spieler 2 eh nicht Schießen kann*/
                        zielstate = Gamestart.States.Schussphase1;
                        return Gamestart.States.Camto1;
                    }
                    else                                                                    //beide haben keine Waffen 
                    {
                        newround(spieler1, spieler2, level);
                        zielstate = Gamestart.States.Bauphase1O;
                        return Gamestart.States.Camto1;
                    }
                }
            }
            #endregion
            #region Schussphase 1
            else if (actuellState == Gamestart.States.Schussphase1)
            {
                if (schussphase2done)                                           //War schon in Schussphase2
                {
                    newround(spieler1, spieler2, level);
                    return Gamestart.States.Bauphase1O;
                }
                else                                                            //War noch nicht in Schussphase2
                {
                    if (spieler2.getList().Count > 0)                                    //Spieler2 hat Waffen
                    {
                        zielstate = Gamestart.States.Schussphase2;
                        return Gamestart.States.Camto2;
                    }
                    else                                                                 //Spieler2 hat keine Waffe
                    {
                        newround(spieler1, spieler2, level);
                        return Gamestart.States.Bauphase1O;
                    }
                }
            }
            #endregion
            #region Schussphase 2
            else if (actuellState == Gamestart.States.Schussphase2)
            {
                if (schussphase1done)                                           //War schon in Schussphase1
                {
                    newround(spieler1, spieler2, level);
                    zielstate = Gamestart.States.Bauphase1O;
                    return Gamestart.States.Camto1;
                }
                else                                                            //War noch nicht in Schussphase1
                {
                    if (spieler1.getList().Count > 0)                                    //Spieler1 hat Waffen
                    {
                        zielstate = Gamestart.States.Schussphase1;
                        return Gamestart.States.Camto1;
                    }
                    else                                                                 //Spieler1 hat keine Waffe
                    {
                        newround(spieler1, spieler2, level);
                        zielstate = Gamestart.States.Bauphase1O;
                        return Gamestart.States.Camto1;
                    }
                }
            }
            #endregion
            #region Cam -> 1
            else if (actuellState == Gamestart.States.Camto1)
            {
                if (zielstate == Gamestart.States.Bauphase1O)
                {
                    return Gamestart.States.Bauphase1O;
                }
                else //(zielstate == Gamestart.States.Schussphase1)
                {
                    schussphase1done = true;
                    return Gamestart.States.Schussphase1;
                }
            }
            #endregion
            #region Cam -> 2
            else if (actuellState == Gamestart.States.Camto2)
            {
                if (zielstate == Gamestart.States.Bauphase2O)
                {
                    return Gamestart.States.Bauphase2O;
                }
                else //(zielstate == Gamestart.States.Schussphase2)
                {
                    schussphase2done = true;
                    return Gamestart.States.Schussphase2;
                }
            }
            #endregion

            return Gamestart.States.End;
        }

        private void newround(Spieler spieler1, Spieler spieler2, Levels level)
        {
            spieler1.setMoney(spieler1.getMoney() + level.getRoundMoney());
            spieler2.setMoney(spieler2.getMoney() + level.getRoundMoney());
            schussphase1done = schussphase2done = false;
        }
    }
}
