using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Scenery;
using Microsoft.Xna.Framework;

namespace Crazy_Castle_Crush
{
    static class Collided
    {
        /*
        public static string Zerstören(object sender, CollisionArgs e, Scene scene, Gamestart.States state, Spieler spieler)
        {
            string winner = "";

            if (Objektverwaltung.getObj(e.Collider) != null)                    //Objekt
            {
                Objekte getroffenesObj = Objektverwaltung.getObj(e.Collider);
                getroffenesObj.decreaseLP();
                if (getroffenesObj.getLP() <= 0)
                {
                    Vector2 pos = getroffenesObj.getPosition().
                    DrawHelper.setmoney(spieler, 200, pos);
                }
            }


            //TODO: Explosion
            if (e.Collider == scene.Find("König1" + e.Collider.ID))
            {
                winner = "Spieler 2";
                
            }
            else if (e.Collider == scene.Find("König2" + e.Collider.ID))
            {
                winner = "Spieler 1";
                
            }

            return winner;
        }*/
    }
}
