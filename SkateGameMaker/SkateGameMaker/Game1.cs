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

namespace SkateGameMaker
{
    class GameObject
    {
        public Texture2D Texture
        {
            get;
            set;
        }

        public Rectangle Rect
        {
            get;
            set;
        }

        public LaneType Lane
        {
            get;
            set;
        }

        public ObjectType Type
        {
            get;
            set;
        }

        public bool Selected
        {
            get;
            set;
        }
    }

    public enum ObjectType
    {
        ROAD_BLOCK_T = 120,
        BARRICADE = 121
    }

    public enum LaneType
    {
        MIN = M_B,
        M_B = 0,
        M_F = 1,
        F_B = 2,
        F_F = 3,
        MAX = F_F
    }

    class Scene
    {
        private Dictionary<LaneType, List<GameObject>> gameObjects;

        public Scene()
        {
            gameObjects = new Dictionary<LaneType, List<GameObject>>();
        }

        public int Id
        {
            get;
            set;
        }

        public List<GameObject> GetObjectsOfLane(LaneType lane)
        {
            if (gameObjects.ContainsKey(lane)) 
            {
                return gameObjects[lane];
            }

            return null;
        }

        public void InitLane(LaneType lane)
        {
            if (!gameObjects.ContainsKey(lane))
            {
                gameObjects.Add(lane, new List<GameObject>());
            }
        }

        public void MoveObject(LaneType from, LaneType to, GameObject obj)
        {
            if (gameObjects.ContainsKey(from) && gameObjects[from].Contains(obj))
            {
                gameObjects[from].Remove(obj);
            }

            AddObjectToLane(to, obj);
        }

        public void AddObjectToLane(LaneType lane, GameObject obj)
        {
            List<GameObject> objs = null;

            if (gameObjects.ContainsKey(lane))
            {
                objs = gameObjects[lane];
            }
            else
            {
                objs = new List<GameObject>();
                gameObjects.Add(lane, objs);
            }

            objs.Add(obj);
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game graphics resources
        private GameObject backGround;
        private GameObject[] roadObjects;

        private SpriteFont smallFont;

        private string helpText;

        private List<Scene> scenes;

        private const int TOP_OFFSET = 50;

        private GameObject selectedObject = null;

        private bool keyDown = false;
        private Keys pressedKey = Keys.None;

        int currentScene = 0;
        private Dictionary<int, int> LanePositionY;
        private Dictionary<ObjectType, Texture2D> textures;

        private bool mouseDown = false;

        private ObjectType currentType = ObjectType.ROAD_BLOCK_T;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 700;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            scenes = new List<Scene>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            int i = 0;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            backGround = new GameObject();
            backGround.Texture = this.Content.Load<Texture2D>("black");
            backGround.Rect = new Rectangle((this.graphics.PreferredBackBufferWidth - backGround.Texture.Width) / 2, TOP_OFFSET, 
                backGround.Texture.Width, backGround.Texture.Height);

            roadObjects = new GameObject[4];

            smallFont = this.Content.Load<SpriteFont>("smallFont");

            Texture2D roadTexture = this.Content.Load<Texture2D>("road");

            for(i = 0; i < roadObjects.Length;i++)
            {
                roadObjects[i] = new GameObject();
                roadObjects[i].Texture = roadTexture;
                roadObjects[i].Rect = new Rectangle(i * roadObjects[i].Texture.Width, 480 - roadObjects[i].Texture.Height / 2 - 100 - TOP_OFFSET,
                    roadObjects[i].Texture.Width, roadObjects[i].Texture.Height);
            }

            LanePositionY = new Dictionary<int, int>();

            LanePositionY.Add(0, 270);
            LanePositionY.Add(1, 290);
            LanePositionY.Add(2, 315);
            LanePositionY.Add(3, 340);

            scenes.Add(CreateScene());

            textures = new Dictionary<ObjectType, Texture2D>();
            textures.Add(ObjectType.ROAD_BLOCK_T, this.Content.Load<Texture2D>("road_block"));
            textures.Add(ObjectType.BARRICADE, this.Content.Load<Texture2D>("barricade"));

            buildHelpText();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private Scene CreateScene()
        {
            Scene s = new Scene();

            for (int i = 0; i < 4; i++)
            {
                s.InitLane((LaneType) i);
            }

            return s;
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            Keys[] keys = Keyboard.GetState().GetPressedKeys();

            if (keys.Length > 0)
            {
                keyDown = true;
                pressedKey = keys[0];
            }

            if (keyDown && Keyboard.GetState().IsKeyUp(pressedKey))
            {
                keyDown = false;
                KeyRelease(pressedKey);
            }

            MouseState st = Mouse.GetState();

            if (st.LeftButton == ButtonState.Pressed)
            {
                mouseDown = true;
            }

            if (mouseDown && st.LeftButton == ButtonState.Released)
            {
                HandleMouse();
                mouseDown = false;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void buildHelpText()
        {
            helpText = 
@"R-Road Block, B-Barricade
Up/Down Arrow - Change Lane
Left/Right Arrow - Change Scene
";
        }

        private void DrawHelpText(SpriteBatch batch)
        {
            batch.DrawString(smallFont, helpText, new Vector2(50, 480 + TOP_OFFSET), Color.White);
        }

        private void KeyRelease(Keys key)
        {
            if (key == Keys.B)
            {
                currentType = ObjectType.BARRICADE;
            }
            else if (key == Keys.R)
            {
                currentType = ObjectType.ROAD_BLOCK_T;
            }
        }

        private void HandleMouse()
        {
            MouseState ms = Mouse.GetState();

            GameObject go = new GameObject();
            go.Type = currentType;
            go.Texture = textures[go.Type];
            go.Rect = new Rectangle(ms.X, LanePositionY[3], go.Texture.Width, go.Texture.Height);
            go.Lane = LaneType.F_F;

            scenes[currentScene].AddObjectToLane(go.Lane, go);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            spriteBatch.Draw(backGround.Texture, backGround.Rect, Color.Black);

            foreach (GameObject road in roadObjects)
            {
                spriteBatch.Draw(road.Texture, road.Rect, road.Selected ? Color.Blue : Color.White);
            }

            for(int lane = 0; lane < 4; lane++)
            {
                foreach (GameObject go in scenes[currentScene].GetObjectsOfLane((LaneType) lane))
                {
                    spriteBatch.Draw(go.Texture, go.Rect, go.Selected ? Color.Blue : Color.White);
                }
            }

            DrawHelpText(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
