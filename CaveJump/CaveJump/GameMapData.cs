using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CaveJump
{
    public class SceneObject
    {
        public int Type { get; set; }
        public int Lane { get; set; }
        public int SceneId { get; set; }
        public int X { get; set; } // reference to scene
        public int W { get; set; }
        public int H { get; set; }
    }

    public class SceneData
    {
        private List<SceneObject> objects;

        public SceneData()
        {
            objects = new List<SceneObject>();
        }

        public List<SceneObject> Objects
        {
            get { return objects; }
        }

        public void AddObject(SceneObject obj)
        {
            objects.Add(obj);
        }

    }

    public class GameMapData
    {
        private List<SceneData> scenes;

        public GameMapData()
        {
            scenes = new List<SceneData>();
        }

        public List<SceneData> Scenes
        {
            get { return scenes; }
        }

        public int RoadType
        {
            get;
            set;
        }
    }
}
