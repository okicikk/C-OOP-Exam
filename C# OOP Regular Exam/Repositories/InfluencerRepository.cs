using InfluencerManagerApp.Models;
using InfluencerManagerApp.Models.Contracts;
using InfluencerManagerApp.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfluencerManagerApp.Repositories
{
    public class InfluencerRepository : IRepository<IInfluencer>
    {
        private List<IInfluencer> influencers;
        public InfluencerRepository()
        {
            influencers = new List<IInfluencer>();
        }
        public IReadOnlyCollection<IInfluencer> Models { get => influencers; private set => influencers = value.ToList(); }

        public void AddModel(IInfluencer model)
        {
            influencers.Add(model);
        }

        public IInfluencer FindByName(string name)
        {
            return influencers.FirstOrDefault(x=>x.Username == name);
        }

        public bool RemoveModel(IInfluencer model)
        {
            return influencers.Remove(model);
        }
    }
}
