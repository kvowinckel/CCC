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

        public SceneObject shoot(Scene scene, float velocity, int richtung)
        {
            SceneObject bullet;
            Vector2 shootdirection = new Vector2(0, 1);

            if (this.getType().Equals("Balliste"))
            {
                bullet = new ModelObject(new Vector3(this.getPosition().X, this.getPosition().Y + 0.5f, this.getPosition().Z), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, "", "Bolzen", 0.05f);
                scene.Add(bullet);
                shootdirection = new Vector2((float)Math.Cos(this.getWinkel()), (float)Math.Sin(this.getWinkel()));
            }
            else if (this.getType().Equals("Kanone"))
            {
                bullet = new SphereObject(new Vector3(this.getPosition().X, this.getPosition().Y + 0.5f, this.getPosition().Z), 0.1f, 6, 6, 0.05f);
                scene.Add(bullet);
                shootdirection = new Vector2((float)Math.Cos(this.getWinkel()), (float)Math.Sin(this.getWinkel()));
            }
            else
            {
                bullet = this.getModelObject();
            }

            float velo = (1 - velocity) * 15f;
            bullet.Physics.LinearVelocity = new Vector3(shootdirection.X *richtung, shootdirection.Y, 0) * velo; 

            return bullet;
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

        public void setLP(int abzug)
        {
            this.lebenspunkte -= abzug;
        }

        public void UpdatePhysics()
        {
            this.mo.IsUpdatingCompoundBody = true;
           //this.mo.UpdateCompoundBody();
        }
    }
}
