using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MASokoban.Environment;

namespace MASokoban.Entities
{
    class EntityLocation
    {
        public Entity Entity { get; set; }
        public Location Location { get; set; }

        public EntityLocation(Entity entity, Location location)
        {
            Entity = entity;
            Location = location;
        }
    }
}
