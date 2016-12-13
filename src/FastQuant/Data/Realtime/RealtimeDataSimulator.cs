using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using FastQuant.Providers;

namespace FastQuant.Data.Realtime
{
    public class RealtimeDataSimulator : Provider, IDataProvider
    {
        public RealtimeDataSimulator(Framework framework) : base(framework)
        {
            this.id = ProviderId.RealtimeDataSimulator;
            this.name = "RealtimeDataSimulator";
            this.description = "Random Realtime Data Simulator";
            this.url = "http://www.smartquant.com";
            InitSettings();
            InitMarketDataProvider();
        }

        private void InitSettings()
        {
            Delay = 1000;
            RandomizeInstruments = false;
            DecimalDigits = 2;
            PriceRange = new Range<double>(10.0, 20.0);
            SpreadRange = new Range<double>(0.1, 1.0);
            SizeRange = new Range<int>(1, 100);
            this.doWork = false;
        }

        private void InitMarketDataProvider()
        {
            this.mdRecords = new Dictionary<Instrument, MarketDataRecord>();
        }

        [DefaultValue(1000), Category("Settings")]
        public int Delay { get; set; }

        [Category("Settings"), DefaultValue(false)]
        public bool RandomizeInstruments { get; set; }

        [Category("Settings"), DefaultValue(2)]
        public int DecimalDigits { get; set; }

        [Category("Settings")]
        public Range<double> PriceRange { get; private set; }

        [Browsable(false)]
        public string PriceRangeStr
        {
            get
            {
                return PriceRange.ToSingleString();
            }
            set
            {
                PriceRange.FromSingleString(value);
            }
        }

        [Category("Settings")]
        public Range<double> SpreadRange { get; private set; }

        [Browsable(false)]
        public string SpreadRangeStr
        {
            get
            {
                return SpreadRange.ToSingleString();
            }
            set
            {
                SpreadRange.FromSingleString(value);
            }
        }

        [Category("Settings")]
        public Range<int> SizeRange { get; private set; }

        [Browsable(false)]
        public string SizeRangeStr
        {
            get
            {
                return SizeRange.ToSingleString();
            }
            set
            {
                SizeRange.FromSingleString(value);
            }
        }

        [Category("Data"), Editor("SmartQuant.Shared.RandomDataEditor, SmartQuant.Shared, Culture=neutral, PublicKeyToken=null", typeof(UITypeEditor))]
        public object Data => null;

        public MarketDataRecord[] GetMDRecords()
        {
            lock (this.mdRecords)
                return this.mdRecords.Values.Select(r => new MarketDataRecord(r.Instrument) {Trades = r.Trades, Quotes = r.Quotes}).ToArray();
        }

        protected override void OnConnect()
        {
            if (Status != ProviderStatus.Disconnected) return;
            Status = ProviderStatus.Connecting;
            this.doWork = true;
            var resetEvent = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(this.method_5, resetEvent);
            resetEvent.WaitOne();
            Status = ProviderStatus.Connected;
        }

        protected override void OnDisconnect()
        {
            if (Status != ProviderStatus.Connected) return;
            Status = ProviderStatus.Disconnecting;
            lock (this.mdRecords)
                this.mdRecords.Clear();
            this.doWork = false;
            Status = ProviderStatus.Disconnected;
        }

        public override void Subscribe(Instrument instrument)
        {
            SendMarketDataRequest(instrument, true, true, false);
            SendMarketDataRequest(instrument, true, false, true);
        }

        public override void Unsubscribe(Instrument instrument)
        {
            SendMarketDataRequest(instrument, false, true, false);
            SendMarketDataRequest(instrument, false, false, true);
        }

        private void SendMarketDataRequest(Instrument instrument, bool subscribe, bool trades, bool quotes)
        {
            Dictionary<Instrument, MarketDataRecord> obj;
            if (subscribe)
            {
                lock (this.mdRecords)
                {
                    MarketDataRecord marketDataRecord;
                    if (!this.mdRecords.TryGetValue(instrument, out marketDataRecord))
                    {
                        marketDataRecord = new MarketDataRecord(instrument);
                        this.mdRecords.Add(instrument, marketDataRecord);
                    }
                    if (trades)
                    {
                        marketDataRecord.Trades = true;
                    }
                    if (quotes)
                    {
                        marketDataRecord.Quotes = true;
                    }
                    return;
                }
            }
            lock (this.mdRecords)
            {
                MarketDataRecord marketDataRecord2;
                if (this.mdRecords.TryGetValue(instrument, out marketDataRecord2))
                {
                    if (trades)
                    {
                        marketDataRecord2.Trades = false;
                    }
                    if (quotes)
                    {
                        marketDataRecord2.Quotes = false;
                    }
                    if (!marketDataRecord2.Trades && !marketDataRecord2.Quotes)
                    {
                        this.mdRecords.Remove(instrument);
                    }
                }
            }
        }

        private void method_5(object state)
        {
            ((ManualResetEvent)state).Set();
            while (this.doWork)
            {
                var list = new List<MarketDataRecord>();
                lock (this.mdRecords)
                    list.AddRange(this.mdRecords.Values.Select(r => new MarketDataRecord(r.Instrument) {Trades = r.Trades, Quotes = r.Quotes}));
                var random = new Random();
                double min = PriceRange.Min;
                double max = PriceRange.Max;
                double min2 = SpreadRange.Min;
                double max2 = SpreadRange.Max;
                int min3 = SizeRange.Min;
                int max3 = SizeRange.Max;
                while (list.Count > 0)
                {
                    int index = RandomizeInstruments ? random.Next(0, list.Count - 1) : 0;
                    MarketDataRecord marketDataRecord = list[index];
                    list.RemoveAt(index);
                    if (marketDataRecord.Trades)
                    {
                        double price = Math.Round(random.NextDouble() * (max - min) + min, DecimalDigits);
                        var trade = new Trade(this.framework.Clock.DateTime, base.Id, marketDataRecord.Instrument.Id, price, random.Next(min3, max3));
                        EmitData(trade, true);
                    }
                    if (marketDataRecord.Quotes)
                    {
                        double num = Math.Round(random.NextDouble() * (max - min) + min, DecimalDigits);
                        double price2 = num + Math.Round(random.NextDouble() * (max2 - min2) + min2, DecimalDigits);
                        var bid = new Bid(this.framework.Clock.DateTime, Id, marketDataRecord.Instrument.Id, num, random.Next(min3, max3));
                        var ask = new Ask(this.framework.Clock.DateTime, Id, marketDataRecord.Instrument.Id, price2, random.Next(min3, max3));
                        EmitData(bid, true);
                        EmitData(ask, true);
                    }
                }
                Thread.Sleep(this.Delay);
            }
        }

        private volatile bool doWork;

        private Dictionary<Instrument, MarketDataRecord> mdRecords;
    }
}
