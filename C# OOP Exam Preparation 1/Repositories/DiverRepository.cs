using NauticalCatchChallenge.Models.Contracts;
using NauticalCatchChallenge.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NauticalCatchChallenge.Repositories
{
    public class DiverRepository : IRepository<IDiver>
    {
        private List<IDiver> _models = new();
        public IReadOnlyCollection<IDiver> Models => _models;

        public void AddModel(IDiver model)
        {
            _models.Add(model);
        }

        public IDiver GetModel(string name)
        {
            return Models.FirstOrDefault(m => m.Name == name);
        }
    }
}
