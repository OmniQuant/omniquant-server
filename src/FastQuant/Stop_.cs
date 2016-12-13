using System;

namespace FastQuant.Experimental.Strategy
{
    public class Stop
    {
        public Strategy Strategy => this.strategy;

        public Position Position => this.position;

        public Instrument Instrument => this.instrument;

        public bool Connected => this.connected;

        public StopType Type => this.type;

        public StopMode Mode => this.mode;

        public StopStatus Status => this.status;

        public double Level => this.level;

        public double Qty => this.qty;

        public PositionSide Side => this.side;

        public DateTime CreationTime => this.creationTime;

        public DateTime CompletionTime => this.completionTime;

        public bool TraceOnQuote
        {
            get
            {
                return this.traceOnQuote;
            }
            set
            {
                this.traceOnQuote = value;
            }
        }

        public bool TraceOnTrade
        {
            get
            {
                return this.traceOnTrade;
            }
            set
            {
                this.traceOnTrade = value;
            }
        }

        public bool TraceOnBar
        {
            get
            {
                return this.traceOnBar;
            }
            set
            {
                this.traceOnBar = value;
            }
        }

        public bool TraceOnBarOpen
        {
            get
            {
                return this.traceOnBarOpen;
            }
            set
            {
                this.traceOnBarOpen = value;
            }
        }

        public bool TrailOnOpen
        {
            get
            {
                return this.trailOnOpen;
            }
            set
            {
                this.trailOnOpen = value;
            }
        }

        public bool TrailOnHighLow
        {
            get
            {
                return this.trailOnHighLow;
            }
            set
            {
                this.trailOnHighLow = value;
            }
        }

        public long FilterBarSize
        {
            get
            {
                return this.filterBarSize;
            }
            set
            {
                this.filterBarSize = value;
            }
        }

        public BarType FilterBarType
        {
            get
            {
                return this.filterBarType;
            }
            set
            {
                this.filterBarType = value;
            }
        }

        public StopFillMode FillMode
        {
            get
            {
                return this.fillMode;
            }
            set
            {
                this.fillMode = value;
            }
        }

        public ObjectTable Fields
        {
            get
            {
                if (this.fields == null)
                {
                    this.fields = new ObjectTable();
                }
                return this.fields;
            }
        }

        public Stop(Strategy strategy, Position position, double level, StopType type, StopMode mode)
        {
            this.type = StopType.Trailing;
            this.mode = StopMode.Percent;
            this.traceOnQuote = true;
            this.traceOnTrade = true;
            this.traceOnBar = true;
            this.traceOnBarOpen = true;
            this.trailOnOpen = true;
            this.filterBarSize = -1L;
            this.filterBarType = BarType.Time;
            this.fillMode = StopFillMode.Stop;
            this.strategy = strategy;
            this.position = position;
            this.instrument = position.Instrument;
            this.qty = position.Qty;
            this.side = position.Side;
            this.level = level;
            this.type = type;
            this.mode = mode;
            this.currPrice = GetInstrumentPrice();
            this.trailPrice = this.currPrice;
            this.stopPrice = GetStopPrice();
            this.creationTime = strategy.Framework.Clock.DateTime;
            this.completionTime = DateTime.MinValue;
            this.method_0();
        }

        public Stop(Strategy strategy, Position position, DateTime time)
        {
            this.type = StopType.Trailing;
            this.mode = StopMode.Percent;
            this.traceOnQuote = true;
            this.traceOnTrade = true;
            this.traceOnBar = true;
            this.traceOnBarOpen = true;
            this.trailOnOpen = true;
            this.filterBarSize = -1L;
            this.filterBarType = BarType.Time;
            this.fillMode = StopFillMode.Stop;
            this.strategy = strategy;
            this.position = position;
            this.instrument = position.Instrument;
            this.qty = position.Qty;
            this.side = position.Side;
            this.type = StopType.Time;
            this.creationTime = strategy.Framework.Clock.DateTime;
            this.completionTime = time;
            this.stopPrice = GetInstrumentPrice();
            if (this.completionTime > this.creationTime)
            {
                strategy.Framework.Clock.AddReminder(new Reminder(this.method_9, this.completionTime, null));
            }
        }

