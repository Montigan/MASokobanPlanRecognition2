using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MASokoban.Environment;
using MASokoban.Entities;

namespace MASokoban.Actions
{
    class Pull : AgentAction
    {
        public Pull()
        {
            this.Cost = 1;
        }

        private bool isPossible(WorldState worldState, Directions agentDir, Directions boxDir)
        {
            Location newAgentLocation = Location.RelativeLocation(worldState.agentLocation, agentDir);
            Location currentBoxLocation = Location.RelativeLocation(worldState.agentLocation, boxDir);

            if (!(worldState.assignedBoxes.TryGetValue(newAgentLocation, out Box box) && box.Colour.Equals(worldState.agent.Colour)) &&
                !worldState.Walls.TryGetValue(newAgentLocation, out Location wallLocation) &&
                worldState.assignedBoxes.TryGetValue(currentBoxLocation, out box) && box.Colour.Equals(worldState.agent.Colour))
            {
                return true;
            }
            return false;
        }

        public override bool TryPerformAction(WorldState worldState, Directions agentDir, Directions boxDir)
        {
            Location newAgentLocation = Location.RelativeLocation(worldState.agentLocation, agentDir);
            Location currentBoxLocation = Location.RelativeLocation(worldState.agentLocation, boxDir);
            Location newBoxLocation = worldState.agentLocation;

            if (this.isPossible(worldState, agentDir, boxDir))
            {
                worldState.assignedBoxes.TryGetValue(currentBoxLocation, out Box box); //Safe because isPossible
                worldState.assignedBoxes.Remove(currentBoxLocation);
                worldState.assignedBoxes.Add(newBoxLocation, box);

                worldState.agentLocation = newAgentLocation;

                return true;
            }
            return false;
        }
    }
}
