using InfluencerManagerApp.Core.Contracts;
using InfluencerManagerApp.Models;
using InfluencerManagerApp.Models.Contracts;
using InfluencerManagerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InfluencerManagerApp.Core
{
    public class Controller : IController
    {
        private InfluencerRepository influencers;
        private CampaignRepository campaigns;

        public Controller()
        {
            influencers = new InfluencerRepository();
            campaigns = new CampaignRepository();
        }
        public string ApplicationReport()
        {
            StringBuilder sb = new StringBuilder();
            List<IInfluencer> sortedInfluencers = influencers.Models
                .OrderByDescending(x=>x.Income)
                .ThenByDescending(x=>x.Followers)
                .ToList();
            foreach (var influencer in sortedInfluencers)
            {
                sb.AppendLine(influencer.ToString());
                List<string> sortedCampaignNames = influencer.Participations.OrderBy(x => x).ToList();
                if (influencer.Participations.Count == 0)
                {
                    continue;
                }
                else
                {
                    sb.AppendLine("Active Campaigns:");
                    foreach (var campaignName in influencer.Participations)
                    {
                        ICampaign campaign = campaigns.FindByName(campaignName);
                        sb.AppendLine($"--{campaign.ToString()}");
                    }
                }
            }
            return sb.ToString().TrimEnd();
        }

        public string AttractInfluencer(string brand, string username)
        {
            if (!influencers.Models.Any(x => x.Username == username))
            {
                return $"{influencers.GetType().Name} has no {username} registered in the application.";
            }
            if (!campaigns.Models.Any(x => x.Brand == brand))
            {
                return $"There is no campaign from {brand} in the application.";
            }
            IInfluencer influencer = influencers.FindByName(username);
            ICampaign campaign = campaigns.FindByName(brand);
            if (influencer.Participations.Any(x => x == brand))
            {
                return $"{username} is already engaged for the {brand} campaign.";
            }
            if (campaign.GetType().Name == "ProductCampaign"
                && (influencer.GetType().Name != nameof(BusinessInfluencer) && influencer.GetType().Name != nameof(FashionInfluencer)))
            {
                return $"{username} is not eligible for the {brand} campaign.";
            }
            if (campaign.GetType().Name == "ServiceCampaign"
                && (influencer.GetType().Name != nameof(BusinessInfluencer) && influencer.GetType().Name != nameof(BloggerInfluencer)))
            {
                return $"{username} is not eligible for the {brand} campaign.";
            }
            if (campaign.Budget - influencer.CalculateCampaignPrice() < 0)
            {
                return $"The budget for {brand} is insufficient to engage {username}.";
            }
            influencer.EarnFee(influencer.CalculateCampaignPrice());
            influencer.EnrollCampaign(campaign.Brand);
            campaign.Engage(influencer);
            return $"{username} has been successfully attracted to the {brand} campaign.";

        }

        public string BeginCampaign(string typeName, string brand)
        {
            if (typeName != "ProductCampaign" && typeName != "ServiceCampaign")
            {
                return $"{typeName} is not a valid campaign in the application.";
            }
            if (campaigns.Models.Any(x => x.Brand == brand))
            {
                return $"{brand} campaign cannot be duplicated.";
            }
            ICampaign campaign = null;
            if (typeName == "ProductCampaign")
            {
                campaign = new ProductCampaign(brand);
            }
            else if (typeName == "ServiceCampaign")
            {
                campaign = new ServiceCampaign(brand);
            }
            campaigns.AddModel(campaign);
            return $"{brand} started a {typeName}.";
        }

        public string CloseCampaign(string brand)
        {
            if (!campaigns.Models.Any(x => x.Brand == brand))
            {
                return "Trying to close an invalid campaign.";
            }
            ICampaign campaign = campaigns.FindByName(brand);
            if (campaign.Budget <= 10_000)
            {
                return $"{brand} campaign cannot be closed as it has not met its financial targets.";
            }
            foreach (var workerName in campaign.Contributors)
            {
                IInfluencer influencer = influencers.FindByName(workerName);
                influencer.EarnFee(2_000);
                influencer.EndParticipation(campaign.Brand);
            }
            campaigns.RemoveModel(campaign);
            return $"{brand} campaign has reached its target.";
        }

        public string ConcludeAppContract(string username)
        {
            if (!influencers.Models.Any(x=>x.Username == username))
            {
                return $"{username} has still not signed a contract.";
            }
            IInfluencer influencer = influencers.FindByName(username);
            if (influencer.Participations.Count != 0)
            {
                return $"{username} cannot conclude the contract while enrolled in active campaigns.";
            }
            influencers.RemoveModel(influencer);
            return $"{username} concluded their contract.";
        }

        public string FundCampaign(string brand, double amount)
        {
            if (!campaigns.Models.Any(x => x.Brand == brand))
            {
                return "Trying to fund an invalid campaign.";
            }
            if (amount <= 0)
            {
                return $"Funding amount must be greater than zero.";
            }
            ICampaign campaign = campaigns.FindByName(brand);
            campaign.Gain(amount);
            return $"{brand} campaign has been successfully funded with {amount} $";
        }

        public string RegisterInfluencer(string typeName, string username, int followers)
        {
            if (typeName != "BusinessInfluencer"
                && typeName != "FashionInfluencer"
                && typeName != "BloggerInfluencer")
            {
                return $"{typeName} has not passed validation.";
            }
            if (influencers.Models.Any(x => x.Username == username))
            {
                return $"{username} is already registered in {influencers.GetType().Name}.";
            }
            IInfluencer influencer = null;
            if (typeName == "BusinessInfluencer")
            {
                influencer = new BusinessInfluencer(username, followers);
            }
            else if (typeName == "FashionInfluencer")
            {
                influencer = new FashionInfluencer(username, followers);
            }
            else if (typeName == "BloggerInfluencer")
            {
                influencer = new BloggerInfluencer(username, followers);
            }
            influencers.AddModel(influencer);
            return $"{username} registered successfully to the application.";
        }
    }
}
