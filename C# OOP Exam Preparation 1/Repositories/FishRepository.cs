using NauticalCatchChallenge.Models.Contracts;
using NauticalCatchChallenge.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NauticalCatchChallenge.Repositories
{
    public class FishRepository : IRepository<IFish>
    {
        private List<IFish> _models = new();
        public IReadOnlyCollection<IFish> Models => _models;

        public void AddModel(IFish model)
        {
            _models.Add(model);
        }

        public IFish GetModel(string name)
        {
            return Models.FirstOrDefault(m => m.Name == name);
        }
    }
}
