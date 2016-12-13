﻿using System;

namespace FastQuant
{
    public class ExecutionMessage : DataObject
    {
        private Instrument instrument;
        private ObjectTable fields;
        private Order order;

        public int Id { get; internal set; } = -1;

        public int InstrumentId { get; internal set; } = -1;

        public int OrderId { get; set; } = -1;

        public int ClientId { get; set; } = -1;

        public bool IsLoaded { get; internal set; }

        public Order Order
        {
            get
            {
                return this.order;
            }
            set
            {
                this.order = value;
                OrderId = this.order.Id;
            }
        }

        public string ClOrderId { get; set; } = string.Empty;

        public string ProviderOrderId { get; set; } = string.Empty;

        public byte CurrencyId { get; set; }

        public Instrument Instrument
        {
            get
            {
                return this.instrument;
            }
            set
            {
                this.instrument = value;
                InstrumentId = this.instrument.Id;
                CurrencyId = this.instrument.CurrencyId;
            }
        }

        public ObjectTable Fields
        {
            get
            {
                return this.fields = this.fields ?? new ObjectTable();
            }
            internal set
            {
                this.fields = value;
            }
        }

        public object this[int index]
        {
            get
            {
                return this.fields?[index];
            }
            set
            {
                Fields[index] = value;
            }
        }

        public ExecutionMessage()
        {
        }

        public ExecutionMessage(ExecutionMessage executionMessage) : base(executionMessage)
        {
            Id = executionMessage.Id;
            this.order = executionMessage.Order;
            OrderId = executionMessage.OrderId;
            ClOrderId = executionMessage.ClOrderId;
            ProviderOrderId = executionMessage.ProviderOrderId;
            this.instrument = executionMessage.Instrument;
            InstrumentId = executionMessage.InstrumentId;
            ClientId = executionMessage.ClientId;
            IsLoaded = executionMessage.IsLoaded;
            CurrencyId = executionMessage.CurrencyId;
            Fields = new ObjectTable(executionMessage.Fields);
        }
    }

    public class AccountReport : ExecutionMessage
    {
        public override byte TypeId => DataObjectType.AccountReport;

        public double Amount { get; internal set; }

        public new byte CurrencyId { get; internal set; }

        public int PortfolioId { get; internal set; }

        public string Text { get; internal set; }

        public AccountReport()
        {
        }

        public AccountReport(AccountReport accountReport) : base(accountReport)
        {
            PortfolioId = accountReport.PortfolioId;
            CurrencyId = accountReport.CurrencyId;
            Amount = accountReport.Amount;
            Text = accountReport.Text;
        }
    }

    public class ExecutionReport : ExecutionMessage
    {
        public override byte TypeId => DataObjectType.ExecutionReport;

        public ExecType ExecType { get; set; }

        public OrderType OrdType { get; set; }

        public OrderSide Side { get; set; }

        public TimeInForce TimeInForce { get; set; }

        public OrderStatus OrdStatus { get; set; }

        public double LastPx { get; set; }

        public double AvgPx { get; set; }

        public double OrdQty { get; set; }

        public double CumQty { get; set; }

        public double LastQty { get; set; }

        public double LeavesQty { get; set; }

        public double Price { get; set; }

        public double StopPx { get; set; }

        public double Commission { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime ExpireTime { get; set; }

        public string ExecId { get; set; } = string.Empty;

        public ExecutionReport()
        {
        }

        public ExecutionReport(ExecutionReport report)
        {
            DateTime = report.DateTime;
            Instrument = report.Instrument;
            Order = report.Order;
            CurrencyId = report.CurrencyId;
            ExecType = report.ExecType;
            OrdType = report.OrdType;
            Side = report.Side;
            TimeInForce = report.TimeInForce;
            OrdStatus = report.OrdStatus;
            LastPx = report.LastPx;
            AvgPx = report.AvgPx;
            OrdQty = report.OrdQty;
            CumQty = report.CumQty;
            LastQty = report.LastQty;
            LeavesQty = report.LeavesQty;
            Price = report.Price;
            StopPx = report.StopPx;
            Commission = report.Commission;
            Text = report.Text;
        }

        public override string ToString() => $"{DateTime} {Instrument?.Symbol ?? InstrumentId.ToString()} {ExecType} {Side} {LastPx}";
    }

