using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Juicy;
using Juicy.Engine;

namespace Cavejump.Objects
{
    public class Skater : GameObj
    {
        private int maxJumpHeight;
        private int currJumpHeight;
        private int currFlyingDistance;
        private int maxFlyingDistance;
        private int lane;

        private GameObj shadowObj;

        enum JumpState
        {
            NO_JUMP, GOING_UP, GOING_DOWN, FLYING
        }

        private JumpState jumpState;

        public Skater() : base()
        {
            this.spriteName = "boy_skate";
            maxJumpHeight = 40;
            currJumpHeight = 0;
            maxFlyingDistance = 20;
            jumpState = JumpState.NO_JUMP;
        }

        public GameObj Shadow
        {
            get { return shadowObj; }
            set { shadowObj = value; }
        }

        public int Lane
        {
            get { return lane; }
            set { lane = value; }
        }

        public void StartJump()
        {
            if (jumpState == JumpState.NO_JUMP)
            {
                currJumpHeight = 0;
                currFlyingDistance = 0;
                jumpState = JumpState.GOING_UP;
            }
        }

        public override void Update(long timer)
        {
            base.Update(timer);
            if (jumpState == JumpState.GOING_UP)
            {
                currJumpHeight += 5;
                UpdatePosition(this.Position.X, this.Position.Y - 5);
                if (currJumpHeight == maxJumpHeight)
                {
                    jumpState = JumpState.FLYING;
                }
            }
            else if (jumpState == JumpState.FLYING)
            {
                currFlyingDistance += 5;
                if (currFlyingDistance == maxFlyingDistance)
                {
                    jumpState = JumpState.GOING_DOWN;
                }
            }
            else if (jumpState == JumpState.GOING_DOWN)
            {
                currJumpHeight -= 5;
                UpdatePosition(this.Position.X, this.Position.Y + 5);
                if (currJumpHeight == 0)
                {
                    jumpState = JumpState.NO_JUMP;
                }
            }
        }

        public void syncShadowPosition()
        {
            if (shadowObj != null && !IsJumping)
            {
                shadowObj.UpdatePosition(position.X, position.Y + (this.W + shadowObj.W) / 2);
            }
        }

        public bool IsJumping
        {
            get { return jumpState != JumpState.NO_JUMP; }
        }

        protected override void updateChildObjs()
        {
            base.updateChildObjs();
            syncShadowPosition();
        }
    }
}
