using FastQuant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace FastQuant.Experimental.Strategy
{
    public class Strategy
    {
        public Framework Framework { get; }

        [ReadOnly(true), Parameter, Category("Information")]
        public int Id { get; internal set; }

        public string Name { get; set; }

        [Category("Information"), Parameter, ReadOnly(true)]
        public virtual string Type { get; } = nameof(Strategy);

        public bool Enabled { get; set; }

        public int ClientId { get; set; }

        public SubscriptionList Subscriptions { get; }

        public StrategyMode Mode { get; internal set; }

        public StrategyStatus Status { get; internal set; }

        public bool IsInstance { get; protected internal set; }

        public IDataProvider DataProvider
        {
            get
            {
                return this.dataProvider;
            }
            set
            {
                this.dataProvider = value;
                foreach (var strategy in Strategies)
                {
                    strategy.DataProvider = this.dataProvider;
                }
            }
        }

        public IExecutionProvider ExecutionProvider
        {
            get
            {
                return this.executionProvider;
            }
            set
            {
                this.executionProvider = value;
                foreach (var strategy in Strategies)
                {
                    strategy.ExecutionProvider = this.executionProvider;
                }
            }
        }

        public Strategy Parent { get; private set; }

        public StrategyList Strategies { get; }

        public Portfolio Portfolio { get; internal set; }

        public BarSeries Bars { get; }

        public TimeSeries Equity { get; }

        public Clock Clock => Framework.Clock;

        public InstrumentManager InstrumentManager => Framework.InstrumentManager;

        public DataManager DataManager => Framework.DataManager;

        public IDataSimulator DataSimulator => ProviderManager.DataSimulator;

        public IExecutionSimulator ExecutionSimulator => ProviderManager.ExecutionSimulator;

        public ProviderManager ProviderManager => Framework.ProviderManager;

        public OrderManager OrderManager => Framework.OrderManager;

        public BarFactory BarFactory => Framework.EventManager.BarFactory;

        public StrategyManager StrategyManager => Framework.ExperimentalStrategyManager;

        public EventManager EventManager => Framework.EventManager;

        public GroupManager GroupManager => Framework.GroupManager;

        public AccountDataManager AccountDataManager => Framework.AccountDataManager;

        public Global Global => Framework.StrategyManager.Global;

        [NotOriginal]
        internal PortfolioManager PortfolioManager => Framework.PortfolioManager;

        [NotOriginal]
        internal SubscriptionManager SubscriptionManager => Framework.SubscriptionManager;

        [NotOriginal]
        internal EventServer EventServer => Framework.EventServer;

        public Strategy(Framework framework, string name)
        {
            this.Id = -1;
            this.ClientId = -1;
            this.IsInstance = true;
            this.Enabled = true;
            this.Status = StrategyStatus.Stopped;
            this.Mode = StrategyMode.Backtest;
            this.Strategies = new StrategyList();
            this.Subscriptions = new SubscriptionList();
            this.subscriptionList_1 = new SubscriptionList();
            this.Bars = new BarSeries();
            this.Equity = new TimeSeries();
            this.list_0 = new List<Stop>();
            this.idArray_0 = new IdArray<List<Stop>>(10000);
            Framework = framework;
            Name = name;
            this.parameterHelper_0 = new ParameterHelper();
        }

        public virtual void Init()
        {
            if (!this.bool_1)
            {
                if (PortfolioManager.Portfolios.Contains(this.Name))
                {
                    this.Portfolio = PortfolioManager.Portfolios.GetByName(this.Name);
                }
                else
                {
                    this.Portfolio = new Portfolio(this.Framework, this.Name);
                    if (this.Parent != null)
                    {
                        this.Portfolio.Account.CurrencyId = this.Parent.Portfolio.Account.CurrencyId;
                    }
                    PortfolioManager.Add(this.Portfolio, true);
                }
                if (this.Parent != null)
                {
                    Portfolio.Parent = Parent.Portfolio;
                }
                if (this.Id == -1)
                {
                    this.Id = StrategyManager.GetNextId();
                }
                StrategyManager.method_0(this);
                this.bool_1 = true;
            }
            foreach (Subscription current in this.Subscriptions)
            {
                Portfolio.GetOrCreatePosition(current.Instrument);
            }
        }

        private Strategy method_0(string string_1)
        {
            foreach (Strategy current in Strategies)
            {
                if (current.Name == string_1)
                {
                    return current;
                }
            }
            return null;
        }

        public Strategy GetStrategy(string name)
        {
            string[] arg_18_0 = name.Split('\\', '/');
            Strategy strategy_ = this;
            string[] array = arg_18_0;
            for (int i = 0; i < array.Length; i++)
            {
                string string_ = array[i];
                strategy_ = strategy_.method_0(string_);
                if (strategy_ == null)
                {
                    return null;
                }
            }
            return strategy_;
        }

        internal IExecutionProvider method_1(Instrument instrument_0 = null)
        {
            IExecutionProvider executionProvider = null;
            if (instrument_0 != null && instrument_0.ExecutionProvider != null)
            {
                executionProvider = instrument_0.ExecutionProvider;
            }
            if (executionProvider == null && this.executionProvider != null)
            {
                executionProvider = this.executionProvider;
            }
            if (this.Mode == StrategyMode.Live)
            {
                if (executionProvider == null)
                {
                    executionProvider = this.Framework.ExecutionProvider;
                }
                return executionProvider;
            }
            if (executionProvider is SellSideStrategy)
            {
                return executionProvider;
            }
            return ExecutionSimulator;
        }

        internal IDataProvider method_2(Subscription subscription_0)
        {
            IDataProvider idataSimulator_;
            if (this.Mode == StrategyMode.Backtest)
            {
                if (subscription_0.Provider == null)
                {
                    if (this.dataProvider is SellSideStrategy)
                    {
                        idataSimulator_ = this.dataProvider;
                    }
                    else
                    {
                        idataSimulator_ = ProviderManager.DataSimulator;
                    }
                }
                else if (subscription_0.Provider is SellSideStrategy)
                {
                    idataSimulator_ = subscription_0.Provider;
                }
                else
                {
                    idataSimulator_ = ProviderManager.DataSimulator;
                }
            }
            else if (subscription_0.Provider == null)
            {
                idataSimulator_ = this.dataProvider;
            }
            else
            {
                idataSimulator_ = subscription_0.Provider;
            }
            return idataSimulator_;
        }

        internal void method_3(Instrument instrument_0, IDataProvider idataProvider_1 = null)
        {
            if (!this.Subscriptions.Contains(instrument_0, idataProvider_1))
            {
                Subscription subscription = new Subscription(instrument_0, idataProvider_1, -1);
                this.Subscriptions.Add(subscription);
                if (this.Status == StrategyStatus.Running)
                {
                    this.method_8(subscription);
                }
                this.vmethod_6(instrument_0, idataProvider_1);
            }
        }

        public virtual void Add(Instrument instrument, IDataProvider provider = null)
        {
            this.method_3(instrument, provider);
        }

        public void Add(string[] symbols)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                string symbol = symbols[i];
                this.Add(InstrumentManager.GetBySymbol(symbol), null);
            }
        }

        public void Add(InstrumentList instruments)
        {
            foreach (Instrument current in instruments)
            {
                this.Add(current, null);
            }
        }

        public void Add(string symbol)
        {
            Instrument bySymbol = InstrumentManager.GetBySymbol(symbol);
            if (bySymbol == null)
            {
                Console.WriteLine("Strategy::AddInstrument instrument with symbol " + symbol + " doesn't exist in the framework");
                return;
            }
            this.Add(bySymbol);
        }

        public void Add(int id)
        {
            var instrument = InstrumentManager.GetById(id);
            if (instrument != null)
                Add(instrument);
            else
                Console.WriteLine($"Strategy::AddInstrument instrument with Id {id} doesn't exist in the framework");
        }

        internal void method_4(Instrument instrument_0, IDataProvider idataProvider_1 = null)
        {
            if (this.Subscriptions.Contains(instrument_0, idataProvider_1))
            {
                Subscription subscription = this.Subscriptions.Get(instrument_0, idataProvider_1);
                if (this.Status == StrategyStatus.Running)
                {
                    this.method_11(subscription);
                }
                this.Subscriptions.Remove(subscription.InstrumentId, subscription.ProviderId);
                this.vmethod_7(instrument_0, idataProvider_1);
            }
        }

        public virtual void Remove(Instrument instrument, IDataProvider provider = null)
        {
            this.method_4(instrument, provider);
        }

        public void Remove(int id)
        {
            Instrument byId = InstrumentManager.GetById(id);
            if (byId == null)
            {
                Console.WriteLine("Strategy::RemoveInstrument instrument with Id " + id + " doesn't exist in the framework");
                return;
            }
            this.Remove(byId, null);
        }

        public void Remove(string symbol)
        {
            Instrument bySymbol = InstrumentManager.GetBySymbol(symbol);
            if (bySymbol == null)
            {
                Console.WriteLine("Strategy::RemoveInstrument instrument with symbol " + symbol + " doesn't exist in the framework");
                return;
            }
            this.Remove(bySymbol, null);
        }

        internal void method_5(Strategy strategy__1)
        {
            if (strategy__1.Parent != null)
            {
                Console.WriteLine("Strategy::Add Can not add a startegy that already has a parent " + strategy__1);
                return;
            }
            if (strategy__1.Id == -1)
            {
                strategy__1.Id = StrategyManager.GetNextId();
            }
            strategy__1.Parent = this;
            this.Strategies.Add(strategy__1);
            if (this.Status == StrategyStatus.Running)
            {
                strategy__1.icojrGfcqNm();
                strategy__1.vmethod_0(this.Mode);
            }
        }

        public virtual void Add(Strategy strategy)
        {
            this.method_5(strategy);
        }

        internal Strategy method_6(string string_1)
        {
            var strategy_ = (Strategy)Activator.CreateInstance(base.GetType(), this.Framework, string_1);
            strategy_.IsInstance = true;
            strategy_.SetRawDataProvider(DataProvider);
            strategy_.SetRawExecutionProvider(ExecutionProvider);
        //    strategy_.idataProvider_0 = this.DataProvider;
        //    strategy_.iexecutionProvider_0 = this.ExecutionProvider;
            this.method_7(strategy_);
            return strategy_;
        }

        private void SetRawExecutionProvider(IExecutionProvider executionProvider)
        {
            this.executionProvider = executionProvider;
        }

        private void SetRawDataProvider(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        internal void method_7(Strategy strategy__1)
        {
            FieldInfo[] fields = strategy__1.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.GetCustomAttributes(typeof(ParameterAttribute), true).Length != 0)
                {
                    fieldInfo.SetValue(strategy__1, fieldInfo.GetValue(this));
                }
            }
            PropertyInfo[] properties = strategy__1.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true).Length != 0 && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(strategy__1, propertyInfo.GetValue(this));
                }
            }
        }

        internal void method_8(Subscription subscription_0)
        {
            Console.WriteLine(string.Concat("Strategy::Subscribe ", Name, " ", subscription_0.Provider, " ", subscription_0.Instrument));
            IDataProvider provider = this.method_2(subscription_0);
            Subscription subscription = new Subscription(subscription_0.Instrument, provider, this.Id);
            this.subscriptionList_1.Add(subscription);
            StrategyManager.method_1(this, subscription);
            if (!this.Framework.IsExternalDataQueue)
            {
                SubscriptionManager.Subscribe(subscription.Provider, subscription.Instrument);
            }
        }

        internal void method_9()
        {
            foreach (Subscription current in this.Subscriptions)
            {
                this.method_8(current);
            }
        }

        internal void method_10()
        {
            foreach (Subscription current in this.Subscriptions)
            {
                this.method_11(current);
            }
        }

        internal void method_11(Subscription subscription_0)
        {
            IDataProvider provider = this.method_2(subscription_0);
            Subscription subscription = this.subscriptionList_1.Get(subscription_0.Instrument, provider);
            this.subscriptionList_1.Remove(subscription);
            StrategyManager.method_2(this, subscription);
            if (!this.Framework.IsExternalDataQueue)
            {
                SubscriptionManager.Unsubscribe(provider, subscription_0.Instrument);
            }
        }

        public virtual double Objective()
        {
            return this.Portfolio.Value;
        }

        public void Log(Event e, Group group)
        {
            EventServer.OnLog(new GroupEvent(e, group));
        }

        public void Log(Event e, int groupId)
        {
            EventServer.OnLog(new GroupEvent(e, GroupManager.GroupById[groupId]));
        }

        public void Log(DataObject data, Group group)
        {
            EventServer.OnLog(new GroupEvent(data, group));
        }

        public void Log(DataObject data, int groupId)
        {
            EventServer.OnLog(new GroupEvent(data, GroupManager.GroupById[groupId]));
        }

        public void Log(DateTime dateTime, double value, Group group)
        {
            EventServer.OnLog(new GroupEvent(new TimeSeriesItem(dateTime, value), group));
        }

        public void Log(DateTime dateTime, double value, int groupId)
        {
            EventServer.OnLog(new GroupEvent(new TimeSeriesItem(dateTime, value), groupId));
        }

        public void Log(DateTime dateTime, string text, Group group)
        {
            EventServer.OnLog(new GroupEvent(new TextInfo(dateTime, text), group));
        }

        public void Log(DateTime dateTime, string text, int groupId)
        {
            EventServer.OnLog(new GroupEvent(new TextInfo(dateTime, text), groupId));
        }

        public void Log(double value, Group group)
        {
            EventServer.OnLog(new GroupEvent(new TimeSeriesItem(Framework.Clock.DateTime, value), group));
        }

        public void Log(double value, int groupId)
        {
            EventServer.OnLog(new GroupEvent(new TimeSeriesItem(Framework.Clock.DateTime, value), groupId));
        }

        public void Log(string text, Group group)
        {
            EventServer.OnLog(new GroupEvent(new TextInfo(Framework.Clock.DateTime, text), group));
        }

        public void Log(string text, int groupId)
        {
            EventServer.OnLog(new GroupEvent(new TextInfo(Framework.Clock.DateTime, text), groupId));
        }

        public bool HasPosition(Instrument instrument)
        {
            return this.Portfolio.HasPosition(instrument);
        }

        public bool HasPosition(Instrument instrument, PositionSide side, double qty)
        {
            return this.Portfolio.HasPosition(instrument, side, qty);
        }

        public bool HasLongPosition(Instrument instrument)
        {
            return this.Portfolio.HasLongPosition(instrument);
        }

        public bool HasLongPosition(Instrument instrument, double qty)
        {
            return this.Portfolio.HasLongPosition(instrument, qty);
        }

        public bool HasShortPosition(Instrument instrument)
        {
            return this.Portfolio.HasShortPosition(instrument);
        }

        public bool HasShortPosition(Instrument instrument, double qty)
        {
            return this.Portfolio.HasShortPosition(instrument, qty);
        }

        public Reminder AddReminder(DateTime dateTime, object data = null)
        {
            return Framework.Clock.AddReminder(new ReminderCallback(this.vmethod_8), dateTime, data);
        }

        public Reminder AddExchangeReminder(DateTime dateTime, object data = null)
        {
            return Framework.ExchangeClock.AddReminder(new ReminderCallback(this.vmethod_9), dateTime, data);
        }

        public void Deposit(DateTime dateTime, double value, byte currencyId = 148, string text = null, bool updateParent = true)
        {
            this.Portfolio.Account.Deposit(dateTime, value, currencyId, text, updateParent);
        }

        public void Withdraw(DateTime dateTime, double value, byte currencyId = 148, string text = null, bool updateParent = true)
        {
            this.Portfolio.Account.Withdraw(dateTime, value, currencyId, text, updateParent);
        }

        public void Deposit(double value, byte currencyId = 148, string text = null, bool updateParent = true)
        {
            this.Portfolio.Account.Deposit(value, currencyId, text, updateParent);
        }

        public void Withdraw(double value, byte currencyId = 148, string text = null, bool updateParent = true)
        {
            Portfolio.Account.Withdraw(value, currencyId, text, updateParent);
        }

        public void AddStop(Stop stop)
        {
            this.list_0.Add(stop);
            if (this.idArray_0[stop.instrument.Id] == null)
            {
                this.idArray_0[stop.instrument.Id] = new List<Stop>();
            }
            this.idArray_0[stop.instrument.Id].Add(stop);
        }

        public void Send(Order order)
        {
            OrderManager.Send(order);
        }

        public void Cancel(Order order)
        {
            OrderManager.Cancel(order);
        }

        public void CancelAll()
        {
            List<Order> list = new List<Order>();
            foreach (Order current in OrderManager.Orders)
            {
                if (current.StrategyId == this.Id && !current.IsDone)
                {
                    list.Add(current);
                }
            }
            foreach (Order current2 in list)
            {
                this.Cancel(current2);
            }
        }

        public void CancelAll(OrderSide side)
        {
            List<Order> list = new List<Order>();
            foreach (Order current in OrderManager.Orders)
            {
                if (current.StrategyId == this.Id && current.Side == side && !current.IsDone)
                {
                    list.Add(current);
                }
            }
            foreach (Order current2 in list)
            {
                this.Cancel(current2);
            }
        }

        public void CancelAll(Instrument instrument)
        {
            List<Order> list = new List<Order>();
            foreach (Order current in OrderManager.Orders)
            {
                if (current.StrategyId == this.Id && current.Instrument == instrument && !current.IsDone)
                {
                    list.Add(current);
                }
            }
            foreach (Order current2 in list)
            {
                this.Cancel(current2);
            }
        }

        public void CancelAll(Instrument instrument, OrderSide side)
        {
            List<Order> list = new List<Order>();
            foreach (Order current in OrderManager.Orders)
            {
                if (current.StrategyId == this.Id && current.Instrument == instrument && current.Side == side && !current.IsDone)
                {
                    list.Add(current);
                }
            }
            foreach (Order current2 in list)
            {
                this.Cancel(current2);
            }
        }

        public void Reject(Order order)
        {
            OrderManager.Reject(order);
        }

        public void Replace(Order order, double price)
        {
            OrderManager.Replace(order, price);
        }

        public void Replace(Order order, double price, double stopPx, double qty)
        {
            OrderManager.Replace(order, price, stopPx, qty);
        }

        public Order Buy(IExecutionProvider provider, Instrument instrument, OrderType type, double qty, double price, double stopPx, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, type, OrderSide.Buy, qty, price, stopPx, TimeInForce.Day, 0, "");
            order.StrategyId = Id;
            order.Text = text;
            this.Send(order);
            return order;
        }

        public Order Buy(IExecutionProvider provider, Instrument instrument, double qty, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Market, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            this.Send(order);
            return order;
        }

        public Order Buy(short providerId, Instrument instrument, OrderType type, double qty, double price, double stopPx, string text = "")
        {
            return this.Buy(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, type, qty, price, stopPx, text);
        }

        public Order Buy(short providerId, Instrument instrument, double qty, string text = "")
        {
            return this.Buy(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, text);
        }

        public Order Buy(Instrument instrument, OrderType type, double qty, double price, double stopPx, string text = "")
        {
            return this.Buy(this.method_1(instrument), instrument, type, qty, price, stopPx, text);
        }

        public Order Buy(Instrument instrument, double qty, string text = "")
        {
            return this.Buy(this.method_1(instrument), instrument, qty, text);
        }

        public Order Sell(IExecutionProvider provider, Instrument instrument, OrderType type, double qty, double price, double stopPx, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, type, OrderSide.Sell, qty, price, stopPx, TimeInForce.Day, 0, "");
            order.StrategyId = Id;
            order.Text = text;
            this.Send(order);
            return order;
        }

        public Order Sell(IExecutionProvider provider, Instrument instrument, double qty, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Market, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            this.Send(order);
            return order;
        }

        public Order Sell(short providerId, Instrument instrument, OrderType type, double qty, double price, double stopPx, string text = "")
        {
            return this.Sell(ProviderManager.GetExecutionProvider((int)providerId), instrument, type, qty, price, stopPx, text);
        }

        public Order Sell(short providerId, Instrument instrument, double qty, string text = "")
        {
            return this.Sell(ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, text);
        }

        public Order Sell(Instrument instrument, OrderType type, double qty, double price, double stopPx, string text = "")
        {
            return this.Sell(this.method_1(instrument), instrument, type, qty, price, stopPx, text);
        }

        public Order Sell(Instrument instrument, double qty, string text = "")
        {
            return this.Sell(this.method_1(instrument), instrument, qty, text);
        }

        public Order BuyPegged(IExecutionProvider provider, Instrument instrument, double qty, double offset, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Pegged, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.StopPx = offset;
            order.Text = text;
            this.Send(order);
            return order;
        }

        public Order BuyPegged(short providerId, Instrument instrument, double qty, double offset, string text = "")
        {
            return this.BuyPegged(ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, offset, text);
        }

        public Order BuyPegged(Instrument instrument, double qty, double offset, string text = "")
        {
            return this.BuyPegged(this.method_1(instrument), instrument, qty, offset, text);
        }

        public Order SellPegged(IExecutionProvider provider, Instrument instrument, double qty, double offset, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Pegged, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.StopPx = offset;
            order.Text = text;
            this.Send(order);
            return order;
        }

        public Order SellPegged(short providerId, Instrument instrument, double qty, double offset, string text = "")
        {
            return this.SellPegged(ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, offset, text);
        }

        public Order SellPegged(Instrument instrument, double qty, double offset, string text = "")
        {
            return this.SellPegged(this.method_1(instrument), instrument, qty, offset, text);
        }

        public Order BuyLimit(IExecutionProvider provider, Instrument instrument, double qty, double price, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Limit, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.Price = price;
            this.Send(order);
            return order;
        }

        public Order BuyLimit(short providerId, Instrument instrument, double qty, double price, string text = "")
        {
            return this.BuyLimit(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, price, text);
        }

        public Order BuyLimit(Instrument instrument, double qty, double price, string text = "")
        {
            return this.BuyLimit(this.method_1(instrument), instrument, qty, price, text);
        }

        public Order SellLimit(IExecutionProvider provider, Instrument instrument, double qty, double price, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Limit, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.Price = price;
            this.Send(order);
            return order;
        }

        public Order SellLimit(short providerId, Instrument instrument, double qty, double price, string text = "")
        {
            return this.SellLimit(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, price, text);
        }

        public Order SellLimit(Instrument instrument, double qty, double price, string text = "")
        {
            return this.SellLimit(this.method_1(instrument), instrument, qty, price, text);
        }

        public Order BuyStop(IExecutionProvider provider, Instrument instrument, double qty, double stopPx, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Stop, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            this.Send(order);
            return order;
        }

        public Order BuyStop(short providerId, Instrument instrument, double qty, double stopPx, string text = "")
        {
            return this.BuyStop(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, stopPx, text);
        }

        public Order BuyStop(Instrument instrument, double qty, double stopPx, string text = "")
        {
            return this.BuyStop(this.method_1(instrument), instrument, qty, stopPx, text);
        }

        public Order SellStop(IExecutionProvider provider, Instrument instrument, double qty, double stopPx, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.Stop, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            this.Send(order);
            return order;
        }

        public Order SellStop(short providerId, Instrument instrument, double qty, double stopPx, string text = "")
        {
            return this.SellStop(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, stopPx, text);
        }

        public Order SellStop(Instrument instrument, double qty, double stopPx, string text = "")
        {
            return this.SellStop(this.method_1(instrument), instrument, qty, stopPx, text);
        }

        public Order BuyStopLimit(IExecutionProvider provider, Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.StopLimit, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            order.Price = price;
            this.Send(order);
            return order;
        }

        public Order BuyStopLimit(short providerId, Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            return this.BuyStopLimit(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, stopPx, price, text);
        }

        public Order BuyStopLimit(Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            return this.BuyStopLimit(this.method_1(instrument), instrument, qty, stopPx, price, text);
        }

        public Order SellStopLimit(IExecutionProvider provider, Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            Order order = new Order(provider, this.Portfolio, instrument, OrderType.StopLimit, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            order.Price = price;
            this.Send(order);
            return order;
        }

        public Order SellStopLimit(short providerId, Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            return this.SellStopLimit(this.Framework.ProviderManager.GetExecutionProvider((int)providerId), instrument, qty, stopPx, price, text);
        }

        public Order SellStopLimit(Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            return this.SellStopLimit(this.method_1(instrument), instrument, qty, stopPx, price, text);
        }

        public Order BuyOrder(Instrument instrument, double qty, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.Market, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            OrderManager.Register(order);
            return order;
        }

        public Order SellOrder(Instrument instrument, double qty, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.Market, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            OrderManager.Register(order);
            return order;
        }

        public Order BuyLimitOrder(Instrument instrument, double qty, double price, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.Limit, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.Price = price;
            OrderManager.Register(order);
            return order;
        }

        public Order SellLimitOrder(Instrument instrument, double qty, double price, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.Limit, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.Price = price;
            OrderManager.Register(order);
            return order;
        }

        public Order BuyStopOrder(Instrument instrument, double qty, double stopPx, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.Stop, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            OrderManager.Register(order);
            return order;
        }

        public Order SellStopOrder(Instrument instrument, double qty, double stopPx, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.Stop, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            OrderManager.Register(order);
            return order;
        }

        public Order Order(Instrument instrument, OrderType type, OrderSide side, double qty, double stopPx, double price, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, type, side, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            order.Price = price;
            OrderManager.Register(order);
            return order;
        }

        public Order BuyStopLimitOrder(Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.StopLimit, OrderSide.Buy, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            order.Price = price;
            OrderManager.Register(order);
            return order;
        }

        public Order SellStopLimitOrder(Instrument instrument, double qty, double stopPx, double price, string text = "")
        {
            Order order = new Order(this.method_1(instrument), this.Portfolio, instrument, OrderType.StopLimit, OrderSide.Sell, qty, 0.0, 0.0, TimeInForce.Day, 0, "");
            order.StrategyId = this.Id;
            order.Text = text;
            order.StopPx = stopPx;
            order.Price = price;
            OrderManager.Register(order);
            return order;
        }

        internal virtual void icojrGfcqNm()
        {
            this.Init();
            for (int i = 0; i < this.Strategies.Count; i++)
            {
                this.Strategies[i].icojrGfcqNm();
            }
        }

        internal virtual void vmethod_0(StrategyMode mode)
        {
            this.Status = StrategyStatus.Running;
            this.Mode = mode;
            this.method_9();
            if (this.IsInstance)
            {
                this.OnStrategyStart();
            }
            for (int i = 0; i < this.Strategies.Count; i++)
            {
                this.Strategies[i].vmethod_0(mode);
            }
        }

        internal virtual void vmethod_1()
        {
            this.Status = StrategyStatus.Stopped;
            this.method_10();
            if (this.IsInstance)
            {
                this.OnStrategyStop();
            }
            for (int i = 0; i < this.Strategies.Count; i++)
            {
                this.Strategies[i].vmethod_1();
            }
        }

        internal virtual void vmethod_2(string source, Event ev, Exception exception)
        {
            if (this.IsInstance)
            {
                this.OnException(source, ev, exception);
            }
        }

        internal virtual void vmethod_3(Provider provider)
        {
            if (this.IsInstance)
            {
                this.OnProviderConnected(provider);
            }
        }

        internal virtual void vmethod_4(Provider provider)
        {
            if (this.IsInstance)
            {
                this.OnProviderDisconnected(provider);
            }
        }

        internal virtual void vmethod_5(ProviderError error)
        {
            if (this.IsInstance)
            {
                this.OnProviderError(error);
            }
        }

        internal virtual void vmethod_6(Instrument instrument, IDataProvider provider)
        {
            if (this.IsInstance)
            {
                this.OnInstrumentAdded(instrument, provider);
            }
        }

        internal virtual void vmethod_7(Instrument instrument, IDataProvider provider)
        {
            if (this.IsInstance)
            {
                this.OnInstrumentRemoved(instrument, provider);
            }
        }

        internal virtual void vmethod_8(DateTime dateTime, object data)
        {
            if (this.IsInstance)
            {
                this.OnReminder(dateTime, data);
            }
        }

        internal virtual void vmethod_9(DateTime dateTime, object data)
        {
            if (this.IsInstance)
            {
                this.OnExchangeReminder(dateTime, data);
            }
        }

        internal virtual void vmethod_10(Bid bid)
        {
            if (this.IsInstance)
            {
                this.OnBid(bid);
                List<Stop> list = this.idArray_0[bid.instrumentId];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Stop stop_ = list[i];
                        if (stop_.Connected)
                        {
                            stop_.method_6(bid);
                        }
                    }
                }
            }
        }

        internal virtual void vmethod_11(Ask ask)
        {
            if (this.IsInstance)
            {
                this.OnAsk(ask);
                List<Stop> list = this.idArray_0[ask.instrumentId];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Stop stop_ = list[i];
                        if (stop_.Connected)
                        {
                            stop_.method_7(ask);
                        }
                    }
                }
            }
        }

        internal virtual void vmethod_12(Trade trade)
        {
            if (this.IsInstance)
            {
                this.OnTrade(trade);
                List<Stop> list = this.idArray_0[trade.instrumentId];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Stop stop_ = list[i];
                        if (stop_.Connected)
                        {
                            stop_.method_5(trade);
                        }
                    }
                }
            }
        }

        internal virtual void vmethod_13(Level2Snapshot snapshot)
        {
            if (this.IsInstance)
            {
                this.OnLevel2(snapshot);
            }
        }

        internal virtual void vmethod_14(Level2Update update)
        {
            if (this.IsInstance)
            {
                this.OnLevel2(update);
            }
        }

        internal virtual void vmethod_15(Bar bar)
        {
            if (this.IsInstance)
            {
                this.OnBarOpen(bar);
                List<Stop> list = this.idArray_0[bar.InstrumentId];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Stop stop_ = list[i];
                        if (stop_.Connected)
                        {
                            stop_.method_4(bar);
                        }
                    }
                }
            }
        }

        internal virtual void vmethod_16(Bar bar)
        {
            if (this.IsInstance)
            {
                this.OnBar(bar);
                List<Stop> list = this.idArray_0[bar.InstrumentId];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Stop stop_ = list[i];
                        if (stop_.Connected)
                        {
                            stop_.method_3(bar);
                        }
                    }
                }
            }
        }

        internal virtual void vmethod_17(BarSlice slice)
        {
            if (this.IsInstance)
            {
                this.OnBarSlice(slice);
            }
        }

        internal virtual void vmethod_18(Order order)
        {
            if (this.IsInstance)
            {
                this.OnSendOrder(order);
            }
            Parent?.vmethod_18(order);
        }

        internal virtual void vmethod_19(Order order)
        {
            if (this.IsInstance)
            {
                this.OnPendingNewOrder(order);
            }
            Parent?.vmethod_19(order);
        }

        internal virtual void vmethod_20(Order order)
        {
            if (this.IsInstance)
            {
                this.OnNewOrder(order);
            }
            Parent?.vmethod_20(order);
        }

        internal virtual void vmethod_21(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderStatusChanged(order);
            }
            Parent?.vmethod_21(order);
        }

        internal virtual void vmethod_22(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderFilled(order);
            }
            Parent?.vmethod_22(order);
        }

        internal virtual void vmethod_23(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderPartiallyFilled(order);
            }
            Parent?.vmethod_23(order);
        }

        internal virtual void vmethod_24(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderCancelled(order);
            }
            Parent?.vmethod_24(order);
        }

        internal virtual void vmethod_25(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderReplaced(order);
            }
            Parent?.vmethod_25(order);
        }

        internal virtual void vmethod_26(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderRejected(order);
            }
            Parent?.vmethod_26(order);
        }

        internal virtual void vmethod_27(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderExpired(order);
            }
            Parent?.vmethod_27(order);
        }

        internal virtual void vmethod_28(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderCancelRejected(order);
            }
            Parent?.vmethod_28(order);
        }

        internal virtual void vmethod_29(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderReplaceRejected(order);
            }
            Parent?.vmethod_29(order);
        }

        internal virtual void vmethod_30(Order order)
        {
            if (this.IsInstance)
            {
                this.OnOrderDone(order);
            }
            Parent?.vmethod_30(order);
        }

        internal virtual void vmethod_31(ExecutionReport report)
        {
            if (this.IsInstance)
            {
                this.OnExecutionReport(report);
            }
        }

        internal virtual void vmethod_32(OnFill fill)
        {
            if (this.IsInstance)
            {
                this.OnFill(fill.Fill);
            }
        }

        internal virtual void vmethod_33(OnTransaction transaction)
        {
            if (this.IsInstance)
            {
                this.OnTransaction(transaction.Transaction);
            }
        }

        internal virtual void vmethod_34(Position position)
        {
            if (this.IsInstance)
            {
                this.OnPositionOpened(position);
            }
        }

        internal virtual void vmethod_35(Position position)
        {
            if (this.IsInstance)
            {
                this.OnPositionClosed(position);
                List<Stop> list = this.idArray_0[position.Instrument.Id];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].position == position)
                        {
                            list[i].Cancel();
                        }
                    }
                }
            }
        }

        internal virtual void vmethod_36(Position position)
        {
            if (this.IsInstance)
            {
                this.OnPositionChanged(position);
            }
        }

        internal virtual void OnStopStatusChanged_(Stop stop)
        {
            if (this.IsInstance)
            {
                StopStatus status = stop.status;
                if (status != StopStatus.Executed)
                {
                    if (status == StopStatus.Canceled)
                    {
                        this.OnStopCancelled(stop);
                    }
                }
                else
                {
                    this.OnStopExecuted(stop);
                }
                this.OnStopStatusChanged(stop);
                this.list_0.Remove(stop);
                this.idArray_0[stop.instrument.Id].Remove(stop);
            }
        }

        internal virtual void vmethod_37(AccountReport report)
        {
            if (this.IsInstance && this.Portfolio != null && this.Portfolio.Id == report.PortfolioId)
            {
                this.OnAccountReport(report);
            }
        }

        internal virtual void vmethod_38(OnPropertyChanged onPropertyChanged)
        {
            if (this.IsInstance)
            {
                this.OnPropertyChanged(onPropertyChanged);
            }
        }

        protected virtual void OnStrategyStart()
        {
        }

        protected virtual void OnStrategyStop()
        {
        }

        protected virtual void OnException(string source, Event ev, Exception exception)
        {
        }

        protected virtual void OnProviderConnected(Provider provider)
        {
        }

        protected virtual void OnProviderDisconnected(Provider provider)
        {
        }

        protected virtual void OnProviderError(ProviderError error)
        {
        }

        protected virtual void OnInstrumentAdded(Instrument instrument, IDataProvider provider = null)
        {
        }

        protected virtual void OnInstrumentRemoved(Instrument instrument, IDataProvider provider = null)
        {
        }

        protected virtual void OnReminder(DateTime dateTime, object data)
        {
        }

        protected virtual void OnExchangeReminder(DateTime dateTime, object data)
        {
        }

        protected virtual void OnBid(Bid bid)
        {
        }

        protected virtual void OnAsk(Ask ask)
        {
        }

        protected virtual void OnTrade(Trade trade)
        {
        }

        protected virtual void OnLevel2(Level2Snapshot snapshot)
        {
        }

        protected virtual void OnLevel2(Level2Update update)
        {
        }

        protected virtual void OnBarOpen(Bar bar)
        {
        }

        protected virtual void OnBar(Bar bar)
        {
        }

        protected virtual void OnBarSlice(BarSlice slice)
        {
        }

        protected virtual void OnSendOrder(Order order)
        {
        }

        protected virtual void OnPendingNewOrder(Order order)
        {
        }

        protected virtual void OnNewOrder(Order order)
        {
        }

        protected virtual void OnOrderStatusChanged(Order order)
        {
        }

        protected virtual void OnOrderFilled(Order order)
        {
        }

        protected virtual void OnOrderPartiallyFilled(Order order)
        {
        }

        protected virtual void OnOrderCancelled(Order order)
        {
        }

        protected virtual void OnOrderReplaced(Order order)
        {
        }

        protected virtual void OnOrderRejected(Order order)
        {
        }

        protected virtual void OnOrderExpired(Order order)
        {
        }

        protected virtual void OnOrderCancelRejected(Order order)
        {
        }

        protected virtual void OnOrderReplaceRejected(Order order)
        {
        }

        protected virtual void OnOrderDone(Order order)
        {
        }

        protected virtual void OnExecutionReport(ExecutionReport report)
        {
        }

        protected virtual void OnFill(Fill fill)
        {
        }

        protected virtual void OnTransaction(Transaction transaction)
        {
        }

        protected virtual void OnPositionOpened(Position position)
        {
        }

        protected virtual void OnPositionClosed(Position position)
        {
        }

        protected virtual void OnPositionChanged(Position position)
        {
        }

        protected virtual void OnStopExecuted(Stop stop)
        {
        }

        protected virtual void OnStopCancelled(Stop stop)
        {
        }

        protected virtual void OnStopStatusChanged(Stop stop)
        {
        }

        protected virtual void OnAccountReport(AccountReport report)
        {
        }

        protected virtual void OnPropertyChanged(OnPropertyChanged onPropertyChanged)
        {
        }

        public Strategy GetRootStrategy()
        {
            if (this.Parent != null)
            {
                return this.Parent.GetRootStrategy();
            }
            return this;
        }

        public object GetParameter(string name)
        {
            return this.parameterHelper_0.GetStrategyParameter(name, this);
        }

        public void SetParameter(string name, object value)
        {
            this.parameterHelper_0.SetStrategyParameter(name, this, value);
        }

        public ParameterList GetParameters()
        {
            return this.parameterHelper_0.GetStrategyParameters(this.Name, this);
        }

        public bool ExecuteMethod(string methodName)
        {
            bool result = false;
            MethodInfo[] methods = base.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo methodInfo = methods[i];
                if (methodInfo.GetCustomAttributes(typeof(StrategyMethodAttribute), true).Length != 0 && methodInfo.GetParameters().Length == 0 && methodInfo.Name == methodName)
                {
                    methodInfo.Invoke(this, null);
                    result = true;
                    return result;
                }
            }
            return result;
        }

        internal virtual void vmethod_39(Command command)
        {
        }

        internal virtual void vmethod_40(string command)
        {
            if (this.IsInstance)
            {
                this.OnUserCommand(command);
            }
        }

        internal virtual void vmethod_41(Event ev)
        {
            if (this.IsInstance)
            {
                this.OnUserEvent(ev);
            }
            for (int i = 0; i < this.Strategies.Count; i++)
            {
                this.Strategies[i].vmethod_41(ev);
            }
        }

        internal virtual void FfoznHaUc5(Parameter oldParameter, Parameter newParameter)
        {
            if (this.IsInstance)
            {
                this.OnParameterChanged(oldParameter, newParameter);
            }
        }

        protected virtual void OnUserCommand(string command)
        {
        }

        protected virtual void OnUserEvent(Event ev)
        {
        }

        protected virtual void OnParameterChanged(Parameter oldParameter, Parameter newParameter)
        {
        }

        internal bool bool_1;

        internal IDataProvider dataProvider;

        internal IExecutionProvider executionProvider;

        internal SubscriptionList subscriptionList_1;

        internal List<Stop> list_0;

        internal IdArray<List<Stop>> idArray_0;

        private ParameterHelper parameterHelper_0;
    }

    public class SellSideStrategy : Strategy, IDataProvider, IExecutionProvider
    {
        byte IProvider.Id
        {
            get
            {
                return (byte)Id;
            }
            set
            {
                Id = value;
            }
        }

        public virtual int AlgoId { get; } = -1;

        [ReadOnly(true), Parameter, Category("Information")]
        public override string Type => nameof(SellSideStrategy);

        public int DataRouteId { get; set; }

        public int ExecutionRouteId { get; set; }

        public new ProviderStatus Status {get;set;}

        public bool IsConnecting { get; } = false;

        public bool IsConnected { get; } = true;

        public bool IsDisconnecting { get; } = false;

        public bool IsDisconnected { get; } = false;

        public SellSideStrategy(Framework framework, string name): base(framework, name)
        {
        }

        public virtual void Send(ExecutionCommand command)
        {
            switch (command.Type)
            {
                case ExecutionCommandType.Send:
                    this.OnSendCommand(command);
                    return;
                case ExecutionCommandType.Cancel:
                    this.OnCancelCommand(command);
                    return;
                case ExecutionCommandType.Replace:
                    this.OnReplaceCommand(command);
                    return;
                default:
                    return;
            }
        }

        public virtual void Connect()
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Connect)}");
            Status = ProviderStatus.Connected;
        }

        public virtual bool Connect(int timeout)
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Connect)}");
            long ticks = DateTime.Now.Ticks;
            Connect();
            while (!IsConnected)
            {
                Thread.Sleep(1);
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds >= timeout)
                {
                    Console.WriteLine($"SellSideStrategy::Connect timed out : {Name}");
                    return false;
                }
            }
            return true;
        }

        public virtual void Disconnect()
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Disconnect)}");
            Status = ProviderStatus.Disconnected;
        }

        public virtual void EmitExecutionReport(ExecutionReport report)
        {
            EventManager.OnEvent(report);
        }

        public virtual void EmitBid(Bid bid)
        {
            EventManager.OnEvent(new Bid(bid){providerId = (byte)Id});
        }

        public virtual void EmitAsk(Ask ask)
        {
            EventManager.OnEvent(new Ask(ask){providerId = (byte)Id});
        }

        public virtual void EmitTrade(Trade trade)
        {
            EventManager.OnEvent(new Trade(trade){providerId = (byte)Id});
        }

        public virtual void EmitBar(Bar bar)
        {
            EventManager.OnEvent(bar);
        }

        public virtual void EmitLevel2Snapshot(Level2Snapshot snapshot)
        {
            EventManager.OnEvent(new Level2Snapshot(snapshot){ProviderId = (byte)Id});
        }

        public virtual void EmitBid(DateTime dateTime, int instrumentId, double price, int size)
        {
            EventManager.OnEvent(new Bid(dateTime, (byte)this.Id, instrumentId, price, size));
        }

        public virtual void EmitAsk(DateTime dateTime, int instrumentId, double price, int size)
        {
            EventManager.OnEvent(new Ask(dateTime, (byte)this.Id, instrumentId, price, size));
        }

        public virtual void EmitTrade(DateTime dateTime, int instrumentId, double price, int size)
        {
            EventManager.OnEvent(new Trade(dateTime, (byte)this.Id, instrumentId, price, size));
        }

        public virtual void Subscribe(Instrument instrument)
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Subscribe)} {instrument}");
            OnSubscribe(instrument);
        }

        public virtual void Subscribe(InstrumentList instruments)
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Subscribe)} {instruments}");
            OnSubscribe(instruments);
        }

        public virtual void Unsubscribe(Instrument instrument)
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Unsubscribe)} {instrument}");
            OnUnsubscribe(instrument);
        }

        public virtual void Unsubscribe(InstrumentList instruments)
        {
            Console.WriteLine($"{nameof(SellSideStrategy)}::{nameof(Unsubscribe)} {instruments}");
            OnUnsubscribe(instruments);
        }

        public virtual void OnSendCommand(ExecutionCommand command)
        {
        }

        public virtual void OnCancelCommand(ExecutionCommand command)
        {
        }

        public virtual void OnReplaceCommand(ExecutionCommand command)
        {
        }

        protected virtual void OnSubscribe(InstrumentList instruments)
        {
        }

        protected virtual void OnSubscribe(Instrument instrument)
        {
        }

        protected virtual void OnUnsubscribe(InstrumentList instruments)
        {
        }

        protected virtual void OnUnsubscribe(Instrument instrument)
        {
        }
    }

    public class InstrumentStrategy : Strategy
    {
        [Category("Information"), Parameter, ReadOnly(true)]
        public override string Type => nameof(InstrumentStrategy);

        public Instrument Instrument { get; private set; }

        public Position Position => Portfolio.GetPosition(Instrument);

        public InstrumentStrategy(Framework framework, string name) : base(framework, name)
        {
            IsInstance = false;
        }

        public override void Add(Instrument instrument, IDataProvider provider = null)
        {
            if (IsInstance)
            {
                Console.WriteLine(string.Concat("InstrumentStrategy::Add Can not add instrument to strategy instance ", Name, " ", instrument));
                return;
            }
            var instrumentStrategy_ = (InstrumentStrategy)base.method_6(Name + " (" + instrument.Symbol + ")");
            instrumentStrategy_.Instrument = instrument;
            instrumentStrategy_.method_3(instrument, provider);
            Add(instrumentStrategy_);
            if (!Subscriptions.Contains(instrument, provider))
            {
                Subscription subscription = new Subscription(instrument, provider, -1);
                Subscriptions.Add(subscription);
            }
        }

        public bool HasPosition() => HasPosition(Instrument);

        public bool HasPosition(PositionSide side, double qty) => HasPosition(Instrument, side, qty);

        public bool HasLongPosition() => HasLongPosition(Instrument);

        public bool HasLongPosition(double qty) => HasLongPosition(Instrument, qty);

        public bool HasShortPosition() => HasShortPosition(Instrument);

        public bool HasShortPosition(double qty) => HasShortPosition(Instrument, qty);
    }

    public class SellSideInstrumentStrategy : SellSideStrategy
    {
        public Instrument Instrument { get; private set; }

        [Parameter, Category("Information"), ReadOnly(true)]
        public override string Type => nameof(SellSideInstrumentStrategy);

        public SellSideInstrumentStrategy(Framework framework, string name): base(framework, name)
        {
            IsInstance = false;
        }

        public override void Add(Instrument instrument, IDataProvider provider = null)
        {
            if (IsInstance)
            {
                Console.WriteLine(string.Concat("SellSideInstrumentStrategy::Add Can not add instrument to strategy instance ", Name, " ", instrument));
                return;
            }
            var sellSideInstrumentStrategy_ = (SellSideInstrumentStrategy)base.method_6(Name + " (" + instrument.Symbol + ")");
            sellSideInstrumentStrategy_.Instrument = instrument;
            if (this.bool_1)
            {
                sellSideInstrumentStrategy_.Init();
            }
            sellSideInstrumentStrategy_.method_3(instrument, provider);
            this.idArray_1[instrument.Id] = sellSideInstrumentStrategy_;
            this.Add(sellSideInstrumentStrategy_);
        }

        public override void Send(ExecutionCommand command)
        {
            var sellSideInstrumentStrategy_ = this.idArray_1[command.Order.Instrument.Id];
            switch (command.Type)
            {
                case ExecutionCommandType.Send:
                    sellSideInstrumentStrategy_.OnSendCommand(command);
                    return;
                case ExecutionCommandType.Cancel:
                    sellSideInstrumentStrategy_.OnCancelCommand(command);
                    return;
                case ExecutionCommandType.Replace:
                    sellSideInstrumentStrategy_.OnReplaceCommand(command);
                    return;
                default:
                    return;
            }
        }

        public override void EmitBid(Bid bid)
        {
            EventManager.OnEvent(new Bid(bid){providerId = IsInstance ? (byte)Parent.Id : (byte)Id});
        }

        public override void EmitAsk(Ask ask)
        {
            EventManager.OnEvent(new Ask(ask){providerId = IsInstance ? (byte)Parent.Id : (byte)Id});
        }

        public override void EmitTrade(Trade trade)
        {
            EventManager.OnEvent(new Trade(trade){providerId = IsInstance ? (byte)Parent.Id : (byte)Id});
        }

        public override void EmitBid(DateTime dateTime, int instrumentId, double price, int size)
        {
            EventManager.OnEvent(new Bid(dateTime, IsInstance ? (byte)Parent.Id : (byte)Id, instrumentId, price, size));
        }

        public override void EmitAsk(DateTime dateTime, int instrumentId, double price, int size)
        {
            EventManager.OnEvent(new Ask(dateTime, IsInstance ? (byte)Parent.Id : (byte)Id, instrumentId, price, size));
        }

        public override void EmitTrade(DateTime dateTime, int instrumentId, double price, int size)
        {
            EventManager.OnEvent(new Trade(dateTime, IsInstance ? (byte)Parent.Id : (byte)Id, instrumentId, price, size));
        }

        internal IdArray<SellSideInstrumentStrategy> idArray_1 = new IdArray<SellSideInstrumentStrategy>();
    }
}
