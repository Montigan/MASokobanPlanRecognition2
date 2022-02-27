using System;
using System.Collections.Generic;
using System.Linq;
using MASokoban.Environment;
using MASokoban.Actions;
using MASokoban.Entities;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Heuristics
{
    class BFS : Heuristic
    {
        private static bool SimulateMove(WorldState state, AgentAction.Directions direction)
        {
            Location newAgentLocation = Location.RelativeLocation(state.agentLocation, direction);
            if (state.Walls.TryGetValue(newAgentLocation, out Location wallLocation)) return false;
            else
            {
                state.agentLocation = newAgentLocation;
                return true;
            }
        }

        public static float CalcH(WorldState state, List<(EntityLocation, int[,], float priority)> hMatrix)
        {
            var locations = state.assignedBoxes.Keys;
            float total = 0;

            foreach ((EntityLocation, int[,], float priority) m in hMatrix)
            {
                int shortest = int.MaxValue;
                Location shortestBox = new Location();

                foreach (Location loc in locations)
                {
                    if (m.Item2[loc.x, loc.y] < shortest && state.assignedBoxes.TryGetValue(loc, out Box box) && box.Name.Equals(m.Item1.Entity.Name))
                    {
                        shortest = m.Item2[loc.x, loc.y];
                        shortestBox = loc;
                    }
                }
                total += (float)(shortest * m.Item3);
            }
            return total;
        }

        public static List<(WorldState, List<(EntityLocation, int[,], float priority)>)> PrecalcH(List<WorldState> states)
        {
            List<(WorldState, List<(EntityLocation, int[,], float priority)>)> initialStatesWithHeuristics = new List<(WorldState, List<(EntityLocation, int[,], float priority)>)>();
            foreach (WorldState state in states)
            {
                WorldState initialState = new WorldState(WorldState.CloneBoxes(state.assignedBoxes), state);
                WorldState current;
                WorldState childState;
                List<AgentAction.Directions> directions = Enum.GetValues(typeof(AgentAction.Directions)).OfType<AgentAction.Directions>().ToList().Where(d => d != AgentAction.Directions.None).ToList();
                List<(EntityLocation, int[,], float priority)> goalHeuristics = new List<(EntityLocation, int[,], float priority)>();

                foreach (EntityLocation goal in initialState.boxGoals)
                {
                    int[,] hMatrix = new int[50, 50];
                    for (int row = 0; row < 50; row++)
                    {
                        for (int column = 0; column < 50; column++)
                        {
                            hMatrix[row, column] = int.MaxValue;
                        }
                    }

                    initialState.agentLocation = goal.Location;
                    initialState.G = 0;
                    HashSet<WorldState> explored = new HashSet<WorldState>();
                    Queue<WorldState> Q = new Queue<WorldState>();
                    Q.Enqueue(initialState);
                    explored.Add(initialState);

                    //Assign value to each tile
                    while (Q.Count > 0)
                    {
                        current = Q.Dequeue();


                        hMatrix[current.agentLocation.x, current.agentLocation.y] = current.G;

                        foreach (AgentAction.Directions agentDirection in directions)
                        {
                            childState = new WorldState(WorldState.CloneBoxes(current.assignedBoxes), current);
                            if (SimulateMove(childState, agentDirection) && !explored.Contains(childState))
                            {
                                childState.Parent = current;
                                childState.G = childState.Parent.G + 1;
                                Q.Enqueue(childState);
                                explored.Add(childState);
                            }
                        }
                    }

                    float priority = 1;
                    foreach (AgentAction.Directions agentDirection in directions)
                    {
                        if (initialState.Walls.TryGetValue(Location.RelativeLocation(goal.Location, agentDirection), out Location loc))
                        {
                            priority += (float)2;
                        }
                        else if (initialState.boxGoals.Any(bG => bG.Location.Equals(Location.RelativeLocation(goal.Location, agentDirection))))
                        {
                            priority += (float)1;
                        }
                    }
                    goalHeuristics.Add((goal, hMatrix, priority));
                }
                initialStatesWithHeuristics.Add((state, goalHeuristics));
            }
            return initialStatesWithHeuristics;
        }

        public override int H(WorldState state)
        {
            WorldState initialState = new WorldState(WorldState.CloneBoxes(state.assignedBoxes), state);
            WorldState current;
            WorldState childState;
            List<AgentAction.Directions> directions = Enum.GetValues(typeof(AgentAction.Directions)).OfType<AgentAction.Directions>().ToList().Where(d => d != AgentAction.Directions.None).ToList();
            int hValue = 0;

            foreach (EntityLocation goal in initialState.boxGoals)
            {
                initialState.agentLocation = goal.Location;
                initialState.G = 0;

                HashSet<WorldState> explored = new HashSet<WorldState>();
                Queue<WorldState> Q = new Queue<WorldState>();
                Q.Enqueue(initialState);

                while (Q.Count > 0)
                {
                    current = Q.Dequeue();
                    explored.Add(current);
                    if (current.assignedBoxes.TryGetValue(current.agentLocation, out Box box) && box.Name.Equals(goal.Entity.Name))
                    {
                        hValue += current.G;
                        break;
                    }

                    foreach (AgentAction.Directions agentDirection in directions)
                    {
                        childState = new WorldState(WorldState.CloneBoxes(current.assignedBoxes), current);
                        if (SimulateMove(childState, agentDirection) && !explored.Contains(childState))
                        {
                            childState.Parent = current;
                            childState.G = childState.Parent.G + 1;
                            Q.Enqueue(childState);
                        }
                    }
                }
            }
            return hValue;
        }
    }
}
