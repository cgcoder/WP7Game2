using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

using Juicy.Engine;
using Cavejump.Objects;
using CaveJump;

namespace Cavejump.Screens
{
    public class PlayScreen2 : JuicyScreen
    {
        private List<GameObj> freeGameObjects;
        private Dictionary<int, List<GameObj>> gameObjects;
        private GameObj[] roadObjects;

        private int speed;
        private int currentSceneId;
        private int concurrentSceneCount = 4;
        private int lastScreenOffset = 0;
        private int distanceTravelled = 0;
        private GameMapData mapData;

        public PlayScreen2()
        {
            freeGameObjects = new List<GameObj>();
            gameObjects = new Dictionary<int, List<GameObj>>();
            roadObjects = new GameObj[6];
        }

        public override void LoadSprites(ContentManager conMan)
        {
            game.SprManager.LoadSprite("road");
            game.SprManager.LoadSprite("road_block");
        }

        public override void ScreenBecomesCurrent()
        {
            for (int i = 0; i < roadObjects.Length; i++)
            {
                roadObjects[i] = new GameObj();
                roadObjects[i].SpriteName = "road";
                base.ObjectManager.AddGameObject(roadObjects[i]);
                roadObjects[i].UpdatePosition(roadObjects[i].W / 2 + i * roadObjects[i].W, 480 - roadObjects[i].H - 50);
            }

            freeGameObjects.Clear();
            gameObjects.Clear();

            LoadMap();
        }

        private void LoadMap()
        {
            currentSceneId = -1;
            mapData = SampleMaps.GetSampleMap1();

            LoadCurrentScene();
        }

        private void LoadCurrentScene()
        {
            List<GameObj> tempList = null;

            if (currentSceneId != -1) // first load
            {
                if (gameObjects.ContainsKey(currentSceneId))
                {
                    tempList = gameObjects[currentSceneId];
                    gameObjects.Remove(currentSceneId);
                    
                    // reclaim object
                    foreach (GameObj obj in tempList)
                    {
                        base.ObjectManager.RemoveGameObj(obj);
                        freeGameObjects.Add(obj);
                    }

                    tempList.Clear(); tempList = null;
                }
            }

            currentSceneId++;

            // first scene is a special case
            if (currentSceneId == 0)
            {
                for (int i = currentSceneId; i < currentSceneId + concurrentSceneCount; i++)
                {
                    foreach (SceneObject so in mapData.Scenes[i].Objects)
                    {
                        GameObj go = null;

                        if (freeGameObjects.Count > 0)
                        {
                            go = freeGameObjects[0];
                            freeGameObjects.RemoveAt(0);
                        }
                        else
                        {
                            go = new GameObj();
                        }

                        List<GameObj> usedLst = null;

                        if (!gameObjects.ContainsKey(currentSceneId))
                        {
                            usedLst = new List<GameObj>();
                            gameObjects[currentSceneId] = usedLst;
                        }
                        else
                        {
                            usedLst = gameObjects[currentSceneId];
                        }

                        usedLst.Add(go);
                        updateGameObjectFromsceneData(i, go, so);
                        
                    }

                    lastScreenOffset += game.Graphics.PreferredBackBufferWidth;
                }
            }
            else
            {
                foreach (SceneObject so in mapData.Scenes[currentSceneId + concurrentSceneCount].Objects)
                {
                    GameObj go = null;

                    if (freeGameObjects.Count > 0)
                    {
                        go = freeGameObjects[0];
                        freeGameObjects.RemoveAt(0);
                    }
                    else
                    {
                        go = new GameObj();
                    }

                    List<GameObj> usedLst = null;

                    if (!gameObjects.ContainsKey(currentSceneId))
                    {
                        usedLst = new List<GameObj>();
                        gameObjects[currentSceneId] = usedLst;
                    }
                    else
                    {
                        usedLst = gameObjects[currentSceneId];
                    }

                    usedLst.Add(go);
                    updateGameObjectFromsceneData(currentSceneId + concurrentSceneCount, go, so);
                }

                lastScreenOffset += game.Graphics.PreferredBackBufferWidth;
            }

        }

        private void updateGameObjectFromsceneData(int scene, GameObj go, SceneObject so)
        {
            go.SpriteName = "road_block";
            base.ObjectManager.AddGameObject(go);

            int offset = lastScreenOffset;

            go.UpdatePosition(so.X + offset, so.Lane == Constants.LANE_F_M ? 480 - 130 : 480 - 180);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            base.Update(time);

            distanceTravelled += speed;
            lastScreenOffset -= speed;

            for(int i = 0; i < roadObjects.Length; i++)
            {
                roadObjects[i].UpdatePosition(roadObjects[i].Position.X - speed, roadObjects[i].Position.Y);

                if (roadObjects[i].Position.X < -roadObjects[0].W/2 - 10) // some grace
                {
                    int tx = (int) (i == 0 ? roadObjects[roadObjects.Length - 1].Position.X : roadObjects[i - 1].Position.X) + roadObjects[i].W - speed;
                    roadObjects[i].UpdatePosition(tx, roadObjects[i].Position.Y);
                }
            }

            foreach (KeyValuePair<int, List<GameObj>> gos in gameObjects)
            {
                foreach (GameObj go in gos.Value)
                {
                    go.UpdatePosition(go.Position.X - speed, go.Position.Y);
                }
            }

            if (distanceTravelled >= 900)
            {
                distanceTravelled -= 900;
                currentSceneId++;
                LoadCurrentScene();
            }
            
        }

        public override bool HandleTouch(TouchCollection tc)
        {
            TouchLocation tl = tc[0];

            if (tl.Position.X < 400 && tl.State == TouchLocationState.Released)
            {
                if (tl.Position.X < 200 && speed > 0)
                {
                    speed--;
                }
                else if (tl.Position.X > 200 && speed < 10)
                {
                    speed++;
                }
            }

            return true;
        }
    }
}
