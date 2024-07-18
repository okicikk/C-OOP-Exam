﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfluencerManagerApp.Models
{
    public class BloggerInfluencer : Influencer
    {
        public BloggerInfluencer(string userName, int followers) : base(userName, followers, 2.0)
        {
        }

        public override int CalculateCampaignPrice()
        {
            int result = (int)Math.Floor(Followers * EngagementRate * 0.2);
            return result;
        }
    }
}
