using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Juicy.Engine;

namespace Cavejump.Objects
{
    public class ArmyMan : GameObj
    {
        private Texture2D body;
        private Texture2D frontHandAndGun;
        private Texture2D backHand;

        private Vector2 frontHandPos;
        private Vector2 frontHandAnchorPos;

        private Vector2 backHandPos;
        private Vector2 backHandAnchorPos;

        private float gunAngle;

        public ArmyMan()
            : base()
        {
            gunAngle = 0f;
        }

        public override void OnObjManagerAdd(GameObjectManager gom)
        {
            body = gom.Game.SprManager.GetSprite("armybody");

            this.sprite = body;

            boundary = new Rectangle(0, 0, (int)(sprite.Width * scale), (int)(sprite.Height * scale));

            center.X = body.Width / 2;
            center.Y = body.Height / 2;
            anchor = new Microsoft.Xna.Framework.Vector2(0f, 0f);

            //-- hnd gun
            frontHandAndGun = gom.Game.SprManager.GetSprite("handgun");

            backHand = gom.Game.SprManager.GetSprite("backhand");
        }

        public float GunAngle
        {
            get { return gunAngle; }
            set { gunAngle = value; }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (!visible) return;

            batch.Draw(backHand, backHandPos, null, Color.White, gunAngle,
                backHandAnchorPos, 1f, SpriteEffects.None, 0.0f);

            base.Draw(batch);

            batch.Draw(frontHandAndGun, frontHandPos, null, Color.White, gunAngle,
                frontHandAnchorPos, 1f, SpriteEffects.None, 0.0f);
        }

        protected override void updateChildObjs()
        {
            updatePositions();
        }

        private void updatePositions()
        {
            backHandPos = new Vector2(Position.X + 18, position.Y + 22);
            backHandAnchorPos = new Vector2(4f, 4f);

            frontHandPos = new Vector2(Position.X + 15, position.Y + 20);
            frontHandAnchorPos = new Vector2(4f, 4f);
        }
    }
}
