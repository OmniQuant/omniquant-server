﻿// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace FastQuant
{
    public class TickStreamer : ObjectStreamer
    {
        public TickStreamer()
        {
            this.typeId = DataObjectType.Tick;
            this.type = typeof(Tick);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            if (version == 0)
                return new Tick(new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
            else
                return new Tick(new DateTime(reader.ReadInt64()), new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var tick = (Tick)obj;
            byte version = tick.ExchangeDateTime.Ticks == 0 ? (byte)0 : (byte)1;
            writer.Write(version);
            writer.Write(tick.DateTime.Ticks);
            if (version == 1)
                writer.Write(tick.ExchangeDateTime.Ticks);
            writer.Write(tick.ProviderId);
            writer.Write(tick.InstrumentId);
            writer.Write(tick.Price);
            writer.Write(tick.Size);
        }
    }

    public class BidStreamer : ObjectStreamer
    {
        public BidStreamer()
        {
            this.typeId = DataObjectType.Bid;
            this.type = typeof(Bid);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            if (version == 0)
                return new Bid(new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
            else
                return new Bid(new DateTime(reader.ReadInt64()), new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var bid = obj as Bid;
            writer.Write(bid.DateTime.Ticks);

            if (bid.ExchangeDateTime.Ticks != 0)
                writer.Write(bid.ExchangeDateTime.Ticks);
            writer.Write(bid.ProviderId);
            writer.Write(bid.InstrumentId);
            writer.Write(bid.Price);
            writer.Write(bid.Size);
        }
    }

    public class AskStreamer : ObjectStreamer
    {
        public AskStreamer()
        {
            this.typeId = DataObjectType.Ask;
            this.type = typeof(Ask);
        }

        public override byte GetVersion(object obj)
        {
            return (obj as Tick).ExchangeDateTime.Ticks != 0 ? (byte)1 : (byte)0;
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return version == 0
                ? new Ask(new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32())
                : new Ask(new DateTime(reader.ReadInt64()), new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());

        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var ask = obj as Ask;
            writer.Write(ask.DateTime.Ticks);
            if (ask.ExchangeDateTime.Ticks != 0)
                writer.Write(ask.ExchangeDateTime.Ticks);
            writer.Write(ask.ProviderId);
            writer.Write(ask.InstrumentId);
            writer.Write(ask.Price);
            writer.Write(ask.Size);
        }
    }

    public class BarStreamer : ObjectStreamer
    {
        public BarStreamer()
        {
            this.typeId = DataObjectType.Bar;
            this.type = typeof(Bar);
            this.version = 1;
        }

        public override object Read(BinaryReader reader, byte version)
        {
            var bar = new Bar();
            bar.DateTime = new DateTime(reader.ReadInt64());
            bar.OpenDateTime = new DateTime(reader.ReadInt64());
            bar.InstrumentId = reader.ReadInt32();
            bar.Size = reader.ReadInt64();
            bar.High = reader.ReadDouble();
            bar.Low = reader.ReadDouble();
            bar.Open = reader.ReadDouble();
            bar.Close = reader.ReadDouble();
            bar.Volume = reader.ReadInt64();
            bar.OpenInt = reader.ReadInt64();
            bar.Status = (BarStatus)reader.ReadByte();
            if (version >= 1)
                bar.Type = (BarType)reader.ReadByte();
            if (version >= 2)
            {
                bar.ProviderId = reader.ReadInt32();
            }
            if (version <= 2)
            {
                int num = reader.ReadInt32();
                if (num != 0)
                {
                    //bar.Fields = new ObjectTable();
                    for (var i = 0; i < num; i++)
                    {
                        bar.Fields[i] = reader.ReadDouble();
                    }
                }
            }
            if (version >= 3 && reader.ReadBoolean())
            {
                var fields = (ObjectTable)this.streamerManager.Deserialize(reader);
                for (int i = 0; i < fields.Size; i++)
                {
                    bar.Fields[i] = fields[i];
                }
                //    bar.Fields = (ObjectTable)this.streamerManager.Deserialize(reader);

            }
            return bar;
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var bar = (Bar)obj;
            writer.Write(bar.DateTime.Ticks);
            writer.Write(bar.OpenDateTime.Ticks);
            writer.Write(bar.InstrumentId);
            writer.Write(bar.Size);
            writer.Write(bar.High);
            writer.Write(bar.Low);
            writer.Write(bar.Open);
            writer.Write(bar.Close);
            writer.Write(bar.Volume);
            writer.Write(bar.OpenInt);
            writer.Write((byte)bar.Status);
            if (this.version >= 1)
            {
                writer.Write((byte)bar.Type);
            }
            if (this.version >= 2)
            {
                writer.Write(bar.ProviderId);
            }
            if (this.version <= 2)
            {
                if (bar.Fields != null)
                {
                    writer.Write(bar.Fields.Size);
                    for (var i = 0; i < bar.Fields.Size; ++i)
                        writer.Write((double)bar.Fields[i]);
                }
                else
                    writer.Write(0);
            }
            if (this.version >= 3)
            {
                if (bar.Fields != null)
                {
                    writer.Write(true);
                    this.streamerManager.Serialize(writer, bar.Fields);
                    return;
                }
                writer.Write(false);
            }
        }
    }

    public class TradeStreamer : ObjectStreamer
    {
        public TradeStreamer()
        {
            this.typeId = DataObjectType.Trade;
            this.type = typeof(Trade);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            if (version == 0)
                return new Trade(new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
            else
                return new Trade(new DateTime(reader.ReadInt64()), new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var trade = obj as Trade;
            byte version = 0;
            if (trade.ExchangeDateTime.Ticks != 0)
                version = 1;
            writer.Write(version);
            writer.Write(trade.DateTime.Ticks);
            if (version == 1)
                writer.Write(trade.ExchangeDateTime.Ticks);
            writer.Write(trade.ProviderId);
            writer.Write(trade.InstrumentId);
            writer.Write(trade.Price);
            writer.Write(trade.Size);
        }
    }

    public class Level2Streamer : ObjectStreamer
    {
        public Level2Streamer()
        {
            this.typeId = DataObjectType.Level2;
            this.type = typeof(Level2);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return new Level2
            {
                DateTime = new DateTime(reader.ReadInt64()),
                ProviderId = reader.ReadByte(),
                InstrumentId = reader.ReadInt32(),
                Price = reader.ReadDouble(),
                Size = reader.ReadInt32(),
                Side = (Level2Side)reader.ReadByte(),
                Action = (Level2UpdateAction)reader.ReadByte(),
                Position = reader.ReadInt32()
            };
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var level = (Level2)obj;
            writer.Write(level.DateTime.Ticks);
            writer.Write(level.ProviderId);
            writer.Write(level.InstrumentId);
            writer.Write(level.Price);
            writer.Write(level.Size);
            writer.Write((byte)level.Side);
            writer.Write((byte)level.Action);
            writer.Write(level.Position);
        }
    }

    public class FillStreamer : ObjectStreamer
    {
        public FillStreamer()
        {
            this.typeId = DataObjectType.Fill;
            this.type = typeof(Fill);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return new Fill
            {
                DateTime = new DateTime(reader.ReadInt64()),
                OrderId = reader.ReadInt32(),
                InstrumentId = reader.ReadInt32(),
                CurrencyId = reader.ReadByte(),
                Side = (OrderSide)reader.ReadByte(),
                Qty = reader.ReadDouble(),
                Price = reader.ReadDouble(),
                Text = reader.ReadString()
            };
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            Fill fill = (Fill)obj;
            writer.Write(fill.DateTime.Ticks);
            writer.Write(fill.OrderId);
            writer.Write(fill.InstrumentId);
            writer.Write(fill.CurrencyId);
            writer.Write((byte)fill.Side);
            writer.Write(fill.Qty);
            writer.Write(fill.Price);
            writer.Write(fill.Text);
        }
    }

    public class QuoteStreamer : ObjectStreamer
    {
        public QuoteStreamer()
        {
            this.typeId = DataObjectType.Quote;
            this.type = typeof(Quote);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return new Quote(new DateTime(reader.ReadInt64()), reader.ReadByte(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32(), reader.ReadDouble(), reader.ReadInt32());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var quote = (Quote)obj;
            writer.Write(quote.DateTime.Ticks);
            writer.Write(quote.Bid.ProviderId);
            writer.Write(quote.Bid.InstrumentId);
            writer.Write(quote.Bid.Price);
            writer.Write(quote.Bid.Size);
            writer.Write(quote.Ask.Price);
            writer.Write(quote.Ask.Size);
        }
    }

    public class Level2SnapshotStreamer : ObjectStreamer
    {
        public Level2SnapshotStreamer()
        {
            this.typeId = DataObjectType.Level2Snapshot;
            this.type = typeof(Level2Snapshot);
        }

        public override byte GetVersion(object obj) => ((Level2Snapshot)obj).ExchangeDateTime.Ticks != 0 ? (byte)1 : (byte)0;

        public override object Read(BinaryReader reader, byte version)
        {
            var l2s = new Level2Snapshot
            {
                DateTime = new DateTime(reader.ReadInt64()),
                ProviderId = reader.ReadByte(),
                InstrumentId = reader.ReadInt32()
            };
            int count;
            count = reader.ReadInt32();
            l2s.Bids = new Bid[count];
            for (var i = 0; i < count; i++)
                l2s.Bids[i] = (Bid)this.streamerManager.Deserialize(reader);
            count = reader.ReadInt32();
            l2s.Asks = new Ask[count];
            for (var i = 0; i < count; i++)
                l2s.Asks[i] = (Ask)this.streamerManager.Deserialize(reader);
            return l2s;
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var l2s = (Level2Snapshot)obj;
            writer.Write(l2s.DateTime.Ticks);
            writer.Write(l2s.ProviderId);
            writer.Write(l2s.InstrumentId);
            if (GetVersion(l2s) == 1)
                writer.Write(l2s.ExchangeDateTime.Ticks);
            writer.Write(l2s.Bids.Length);
            foreach (var bid in l2s.Bids)
                this.streamerManager.Serialize(writer, bid);
            writer.Write(l2s.Asks.Length);
            foreach (Ask ask in l2s.Asks)
                this.streamerManager.Serialize(writer, ask);
        }
    }

    public class Level2UpdateStreamer : ObjectStreamer
    {
        public Level2UpdateStreamer()
        {
            this.typeId = DataObjectType.Level2Update;
            this.type = typeof(Level2Update);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            var l2u = new Level2Update();
            l2u.DateTime = new DateTime(reader.ReadInt64());
            l2u.ProviderId = reader.ReadByte();
            l2u.InstrumentId = reader.ReadInt32();
            int count = reader.ReadInt32();
            l2u.Entries = new Level2[count];
            for (int i = 0; i < count; i++)
                l2u.Entries[i] = (Level2)this.streamerManager.Deserialize(reader);
            return l2u;
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var l2u = (Level2Update)obj;
            writer.Write(l2u.DateTime.Ticks);
            writer.Write(l2u.ProviderId);
            writer.Write(l2u.InstrumentId);
            writer.Write(l2u.Entries.Length);
            for (int i = 0; i < l2u.Entries.Length; i++)
                this.streamerManager.Serialize(writer, l2u.Entries[i]);
        }
    }
}
