using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace FastQuant
{
    public class FundamentalData
    {
        public const byte CashFlow = 1;

        public const byte PE = 2;

        public const byte Beta = 3;

        public const byte ProfitMargin = 4;

        public const byte ReturnOnEquity = 5;

        public const byte PriceBook = 6;

        public const byte DebtEquity = 7;

        public const byte InterestCoverage = 8;

        public const byte BookValue = 9;

        public const byte PriceSales = 10;

        public const byte DividendPayout = 11;

        public const byte Split = 12;
    }

    [DataContract]
    public class Fundamental : DataObject
    {
        public override byte TypeId => DataObjectType.Fundamental;

        internal int ProviderId { get; }

        internal int InstrumentId { get; set; }

        internal ObjectTable Fields { get; set; } = new ObjectTable();

        public Fundamental()
        {
        }

        public Fundamental(DateTime dateTime, int providerId, int instrumentId) : base(dateTime)
        {
            ProviderId = providerId;
            InstrumentId = instrumentId;
        }

        public override string ToString() => string.Join(";", Enumerable.Range(0, Fields.Size).Select(i => $"{i}={this[(byte)i]}"));

        public object this[byte index]
        {
            get
            {
                return Fields[index];
            }
            set
            {
                Fields[index] = value;
            }
        }

        public object this[string name]
        {
            get
            {
                return this[mapping[name]];
            }
            set
            {
                this[mapping[name]] = value;
            }
        }


        public static void AddField(string name, byte index)
        {
            if (mapping.ContainsKey(name))
            {
                Console.WriteLine($"{nameof(Fundamental)}::{nameof(AddField)} with Name = {name} is already exist");
                return;
            }
            if (mapping.ContainsValue(index))
            {
                Console.WriteLine($"{nameof(Fundamental)}::{nameof(AddField)} with Index = {index} is already exist");
                return;
            }
            mapping.Add(name, index);
        }

        public static bool Contains(string name) => mapping.ContainsKey(name);

        public static byte GetIndex(string name) => mapping[name];

        public static void Print() => Console.WriteLine(string.Join(Environment.NewLine, mapping.Keys.Select(k => $"{k}-{mapping[k]}")));

        [UglyNaming]
        public void Print_Temporary()
        {
            var sb = new StringBuilder();
            sb.Append($"Fundamental, Time: {DateTime}{Environment.NewLine}");
            sb.Append($"InstrumentId: {InstrumentId}{Environment.NewLine}");
            sb.Append($"ProviderId: {ProviderId}{Environment.NewLine}");

            foreach (var k in mapping.Keys)
            {
                if (this[k] != null)
                {
                    sb.Append($"{k}={this[k]}{Environment.NewLine}");
                }
            }
            Console.Write(sb.ToString());
        }

        public static void RemoveField(string name) => mapping.Remove(name);

        private static readonly Dictionary<string, byte> mapping;

        static Fundamental()
        {
            var fields = typeof(FundamentalData).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var f in fields.Where(f => f.FieldType == typeof(byte) && f.IsLiteral && !f.IsInitOnly))
                mapping.Add(f.Name, (byte) f.GetRawConstantValue());
        }

    }
}