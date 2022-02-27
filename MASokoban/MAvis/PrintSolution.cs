using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASokoban.Actions;
using MASokoban.Environment;
using System.Threading.Tasks;

namespace MASokoban.MAvis
{
    class PrintSolution
    {
        public static List<string> printSolutionToFile(List<List<WorldState>> solution)
        {
            StringBuilder currentLine = new StringBuilder("");
            List<string> lines = new List<string>();

            int longestPlanLength = 0;
            foreach (List<WorldState> plan in solution)
            {
                if (plan.Count > longestPlanLength) longestPlanLength = plan.Count;
            }

            for (int j = 1; j < longestPlanLength; j++)
            {
                currentLine = new StringBuilder("");
                for (int i = 0; i < 10; i++)
                {
                    List<WorldState> plan = solution.FirstOrDefault(p => char.GetNumericValue(p.First().agent.Name) == i);

                    if (plan == null)
                    {

                    }
                    else if (plan.Count <= j)
                    {
                        currentLine.Append("NoOp;");
                    }
                    else
                    {
                        string action = AgentAction.actionToString(plan[j].ActionToGetHere.Item1);
                        string agentDir = AgentAction.directionToString(plan[j].ActionToGetHere.Item2);

                        if (action.Equals("NoOp"))
                        {
                            currentLine.Append(action + ";");
                        }
                        else
                        {
                            currentLine.Append(action + "(");
                            currentLine.Append(agentDir);
                            if (!action.Equals("Move"))
                            {
                                string boxDir = AgentAction.directionToString(plan[j].ActionToGetHere.Item3);
                                currentLine.Append("," + boxDir);
                            }
                            currentLine.Append(");");
                        }
                    }
                }
                currentLine.Remove(currentLine.Length - 1, 1);
                lines.Add(currentLine.ToString());
            }
            return lines;
        }
    }
}
