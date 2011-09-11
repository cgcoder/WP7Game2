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
        private Dictionary<int, int> LanePositionY;
        private GameObj[] roadObjects;
        private Skater skateObj;
        private int speed;
        private int currentSceneId;
        private int concurrentSceneCount = 4;
        private int lastScreenOffset = 0;
        private int distanceTravelled = 0;
        private GameMapData mapData;
        private GameObj speedBar;
        private GameObj speedKnob;

        public PlayScreen2()
        {
            freeGameObjects = new List<GameObj>();
            gameObjects = new Dictionary<int, List<GameObj>>();
            roadObjects = new GameObj[6];

            LanePositionY = new Dictionary<int, int>();
        }

        public override void LoadSprites(ContentManager conMan)
        {
            game.SprManager.LoadSprite("road");
            game.SprManager.LoadSprite("road_block");
            game.SprManager.LoadSprite("boy_skate");
            game.SprManager.LoadSprite("speed");
            game.SprManager.LoadSprite("knob");
            game.SprManager.LoadSprite("boy_shadow");
        }

        public override void ScreenBecomesCurrent()
        {
            freeGameObjects.Clear();
            gameObjects.Clear();

            for (int i = 0; i < roadObjects.Length; i++)
            {
                roadObjects[i] = new GameObj();
                roadObjects[i].SpriteName = "road";
                base.ObjectManager.AddGameObject(roadObjects[i], Constants.ROAD);
                roadObjects[i].UpdatePosition(roadObjects[i].W / 2 + i * roadObjects[i].W, 480 - roadObjects[i].H - 50);
            }

            skateObj = new Skater();

            base.ObjectManager.AddGameObject(skateObj, Constants.LANE_F_M);

            int roadTopEdge = (int) roadObjects[0].Position.Y - roadObjects[0].H/2 - skateObj.H/2 + 20;
            LanePositionY.Add(Constants.LANE_B_B, roadTopEdge);
            LanePositionY.Add(Constants.LANE_B_M, roadTopEdge + 15);
            LanePositionY.Add(Constants.LANE_B_F, roadTopEdge + 30);
            int roadBottomEdge = (int)roadObjects[0].Position.Y + roadObjects[0].H / 2 - skateObj.H/2 + 10;

            LanePositionY.Add(Constants.LANE_F_B, roadBottomEdge - 30);
            LanePositionY.Add(Constants.LANE_F_M, roadBottomEdge - 15);
            LanePositionY.Add(Constants.LANE_F_F, roadBottomEdge);
            skateObj.UpdatePosition(100, LanePositionY[skateObj.ZOrder]);

            speedBar = new GameObj
            {
                SpriteName = "speed"
            };

            speedKnob = new GameObj
            {
                SpriteName = "knob"
            };


            base.ObjectManager.AddScreenObject(speedBar);
            speedBar.UpdatePosition(200, game.Graphics.PreferredBackBufferHeight - speedBar.H - 10);

            base.ObjectManager.AddScreenObject(speedKnob);
            speedKnob.UpdatePosition(50, speedBar.Position.Y);

            skateObj.Shadow = new GameObj
            {
                SpriteName = "boy_shadow"
            };

            base.ObjectManager.AddGameObject(skateObj.Shadow, Constants.ON_ROAD);
            skateObj.syncShadowPosition();
            LoadMap();
        }

        private void LoadMap()
        {
            currentSceneId = -1;
            mapData = SampleMaps.GetSampleMap1();

            LoadNextScene();
        }

        private void LoadNextScene()
        {
            List<GameObj> tempList = null;

            if (currentSceneId != -1) // not first load
            {
                if (gameObjects.ContainsKey(currentSceneId))
                {
                    tempList = gameObjects[currentSceneId];
                    gameObjects.Remove(currentSceneId);
                    
                    // reclaim object
                    foreach (GameObj obj in tempList)
                    {
                        base.ObjectManager.RemoveGameObj(obj, obj.ZOrder);
                        freeGameObjects.Add(obj);
                    }

                    tempList.Clear(); tempList = null;
                }
            }

            currentSceneId++;

            // first scene is a special case
            if (currentSceneId == 0)
            {
                for (int i = 0; i < concurrentSceneCount; i++)
                {
                    loadObjectsOfScene(i);
                    lastScreenOffset += game.Graphics.PreferredBackBufferWidth;
                }
            }
            else
            {
                loadObjectsOfScene(currentSceneId + concurrentSceneCount - 1);
                lastScreenOffset += game.Graphics.PreferredBackBufferWidth;
            }

        }

        private void updateGameObjectFromsceneData(int scene, GameObj go, SceneObject so)
        {
            go.SpriteName = "road_block";
            base.ObjectManager.AddGameObject(go, so.Lane);

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
                //currentSceneId++;
                LoadNextScene();
            }
            
        }

        public override bool HandleTouch(TouchCollection tc)
        {
            TouchLocation tl = tc[0];

            if (tl.Position.X < 400 && tl.State == TouchLocationState.Released)
            {
                int startTemp = (int)  (speedBar.Position.X - speedBar.W / 2);
                int temp =(int) (tl.Position.X - startTemp);

                if (temp < 0) temp = 0;
                if (temp > 300) temp = 300;

                speed = temp / 30;

                speedKnob.UpdatePosition(startTemp + speed * 30, speedKnob.Position.Y);
            }
            else if (tl.State == TouchLocationState.Released && !skateObj.IsJumping)
            {
                skateObj.StartJump();
            }

            return true;
        }

        private void loadObjectsOfScene(int scene)
        {
            foreach (SceneObject so in mapData.Scenes[scene].Objects)
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

                if (!gameObjects.ContainsKey(scene))
                {
                    usedLst = new List<GameObj>();
                    gameObjects[scene] = usedLst;
                }
                else
                {
                    usedLst = gameObjects[scene];
                }

                usedLst.Add(go);
                updateGameObjectFromsceneData(scene, go, so);

            }
        }
    }
}