    public delegate void ExecutionReportEventHandler(object sender, ExecutionReport report);

    public enum ExecutionCommandType
    {
        Send,
        Cancel,
        Replace
    }

    public class ExecutionCommand : ExecutionMessage
    {
        public override byte TypeId => DataObjectType.ExecutionCommand;

        public Portfolio Portfolio { get; internal set; }

        public IExecutionProvider Provider { get; internal set; }

        public int AlgoId { get; internal set; }

        public string OCA { get; internal set; } = string.Empty;

        public string Text { get; internal set; } = string.Empty;

        public double StopPx { get; internal set; }

        public double Price { get; internal set; }

        public OrderSide Side { get; internal set; }

        public OrderType OrdType { get; internal set; }

        public TimeInForce TimeInForce { get; internal set; }

        public double Qty { get; internal set; }

        public DateTime TransactTime { get; internal set; }

        public ExecutionCommandType Type { get; internal set; }

        public string Account { get; internal set; }

        public string ClientID { get; internal set; }

        public byte ProviderId { get; internal set; }

        public int PortfolioId { get; internal set; }

        public byte RouteId { get; internal set; }

        public DateTime ExpireTime { get; internal set; }

        public int StrategyId { get; internal set; }

        public string ExecInst { get; internal set; } = string.Empty;

        public double MinQty { get; internal set; }

        public double PegDifference { get; internal set; }

        public string Symbol { get; internal set; }

        public ExecutionCommand()
        {
        }

        public ExecutionCommand(ExecutionCommandType type, Order order)
            : this()
        {
            Type = type;
            Order = order;
            OrderId = order.Id;
            ProviderId = order.ProviderId;
            RouteId = order.RouteId;
            AlgoId = order.AlgoId;
            PortfolioId = order.PortfolioId;
            InstrumentId = order.InstrumentId;
            TransactTime = order.TransactTime;
            DateTime = order.DateTime;
            Instrument = order.Instrument;
            Provider = order.Provider;
            Portfolio = order.Portfolio;
            Side = order.Side;
            OrdType = order.Type;
            TimeInForce = order.TimeInForce;
            ExpireTime = order.ExpireTime;
            Price = order.Price;
            StopPx = order.StopPx;
            Qty = order.Qty;
            MinQty = order.MinQty;
            PegDifference = order.PegDifference;
            ExecInst = order.ExecInst;
            OCA = order.OCA;
            Text = order.Text;
            Account = order.Account;
            ClientID = order.ClientID;
            Fields = new ObjectTable(order.Fields);
        }

        public ExecutionCommand(ExecutionCommand command)
            : this()
        {
            Type = command.Type;
            Id = command.Id;
            ProviderId = command.ProviderId;
            RouteId = command.RouteId;
            AlgoId = command.AlgoId;
            PortfolioId = command.PortfolioId;
            InstrumentId = command.InstrumentId;
            TransactTime = command.TransactTime;
            DateTime = command.DateTime;
            Instrument = command.Instrument;
            Provider = command.Provider;
            Portfolio = command.Portfolio;
            Side = command.Side;
            OrdType = command.OrdType;
            TimeInForce = command.TimeInForce;
            ExpireTime = command.ExpireTime;
            Price = command.Price;
            StopPx = command.StopPx;
            Qty = command.Qty;
            OCA = command.OCA;
            Text = command.Text;
            Account = command.Account;
            ClientID = command.ClientID;
            Fields = new ObjectTable(command.Fields);
        }

        public override string ToString() => $"{DateTime} {Type} {Instrument?.Symbol ?? InstrumentId.ToString()} {Side} {OrdType} {Qty}";

    }

    public delegate void ExecutionCommandEventHandler(object sender, ExecutionCommand command);

}