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
        private ModelObject mo;
        private float schusswinkel;
        private float shootspeed;
        string waffentyp;

        public Waffen(ModelObject MO, int Lebenspunkte, float Schusswinkel, float ShootSpeed,string Waffentyp) //Controller rausgenommen 
        {
            mo = MO;
            lebenspunkte = Lebenspunkte;
            schusswinkel = Schusswinkel;
            shootspeed = ShootSpeed;
            waffentyp = Waffentyp;
        }

        public ModelObject getModelObject()
        {
            return mo;
        }
        public string getType()
        {
            return waffentyp;
        }

        public void setWinkel(float rHandY)
        {
            /*
            rHandY = (1-rHandY) / 1.6f; // Schusswinkel hängt nur mit Höhe der rechten Hand zusammen
            schusswinkel = rHandY * 3.1415f/2; // mit float math.pi ersetzen!!!
            */
            mo.IsUpdatingCompoundBody = false;
            mo.SubModels[1].Orientation = Quaternion.CreateFromYawPitchRoll(0, 0, /*schusswinkel*/ (1-rHandY));

            /*
            revolute.Motor.Settings.Servo.Goal = schusswinkel;//stellt Motor auf Winkel ein
            */
        }

        public float getWinkel()
        {
            return schusswinkel;
        }

        public void setPosition(Vector3 vec)
        {
            mo.Position = vec;
        }

        public Vector3 getPosition()
        {
            return mo.Position;
        }

        public int getLP()
        {
            return lebenspunkte;
        }

        public void UpdatePhysics()
        {
            this.mo.IsUpdatingCompoundBody = true;
           //this.mo.UpdateCompoundBody();
        }
    }
}
