using Microsoft.Xna.Framework;
using NOVA.Scenery;
using NOVA.Graphics;
using NOVA;
using Microsoft.Xna.Framework.Graphics;
using NOVA.UI;

namespace Crazy_Castle_Crush
{
    class StartObjects
    {
        BoxObject hintergrund;
        Vector3 pos = new Vector3(0, 0, -11);



        public StartObjects(Scene scene)
        {
            this.scene = scene;
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

        public void showObjects(Vector3 position)
        {


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
            //hintergrund.MoveToPosition(verschiebung);
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
    }
}
