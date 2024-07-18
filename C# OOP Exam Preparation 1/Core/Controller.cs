using NauticalCatchChallenge.Core.Contracts;
using NauticalCatchChallenge.Models;
using NauticalCatchChallenge.Models.Contracts;
using NauticalCatchChallenge.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NauticalCatchChallenge.Core
{
    public class Controller : IController
    {
        private DiverRepository divers = new DiverRepository();
        private FishRepository fishes = new();
        public Controller()
        {

        }

        public string ChaseFish(string diverName, string fishName, bool isLucky)
        {
            if (divers.GetModel(diverName) == null)
            {
                return $"{divers.GetType().Name} has no {diverName} registered for the competition.";
            }
            IDiver diver = divers.GetModel(diverName);
            if (fishes.GetModel(fishName) == null)
            {
                return $"{fishName} is not allowed to be caught in this competition.";
            }
            IFish fish = fishes.GetModel(fishName);
            if (diver.HasHealthIssues == true)
            {
                return $"{diverName} will not be allowed to dive, due to health issues.";
            }
            if (diver.OxygenLevel < fish.TimeToCatch)
            {
                diver.Miss(fish.TimeToCatch);
                if (diver.OxygenLevel is 0)
                {
                    diver.UpdateHealthStatus();
                }
                return $"{diverName} missed a good {fishName}.";
            }
            else if (diver.OxygenLevel == fish.TimeToCatch)
            {
                if (isLucky)
                {
                    diver.Hit(fish);
                    fishes.AddModel(fish);
                    if (diver.OxygenLevel is 0)
                    {
                        diver.UpdateHealthStatus();
                    }
                    return $"{diverName} hits a {fish.Points}pt. {fishName}.";
                }
                else if (!isLucky)
                {
                    diver.Miss(fish.TimeToCatch);
                    return $"{diverName} missed a good {fishName}.";
                }
            }
            else if (diver.OxygenLevel > fish.TimeToCatch)
            {
                diver.Hit(fish);
                fishes.AddModel(fish);
                if (diver.OxygenLevel is 0)
                {
                    diver.UpdateHealthStatus();
                }
            }
            return $"{diverName} hits a {fish.Points}pt. {fishName}.";

        }

        public string CompetitionStatistics()
        {
            StringBuilder sb = new StringBuilder();
            List<IDiver> healthyDivers = divers.Models.Where(m => !m.HasHealthIssues)
                .OrderByDescending(m => m.CompetitionPoints)
                .ThenByDescending(m => m.Catch.Count)
                .ThenBy(m => m.Name)
                .ToList();
            sb.AppendLine("**Nautical-Catch-Challenge**");
            foreach (var diver in healthyDivers)
            {
                sb.AppendLine(diver.ToString());
            }
            return sb.ToString().TrimEnd();
        }

        public string DiveIntoCompetition(string diverType, string diverName)
        {
            IDiver diver;
            if (diverType == "FreeDiver")
            {
                diver = new FreeDiver(diverName);
            }
            else if (diverType == "ScubaDiver")
            {
                diver = new ScubaDiver(diverName);
            }
            else
            {
                return $"{diverType} is not allowed in our competition.";
            }
            if (divers.GetModel(diverName) == null)
            {
                divers.AddModel(diver);
            }
            else
            {
                return $"{diverName} is already a participant -> {divers.GetType().Name}.";
            }
            return $"{diverName} is successfully registered for the competition -> {divers.GetType().Name}.";
        }

        public string DiverCatchReport(string diverName)
        {
            StringBuilder sb = new StringBuilder();
            IDiver diver = divers.GetModel(diverName);
            sb.AppendLine($"Diver [ Name: {diver.Name}, Oxygen left: {diver.OxygenLevel}, Fish caught: {diver.Catch.Count}, Points earned: {diver.CompetitionPoints} ]");
            sb.AppendLine($"Catch Report:");
            foreach (string fish in diver.Catch)
            {
                sb.AppendLine(fishes.GetModel(fish).ToString());
            }
            return sb.ToString().TrimEnd();
        }

        public string HealthRecovery()
        {
            int unhealthyDivers = divers.Models.Where(m => m.HasHealthIssues).Count();
            foreach (var diver in divers.Models.Where(m=>m.HasHealthIssues))
            {
                diver.UpdateHealthStatus();
                diver.RenewOxy();
            }
            return $"Divers recovered: {unhealthyDivers}";
        }

        public string SwimIntoCompetition(string fishType, string fishName, double points)
        {
            IFish fish;
            if (fishType is not "ReefFish" and not "DeepSeaFish" and not "PredatoryFish")
            {
                return $"{fishType} is forbidden for chasing in our competition.";
            }
            if (fishes.GetModel(fishName) != null)
            {
                return $"{fishName} is already allowed -> {fishes.GetType().Name}.";
            }
            if (fishType == "ReefFish")
            {
                fish = new ReefFish(fishName, points);
                fishes.AddModel(fish);
            }
            else if (fishType == "DeepSeaFish")
            {
                fish = new DeepSeaFish(fishName, points);
                fishes.AddModel(fish);
            }
            else if (fishType == "PredatoryFish")
            {
                fish = new PredatoryFish(fishName, points);
                fishes.AddModel(fish);
            }
            return $"{fishName} is allowed for chasing.";
        }
    }
}
