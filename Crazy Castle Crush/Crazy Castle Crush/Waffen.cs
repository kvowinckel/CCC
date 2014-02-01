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
            this.mo = MO;
            mo.Name = Waffentyp;
            this.lebenspunkte = Lebenspunkte;
            this.schusswinkel = Schusswinkel;
            this.shootspeed = ShootSpeed;
            this.waffentyp = Waffentyp;
        }

        public ModelObject getModelObject()
        {
            return this.mo;
        }
        public string getType()
        {
            return this.waffentyp;
        }

        public void setWinkel(float rHandY)
        {
            if (!getType().Equals("Rakete"))
            {
                mo.IsUpdatingCompoundBody = false;
                mo.SubModels[1].Orientation = Quaternion.CreateFromYawPitchRoll(0, 0, /*schusswinkel*/ (1 - rHandY));
            }
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
