using System;

namespace FastQuant.Data.Compression
{
    public class RenkoBarCompressor : BarCompressor
    {
        public RenkoBarCompressor(int instrumentId, double newBarSize)
        {
            Instrument byId = Framework.Current.InstrumentManager.GetById(instrumentId);
            if (byId != null)
            {
                this.double_0 = newBarSize * byId.TickSize;
                return;
            }
            Console.WriteLine($"Instrument with id:{instrumentId} not found");
        }

        public override void Add(DataEntry entry)
        {
            double price = entry.Items[0].Price;
            if (this.bar == null)
            {
                this.double_1 = (double)((decimal)price + (decimal)this.double_0);
                this.double_2 = (double)((decimal)price - (decimal)this.double_0);
                this.bar = new Bar(entry.DateTime, entry.DateTime, this.instrumentId, BarType.Custom, this.newBarSize, price, price, price, price, 0L, 0L);
                base.AddItemsToBar(entry.Items);
                return;
            }
            if (price >= this.double_1 + this.double_0)
            {
                if (!this.bool_0)
                {
                    this.bar.High = this.double_1;
                    this.bar.Close = this.double_1;
                    this.bool_0 = true;
                }
                this.double_2 = this.double_1;
                this.double_1 = (double)((decimal)this.double_1 + (decimal)this.double_0);
                this.bar.DateTime = entry.DateTime;
                base.EmitNewCompressedBar();
                double high = (price > this.double_1) ? price : this.double_1;
                this.bar = new Bar(entry.DateTime, entry.DateTime, this.instrumentId, BarType.Custom, this.newBarSize, this.double_2, high, this.double_2, this.double_1, 0L, 0L);
                base.AddItemsToBar(entry.Items);
                return;
            }
            if (price <= this.double_2 - this.double_0)
            {
                if (!this.bool_0)
                {
                    this.bar.Low = this.double_2;
                    this.bar.Close = this.double_2;
                    this.bool_0 = true;
                }
                this.double_1 = this.double_2;
                this.double_2 = (double)((decimal)this.double_2 - (decimal)this.double_0);
                this.bar.DateTime = entry.DateTime;
                base.EmitNewCompressedBar();
                double low = (price < this.double_2) ? price : this.double_2;
                this.bar = new Bar(entry.DateTime, entry.DateTime, this.instrumentId, BarType.Custom, this.newBarSize, this.double_1, this.double_1, low, this.double_2, 0L, 0L);
                base.AddItemsToBar(entry.Items);
                return;
            }
            AddItemsToBar(entry.Items);
        }

        private double double_0;

        private double double_1;

        private double double_2;

        private bool bool_0;
    }
}