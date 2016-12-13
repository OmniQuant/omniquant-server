// Licensed under the Apache License, Version 2.0. 
// Copyright (c) Alex Lee. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    [DataContract]
    public class AltId
    {
        [DataMember]
        public byte ProviderId { get; set; }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public string Exchange { get; set; }

        [DataMember]
        public byte CurrencyId { get; set; }

        public AltId()
        {
            throw new NotSupportedException("Don't use this!");
        }

        public AltId(byte providerId, string symbol, string exchange) : this(providerId, symbol, exchange, 0)
        {
        }

        public AltId(byte providerId, string symbol, string exchange, byte currencyId)
        {
            ProviderId = providerId;
            Symbol = symbol;
            Exchange = exchange;
            CurrencyId = currencyId;
        }

        public override string ToString() => $"[{ProviderId}] {Symbol}@{Exchange} {CurrencyId}";
    }
}
