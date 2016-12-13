// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    [DataContract]
    public class Ask : Tick
    {
        public override byte TypeId => DataObjectType.Ask;

        public Ask(DateTime dateTime, byte providerId, int instrument, double price, int size)
            : base(dateTime, providerId, instrument, price, size)
        {
        }

        public Ask(DateTime dateTime, DateTime exchangeDateTime, byte providerId, int instrument, double price, int size)
            : base(dateTime, exchangeDateTime, providerId, instrument, price, size)
        {
        }

        public Ask()
        {
        }

        public Ask(Ask ask) : base(ask)
        {
        }

        public Ask(Tick tick) : base(tick)
        {
        }

        public override DataObject Clone()
        {
            return new Ask(this);
        }

        public override string ToString() => $"{nameof(Ask)} {DateTime} {ProviderId} {InstrumentId} {Price} {Size}";
    }
}
