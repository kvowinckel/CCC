using Microsoft.Xna.Framework;
using NOVA.Scenery;
using NOVA.Graphics;
using NOVA;
using Microsoft.Xna.Framework.Graphics;
using NOVA.UI;
using System.Collections.Generic;

namespace Crazy_Castle_Crush
{
    class StartObjects
    {
        BoxObject hintergrund;
        Vector3 pos = new Vector3(0, 0, -11);
        bool lastR = false;
        
        
        public StartObjects(Scene scene, Levels level)
        {
            this.scene = scene;
            this.level = level;
        }

        public BoxObject RightHand()
        {
            RenderMaterial gruen = new RenderMaterial();
            gruen.Diffuse = Color.Green.ToVector4();


            BoxObject rightHand = new BoxObject(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0f), 0f);
            rightHand.RenderMaterial = gruen;
            rightHand.RenderMaterial.Transparency = 0.3f;
            
            scene.Add(rightHand);
            
            return rightHand;
        }

        public BoxObject LeftHand()
        {

            RenderMaterial rot = new RenderMaterial();
            rot.Diffuse = Color.Red.ToVector4();

            BoxObject leftHand = new BoxObject(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0f), 0f);
            leftHand.RenderMaterial = rot;
            leftHand.RenderMaterial.Transparency = 0.3f;

            scene.Add(leftHand);

            return leftHand;
        }

        public BoxObject showObjects(string bild)
        {
            
            Vector3 vector = new Vector3(0, 2.0f, -6);

            RenderMaterial bildobjekte = new RenderMaterial();
            BoxObject blende = new BoxObject(vector, new Vector3(4, 0.8f, 0), 0f);
            bildobjekte.Texture = Core.Content.Load<Texture2D>(bild);
            bildobjekte.Diffuse = Color.White.ToVector4();
            blende.RenderMaterial = bildobjekte;

            scene.Add(blende);
            return blende;
            
        }

        public void einausblender(BoxObject Ob, int state, float time)
        {
            // 1=ObjSp1  11=TexSp1  2=ObjSp2  22=TexSp2  0=else
            
            if (state == 0)
            {
                rausblend(Ob, Ob.Position.X);
            }
            if (state == 1)
            {
                reinblend(Ob, level.getSpieler1Pos());
                overblend(Ob, "Bau");
            }
            if (state == 11)
            {
                overblend(Ob, "hpic");
            }
            if (state == 2)
            {
                reinblend(Ob, level.getSpieler2Pos());
                overblend(Ob, "Bau");
            }
            if (state == 22)
            {
                overblend(Ob, "hpic");
            }


        }

        private void rausblend(BoxObject box, float x)
        {
            //TODO: rausblenden
            if (lastR == false) { }//box.Visible = false; }
            box.Position = new Vector3(x, 4f, -6f);
            lastR = true;
        }
        private void reinblend(BoxObject box, float x)
        {
            //TODO: einblenden
            if (lastR) { }//box.Visible = true; }
            box.Physics.Position = new Vector3(x, 2f, -6f);
            lastR = false;
        }
        
        private void overblend(BoxObject box, string bild)
        {
            if (box.RenderMaterial.Texture.Name != bild)
            {
                RenderMaterial xmenu = new RenderMaterial();
                xmenu.Texture = Core.Content.Load<Texture2D>(bild);
                xmenu.Diffuse = Color.White.ToVector4();
                box.RenderMaterial = xmenu;
            }
        }
         



        public void LoadStartObjects(int level)
        {
            if (level == 1)
            {
                //Lädt Spieluntergrund
                LoadBox(new Vector3(0, -1.5f, -5), new Vector3(46, 0.2f, 1), 0f);

                //Lädt Spielhintergrund
                LoadBackground("himmel");
            }
        }

        private void LoadBackground(string bildname)
        {
            RenderMaterial hintergrundsbild = new RenderMaterial();
            hintergrund = new BoxObject(pos,
                                        new Vector3(58, 12, 0),
                                        0f);


            hintergrundsbild.Texture = Core.Content.Load<Texture2D>(bildname);
            hintergrundsbild.Diffuse = Color.White.ToVector4();
            hintergrundsbild.EnableGlow = true;
            hintergrund.RenderMaterial = hintergrundsbild;

            scene.Add(hintergrund);
        }

        public void MoveBackground(float xVerschiebung, float yVerschiebung)
        {
            Vector3 verschiebung = new Vector3(pos.X - 2 * xVerschiebung, pos.Y + 2 * yVerschiebung, pos.Z);
            hintergrund.Position = verschiebung;

        }
 
        private BoxObject LoadBox(Vector3 position, Vector3 dimension, float masse)
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box = new BoxObject(position,             //Position
                               dimension,                          //Kantenlängen
                               masse);
            scene.Add(box);

            return box;
            
        }

        private Scene scene;
        private Levels level;
    }
}
