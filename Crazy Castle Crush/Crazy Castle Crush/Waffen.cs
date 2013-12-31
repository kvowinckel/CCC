using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Graphics;
using NOVA.Scenery;
using NOVA;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SolverGroups;

namespace Crazy_Castle_Crush
{
    public class Waffen
    {
        private int lebenspunkte;
        private Controller controller;
        private float schusswinkel;
        private float shootspeed;
        private RevoluteJoint revolute;

        public Waffen( Controller Kontroller, RevoluteJoint Revolute, int Lebenspunkte, float Schusswinkel, float ShootSpeed) //Controller rausgenommen 
        {
            controller = Kontroller;
            lebenspunkte = Lebenspunkte;
            schusswinkel = Schusswinkel;
            shootspeed = ShootSpeed;
            revolute = Revolute;
            
        }

        public void setWinkel(float rHandY)
        {
            
            rHandY = rHandY / 1.6f; // Schusswinkel hängt nur mit Höhe der rechten Hand zusammen

            schusswinkel = rHandY * 3.1415f/2; // mit float math.pi ersetzen!!!
            
            revolute.Motor.Settings.Servo.Goal = schusswinkel;//stellt Motor auf Winkel ein
            
        }

        public float getWinkel()
        {
            return schusswinkel;
        }

       /* public Vector3 getPosition()
        {
            
        }
       */
       public void setPosition(Vector3 vektor)
        {
            controller.Position = vektor;
            
          // controller.Orientation = quad = new Quaternion( new Vector3(0,0,1),0f);
           
           
        }
       public Vector3 getPosition()
       {
           return controller.Position;
          
       }
            
       
       

        public int getLP()
        {
            return lebenspunkte;
        }

      

        public Controller getController()
        {
            return controller;
        }
       
    }
}
