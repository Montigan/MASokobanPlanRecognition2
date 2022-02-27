using System;
using System.Collections.Generic;
using System.Linq;
using MASokoban.Environment;
using MASokoban.Entities;
using MASokoban.Assigner;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MASokoban.MAvis
{
    class LevelReader
    {
        private const string readError = "Error reading server communication";

        public static List<WorldState> readLevelFromFile(string path)
        {
            List<EntityLocation> agents = new List<EntityLocation>();
            List<EntityLocation> boxes = new List<EntityLocation>();
            List<EntityLocation> agentGoals = new List<EntityLocation>();
            List<EntityLocation> boxGoals = new List<EntityLocation>();
            HashSet<Location> wallLocations = new HashSet<Location>();

            Regex colourPrefix = new Regex(@"^[a-zA-Z]+:");
            List<(string, char)> boxColours = new List<(string, char)>();
            List<(string, char)> agentColours = new List<(string, char)>();

            string[] fileLines = System.IO.File.ReadAllLines(path);

            int index = 5;

            // Read all colour lines
            string colourLine = fileLines[index];
            while (colourPrefix.IsMatch(colourLine))
            {
                string colour = colourLine.Substring(0, colourLine.IndexOf(":"));
                string entityString = colourLine.Remove(0, colour.Length + 1);
                string[] entities = entityString.Split(",");

                for (int i = 0; i < entities.Count(); i++)
                {
                    entities[i] = entities[i].Replace(" ", String.Empty);
                }

                // Iterate through each char after the "<colour>:" prefix.
                foreach (string entity in entities)
                {
                    if (entity.Length > 1) throw new Exception();
                    char cEntity = Convert.ToChar(entity);
                    if (Char.IsDigit(cEntity)) //Agent
                    {
                        agentColours.Add((colour, cEntity));
                    }
                    else //Box
                    {
                        boxColours.Add((colour, cEntity));
                    }
                }
                index++;
                colourLine = fileLines[index];
            }

            // colourLine now contains the one after
            if (!colourLine.Equals("#initial")) throw new Exception(readError);

            int xCount = 0;
            int yCount = 0;
            index++;
            string levelLine = fileLines[index];
            while (!levelLine.Equals("#goal"))
            {
                xCount = 0;
                foreach (char entity in levelLine.ToCharArray())
                {
                    if (entity.Equals(' '))
                    {
                        xCount++;
                        continue; //Space
                    }
                    else if (entity.Equals('+')) //Wall
                    {
                        wallLocations.Add(new Location(xCount, yCount));
                    }
                    else if (Char.IsDigit(entity)) //Agent
                    {
                        string colour = null;
                        foreach (var agent in agentColours)
                        {
                            if (agent.Item2.Equals(entity))
                            {
                                colour = agent.Item1;
                            }
                        }
                        agents.Add(new EntityLocation(new Agent(entity, colour), new Location(xCount, yCount)));
                    }
                    else //Box
                    {
                        string colour = null;
                        foreach (var box in boxColours)
                        {
                            if (box.Item2.Equals(entity))
                            {
                                colour = box.Item1;
                            }
                        }

                        //hvis der ikke er en agent med farven colour
                        bool hasAgent = false;
                        foreach (var ac in agentColours)
                        {
                            if (ac.Item1 == colour) hasAgent = true;
                        }

                        if (hasAgent) boxes.Add(new EntityLocation(new Box(entity, colour), new Location(xCount, yCount)));
                        else wallLocations.Add(new Location(xCount, yCount));
                    }

                    xCount++;
                }
                index++;
                levelLine = fileLines[index];
                yCount++;
            }



            yCount = 0;
            index++;
            levelLine = fileLines[index];
            while (!levelLine.Equals("#end"))
            {
                xCount = 0;
                foreach (char entity in levelLine.ToCharArray())
                {
                    if (Char.IsDigit(entity)) // If agent goal
                    {
                        Agent agent = (Agent)agents.First(a => a.Entity.Name.Equals(entity)).Entity;
                        agentGoals.Add(new EntityLocation(agent, new Location(xCount, yCount)));
                    }
                    else if (boxes.Any(b => b.Entity.Name.Equals(entity))) //(!entity.Equals(' ') && !entity.Equals('+')) //Then it must be a box
                    {
                        Box box = (Box)boxes.First(b => b.Entity.Name.Equals(entity)).Entity;
                        boxGoals.Add(new EntityLocation(box, new Location(xCount, yCount)));
                    }
                    xCount++;
                }
                index++;
                levelLine = fileLines[index];
                yCount++;
            }
            //List<WorldState> initialWorldstates = SimpleAssignBoxesToAgents(agents, boxes, agentGoals, boxGoals, wallLocations);
            List<WorldState> initialWorldstates = BoxAssigner.AssignBoxesToAgents(agents, boxes, agentGoals, boxGoals, wallLocations);

            // create initial state, goal state and return 
            // Construct initial states
            // Agent's own goals
            return initialWorldstates;

        }
    }
}
