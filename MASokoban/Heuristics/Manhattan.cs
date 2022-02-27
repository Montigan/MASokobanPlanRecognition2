using System;
using System.Collections.Generic;
using System.Linq;
using MASokoban.Environment;
using MASokoban.Entities;

namespace MASokoban.Heuristics
{
    class Manhattan : Heuristic
    {
        public override int H(WorldState state)
        {
            var agentLocation = state.agentLocation;
            var assignedBoxes = state.assignedBoxes;
            var goals = state.boxGoals;
            int agentToBoxDistance;
            if (assignedBoxes.Count > 0) agentToBoxDistance = int.MaxValue;
            else agentToBoxDistance = 0;

            int boxToGoalDistance = int.MaxValue;
            int totalBoxesToGoalDistance = 0;
            foreach (EntityLocation goal in goals)
            {
                foreach (KeyValuePair<Location, Box> boxLocation in assignedBoxes.Where(b => b.Value.Name.Equals(goal.Entity.Name)))
                {

                    if (boxLocation.Key.ManhattanDistanceTo(goal.Location) < boxToGoalDistance) boxToGoalDistance = boxLocation.Key.ManhattanDistanceTo(goal.Location);
                }

                totalBoxesToGoalDistance += boxToGoalDistance;
                boxToGoalDistance = int.MaxValue;
            }

            if (totalBoxesToGoalDistance < 0) throw new Exception("waat");
            return totalBoxesToGoalDistance;
        }
    }
}
