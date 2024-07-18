using HighwayToPeak.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Models
{
    public abstract class Climber : IClimber
    {
        private string name;
        private int stamina;
        private List<string> conqueredPeaks = new();

        protected Climber(string name, int stamina)
        {
            Name = name;
            Stamina = stamina;
        }

        public string Name
        {
            get => name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Climber's name cannot be null or whitespace.");
                }
                name = value;
            }
        }

        public int Stamina
        {
            get => stamina;
            protected set
            {
                if (value > 10)
                {
                    value = 10;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                stamina = value;
            }
        }

        public IReadOnlyCollection<string> ConqueredPeaks
        {
            get => conqueredPeaks;
            private set => conqueredPeaks = value.ToList();
        }

        public void Climb(IPeak peak)
        {
            if (!ConqueredPeaks.Contains(peak.Name))
            {
                conqueredPeaks.Add(peak.Name);
            }
            if (peak.DifficultyLevel is "Extreme")
            {
                Stamina -= 6;
            }
            else if (peak.DifficultyLevel is "Hard")
            {
                Stamina -= 4;
            }
            else if (peak.DifficultyLevel is "Moderate")
            {
                Stamina -= 2;
            }
        }

        public abstract void Rest(int daysCount);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetType().Name} - Name: {Name}, Stamina: {Stamina}");
            if (ConqueredPeaks.Count == 0)
            {
                sb.AppendLine($"Peaks conquered: no peaks conquered");
            }
            else
            {
                sb.AppendLine($"Peaks conquered: {ConqueredPeaks.Count}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
