using MASokoban.Environment;
using MASokoban.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Actions
{
    class Move : AgentAction
    {
        public Move()
        {
            this.Cost = 1;
        }

        private bool isPossible(WorldState worldState, Directions agentDir)
        {
            Location newAgentLocation = Location.RelativeLocation(worldState.agentLocation, agentDir);

            if (!(worldState.assignedBoxes.TryGetValue(newAgentLocation, out Box box) && box.Colour.Equals(worldState.agent.Colour)) &&
                !worldState.Walls.TryGetValue(newAgentLocation, out Location wallLocation))
            {
                return true;
            }
            return false;
        }

        public override bool TryPerformAction(WorldState worldState, Directions agentDir, Directions boxDir = Directions.None)
        {
            if (this.isPossible(worldState, agentDir))
            {
                Location newAgentLocation = Location.RelativeLocation(worldState.agentLocation, agentDir);
                worldState.agentLocation = newAgentLocation;
                return true;
            }
            return false;
        }
    }
}
