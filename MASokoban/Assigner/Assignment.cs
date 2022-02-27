using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MASokoban.Entities;

namespace MASokoban.Assigner
{
    class Assignment
    {
        public Agent Agent { get; set; }
        public EntityLocation EntityLocation { get; set; }

        public Assignment(Agent agent, EntityLocation entityLocation)
        {
            Agent = agent;
            EntityLocation = entityLocation;
        }
    }
}
