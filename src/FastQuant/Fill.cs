// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    [DataContract]
    public class Fill : DataObject
    {
        public override byte TypeId => DataObjectType.Fill;

        public Order Order { get; internal set; }

        public Instrument Instrument { get; internal set; }

        [DataMember]
        public byte CurrencyId { get; internal set; }

        [DataMember]
        public OrderSide Side { get; internal set; }

        [DataMember]
        public double Qty { get; internal set; }

        [DataMember]
        public double Price { get; internal set; }

        [DataMember]
        public string Text { get; internal set; }

        [DataMember]
        public double Commission { get; internal set; }

        public double Value => Instrument.Factor != 0.0 ? Price * Qty * Instrument.Factor : Price * Qty;

        public double NetCashFlow => Side == OrderSide.Buy ? -Value : Value;

        public double CashFlow => NetCashFlow - Commission;

        public Fill()
        {
        }

        public Fill(DateTime dateTime, Order order, Instrument instrument, byte currencyId, OrderSide side, double qty, double price, string text = "")
        {
            DateTime = dateTime;
            Order = order;
            Instrument = instrument;
            OrderId = order.Id;
            InstrumentId = instrument.Id;
            CurrencyId = currencyId;
            Side = side;
            Qty = qty;
            Price = price;
            Text = text;
        }

        public Fill(ExecutionReport report)
        {
            DateTime = report.DateTime;
            Order = report.Order;
            Instrument = report.Instrument;
            OrderId = report.Order.Id;
            InstrumentId = report.InstrumentId;
            CurrencyId = report.CurrencyId;
            Side = report.Side;
            Qty = report.LastQty;
            Price = report.Price;
            Commission = report.Commission;
            Text = report.Text;
        }

        public Fill(Fill fill)
        {
            DateTime = fill.DateTime;
            Order = fill.Order;
            Instrument = fill.Instrument;
            OrderId = fill.OrderId;
            InstrumentId = fill.InstrumentId;
            CurrencyId = fill.CurrencyId;
            Side = fill.Side;
            Qty = fill.Qty;
            Price = fill.Price;
            Commission = fill.Commission;
            Text = fill.Text;
        }

        public string GetSideAsString()
        {
            switch (Side)
            {
                case OrderSide.Buy:
                    return "Buy";
                case OrderSide.Sell:
                    return "Sell";
                default:
                    return "Undefined";
            }
        }

        public override string ToString() => $"{DateTime} {GetSideAsString()} {Instrument?.Symbol ?? InstrumentId.ToString()} {Qty} {Price} {Text}";

        #region Extra

        [NotOriginal]
        [DataMember]
        public int OrderId { get; set; }

        [NotOriginal]
        [DataMember]
        public int InstrumentId { get; set; }

        #endregion
    }
}
