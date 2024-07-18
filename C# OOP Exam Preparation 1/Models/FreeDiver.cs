﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NauticalCatchChallenge.Models
{
    public class FreeDiver : Diver
    {
        private static int oxyValue = 120;

        public FreeDiver(string name) : base(name, oxyValue)
        {

        }

        public override void Miss(int TimeToCatch)
        {
            OxygenLevel -= (int)Math.Round(TimeToCatch * 0.60, MidpointRounding.AwayFromZero);
            if (OxygenLevel < 0)
            {
                OxygenLevel = 0;
            }   
        }

        public override void RenewOxy()
        {
            OxygenLevel = oxyValue;
        }
    }
}