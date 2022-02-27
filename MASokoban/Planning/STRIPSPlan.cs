using System;
using System.Collections.Generic;
using MASokoban.Actions;
using MASokoban.Entities;
using MASokoban.Fluents;
using MASokoban.Landmark;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.STRIPS
{
    class STRIPSPlan
    {
        List<Fluent> Fluents;
        List<AgentAction> Actions;
        public STRIPSPlan(List<Fluent> fluents, List<AgentAction> actions)
        {
            Fluents = fluents;
            Actions = actions;

        }
    }
}
