using NauticalCatchChallenge.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NauticalCatchChallenge.Models
{
    public abstract class Diver : IDiver
    {
        private string _name;
        private int _oxygen;
        private List<string> _caughtFish = new();
        private double _competitionPoints = 0;
        public Diver(string name, int oxyLevel)
        {
            Name = name;
            OxygenLevel = oxyLevel;
            CompetitionPoints = 0;
        }
        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Diver's name cannot be null or empty.");
                }
                _name = value;
            }
        }

        public int OxygenLevel
        {
            get => _oxygen;
            protected set
            {
                if (value < 0)
                {
                    _oxygen = 0;
                    return;
                }
                _oxygen = value;
            }
        }

        public IReadOnlyCollection<string> Catch => _caughtFish;

        public double CompetitionPoints
        {
            get => Math.Round(_competitionPoints, 1);
            private set => _competitionPoints = value;
        }

        public bool HasHealthIssues { get; private set; } = false;

        public void Hit(IFish fish)
        {
            this.OxygenLevel -= fish.TimeToCatch;
            _caughtFish.Add(fish.Name);
            this.CompetitionPoints += fish.Points;
        }

        public abstract void Miss(int TimeToCatch);
        //this.OxygenLevel -= (int)Math.Round(TimeToCatch ,MidpointRounding);

        public abstract void RenewOxy();

        public void UpdateHealthStatus()
        {
            HasHealthIssues = !HasHealthIssues;
        }
        public override string ToString()
        {
            return
            $"Diver [ Name: {Name}, Oxygen left: {OxygenLevel}, Fish caught: {Catch.Count}, Points earned: {CompetitionPoints} ]";
        }
    }
}
