using System;
using System.Collections.Generic;
using System.Linq;
using MASokoban.Entities;
using MASokoban.Environment;
using MASokoban.Planning;
using Priority_Queue;

namespace MASokoban.ConflictBasedSolver
{
    class CBS
    {
        private FastPriorityQueue<ConstraintState> frontier;
        //private List<Agent> agents;
        private List<(WorldState, List<(EntityLocation, int[,], float priority)>)> initialStatesWithHeuristics;

        public CBS(List<(WorldState, List<(EntityLocation, int[,], float priority)>)> initialStatesWithHeuristics)
        {
            const int searchNodeLimit = 100000;
            this.frontier = new FastPriorityQueue<ConstraintState>(searchNodeLimit);
            //this.agents = agents;
            this.initialStatesWithHeuristics = initialStatesWithHeuristics;
        }

        public List<List<WorldState>> findSolution()
        {
            ConstraintState initialNode = new ConstraintState();
            foreach ((WorldState, List<(EntityLocation, int[,], float priority)>) initialStatesWithHeuristic in initialStatesWithHeuristics)
            {
                initialNode.solution.Add(CBSAStar.MakePlan(initialStatesWithHeuristic.Item1.agent, initialStatesWithHeuristic.Item1, initialNode.constraints, initialStatesWithHeuristic.Item2));


                //initialNode.solution.Add(initialState.agent.SearchClient.MakePlan(initialState, initialNode.constraints));
            }

            initialNode.totalCost = calcSolutionCost(initialNode.solution);

            this.frontier.Enqueue(initialNode, initialNode.totalCost);

            while (frontier.Count > 0)
            {
                ConstraintState currentNode = frontier.Dequeue();
                (List<Agent>, int, Location) conflict = currentNode.Validate();
                if (conflict.Item1.Count == 0)
                {
                    return currentNode.solution;
                }

                foreach (Agent agent in conflict.Item1)
                {
                    ConstraintState childNode = new ConstraintState(currentNode, new ConstraintState.Constraint(conflict.Item2, conflict.Item3, agent));

                    int counter = 0;
                    int agentIndex = -1;
                    foreach (List<WorldState> childPlan in childNode.solution)
                    {
                        if (childPlan.First().agent.Equals(agent))
                        {
                            agentIndex = counter;
                        }
                        counter++;
                    }


                    (WorldState, List<(EntityLocation, int[,], float priority)>) relevantinitialStatesWithHeuristic = initialStatesWithHeuristics.First(m => m.Item1.agent.Name.Equals(agent.Name));
                    //initialNode.solution.Add(Astar.MakePlan(hMatrix.First().Item4.agent, hMatrix.First().Item4, initialNode.constraints, hMatrix));
                    childNode.solution[agentIndex] = CBSAStar.MakePlan(relevantinitialStatesWithHeuristic.Item1.agent, relevantinitialStatesWithHeuristic.Item1,
                        currentNode.constraints, relevantinitialStatesWithHeuristic.Item2);


                    //childNode.solution[agentIndex] = agent.SearchClient.MakePlan(this.initialStates.First(a => a.agent.Equals(agent)), childNode.constraints);
                    if (childNode.solution[agentIndex].Count > 0)
                    {
                        childNode.totalCost = calcSolutionCost(childNode.solution);
                        frontier.Enqueue(childNode, childNode.totalCost);
                    }
                }
            }
            return null; //Unsolvable
        }

        private int calcSolutionCost(List<List<WorldState>> solution)
        {
            int longestPlanLength = 0;
            foreach (List<WorldState> plan in solution)
            {
                if (plan.Count > longestPlanLength) longestPlanLength = plan.Count;
            }
            return longestPlanLength;
        }
    }
}
