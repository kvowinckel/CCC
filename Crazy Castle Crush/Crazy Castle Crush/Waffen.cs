using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Graphics;
using NOVA.Scenery;
using NOVA;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Crazy_Castle_Crush
{
    public class Waffen
    {
        private int lebenspunkte;
        private RenderMaterial material;
        private SceneObject waffe;
        private float schusswinkel;
        private float shootspeed;

        public Waffen(SceneObject Waffe, int Lebenspunkte, String Material, float Schusswinkel, float ShootSpeed)
        {
            waffe = Waffe;
            lebenspunkte = Lebenspunkte;
            schusswinkel = Schusswinkel;
            shootspeed = ShootSpeed;
            setMaterial(Material);
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
            return waffe.Position;
        }

        public void setPosition(Vector3 vektor)
        {
            waffe.MoveToPosition(vektor);
        }

        public Objekte shoot()
        {
            return Objektverwaltung.projektil(1, getSceneObject().Position, getWinkel());
        }

        public int getLP()
        {
            return lebenspunkte;
        }

        public SceneObject getSceneObject()
        {
            return waffe;
        }

        public void setMaterial(String bild)
        {
            material = new RenderMaterial();
            material.Texture = Core.Content.Load<Texture2D>(bild);
            material.Diffuse = Color.White.ToVector4();
            
            waffe.RenderMaterial = material;
        }
    }
}
