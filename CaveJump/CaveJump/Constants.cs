using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaveJump
{
    public static class Constants
    {
        public const int LANE_F_F = 11;
        public const int LANE_F_B = 12;

        public const int LANE_M_B = 14;
        public const int LANE_M_F = 13;

        public const int LANE_B_B = 13;
        public const int LANE_B_F = 12;

        public const int MIN_LANE = LANE_F_F;
        public const int MAX_LANE = LANE_M_B;

        public const int ROAD = 1;
        public const int ON_ROAD = 2;

        public const int TWO_LANE_ROAD = 100;
        public const int THREE_LANE_ROAD = 101;

        public const int ROAD_BLOCK_T = 120;
        public const int BARRICADE = 121;

    }
}