        protected virtual double GetPrice(double price)
        {
            return price;
        }

        protected virtual double GetInstrumentPrice()
        {
            if (this.position.Side == PositionSide.Long)
            {
                Bid bid = this.strategy.Framework.DataManager.GetBid(this.instrument);
                if (bid != null)
                {
                    return this.GetPrice(bid.Price);
                }
            }
            if (this.position.Side == PositionSide.Short)
            {
                Ask ask = this.strategy.Framework.DataManager.GetAsk(this.instrument);
                if (ask != null)
                {
                    return this.GetPrice(ask.Price);
                }
            }
            Trade trade = this.strategy.Framework.DataManager.GetTrade(this.instrument);
            if (trade != null)
            {
                return this.GetPrice(trade.Price);
            }
            Bar bar = this.strategy.Framework.DataManager.GetBar(this.instrument);
            if (bar != null)
            {
                return this.GetPrice(bar.Close);
            }
            return 0.0;
        }

        protected virtual double GetStopPrice()
        {
            this.initPrice = this.trailPrice;
            StopMode stopMode = this.mode;
            if (stopMode != StopMode.Absolute)
            {
                if (stopMode != StopMode.Percent)
                {
                    throw new ArgumentException("Unknown stop mode : " + this.mode);
                }
                PositionSide positionSide = this.position.Side;
                if (positionSide == PositionSide.Long)
                {
                    return this.trailPrice - Math.Abs(this.trailPrice * this.level);
                }
                if (positionSide != PositionSide.Short)
                {
                    throw new ArgumentException("Unknown position side : " + this.position.Side);
                }
                return this.trailPrice + Math.Abs(this.trailPrice * this.level);
            }
            else
            {
                PositionSide positionSide = this.side;
                if (positionSide == PositionSide.Long)
                {
                    return this.trailPrice - Math.Abs(this.level);
                }
                if (positionSide != PositionSide.Short)
                {
                    throw new ArgumentException("Unknown position side : " + this.position.Side);
                }
                return this.trailPrice + Math.Abs(this.level);
            }
        }

        public void Cancel()
        {
            if (this.status != StopStatus.Active)
            {
                return;
            }
            this.Disconnect();
            this.method_8(StopStatus.Canceled);
        }

        private void method_0()
        {
            this.connected = true;
        }

        public void Disconnect()
        {
            if (this.type == StopType.Time)
            {
                this.strategy.Framework.Clock.RemoveReminder(new ReminderCallback(this.method_9), this.completionTime);
                return;
            }
            this.connected = false;
        }

        private void method_1()
        {
            if (this.currPrice == 0.0)
            {
                return;
            }
            PositionSide positionSide = this.side;
            if (positionSide != PositionSide.Long)
            {
                if (positionSide != PositionSide.Short)
                {
                    return;
                }
                if (this.currPrice >= this.stopPrice)
                {
                    this.Disconnect();
                    this.method_8(StopStatus.Executed);
                    return;
                }
                if (this.type == StopType.Trailing && this.trailPrice < this.initPrice)
                {
                    this.stopPrice = this.GetStopPrice();
                }
            }
            else
            {
                if (this.currPrice <= this.stopPrice)
                {
                    this.Disconnect();
                    this.method_8(StopStatus.Executed);
                    return;
                }
                if (this.type == StopType.Trailing && this.trailPrice > this.initPrice)
                {
                    this.stopPrice = this.GetStopPrice();
                    return;
                }
            }
        }

        internal void method_2(Position position_0)
        {
            if (this.position == position_0)
            {
                this.Disconnect();
                this.method_8(StopStatus.Canceled);
            }
        }

