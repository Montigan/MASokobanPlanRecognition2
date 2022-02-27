using System;
using System.Collections.Generic;
using MASokoban.Environment;
using MASokoban.Planning;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Entities
{
    class Agent : Entity, IEquatable<Agent>
    {
        public Planner SearchClient;
        public List<WorldState> plan { get; set; }

        public Agent(char name, string colour)
        {
            this.Name = name;
            this.Colour = colour;
            this.SearchClient = new AStar(this);
        }

        #region Hash and comparison tools
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name, this.Colour);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Agent);
        }

        public bool Equals([AllowNull] Agent agent)
        {
            return agent != null && agent.Name == this.Name && agent.Colour.Equals(this.Colour);
        }
        #endregion
    }
}
