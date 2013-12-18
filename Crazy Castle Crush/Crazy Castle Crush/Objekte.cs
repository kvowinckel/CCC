using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOVA.Scenery;
using NOVA.UI;
using NOVA.Graphics;
using Microsoft.Xna.Framework;
using NOVA;
using Microsoft.Xna.Framework.Graphics;

namespace Crazy_Castle_Crush
{
    class Objekte
    {
        private int lebenspunkte;
        private RenderMaterial material;
        private SceneObject objekt;

        public Objekte(SceneObject Objekt, int Lebenspunkte, String Material)
        {
            objekt = Objekt;
            lebenspunkte = Lebenspunkte;

            setMaterial(Material);
        }

        public SceneObject getSceneObject()
        {
            return objekt;
        }

        public Vector3 getPosition()
        {
            return objekt.Position;
        }

        public void setPosition(Vector3 vektor)
        {
            objekt.Position = vektor;
        }

        public void setMasse(float masse)
        {
            objekt.Physics.Mass = masse;
        }

        public int getLP()
        {
            return lebenspunkte;
        }

        public String getMaterial()
        {
            return material.ToString();
        }

        public void setMaterial(String bild)
        {
            if (bild.Equals("blank"))
            {
                material = new RenderMaterial();
                material.Diffuse = Color.Gray.ToVector4();
            }
            else
            {
                material.Texture = Core.Content.Load<Texture2D>(bild);
                material.Diffuse = Color.White.ToVector4();
            }
            objekt.RenderMaterial = material;
        }

        public void decreaseLP(int minus)
        {
            lebenspunkte -= minus;
        }

        public void decreaseLP()
        {
            lebenspunkte -= 1;
        }

        public void increaseLP(int plus)
        {
            lebenspunkte += plus;
        }


        

    }
}
