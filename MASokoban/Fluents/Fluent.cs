using MASokoban.Entities;

namespace MASokoban.Fluents
{
    abstract class Fluent
    {
        public string fluentName;
        public int timeStep;
        public EntityLocation entityLocation;
        public bool isNegated;

        public abstract void Update(EntityLocation entityLocation, int timestep, bool isNegated);

        public abstract string Print();
    }
}
