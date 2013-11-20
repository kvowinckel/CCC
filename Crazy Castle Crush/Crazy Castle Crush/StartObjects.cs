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


        public StartObjects(Scene scene)
        {
            this.scene = scene;
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
            Vector3 pos = new Vector3(0, 0, -11);
            BoxObject hintergrund = new BoxObject(pos,
                                        new Vector3(56, 10, 0),
                                        0f);


            hintergrundsbild.Texture = Core.Content.Load<Texture2D>(bildname);
            hintergrundsbild.Diffuse = Color.White.ToVector4();
            hintergrundsbild.EnableGlow = true;
            hintergrund.RenderMaterial = hintergrundsbild;

            scene.Add(hintergrund);
        }

        private void LoadBox(Vector3 position, Vector3 dimension, float masse)
        {
            //Erstellt ein Objekt in der Scene.
            BoxObject box = new BoxObject(position,             //Position
                               dimension,                          //Kantenlängen
                               masse);
            scene.Add(box);
            
        }

        private Scene scene;
    }
}
