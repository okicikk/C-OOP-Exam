using HighwayToPeak.Models.Contracts;
using HighwayToPeak.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Repositories
{
    public class ClimberRepository : IRepository<IClimber>
    {
        private List<IClimber> _climbers = new();
        public IReadOnlyCollection<IClimber> All => _climbers;

        public void Add(IClimber model)
        {
            _climbers.Add(model);
        }

        public IClimber Get(string name)
        {
            return _climbers.FirstOrDefault(c => c.Name == name);
        }
    }
}
