using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Crazy_Castle_Crush
{
    class Auswahl
    {


        public int auswahl(Vector2 normScreenPos)
        {
            int x = 0;
            //Wenn sich die linke Hand in der y-Achsen-Auszahlzone befindet
            if (normScreenPos.Y <= 0.15f)
            {
                //Menü
                if (normScreenPos.X > 0.25f && normScreenPos.X <0.75f) 
                {
                    if (normScreenPos.X < 3f / 8)
                    {
                        x = 1;
                    }
                    else if (normScreenPos.X < 0.5f)
                    {
                        x = 2;
                    }
                    else if (normScreenPos.X < 5f / 8)
                    {
                        x = 3;
                    }
                    else if (normScreenPos.X > 5f / 8)
                    {
                        x = 4;
                    }
                    else
                    {
                        x = 0;
                    }
                }

                //Bau->Waffen
                else if (normScreenPos.X > 7f / 8)
                    {
                        x = 5;
                    }

                else
                {
                    x = 0;
                }
            }
            else
            {
                x = 0;
            }

            return x;
        }


    }
}
