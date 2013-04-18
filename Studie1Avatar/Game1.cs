using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Schnittstellen;
using Microsoft.Kinect;

namespace Studie1Avatar
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Avatar : Microsoft.Xna.Framework.Game, Triggerable
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        const int width = 1920;
        const int height = 1080;
        const float smileyWidth = 0.7f/3.0f*16.0f/9.0f;
        const float smileyHeight = smileyWidth/4.0f;
        Vector3 displayOrigin = new Vector3(-1.352501f, -0.1293522f, 2.629021f);
        Vector3 displayRight = Vector3.Forward-Vector3.Right;
        const float smileyY = 215.0f / height;
        List<Quad> smileys;
        Matrix View, Projection, World;
        Texture2D[] texture;
        Texture2D background;
        BasicEffect[] quadEffect;
        Rectangle borders;

        public Avatar()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = Avatar.width;
            this.graphics.PreferredBackBufferHeight = Avatar.height;

            smileys = new List<Quad>();
            persons = new List<Vector3>();
            
            this.graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Vector3 cameraPosition = Vector3.Cross(displayRight, Vector3.Up);
            cameraPosition.Normalize();
            cameraPosition = cameraPosition / 2;
            View = Matrix.CreateLookAt(displayOrigin+cameraPosition, displayOrigin,
                Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver2, 16.0f / 9.0f, 0.1f, 10.0f);
            World = Matrix.Identity;

            displayRight.Normalize();

            Vector3 viewpoint = displayOrigin + cameraPosition;
            this.persons.Add(viewpoint);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = new Texture2D[4];
            texture[0] = Content.Load<Texture2D>("snickers");
            texture[1] = Content.Load<Texture2D>("mars");
            texture[2] = Content.Load<Texture2D>("twix");
            texture[3] = Content.Load<Texture2D>("milkyway");

            quadEffect = new BasicEffect[4];
            
            for(int i = 0; i<texture.Length; i++) {
                quadEffect[i] = new BasicEffect(graphics.GraphicsDevice);

                quadEffect[i].World = World;
                quadEffect[i].View = View;
                quadEffect[i].Projection = Projection;
                quadEffect[i].TextureEnabled = true;
                quadEffect[i].Texture = texture[i];
                quadEffect[i].VertexColorEnabled = false;
            }

            // 650

            background = Content.Load<Texture2D>("avatar_bg_v2");
            int screenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            borders = new Rectangle(0, 0, screenWidth, screenHeight);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        float[][][] positions = new float[][][]{
            new float[][]{ new float[]{0.5f, 0.5f}},
            new float[][]{ new float[]{0.25f, 0.75f}, new float[]{0.75f, 0.25f}}, 
            new float[][]{ new float[]{0.25f, 0.25f}, new float[]{0.25f, 0.75f}, new float[]{0.75f, 0.5f}}, 
            new float[][]{ new float[]{0.25f, 0.25f}, new float[]{0.25f, 0.75f}, new float[]{0.75f, 0.25f}, new float[]{0.75f, 0.75f}}, 
            new float[][]{ new float[]{0.25f, 0.25f}, new float[]{0.25f, 0.75f}, new float[]{0.75f, 1.0f/6.0f}, new float[]{0.75f, 3.0f/6.0f}, new float[]{0.75f, 5.0f/6.0f}}, 
            new float[][]{ new float[]{0.25f, 1.0f/6.0f}, new float[]{0.25f, 3.0f/6.0f}, new float[]{0.25f, 5.0f/6.0f}, new float[]{0.75f, 1.0f/6.0f}, new float[]{0.75f, 3.0f/6.0f}, new float[]{0.75f, 5.0f/6.0f}}
        };

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Vector3 cameraPosition = Vector3.Cross(displayRight, Vector3.Up);
            //cameraPosition.Normalize();
            //cameraPosition = cameraPosition / 2;
            //Vector3 viewpoint = displayOrigin + cameraPosition;

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            /*else if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                this.persons.RemoveRange(0,persons.Count);
                this.persons.Add(viewpoint);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                this.persons.RemoveRange(0, persons.Count);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                this.persons.RemoveRange(0, persons.Count);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                this.persons.RemoveRange(0, persons.Count);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                this.persons.RemoveRange(0, persons.Count);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D6))
            {
                this.persons.RemoveRange(0, persons.Count);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
                this.persons.Add(viewpoint);
            }*/

            smileys.RemoveRange(0, smileys.Count);
            if (persons.Count > 0)
            {
                Vector3 displayLeftTop = displayOrigin - 0.5f * 16.0f / 9.0f * displayRight + 0.5f * Vector3.Up;
                int smileyIndex = 0;
                foreach (Vector3 person in persons)
                {
                    float[] positionOffset = positions[persons.Count-1][smileyIndex];
                    Vector3 smileyOrigin = displayLeftTop - (650.0f / 1080.0f * positionOffset[0]) * Vector3.Up + displayRight * (16.0f / 9.0f * positionOffset[1]);
                    Vector3 display_to_person = -1 * (smileyOrigin - person);
                    display_to_person.Normalize();
                    Vector3 up = Vector3.Cross(display_to_person, displayRight);
                    up.Normalize();
                    smileys.Add(new Quad(smileyOrigin, display_to_person, up, Avatar.smileyWidth, Avatar.smileyHeight));
                    smileyIndex++;
                }
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(background, borders, Color.White);
            spriteBatch.End();

            foreach (Quad smiley in smileys)
            {
                foreach (EffectPass pass in quadEffect[smileys.IndexOf(smiley) % 4].CurrentTechnique.Passes)
                {
                    pass.Apply();

                    GraphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionColorTexture>(
                        PrimitiveType.TriangleList,
                        smiley.Vertices, 0, 4,
                        smiley.Indices, 0, 2);
                }
            }

            base.Draw(gameTime);
        }

        List<Vector3> persons;
        public void triggerAction(List<Skeleton> skeletonData)
        {
            persons.RemoveRange(0, persons.Count);
            Vector3 cameraPosition = Vector3.Cross(displayRight, Vector3.Up);
            cameraPosition.Normalize();
            cameraPosition = cameraPosition / 2;
            Vector3 viewpoint = displayOrigin + cameraPosition;
            this.persons.Add(viewpoint);
            foreach (Skeleton s in skeletonData)
            {
                if (s.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked || s.Joints[JointType.Head].TrackingState == JointTrackingState.Inferred)
                {
                    persons.Add(new Vector3(s.Joints[JointType.Head].Position.X, s.Joints[JointType.Head].Position.Y, s.Joints[JointType.Head].Position.Z));
                }
                else
                {
                    persons.Add(new Vector3(s.Position.X, s.Position.Y * 2, s.Position.Z));
                }
                //System.Console.WriteLine(s.Position.X+" "+s.Position.Y + " " + s.Position.Z);
            }
        }
    }

    public struct Quad
    {
        public Vector3 Origin;
        public Vector3 UpperLeft;
        public Vector3 LowerLeft;
        public Vector3 UpperRight;
        public Vector3 LowerRight;
        public Vector3 Normal;
        public Vector3 Up;
        public Vector3 Left;

        public VertexPositionColorTexture[] Vertices;
        public int[] Indices;

        public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height)
        {
            Vertices = new VertexPositionColorTexture[4];
            Indices = new int[6];
            Origin = origin;
            Normal = normal;
            Up = up;

            // Calculate the quad corners
            Left = Vector3.Cross(normal, Up);
            Vector3 uppercenter = (Up * height / 2) + origin;
            UpperLeft = uppercenter + (Left * width / 2);
            UpperRight = uppercenter - (Left * width / 2);
            LowerLeft = UpperLeft - (Up * height);
            LowerRight = UpperRight - (Up * height);

            FillVertices();
        }

        private void FillVertices()
        {
            // Fill in texture coordinates to display full texture
            // on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Provide a normal for each vertex
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Color = Color.Transparent;
            }

            // Set the position and texture coordinate for each
            // vertex
            Vertices[0].Position = LowerLeft;
            Vertices[0].TextureCoordinate = textureLowerLeft;
            Vertices[1].Position = UpperLeft;
            Vertices[1].TextureCoordinate = textureUpperLeft;
            Vertices[2].Position = LowerRight;
            Vertices[2].TextureCoordinate = textureLowerRight;
            Vertices[3].Position = UpperRight;
            Vertices[3].TextureCoordinate = textureUpperRight;

            // Set the index buffer for each vertex, using
            // clockwise winding
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 2;
            Indices[4] = 1;
            Indices[5] = 3;
        }
    }
}
