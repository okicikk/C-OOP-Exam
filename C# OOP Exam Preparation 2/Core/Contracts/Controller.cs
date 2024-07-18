using HighwayToPeak.Models;
using HighwayToPeak.Models.Contracts;
using HighwayToPeak.Repositories;
using HighwayToPeak.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Core.Contracts
{
    public class Controller : IController
    {
        private PeakRepository _peakRepository = new PeakRepository();
        private ClimberRepository _climberRepository = new ClimberRepository();
        private BaseCamp _baseCamp = new BaseCamp();
        public string AddPeak(string name, int elevation, string difficultyLevel)
        {
            if (_peakRepository.Get(name) != null)
            {
                return $"{name} is already added as a valid mountain destination.";
            }
            if (difficultyLevel != "Extreme" && difficultyLevel != "Hard" && difficultyLevel != "Moderate")
            {
                return $"{difficultyLevel} peaks are not allowed for international climbers.";
            }
            IPeak peak = new Peak(name, elevation, difficultyLevel);
            _peakRepository.Add(peak);
            return $"{name} is allowed for international climbing. See details in {_peakRepository.GetType().Name}.";
        }

        public string AttackPeak(string climberName, string peakName)
        {
            if (_climberRepository.Get(climberName) is null)
            {
                return $"Climber - {climberName}, has not arrived at the BaseCamp yet.";
            }
            if (_peakRepository.Get(peakName) is null)
            {
                return $"{peakName} is not allowed for international climbing.";
            }
            if (_baseCamp.Residents.Contains(climberName) == false)
            {
                return $"{climberName} not found for gearing and instructions. The attack of {peakName} will be postponed.";
            }
            IPeak peak = _peakRepository.Get(peakName);
            IClimber climber = _climberRepository.Get(climberName);
            if (peak.DifficultyLevel is "Extreme" && climber.GetType().Name is "NaturalClimber")
            {
                return $"{climberName} does not cover the requirements for climbing {peakName}.";
            }

            _baseCamp.LeaveCamp(climberName);

            climber.Climb(peak);
            if (climber.Stamina <= 0)
            {
                return $"{climberName} did not return to BaseCamp.";
            }
            _baseCamp.ArriveAtCamp(climberName);
            return $"{climberName} successfully conquered {peakName} and returned to BaseCamp.";

        }

        public string BaseCampReport()
        {
            StringBuilder sb = new StringBuilder();
            if (_baseCamp.Residents.Count == 0)
            {
                return "BaseCamp is currently empty.";
            }
            sb.AppendLine("BaseCamp residents:");
            foreach (string climberName in _baseCamp.Residents)
            {
                IClimber climber = _climberRepository.Get(climberName);
                sb.AppendLine($"Name: {climber.Name}, Stamina: {climber.Stamina}, Count of Conquered Peaks: {climber.ConqueredPeaks.Count}");
            }


            return sb.ToString().TrimEnd();
        }

        public string CampRecovery(string climberName, int daysToRecover)
        {
            if (!_baseCamp.Residents.Contains(climberName))
            {
                return $"{climberName} not found at the BaseCamp.";
            }
            IClimber climber = _climberRepository.Get(climberName);
            if (climber.Stamina == 10)
            {
                return $"{climberName} has no need of recovery.";
            }
            climber.Rest(daysToRecover);
            return $"{climberName} has been recovering for {daysToRecover} days and is ready to attack the mountain.";
        }

        public string NewClimberAtCamp(string name, bool isOxygenUsed)
        {
            IClimber climber;
            if (_climberRepository.Get(name) != null)
            {
                return $"{name} is a participant in {_climberRepository.GetType().Name} and cannot be duplicated.";
            }
            else
            {
                if (isOxygenUsed is true)
                {
                    climber = new OxygenClimber(name);
                }
                else
                {
                    climber = new NaturalClimber(name);
                }
                _climberRepository.Add(climber);
                _baseCamp.ArriveAtCamp(name);
                return $"{name} has arrived at the BaseCamp and will wait for the best conditions.";
            }
        }

        public string OverallStatistics()
        {
            StringBuilder sb = new StringBuilder();
            List<IClimber> climbers = _climberRepository.All.ToList();
            climbers = climbers.OrderByDescending(x => x.ConqueredPeaks.Count)
                .ThenBy(x => x.Name).ToList();

            sb.AppendLine("***Highway-To-Peak***");
            foreach (IClimber climber in climbers)
            {
                List<IPeak> peaks = new List<IPeak>();
                sb.AppendLine($"{climber.ToString()}");
                foreach (string peakName in climber.ConqueredPeaks)
                {
                    peaks.Add(_peakRepository.Get(peakName));
                }
                peaks = peaks.OrderByDescending(x => x.Elevation).ToList();
                foreach (var peak in peaks)
                {
                    sb.AppendLine(peak.ToString());
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
