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
    public class Objekte  //:SceneObject
    {
        private int lebenspunkte;
        private RenderMaterial material;
        private string materialString;
        new SceneObject objekt;
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
            objekt.MoveToPosition(vektor);
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
            return materialString;
        }

        public void setMaterial(String bild)
        {
            materialString = bild;

            if (bild.Equals("blank"))
            {
                material = new RenderMaterial();
                material.Diffuse = Color.Gray.ToVector4();
            }
            else
            {

                material.Texture = Core.Content.Load<Texture2D>(bild);
                material.Diffuse = Color.White.ToVector4();
                material.Specular=new Vector4(0.1f, 0.1f, 0.1f, 1);
            }
            
            if (objekt.Name.Contains("L"))
            {
                ((ModelObject)objekt).SubModels[0].RenderMaterial = material;
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
