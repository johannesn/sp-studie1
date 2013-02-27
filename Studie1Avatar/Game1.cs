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

        public Avatar()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1080;

            this.graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        Quad quad;
        Matrix View, Projection, World;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            quad = new Quad(new Vector3(-0.8f, 0.0f, 2.4f), Vector3.Left, Vector3.Up, 1, 1);
            View = Matrix.CreateLookAt(new Vector3(-quad.Origin.X, quad.Origin.Y, quad.Origin.Z), quad.Origin,
                Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 4.0f / 3.0f, 1, 500);
            World = Matrix.Identity;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            persons = new List<Vector3>();

            base.Initialize();
        }

        Texture2D texture;
        Texture2D background;
        BasicEffect quadEffect;
        Rectangle borders;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("face");
            quadEffect = new BasicEffect(graphics.GraphicsDevice);

            quadEffect.World = World;
            quadEffect.View = View;
            quadEffect.Projection = Projection;
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = texture;
            quadEffect.VertexColorEnabled = false;

            background = Content.Load<Texture2D>("avatar_bg");
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            //float l = 2.0f;
            //float x = (float)Math.Sin(b) * l;
            //float y = (float)Math.Cos(b) * l;
            //View = Matrix.CreateLookAt(new Vector3(x, 0, y), Vector3.Zero, Vector3.Up);

            //Matrix.CreateRotationY(b, out World);

            if (persons.Count > 0)
            {
                Vector3 person = persons.First<Vector3>();
                Vector3 display_to_person = -1 * (quad.Origin - person);
                System.Console.WriteLine(quad.Origin + " " + person + " " + display_to_person);
                display_to_person.Normalize();
                /* Vector2 ptod_xz = new Vector2(ptod.X, ptod.Z);
                float a = (float)(Math.PI / 2 - Math.Cos(-ptod.Z) / ptod_xz.Length());
                System.Console.WriteLine(a);
                Vector3 rot = new Vector3((float)Math.Cos(a) * 1.0f, 0.0f, (float)Math.Sin(a) * 1.0f);*/

                quad = new Quad(quad.Origin, display_to_person, Vector3.Up, 1, 1);
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

            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives
                    <VertexPositionColorTexture>(
                    PrimitiveType.TriangleList,
                    quad.Vertices, 0, 4,
                    quad.Indices, 0, 2);
            }

            base.Draw(gameTime);
        }

        List<Vector3> persons;
        public void triggerAction(List<Skeleton> skeletonData)
        {
            persons.RemoveRange(0, persons.Count);
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