        internal void method_3(Bar bar_0)
        {
            if (this.traceOnBar && (this.filterBarSize < 0L || (this.filterBarSize == bar_0.Size && this.filterBarType == BarType.Time)))
            {
                this.trailPrice = this.GetPrice(bar_0.Close);
                PositionSide positionSide = this.side;
                if (positionSide != PositionSide.Long)
                {
                    if (positionSide == PositionSide.Short)
                    {
                        this.currPrice = this.GetPrice(bar_0.High);
                        this.fillPrice = this.currPrice;
                        if (this.trailOnHighLow)
                        {
                            this.trailPrice = this.GetPrice(bar_0.Low);
                        }
                    }
                }
                else
                {
                    this.currPrice = this.GetPrice(bar_0.Low);
                    this.fillPrice = this.currPrice;
                    if (this.trailOnHighLow)
                    {
                        this.trailPrice = this.GetPrice(bar_0.High);
                    }
                }
                StopFillMode stopFillMode = this.fillMode;
                if (stopFillMode != StopFillMode.Close)
                {
                    if (stopFillMode == StopFillMode.Stop)
                    {
                        this.fillPrice = this.stopPrice;
                    }
                }
                else
                {
                    this.fillPrice = this.GetPrice(bar_0.Close);
                }
                this.method_1();
            }
        }

        internal void method_4(Bar bar_0)
        {
            if (this.traceOnBar && this.traceOnBarOpen && (this.filterBarSize < 0L || (this.filterBarSize == bar_0.Size && this.filterBarType == BarType.Time)))
            {
                this.currPrice = this.GetPrice(bar_0.Open);
                this.fillPrice = this.currPrice;
                if (this.trailOnOpen)
                {
                    this.trailPrice = this.GetPrice(bar_0.Open);
                }
                this.method_1();
            }
        }

        internal void method_5(Trade trade_0)
        {
            if (this.traceOnTrade)
            {
                this.currPrice = this.GetPrice(trade_0.price);
                this.fillPrice = this.currPrice;
                this.trailPrice = this.currPrice;
                this.method_1();
            }
        }

        internal void method_6(Bid bid_0)
        {
            if (this.traceOnQuote && this.side == PositionSide.Long)
            {
                this.currPrice = this.GetPrice(bid_0.price);
                this.fillPrice = this.currPrice;
                this.trailPrice = this.currPrice;
                this.method_1();
            }
        }

        internal void method_7(Ask ask_0)
        {
            if (this.traceOnQuote && this.side == PositionSide.Short)
            {
                this.currPrice = this.GetPrice(ask_0.price);
                this.fillPrice = this.currPrice;
                this.trailPrice = this.currPrice;
                this.method_1();
            }
        }

        private void method_8(StopStatus stopStatus_0)
        {
            this.status = stopStatus_0;
            this.completionTime = this.strategy.Framework.Clock.DateTime;
            this.strategy.OnStopStatusChanged_(this);
        }

        private void method_9(DateTime dateTime_0, object object_0)
        {
            this.stopPrice = this.GetInstrumentPrice();
            this.method_8(StopStatus.Executed);
        }

        protected internal Strategy strategy;

        protected internal Position position;

        protected internal Instrument instrument;

        protected internal bool connected;

        protected internal StopType type;

        protected internal StopMode mode;

        protected internal StopStatus status;

        protected internal double level;

        protected internal double initPrice;

        protected internal double currPrice;

        protected internal double stopPrice;

        protected internal double fillPrice;

        protected internal double trailPrice;

        protected internal double qty;

        protected internal PositionSide side;

        protected internal DateTime creationTime;

        protected internal DateTime completionTime;

        protected internal bool traceOnQuote;

        protected internal bool traceOnTrade;

        protected internal bool traceOnBar;

        protected internal bool traceOnBarOpen;

        protected internal bool trailOnOpen;

        protected internal bool trailOnHighLow;

        protected internal long filterBarSize;

        protected internal BarType filterBarType;

        protected internal StopFillMode fillMode;

        protected internal ObjectTable fields;
    }
}
