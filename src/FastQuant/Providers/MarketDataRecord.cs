namespace FastQuant.Providers
{
    public class MarketDataRecord : InstrumentRecord
    {
        public MarketDataRecord(Instrument instrument) : base(instrument)
        {
            Trades = false;
            Quotes = false;
            Level2 = false;
        }

        public bool Trades { get; set; }

        public bool Quotes { get; set; }

        public bool Level2 { get; set; }
    }
}