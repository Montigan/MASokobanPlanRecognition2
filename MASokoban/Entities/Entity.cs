using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASokoban.Entities
{
    abstract class Entity
    {
        public char Name { get; set; }
        public string Colour { get; set; }
    }
}
