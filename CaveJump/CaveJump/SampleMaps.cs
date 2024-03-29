using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaveJump
{
    static class SampleMaps
    {
        public static GameMapData GetSampleMap1()
        {
            GameMapData data = new GameMapData();
            data.RoadType = Constants.TWO_LANE_ROAD;

            for (int i = 0; i < 50; i++)
            {
                SceneData scene = new SceneData();

                for (int j = 0; j < 3; j++)
                {
                    scene.Objects.Add(new SceneObject
                    {
                        Type = Constants.ROAD_BLOCK_T,
                        Lane = (j % 2) == 0 ? Constants.LANE_F_F : Constants.LANE_F_F,
                        SceneId = i,
                        X = (j+1)*(800/3),
                        W = 30,
                        H = 70
                    });
                }

                scene.Objects.Add(new SceneObject
                {
                    Type = Constants.BARRICADE,
                    Lane = Constants.LANE_F_F,
                    SceneId = i,
                    X = 350,
                    W = 50,
                    H = 90
                });

                data.Scenes.Add(scene);
            }
            
            return data;
        }
    }
}
