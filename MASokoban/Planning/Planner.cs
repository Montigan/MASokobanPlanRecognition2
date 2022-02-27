using System.Collections.Generic;
using MASokoban.Actions;
using MASokoban.Entities;
using MASokoban.Heuristics;
using MASokoban.Environment;
using static MASokoban.Planning.ConstraintState;

namespace MASokoban.Planning
{
    abstract class Planner
    {
        protected const int searchNodeLimit = 1000000;
        protected Agent agent;
        protected Heuristic heuristic;
        protected Move move;
        protected Pull pull;
        protected Push push;
        protected NoOp noOp;
        protected const int conflictTimeRange = 0;
        protected const int conflictDistRange = 0;

        protected Planner()
        {
            this.move = new Move();
            this.pull = new Pull();
            this.push = new Push();
            this.noOp = new NoOp();
        }

        public abstract List<WorldState> MakePlan(WorldState initialState, HashSet<Constraint> constraints);
    }
}
