using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NOVA.Scenery;
using NOVA.ScreenManagement;
using NOVA.ScreenManagement.BaseScreens;

namespace WindowsGame1
{
    class Controlling: GameplayScreen
    {
        
        Vector2 angles;
        int widthOver2;
        int heightOver2;
        MouseState prevMouseState = new MouseState();
        public override void Initialize ()
        {
            widthOver2 = Scene . Game . Window . ClientBounds . Width / 2;
            heightOver2 = Scene . Game . Window . ClientBounds . Height / 2;
            angles . X = Scene . Camera . Angles . X ;
            angles . Y = Scene . Camera . Angles . Y ;
            Mouse. SetPosition ( widthOver2 , heightOver2 );
        }

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
            MouseState currentMouseState = Mouse. GetState ();
            if ( currentMouseState.X != widthOver2 )
            angles.Y -= 0.001f * ( currentMouseState.X - widthOver2 );
            if ( currentMouseState.Y != heightOver2 )
            angles.X -= 0.001f * ( currentMouseState.Y - heightOver2 );
            Mouse. SetPosition ( widthOver2 , heightOver2 );
            // Update the camera position and orientation
            Matrix cameraRotation = Matrix. CreateRotationX ( angles . X ) *
            Matrix. CreateRotationY ( angles . Y );
            Scene . Camera . Orientation = Quaternion. CreateFromRotationMatrix ( cameraRotation );
            prevMouseState = currentMouseState ;
        }
    }
}
