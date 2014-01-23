using Microsoft.Xna.Framework;
using NOVA.Scenery;
using NOVA.Graphics;
using NOVA;
using Microsoft.Xna.Framework.Graphics;
using NOVA.UI;
using System.Collections.Generic;
using BEPUphysics.Constraints.SolverGroups;


namespace Crazy_Castle_Crush
{
    class StartObjects
    {
        BoxObject hintergrund;
        Vector3 pos = new Vector3(0, 0, -11);
        
        public StartObjects(Scene scene, Levels level)
        {
            this.scene = scene;
            this.level = level;
        }
         
        public void LoadStartObjects(int level)
        {
            if (level == 1)
            {
                ModelObject Welt = new ModelObject(new Vector3(0, -1.5f, -5f), Quaternion.CreateFromYawPitchRoll(0, -1.57f, 0), new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Welt_xna", 0f);
                Welt.PhysicsMaterial.Bounciness = 0.2f;
                Welt.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
                scene.Add(Welt);
                Objektverwaltung.addToUmgebungsListe(Welt);

                //Lädt Spielhintergrund
                LoadBackground("himmel");
            }
        }

        private void LoadBackground(string bildname)
        {
            RenderMaterial hintergrundsbild = new RenderMaterial();
            hintergrund = new BoxObject(pos,
                                        new Vector3(110, 35, 0),
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
 
        private Scene scene;
        private Levels level;
    }
}
