using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MASokoban.Environment;

namespace MASokoban.Actions
{
    class NoOp : AgentAction
    {
        public NoOp()
        {
            Cost = 1;
        }

        public override bool TryPerformAction(WorldState worldState, Directions agentDir = Directions.None, Directions boxDir = Directions.None)
        {
            return true; //Always possible and doesn't change anything
        }
    }
}
