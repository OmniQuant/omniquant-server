// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace FastQuant.Providers
{
    public class InstrumentRecord
    {
        public InstrumentRecord(Instrument instrument)
        {
            Instrument = instrument;
        }

        public Instrument Instrument
        {
            get;
            private set;
        }
    }
    public class OrderRecord : InstrumentRecord
    {
        public OrderRecord(Order order) : base(order.Instrument)
        {
            Order = order;
        }

        public Order Order { get; private set; }

    }

    public class PriceComparer : IComparer<double>
    {
        public PriceComparer(PriceSortOrder priceSortOrder)
        {
            this.priceSortOrder = priceSortOrder;
        }

        public int Compare(double x, double y)
        {
            switch (this.priceSortOrder)
            {
                case PriceSortOrder.Ascending:
                    return x.CompareTo(y);
                case PriceSortOrder.Descending:
                    return y.CompareTo(x);
                default:
                    throw new ArgumentException($"Unknown price sort order - {this.priceSortOrder}");
            }
        }

        private PriceSortOrder priceSortOrder;
    }

    public enum PriceSortOrder
    {
        Ascending,
        Descending
    }

    public class ProviderSettingError
    {
        internal ProviderSettingError(bool isWarning, string text)
        {
            this.IsWarning = isWarning;
            this.Text = text;
        }

        public bool IsWarning { get; private set; }

        public string Text { get; private set; }
    }

    public class ProviderSettingErrorList : IEnumerable<ProviderSettingError>
    {
        public ProviderSettingErrorList()
        {

            this.list_0 = new List<ProviderSettingError>();
            HasErrors = false;
            HasWarnings = false;
        }

        public bool HasErrors { get; private set; }

        public bool HasWarnings { get; private set; }

        public void AddError(string text)
        {
            this.method_0(false, text);
        }

        public void AddError(string format, params object[] args)
        {
            AddError(string.Format(format, args));
        }

        public void AddWarning(string text)
        {
            this.method_0(true, text);
        }

        public void AddWarning(string format, params object[] args)
        {
            AddWarning(string.Format(format, args));
        }

        private void method_0(bool bool_2, string text)
        {
            this.list_0.Add(new ProviderSettingError(bool_2, text));
            if (bool_2)
                HasWarnings = true;
            else
                HasErrors = true;
        }

        public IEnumerator<ProviderSettingError> GetEnumerator() => this.list_0.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private List<ProviderSettingError> list_0;
    }
}