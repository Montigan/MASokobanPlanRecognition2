using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MASokoban.Environment;

namespace MASokoban.Heuristics
{
    abstract class Heuristic
    {
        public abstract int H(WorldState state);
    }
}
