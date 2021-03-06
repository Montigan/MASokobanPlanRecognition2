using System;
using System.Diagnostics.CodeAnalysis;
using MASokoban.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Environment
{
    class Location : IEquatable<Location>, ICloneable
    {
        public int x { get; private set; } //horizontal
        public int y { get; private set; } //vertical

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Location()
        {

        }

        public Location(Location location)
        {
            this.x = location.x;
            this.y = location.y;
        }

        public int ManhattanDistanceTo(Location other)
        {
            return Math.Abs(this.x - other.x) + Math.Abs(this.y - other.y);
        }


        public static Location RelativeLocation(Location location, AgentAction.Directions direction)
        {
            if (direction.Equals(AgentAction.Directions.North))
            {
                return new Location(location.x, location.y - 1);
            }
            else if (direction.Equals(AgentAction.Directions.South))
            {
                return new Location(location.x, location.y + 1);
            }
            else if (direction.Equals(AgentAction.Directions.West))
            {
                return new Location(location.x - 1, location.y);
            }
            else if (direction.Equals(AgentAction.Directions.East))
            {
                return new Location(location.x + 1, location.y);
            }
            else
            {
                return new Location(location.x, location.y);
            }
        }

        #region Hash and comparison tools
        public override int GetHashCode()
        {
            return HashCode.Combine(this.x, this.y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Location);
        }

        public bool Equals([AllowNull] Location obj)
        {
            return obj != null && obj.x == this.x && obj.y == this.y;
        }

        public object Clone()
        {
            return new Location(this.x, this.y);
        }
        #endregion
    }
}
