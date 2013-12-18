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

        public Waffen(Controller Controller, RevoluteJoint Revolute, int Lebenspunkte, float Schusswinkel, float ShootSpeed)
        {
            controller = Controller;
            lebenspunkte = Lebenspunkte;
            schusswinkel = Schusswinkel;
            shootspeed = ShootSpeed;
            revolute = Revolute;
        }

        public void setWinkel(float Schusswinkel)
        {
            schusswinkel = Schusswinkel;
        }

        public float getWinkel()
        {
            return schusswinkel;
        }

        public Vector3 getPosition()
        {
            return controller.Position;
        }

        public void setPosition(Vector3 vektor)
        {
            controller.Position = vektor;
        }

        public Objekte shoot()
        {
            return Objektverwaltung.projektil(1, getController().Position, getWinkel());
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
