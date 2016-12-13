using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    public enum TickType
    {
        Bid,
        Ask,
        Trade
    }

    [DataContract]
    public class Tick : DataObject
    {
        public byte ProviderId
        {
            get
            {
                return this.providerId;
            }
            set
            {
                this.providerId = value;
            }
        }
        public int InstrumentId
        {
            get
            {
                return this.instrumentId;
            }
            set
            {
                this.instrumentId = value;
            }
        }
        public double Price
        {
            get
            {
                return this.price;
            }
            set
            {
                this.price = value;
            }
        }
        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }
        public DateTime ExchangeDateTime
        {
            get
            {
                return this.exchangeDateTime;
            }
            set
            {
                this.exchangeDateTime = value;
            }
        }

        public Tick()
        {
        }

        public Tick(Tick tick)
            : this(tick.DateTime, tick.ExchangeDateTime, tick.ProviderId, tick.InstrumentId, tick.Price, tick.Size)
        {
        }

        public Tick(DateTime dateTime, byte providerId, int instrumentId, double price, int size)
            : this(dateTime, default(DateTime), providerId, instrumentId, price, size)
        {
        }

        public Tick(DateTime dateTime, DateTime exchangeDateTime, byte providerId, int instrumentId, double price, int size)
            : base(dateTime)
        {
            ExchangeDateTime = exchangeDateTime;
            ProviderId = providerId;
            InstrumentId = instrumentId;
            Price = price;
            Size = size;
        }

        public override DataObject Clone()
        {
            return new Tick(this);
        }

        public override string ToString() => $"{nameof(Tick)} {DateTime} {ProviderId} {InstrumentId} {Price} {Size}";

        [DataMember]
        protected internal DateTime exchangeDateTime;

        [DataMember]
        protected internal byte providerId;

        [DataMember]
        protected internal int instrumentId;

        [DataMember]
        protected internal double price;

        [DataMember]
        protected internal int size;
    }
}