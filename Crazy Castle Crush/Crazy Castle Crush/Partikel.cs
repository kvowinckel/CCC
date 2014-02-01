using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMercury;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Modifiers;
using NOVA;
using ProjectMercury.Controllers;

namespace Crazy_Castle_Crush
{
    class Partikel
    {
        public ParticleEffect Explosion_neu()
        {
            ParticleEffect Explosion2 = new ParticleEffect
            {
                Emitters = new EmitterCollection
            {
                new CircleEmitter

                {
                    Name="Smoke",
                    Budget = 500,
					Term = 2.5f,
					Enabled = true,
					BlendMode = EmitterBlendMode.Add ,
					ReleaseColour = new Vector3 (0.5019608f, 0.5019608f, 0.5019608f),
					ReleaseOpacity = 1f,
					ReleaseQuantity = 16,
					ReleaseRotation = new Vector3(0f, 0f, 0f), //variation 3.14
					ReleaseScale = new Range(0.16f, 0f),
					ReleaseSpeed = new Range(-0.64f, 0.64f),
					ParticleTexture = Core.Content.Load<Texture2D>("Cloud001"),      
                    Shell=true,
                    Radius=1,
                    Radiate=true,

                    Modifiers = new ModifierCollection
					{
						new OpacityInterpolator2
						{
							InitialOpacity = 0.2f,
                            FinalOpacity=0f,
							
						},
                        new DampingModifier
                        {
                            DampingCoefficient=1,
                        },
					
						new ScaleInterpolator2
						{
							InitialScale = 0.48f,
							FinalScale = 0.255f,
						},
						new RotationModifier
						{
                            
							RotationRate = new Vector3(0f,0f,1f),
						},
                        new ColourInterpolator2
                        {
                            InitialColour=new Vector3(0.305882365f, 0.254901975f, 0.211764708f),
                            FinalColour = new Vector3(0.5019608f, 0.5019608f, 0.5019608f),
                        },
                    },

                           Controllers= new ControllerPipeline
                           {
                               new CooldownController
                               {
                                   CooldownPeriod=0.25f,
                               },
                           },
                },
                new SphereEmitter
                {
                    Name="Flames",
                    Budget = 1000,
					Term = 1f,
					Enabled = true,
					BlendMode = EmitterBlendMode.Add ,
					ReleaseColour = new ColourRange()
                    {
                        Red=new Range(0.8f,1f),
                        Green=new Range(.5f,.5f),
                        Blue=new Range(0,0),
                    },

					ReleaseOpacity = 1f,
					ReleaseQuantity = 64,
					ReleaseRotation =new RotationRange()
                    {
                        Roll= new Range(-1.471f,1.471f),
                    },

					ReleaseScale = new Range(0.32f,0.64f),
					ReleaseSpeed = new Range(-0.50f, 0.50f),
					ParticleTexture = Core.Content.Load<Texture2D>("Particle004"),      
                    Shell=true,
                    Radius=0.1f,
                    Radiate=true,

                     Modifiers = new ModifierCollection
					{
						new OpacityInterpolator2
						{
							InitialOpacity = 0.5f,
                            FinalOpacity=0f,
							
						},
                        new DampingModifier
                        {
                            DampingCoefficient=1,
                        },					
						
						new RotationModifier
						{
                            
							RotationRate = new Vector3(0f,0f,1f),
						},
                      
                    },
                     Controllers= new ControllerPipeline
                           {
                               new CooldownController
                               {
                                   CooldownPeriod=0.25f,
                               },
                           },
                },

                 new PointEmitter
                {
                    Name="Sparks",
                    Budget = 250,
					Term = 0.75f,
					Enabled = true,
					BlendMode = EmitterBlendMode.Add ,
					ReleaseColour = new ColourRange()
                    {
                        Red=new Range(1,1f),
                        Green=new Range(0.878f,0.878f),
                        Blue=new Range(0.752f,0.752f),
                    },

					ReleaseOpacity = 1f,
					ReleaseQuantity = 35,
				

					ReleaseScale = new Range(0.3f,0.7f),
					ReleaseSpeed = new Range(-0.125f, 0.125f),
					ParticleTexture = Core.Content.Load<Texture2D>("Particle005"),      
                    

                     Modifiers = new ModifierCollection
					{
						new OpacityInterpolator2
						{
							InitialOpacity = 1f,
                            FinalOpacity=0f,
							
						},
                        new DampingModifier
                        {
                            DampingCoefficient=2,
                        },					
						
						new RotationModifier
						{
                            
							RotationRate = new Vector3(0f,0f,1f),
						},
                      
                    },
                     Controllers= new ControllerPipeline
                           {
                               new CooldownController
                               {
                                   CooldownPeriod=0.25f,
                               },
                           },
                },
                    new PointEmitter
                {
                    Name="Flash",
                    Budget = 32,
					Term = 0.1f,
					Enabled = true,
					BlendMode = EmitterBlendMode.Add ,
					ReleaseColour = new ColourRange()
                    {
                        Red=new Range(1,1f),
                        Green=new Range(1,1),
                        Blue=new Range(1,1),
                    },

					ReleaseOpacity = 0.5f,
					ReleaseQuantity = 1,
				

					ReleaseScale = new Range(0.192f,0.192f),
					ReleaseSpeed = new Range(0.50f, 0.50f),
					ParticleTexture = Core.Content.Load<Texture2D>("Particle005"),      
                    

                     Modifiers = new ModifierCollection
					{
						new OpacityInterpolator2
						{
							InitialOpacity = 1f,
                            FinalOpacity=0f,
							
						},
                       
                      
                    },
                     Controllers= new ControllerPipeline
                           {
                               new CooldownController
                               {
                                   CooldownPeriod=0.25f,
                               },
                           },
                },
            },
            };
            return Explosion2;
        }
    }
}
