using System;
using System.Collections.Generic;
using MASokoban.Entities;
using MASokoban.Actions;
using MASokoban.Fluents;
using MASokoban.Landmark;
using MASokoban.Heuristics;
using MASokoban.Planning;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Environment
{
    class Map
    {
        public Dictionary<Location, Entity> LevelByLocation { get; private set; }
        public Dictionary<Entity, Location> LevelByEntity { get; private set; } // PUT NO GOALS IN HERE!
        public List<EntityLocation> Goals { get; private set; }
        public HashSet<Location> Walls { get; private set; }

        public Map(Dictionary<Location,Entity> levelByLocation, List<EntityLocation> goals, HashSet<Location> walls)
        {
            InitialiseMap(levelByLocation, goals, walls);

           
        }

        private void InitialiseMap(Dictionary<Location, Entity> levelByLocation, List<EntityLocation> goals, HashSet<Location> walls)
        {
            LevelByLocation = levelByLocation;
            Goals = goals;
            Walls = walls;

            LevelByEntity = new Dictionary<Entity, Location>();
            foreach (var entry in levelByLocation)
            {
                LevelByEntity.Add(entry.Value, entry.Key);
            }
        }
    }
}
