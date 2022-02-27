using System;
using System.Collections.Generic;
using MASokoban.Entities;
using MASokoban.Environment;
using MASokoban.Heuristics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Assigner
{
    class BoxAssigner
    {
        public static List<WorldState> AssignBoxesToAgents(List<EntityLocation> agents, List<EntityLocation> boxes, List<EntityLocation> agentGoals, List<EntityLocation> boxGoals, HashSet<Location> walls)
        {
            List<WorldState> initialStates = new List<WorldState>();
            List<Assignment> boxAssignments = new List<Assignment>();
            List<Assignment> goalAssignments = new List<Assignment>();
            bool isAgentGoal = false;

            List<WorldState> completeInitStates = new List<WorldState>();
            foreach (EntityLocation agentEntity in agents)
            {
                WorldState completeInitState = new WorldState();
                completeInitState.agent = (Agent)agentEntity.Entity;
                completeInitState.agentLocation = agentEntity.Location;
                completeInitState.Walls = walls;
                completeInitState.assignedBoxes = new Dictionary<Location, Box>();
                completeInitState.boxGoals = new List<EntityLocation>();
                completeInitState.boxGoals.Add(new EntityLocation(completeInitState.agent, completeInitState.agentLocation));
                completeInitStates.Add(completeInitState);
            }

            List<(WorldState, List<(EntityLocation, int[,], float priority)>)> completeInitStatesWithH = BFS.PrecalcH(completeInitStates);

            while (boxGoals.Count > 0)
            {
                foreach (EntityLocation agentEntity in agents)
                {
                    double shortestDistance = double.PositiveInfinity;
                    EntityLocation bestBoxGoalLocation = null;
                    EntityLocation bestInitialBoxGoalLocation = null;

                    Agent agent = (Agent)agentEntity.Entity;
                    EntityLocation agentGoal = null;

                    if (agentGoals.Count > 0)
                    {
                        isAgentGoal = agentGoals.Any(a => a.Entity.Name.Equals(agentEntity.Entity.Name));
                        if (isAgentGoal) agentGoal = agentGoals.First(a => a.Entity.Name.Equals(agentEntity.Entity.Name));
                    }

                    foreach (EntityLocation boxGoalLocation in boxGoals.Where(b => b.Entity.Colour.Equals(agent.Colour)))
                    {
                        // Assign a goal and a box
                        foreach (EntityLocation boxInitialLocation in boxes.Where(b => b.Entity.Name.Equals(boxGoalLocation.Entity.Name)))
                        {
                            double total_dist = 0.0;
                            total_dist += completeInitStatesWithH.First(c => c.Item1.agent.Name.Equals(agentEntity.Entity.Name)).Item2.First()
                                .Item2[boxInitialLocation.Location.x, boxInitialLocation.Location.y];               //agentEntity.ManhattanDistance(boxInitialLocation);
                            total_dist += completeInitStatesWithH.First(c => c.Item1.agent.Name.Equals(agentEntity.Entity.Name)).Item2.First()
                                .Item2[boxGoalLocation.Location.x, boxGoalLocation.Location.y];                             //boxInitialLocation.ManhattanDistance(boxGoalLocation);
                            if (isAgentGoal) total_dist += completeInitStatesWithH.First(c => c.Item1.agent.Name.Equals(agentEntity.Entity.Name)).Item2.First()
                                .Item2[agentGoal.Location.x, agentGoal.Location.y];  //boxGoalLocation.ManhattanDistance(agentGoal);

                            if (total_dist < shortestDistance)
                            {
                                shortestDistance = total_dist;
                                bestBoxGoalLocation = boxGoalLocation;
                                bestInitialBoxGoalLocation = boxInitialLocation;
                            }
                        }
                    }

                    if (bestBoxGoalLocation != null && bestInitialBoxGoalLocation != null && shortestDistance < int.MaxValue)
                    {
                        boxAssignments.Add(new Assignment(agent, bestInitialBoxGoalLocation));
                        goalAssignments.Add(new Assignment(agent, bestBoxGoalLocation));
                        EntityLocation boxToRemove = null;
                        EntityLocation boxGoalToRemove = null;


                        foreach (EntityLocation b in boxes)
                        {
                            if (b.Location.Equals(bestInitialBoxGoalLocation.Location))
                            {
                                boxToRemove = b;
                            }
                        }

                        foreach (EntityLocation b in boxGoals)
                        {
                            if (b.Location.Equals(bestBoxGoalLocation.Location))
                            {
                                boxGoalToRemove = b;
                            }
                        }

                        boxes.Remove(boxToRemove);
                        boxGoals.Remove(boxGoalToRemove);
                    }
                }
            }

            foreach (EntityLocation b in boxes)
            {
                int minBoxCount = int.MaxValue;
                Agent minAgent = null;

                foreach (EntityLocation agent in agents.Where(ass => ass.Entity.Colour.Equals(b.Entity.Colour)))
                {
                    int thisAgentCount = 0;
                    foreach (Assignment a in boxAssignments.Where(ass => ass.Agent.Colour.Equals(b.Entity.Colour)))
                    {
                        if (a.Agent.Equals(agent.Entity))
                        {
                            thisAgentCount++;
                        }
                    }
                    if (thisAgentCount < minBoxCount)
                    {
                        minBoxCount = thisAgentCount;
                        minAgent = (Agent)agent.Entity;
                    }
                }

                if (minAgent == null) throw new Exception("");
                boxAssignments.Add(new Assignment(minAgent, b));
            }

            foreach (EntityLocation agentEntity in agents)
            {

                Location agentLocation = agentEntity.Location;
                Location agentGoal;

                List<EntityLocation> test = agentGoals.Where(a => a.Entity.Name.Equals(agentEntity.Entity.Name)).ToList();
                if (test.Count > 0)
                {
                    agentGoal = test.ElementAt(0).Location;
                }
                else agentGoal = null;

                Dictionary<Location, Box> agentBoxes = new Dictionary<Location, Box>();
                List<EntityLocation> boxGoalsforThisAgent = new List<EntityLocation>();

                foreach (Assignment boxAssignment in boxAssignments)
                {
                    if (boxAssignment.Agent.Name.Equals(agentEntity.Entity.Name))
                    {
                        agentBoxes.Add(boxAssignment.EntityLocation.Location, (Box)boxAssignment.EntityLocation.Entity);
                    }
                }

                foreach (Assignment goalAssignment in goalAssignments)
                {
                    if (goalAssignment.Agent.Name.Equals(agentEntity.Entity.Name))
                    {
                        boxGoalsforThisAgent.Add(goalAssignment.EntityLocation);
                    }
                }

                initialStates.Add(new WorldState(agentLocation, agentGoal, agentBoxes, boxGoalsforThisAgent, walls, (Agent)agentEntity.Entity));
            }
            return initialStates;
        }

    }
}
