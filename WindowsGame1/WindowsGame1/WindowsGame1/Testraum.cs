using BEPUphysics.Constraints.SolverGroups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NOVA;
using NOVA.Scenery;
using NOVA.ScreenManagement;
using NOVA.ScreenManagement.BaseScreens;
using NOVA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace WindowsGame1
{
    internal class Testraum : GameplayScreen
    {
        private Vector2 angles;
        private int heightOver2;
        private MouseState prevMouseState = new MouseState();
        private int widthOver2;

        ModelObject Balliste;

        public override void HandleInput(InputState input)
        {
            Vector3 moveVector = new Vector3();
            // Geschwindigkeit der Kamerabewegung
            float speed = 0.1f;
            // Offset beim Dr¨ucken der Pfeiltasten setzen
            if (input.IsKeyPressed(Keys.Left, PlayerIndex.One))
                moveVector.X -= speed;
            if (input.IsKeyPressed(Keys.Right, PlayerIndex.One))
                moveVector.X += speed;
            if (input.IsKeyPressed(Keys.Up, PlayerIndex.One))
                moveVector.Z -= speed;
            if (input.IsKeyPressed(Keys.Down, PlayerIndex.One))
                moveVector.Z += speed;
            // Kamera um Offset verschieben
            Scene.Camera.Position += moveVector;
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.X != widthOver2)
                angles.Y -= 0.001f * (currentMouseState.X - widthOver2);
            if (currentMouseState.Y != heightOver2)
                angles.X -= 0.001f * (currentMouseState.Y - heightOver2);
            Mouse.SetPosition(widthOver2, heightOver2);
            // Update the camera position and orientation
            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) *
            Matrix.CreateRotationY(angles.Y);
            Scene.Camera.Orientation = Quaternion.CreateFromRotationMatrix(cameraRotation);
            prevMouseState = currentMouseState;
            if(currentMouseState.LeftButton == ButtonState.Pressed &&prevMouseState.LeftButton == ButtonState.Released)
            {
                Shoot(Balliste.Position, Balliste.Orientation);
            }
            prevMouseState = currentMouseState;
        }

        public void Shoot(Vector3 Source,Quaternion Richtung)
        {
                       
            
            ModelObject Bolzen =new ModelObject(Source+ new Vector3(5,0,0),Richtung,Vector3.One,CollisionType.ExactMesh, " ", "Balliste/bolzen", 1f);
            Scene.Add(Bolzen);
            
        }
        public override void Initialize()
        {
            Scene.ShowGrid = true;
            Scene.ShowCollisionMeshes = false;
            Scene.ShowConstraints = true;
            Scene.ShowWireframe = false;
            Scene.ShowFPS = true;
            
            
            base.Initialize();
            CameraObject cam = new CameraObject(new Vector3(10, 10, 50), new Vector3(-50, 0, 0));
            Scene.Add(cam);
            Scene.Camera = cam;
            Scene.Physics.ForceUpdater.Gravity = new Vector3(0f,-9.81f, 0);

            Balliste = new ModelObject(new Vector3(-50f, 10, 0), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Balliste/Balliste_rotierty_000", 1f);
            Balliste.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            //Balliste.PhysicsMaterial.Bounciness = 0;
            Scene.Add(Balliste);

            ModelObject Ballistenstandbein = new ModelObject(new Vector3(-50 - 0.98418f, 10 - 4.2973f, +0.07989f), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Balliste/Balliste_Standbein_rotierty_000", 1f);
            Ballistenstandbein.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            Scene.Add(Ballistenstandbein);

            // Neuer Controller an der Position 0/0/0
            /*Controller Controller_Balliste = new Controller(new Vector3(0, 0, 0));
            Scene.Add(Controller_Balliste);
            Controller_Balliste.Add(Balliste);
            Controller_Balliste.Add(Ballistenstandbein);
            //Controller_Balliste.Position = Controller_Balliste.Position + Vector3.One*10;
            //Controller_Balliste.Orientation = Quaternion.CreateFromYawPitchRoll(0, -1.57f, 0);*/

            Balliste.Orientation=Quaternion.CreateFromYawPitchRoll(0, 0, 0.4f);

            RevoluteJoint revolute = new RevoluteJoint(Ballistenstandbein.Physics, Balliste.Physics, Balliste.Position, Vector3.Backward);
            Scene.Physics.Add(revolute);
            revolute.Limit.IsActive = true;
            revolute.Limit.BounceVelocityThreshold = revolute.Limit.BounceVelocityThreshold * 50;

            /*BoxObject Box1=new BoxObject(new Vector3(0, 0, 0),new Vector3(1, 1, 1),0f);
            BoxObject Box2 = new BoxObject(new Vector3(5, 0, 0), new Vector3(1, 1, 1), 0f);
            Scene.Add(Box1);
            Scene.Add(Box2);
            Scene.Physics.Add(new BallSocketJoint(Box2.Physics, Box1.Physics, Box2.Position));*/


            //revolute.AngularJoint.ConnectionA.BecomeKinematic();
            //revolute.Limit.Basis.SetWorldAxes(Vector3.Right, Vector3.Up);
            //revolute.Limit.TestAxis = Vector3.Backward;
            //revolute.Limit.MinimumAngle = -MathHelper.Pi;
            //revolute.Limit.MaximumAngle = 0;

            ModelObject Kanonenrohr = new ModelObject(new Vector3(-50, 10, 10), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Kanone/Kanonenrohr", 0.001f);
            Kanonenrohr.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            Scene.Add(Kanonenrohr);
            ModelObject Kanonenhalterung = new ModelObject(new Vector3(-50 - 0.95f, 10 - 1.82896f, 10), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Kanone/Kanonenhalter", 1f);
            Kanonenhalterung.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            Scene.Add(Kanonenhalterung);

            // Neuer Controller an der Position 0/0/0
            /*Controller Kanone = new Controller(new Vector3(0, 0, 0));
            Scene.Add(Kanone);
            Kanone.Add(Kanonenrohr);
            Kanone.Add(Kanonenhalterung);*/

            RevoluteJoint revolute2 = new RevoluteJoint(Kanonenhalterung.Physics, Kanonenrohr.Physics, Kanonenhalterung.Position + new Vector3(0, +1f, 0f), Vector3.Backward);
            //revolute2.Limit.IsActive = true;           
            //revolute2.Limit.MinimumAngle = -MathHelper.Pi;
            //revolute2.Limit.MaximumAngle = -MathHelper.Pi+1;
            //revolute2.Limit.BounceVelocityThreshold = revolute.Limit.BounceVelocityThreshold * 50;
            Scene.Physics.Add(revolute2);
            revolute2.Motor.IsActive = true;
            revolute2.Motor.Settings.Mode = MotorMode.Servomechanism;

            revolute2.Motor.Settings.Servo.Goal = MathHelper.Pi/3;

            //Kanonenhalterung.MoveToPosition(new Vector3(-55f, 9f, 10f));
            //Kanone.Position+= new Vector3(5,0,0);

            //Kanonenrohr.Orientation = Quaternion.CreateFromYawPitchRoll(0, 0, 0.2f);
            //Kanone.Orientation = Quaternion.CreateFromYawPitchRoll(1.57f, 0, 0);
            //controller.Position += new Vector3(5, 0, 0);*/

            /*ModelObject Pyramide = new ModelObject(new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Pyramide", 0f);
            Pyramide.RenderMaterial.Texture = Core.Content.Load<Texture2D>("Standardtexturen/wood7_edited");
            Pyramide.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            Scene.Add(Pyramide);*/

            /*ModelObject Würfel = new ModelObject(new Vector3(0, 0, 0), Quaternion.CreateFromYawPitchRoll(0,-1.57f,0), new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Bauobjekte/L", 0f);
            Würfel.RenderMaterial.Texture = Core.Content.Load<Texture2D>("Standardtexturen/Ziegel");
            Würfel.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            Scene.Add(Würfel);*/

            //RiggedModel model = new RiggedModel(new Vector3(5, 0 , 0) , // Position
            // Modelname aus XNAnimation -Prozessor
            // f¨ur die Animation
            //"Münze_animiert" ,
            // Modelname aus Standard -Prozessor
            // f¨ur das Kollisionsmodell
            //"Münze_mesh" ,
            //0f ); // Masse
            //Scene.Add(model);
            //model.StartClip("Default Take");

            ModelObject Welt = new ModelObject(new Vector3(0, 0, 0), Quaternion.CreateFromYawPitchRoll(0, -1.57f, 0), new Vector3(1, 1, 1), CollisionType.ExactMesh, " ", "Welt/Welt_xna", 0f);
            Welt.RenderMaterial.Diffuse = new Vector4(1, 1, 1, 1);
            Scene.Add(Welt);

            widthOver2 = Scene.Game.Window.ClientBounds.Width / 2;
            heightOver2 = Scene.Game.Window.ClientBounds.Height / 2;
            angles.X = Scene.Camera.Angles.X;
            angles.Y = Scene.Camera.Angles.Y;
            Mouse.SetPosition(widthOver2, heightOver2);
        }
    }
}