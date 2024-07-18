using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NauticalCatchChallenge.Models.Contracts;
using NauticalCatchChallenge.Utilities.Messages;

namespace NauticalCatchChallenge.Models
{
    public abstract class Fish : IFish
    {
        private string _name;
        private double _points;

        public Fish(string name, double points, int timeToCatch)
        {
            Name = name;
            Points = points;
            TimeToCatch = timeToCatch;
        }

        public string Name
        {
            get { return _name; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"Fish name should be determined");
                }
                _name = value;
            }
        }


        public double Points
        {
            get => _points;
            private set
            {
                if (value is < 1 or > 10)
                {
                    throw new ArgumentException("Points must be a numeric value, between 1 and 10");
                }
                _points = value;
            }
        }

        public int TimeToCatch { get; private set; }
        public override string ToString()
        {
            return $"{this.GetType().Name}: {Name} [ Points: {Points}, Time to Catch: {TimeToCatch} seconds ]";
        }
    }
}
