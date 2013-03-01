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
        const int width = 1440;
        const int height = 900;
        const float smileyY = 55.0f / height;
        const float smileyWidth = 300.0f / height;
        const float smileyHeight = 300.0f / height;
        Vector3 displayOrigin = new Vector3(-1.5f, 0.0f, 2.3f);


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

        List<Quad> smileys;
        Matrix View, Projection, World;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            View = Matrix.CreateLookAt(new Vector3(displayOrigin.X + 0.5f, displayOrigin.Y, displayOrigin.Z), displayOrigin,
                Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver2, 16.0f / 9.0f, 0.1f, 10.0f);
            World = Matrix.Identity;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

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

            smileys.RemoveRange(0, smileys.Count);
            if (persons.Count > 0)
            {
                int smileyIndex = 0;
                float simleyRegionWidth = 1.0f * 16.0f / 9.0f / (float)persons.Count;
                float smileyOffset = displayOrigin.Z + 0.5f * 16.0f / 9.0f - simleyRegionWidth / 2.0f;
                foreach (Vector3 person in persons)
                {
                    Vector3 smileyOrigin = new Vector3(displayOrigin.X, smileyY + displayOrigin.Y, smileyOffset - simleyRegionWidth * (float)smileyIndex);
                    Vector3 display_to_person = -1 * (smileyOrigin - person);
                    display_to_person.Normalize();
                    Vector3 up = Vector3.Cross(display_to_person, Vector3.Forward);
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
                foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
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
