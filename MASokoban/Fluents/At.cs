using MASokoban.Entities;

namespace MASokoban.Fluents
{
    class At : Fluent
    {
        public At(EntityLocation entityLocation, int timestep, bool isNegated)
        {
            fluentName = "At";
            Update(entityLocation, timestep, isNegated);
        }

        public override void Update(EntityLocation entityLocation, int timestep, bool isNegated)
        {
            this.entityLocation = entityLocation;
            this.timeStep = timestep;
            this.isNegated = isNegated;
        }

        public override string Print()
        {
            return $"{fluentName}(({entityLocation.Entity.Name},{entityLocation.Entity.Colour}),({entityLocation.Location.x},{entityLocation.Location.y}),{timeStep}):{isNegated}";
        }
    }
}
