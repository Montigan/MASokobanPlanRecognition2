using System;
using System.Collections.Generic;
using System.Linq;
using MASokoban.Environment;
using MASokoban.Entities;
using MASokoban.Actions;
using MASokoban.Heuristics;
using static MASokoban.Planning.ConstraintState;
using Priority_Queue;

using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Planning
{
    class AStar : Planner
    {
        private HashSet<WorldState> explored;
        private FastPriorityQueue<WorldState> frontier;

        public AStar(Agent agent) : base()
        {
            this.agent = agent;
            this.heuristic = new Manhattan();
        }

        public override List<WorldState> MakePlan(WorldState initialState, HashSet<Constraint> constraints)
        {
            long nodesChecked = 0;
            long amountOfStatesAddedToFrontier = 0;
            this.frontier = new FastPriorityQueue<WorldState>(searchNodeLimit);
            this.explored = new HashSet<WorldState>();

            List<Constraint> listOfConstraints = constraints.ToList().Where(c => c.agentUnderConstraint.Equals(initialState.agent)).ToList();
            initialState.H = heuristic.H(initialState);
            initialState.G = 0;
            frontier.Enqueue(initialState, initialState.H + initialState.G);
            WorldState currentNode;
            var Directions = Enum.GetValues(typeof(AgentAction.Directions));
            List<AgentAction.Directions> directions = Directions.OfType<AgentAction.Directions>().ToList().Where(d => d != AgentAction.Directions.None).ToList();

            while (frontier.Count > 0)
            {

                currentNode = frontier.Dequeue();
                nodesChecked++;
                if (currentNode.isGoal())
                {
                    Console.WriteLine("old visited " + nodesChecked + " states");
                    this.frontier.ResetNode(initialState);
                    this.frontier = null;
                    this.explored = null;
                    return constructPath(currentNode);
                }

                bool conflictNearby = false;
                if (listOfConstraints.Any(a => (a.timeStamp > currentNode.timeStep - conflictTimeRange) && (a.timeStamp < currentNode.timeStep + conflictTimeRange) &&
                     a.constraintLocation.ManhattanDistanceTo(currentNode.agentLocation) < conflictDistRange))
                {
                    conflictNearby = true;
                }

                // Expand for all possible action, direction combination.
                foreach (AgentAction.Actions action in Enum.GetValues(typeof(AgentAction.Actions)))
                {
                    if (action.Equals(AgentAction.Actions.Move))
                    {
                        foreach (AgentAction.Directions agentDirection in directions)
                        {
                            WorldState childNode = new WorldState(WorldState.CloneBoxes(currentNode.assignedBoxes), currentNode);

                            if (this.move.TryPerformAction(childNode, agentDirection) && (!explored.Contains(childNode) || conflictNearby) && childNode.Validate(constraints))
                            {
                                childNode.ActionToGetHere = (AgentAction.Actions.Move, agentDirection, AgentAction.Directions.None);
                                childNode.H = heuristic.H(childNode);
                                childNode.G = currentNode.G + this.move.Cost;
                                frontier.Enqueue(childNode, childNode.H + childNode.G);

                                //Console.WriteLine("Added node with H = " + childNode.H + " G = " + childNode.G + ". At " + amountOfStatesAddedToFrontier + " explored states.");
                            }
                        }
                    }
                    else if (action.Equals(AgentAction.Actions.Pull))
                    {
                        foreach (AgentAction.Directions agentDirection in directions)
                        {
                            foreach (AgentAction.Directions boxDirection in directions)
                            {
                                WorldState childNode = new WorldState(WorldState.CloneBoxes(currentNode.assignedBoxes), currentNode);

                                if (this.pull.TryPerformAction(childNode, agentDirection, boxDirection) && (!explored.Contains(childNode) || conflictNearby) && childNode.Validate(constraints))
                                {
                                    childNode.ActionToGetHere = (AgentAction.Actions.Pull, agentDirection, boxDirection);
                                    childNode.H = heuristic.H(childNode);
                                    childNode.G = currentNode.G + this.pull.Cost;
                                    frontier.Enqueue(childNode, childNode.H + childNode.G);

                                    //Console.WriteLine("Added node with H = " + childNode.H + " G = " + childNode.G + ". At " + amountOfStatesAddedToFrontier + " explored states.");
                                }
                            }
                        }
                    }
                    else if (action.Equals(AgentAction.Actions.Push))
                    {
                        foreach (AgentAction.Directions agentDirection in directions)
                        {
                            foreach (AgentAction.Directions boxDirection in directions)
                            {
                                WorldState childNode = new WorldState(WorldState.CloneBoxes(currentNode.assignedBoxes), currentNode);

                                if (this.push.TryPerformAction(childNode, agentDirection, boxDirection) && (!explored.Contains(childNode) || conflictNearby) && childNode.Validate(constraints))
                                {
                                    childNode.ActionToGetHere = (AgentAction.Actions.Push, agentDirection, boxDirection);
                                    childNode.H = heuristic.H(childNode);
                                    childNode.G = currentNode.G + this.push.Cost;
                                    frontier.Enqueue(childNode, childNode.H + childNode.G);

                                    //Console.WriteLine("Added node with H = " + childNode.H + " G = " + childNode.G + ". At " + amountOfStatesAddedToFrontier + " explored states.");
                                }
                            }
                        }
                    }
                    else if (action.Equals(AgentAction.Actions.NoOp))
                    {
                        WorldState childNode = new WorldState(WorldState.CloneBoxes(currentNode.assignedBoxes), currentNode);

                        if (this.noOp.TryPerformAction(childNode) && (!explored.Contains(childNode) || conflictNearby) && childNode.Validate(constraints))
                        {
                            childNode.ActionToGetHere = (AgentAction.Actions.NoOp, AgentAction.Directions.None, AgentAction.Directions.None);
                            childNode.H = heuristic.H(childNode);
                            childNode.G = currentNode.G + this.noOp.Cost;
                            frontier.Enqueue(childNode, childNode.H + childNode.G);

                            //Console.WriteLine("Added node with H = " + childNode.H + " G = " + childNode.G + ". At " + amountOfStatesAddedToFrontier + " explored states.");
                        }
                    }
                }
                explored.Add(currentNode);
                amountOfStatesAddedToFrontier++;
            }
            this.frontier.ResetNode(initialState);
            this.frontier = null;
            this.explored = null;
            return null;
        }

        private List<WorldState> constructPath(WorldState goalState)
        {
            List<WorldState> plan = new List<WorldState>();
            WorldState currentState = goalState;

            do
            {
                plan.Add(currentState);
                currentState = currentState.Parent;

            } while (currentState != null);

            plan.Reverse();
            return plan;
        }
    }
}
