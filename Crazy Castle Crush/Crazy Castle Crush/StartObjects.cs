using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.ScreenManagement.BaseScreens;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.Events;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NOVA.Scenery;
using NOVA.UI;
using NOVA.ScreenManagement;
using NOVA.Physics;
using NOVA.Utilities;

namespace Crazy_Castle_Crush
{


    public class StartObjects : Gamestart
    {


        public override void Initialize()
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box0 = new BoxObject(new Vector3(0, -1.5f, -5),             //Position
                               new Vector3(46, 0.2f, 1),                          //Kantenlängen
                               0f);

            
            Scene.Add(box0);
        }


        public static BoxObject Box(Vector3 Position, Vector3 Dimension, float Masse)
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box = new BoxObject(Position, Dimension, Masse);
            return box;
        }


    }
}
