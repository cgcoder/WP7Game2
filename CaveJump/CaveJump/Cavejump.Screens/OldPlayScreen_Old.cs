using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

using Juicy.Engine;
using Cavejump.Objects;

namespace Cavejump.Screens
{
    public class PlayScreen : JuicyScreen
    {
        
        private GameObj world;
        private ArmyMan armyMan;

        private TouchLocation pressedLocation;
        private bool bullet;
        private GameObj bulletObj;

        public PlayScreen()
            : base()
        {
            bullet = false;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void LoadSprites(ContentManager conMan)
        {
            game.SprManager.LoadSprite("world");
            game.SprManager.LoadSprite("armybody");
            game.SprManager.LoadSprite("handgun");
            game.SprManager.LoadSprite("backhand");
            game.SprManager.LoadSprite("bullet");
        }

        private void BuildObjects()
        {
            world = new GameObj();
            world.SpriteName = "world";

            bulletObj = new GameObj();
            bulletObj.SpriteName = "bullet";
            bulletObj.Visible = false;

            armyMan = new ArmyMan();
            objectManager.AddGameObject(world);
            objectManager.AddGameObject(bulletObj);
            objectManager.AddGameObject(armyMan);
        }

        public override void ScreenBecomesCurrent()
        {
            base.ScreenBecomesCurrent();
            BuildObjects();

            armyMan.UpdatePosition(73.0f - armyMan.W/4, 250.0f - armyMan.H);

            world.UpdateAnchorPoint(0, 0);
            world.UpdatePosition(0, 0);
        }

        public override void ScreenBecomesNotCurrent()
        {
            base.ScreenBecomesNotCurrent();

            world.Dispose();
            objectManager.ClearObjects(false);
        }

        private void PerformShoot()
        {
            bulletObj.Visible = true;
            bulletObj.Rotation = armyMan.GunAngle;
            bulletObj.UpdatePosition(armyMan.Position.X + 15, armyMan.Position.Y + 20);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            base.Update(time);

            if (bulletObj.Visible)
            {
                float x, y;
                x = bulletObj.Position.X + (float) (50 * Math.Cos(bulletObj.Rotation));
                y = bulletObj.Position.Y + (float) (50 * Math.Sin(bulletObj.Rotation));
                bulletObj.UpdatePosition(x, y);
            }
        }

        public override bool HandleTouch(TouchCollection tc)
        {
            TouchLocation tl = tc[0];
            TouchLocation tl2;

            bool shoot = false;
            bool moveGun = false;

            if ((tc[0].State == TouchLocationState.Released && tc[0].Position.X > 400) 
                    || (tc.Count > 1 && tc[1].Position.X > 400 && tc[1].State == TouchLocationState.Released))
            {
                shoot = true;    
            }

            if (tc[0].Position.X < 400)
            {
                tl = tc[0];
                moveGun = true;
            }
            else if(tc.Count > 1 && tc[1].Position.X < 400)
            {
                tl = tc[1];
                moveGun = true;
            }

            if (shoot)
            {
                PerformShoot();
            }

            if (moveGun)
            {
                if (tl.State == TouchLocationState.Pressed)
                {
                    pressedLocation = tl;
                }
                else if (tl.State == TouchLocationState.Moved)
                {
                    armyMan.GunAngle = (float)Math.PI * (tl.Position.Y - pressedLocation.Position.Y) / 100;
                }
            }

            return true;
        }
    }
}
