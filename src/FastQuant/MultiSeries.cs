using System;
using System.Collections.Generic;

namespace FastQuant
{
    public class MultiSeries : ISeries
    {
        public string Name { get; }
        public string Description { get; }
        public int Count { get; }
        public List<Indicator> Indicators { get; }
        public double First { get; }
        public double Last { get; }
        public DateTime FirstDateTime { get; }
        public DateTime LastDateTime { get; }

        double ISeries.this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        double ISeries.this[int index, BarData barData]
        {
            get { throw new NotImplementedException(); }
        }

        public int GetIndex(DateTime dateTime, IndexOption option = IndexOption.Null)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int index)
        {
            throw new NotImplementedException();
        }

        public double GetMin(DateTime dateTime1, DateTime dateTime2)
        {
            throw new NotImplementedException();
        }

        public double GetMin(int index1, int index2, BarData barData)
        {
            throw new NotImplementedException();
        }

        public double GetMax(DateTime dateTime1, DateTime dateTime2)
        {
            throw new NotImplementedException();
        }

        public double GetMax(int index1, int index2, BarData barData)
        {
            throw new NotImplementedException();
        }
    }
}