// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    public enum Level2Side : byte
    {
        Bid,
        Ask
    }

    public enum Level2UpdateAction
    {
        New,
        Change,
        Delete,
        Reset
    }

    [DataContract]
    public class Level2 : Tick
    {
        public override byte TypeId => DataObjectType.Level2;

        [DataMember]
        public Level2Side Side { get; set; }

        [DataMember]
        public Level2UpdateAction Action { get; set; }

        [DataMember]
        public int Position { get; set; }

        public Level2()
        {
        }

        public Level2(DateTime dateTime, byte providerId, int instrumentId, double price, int size, Level2Side side, Level2UpdateAction action, int position) : base(dateTime, providerId, instrumentId, price, size)
        {
            Side = side;
            Action = action;
            Position = position;
        }

        public Level2(Bid bid, Level2UpdateAction action, int position): base(bid)
        {
            Side = Level2Side.Bid;
            Action = action;
            Position = position;
        }

        public Level2(Ask ask, Level2UpdateAction action, int position): base(ask)
        {
            Side = Level2Side.Ask;
            Action = action;
            Position = position;
        }

        public override string ToString() => $"Level2 {DateTime} {ProviderId} {Side} {Position} {InstrumentId} {Price} {Size} {Action}";
    }

    [DataContract]
    public class Level2Update : DataObject
    {
        public override byte TypeId => DataObjectType.Level2Update;

        [DataMember]
        public byte ProviderId { get; set; }

        [DataMember]
        public int InstrumentId { get; set; }

        [DataMember]
        public Level2[] Entries { get; internal set; }

        public Level2Update()
        {
        }

        public Level2Update(DateTime dateTime, byte providerId, int instrumentId, Level2[] entries)
            : base(dateTime)
        {
            ProviderId = providerId;
            InstrumentId = instrumentId;
            Entries = entries;
        }

    }

    [DataContract]
    public class Level2Snapshot : DataObject
    {
        public override byte TypeId => DataObjectType.Level2Snapshot;

        [DataMember]
        public byte ProviderId { get; set; }

        [DataMember]
        public int InstrumentId { get; set; }

        [DataMember]
        public Bid[] Bids { get; internal set; }

        [DataMember]
        public Ask[] Asks { get; internal set; }

        [DataMember]
        public DateTime ExchangeDateTime { get; set; }

        public Level2Snapshot(DateTime dateTime, byte providerId, int instrumentId, Bid[] bids, Ask[] asks)
            : base(dateTime)
        {
            ProviderId = providerId;
            InstrumentId = instrumentId;
            Bids = bids;
            Asks = asks;
        }

        public Level2Snapshot(DateTime dateTime, DateTime exchangeDateTime, byte providerId, int instrumentId, Bid[] bids, Ask[] asks) : this(dateTime, providerId, instrumentId, bids, asks)
        {
            ExchangeDateTime = exchangeDateTime;
        }

        public Level2Snapshot()
        {
        }

        public Level2Snapshot(Level2Snapshot snapshot)
            : this(snapshot.dateTime, snapshot.ProviderId, snapshot.InstrumentId, snapshot.Bids, snapshot.Asks)
        {
            ExchangeDateTime = snapshot.ExchangeDateTime;
        }
    }
}
