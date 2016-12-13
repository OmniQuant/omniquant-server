﻿// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    [DataContract]
    public class Bid : Tick
    {
        public override byte TypeId => DataObjectType.Bid;

        public Bid(DateTime dateTime, byte providerId, int instrumentId, double price, int size)
            : base(dateTime, providerId, instrumentId, price, size)
        {
        }

        public Bid(DateTime dateTime, DateTime exchangeDateTime, byte providerId, int instrumentId, double price, int size)
            : base(dateTime, exchangeDateTime, providerId, instrumentId, price, size)
        {
        }

        public Bid()
        {
        }

        public Bid(Bid bid) : base(bid)
        {
        }

        public Bid(Tick tick) : base(tick)
        {
        }

        public override DataObject Clone()
        {
            return new Bid(this);
        }

        public override string ToString() => $"{nameof(Bid)} {DateTime} {ProviderId} {InstrumentId} {Price} {Size}";
    }
}
