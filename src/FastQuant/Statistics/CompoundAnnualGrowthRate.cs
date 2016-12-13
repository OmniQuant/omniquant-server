﻿using System;
using static System.Math;
using System.Linq;

namespace FastQuant.Statistics
{
    public class CompoundAnnualGrowthRate : PortfolioStatisticsItem
    {
        protected double initial;

        protected bool isSet;

        protected internal override void OnEquity(double equity)
        {
            if (!this.isSet)
            {
                this.isSet = true;
                this.initial = equity;
            }
        }

        protected internal override void OnInit()
        {
            Subscribe(PortfolioStatisticsType.AnnualReturn);
        }

        protected internal override void OnStatistics(PortfolioStatisticsItem statistics)
        {
            if (statistics.Type == PortfolioStatisticsType.AnnualReturn)
            {
                this.totalValue = Pow(1 + Enumerable.Range(0, statistics.TotalValues.Count).Sum(i => statistics.TotalValues[i]) / this.initial, 1.0 / statistics.TotalValues.Count) - 1;
                TotalValues.Add(Clock.DateTime, this.totalValue);
                Emit();
            }
        }

        public override string Category => "Daily / Annual returns";

        public override string Format => "P2";

        public override string Name => "Compound Annual Growth Rate";

        public override int Type => PortfolioStatisticsType.CompoundAnnualGrowthRate;
    }
}
