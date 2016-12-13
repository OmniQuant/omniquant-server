using System;
using System.Collections.Generic;

namespace FastQuant.Experimental.Strategy
{
    public class StrategyManager
    {
        public Strategy Strategy => this.strategy__0;

        public StrategyStatus Status { get; internal set; }

        public StrategyMode Mode { get; internal set; }

        public StrategyPersistence Persistence { get; set; }

        public StrategyManager(Framework framework)
        {
            Mode = StrategyMode.Backtest;
            Status = StrategyStatus.Stopped;
            this.int_0 = 101;
            this.list_0 = new List<Strategy>();
            this.idArray_0 = new IdArray<Strategy>();
            this.idArray_1 = new IdArray<Strategy>();
            this.idArray_2 = new IdArray<List<Strategy>>();
            this.idArray_3 = new IdArray<IdArray<List<Strategy>>>();
            this.framework = framework;
        }

        internal void method_0(Strategy strategy__1)
        {
            this.list_0.Add(strategy__1);
            this.idArray_0[strategy__1.Id] = strategy__1;
            this.idArray_1[strategy__1.Portfolio.int_0] = strategy__1;
        }

        internal void method_1(Strategy strategy__1, Subscription subscription_0)
        {
            if (this.idArray_3[subscription_0.int_2] == null)
            {
                this.idArray_3[subscription_0.int_2] = new IdArray<List<Strategy>>();
            }
            if (this.idArray_3[subscription_0.int_2][subscription_0.int_3] == null)
            {
                this.idArray_3[subscription_0.int_2][subscription_0.int_3] = new List<Strategy>();
            }
            if (!this.idArray_3[subscription_0.int_2][subscription_0.int_3].Contains(strategy__1))
            {
                this.idArray_3[subscription_0.int_2][subscription_0.int_3].Add(strategy__1);
            }
            if (this.idArray_2[subscription_0.int_2] == null)
            {
                this.idArray_2[subscription_0.int_2] = new List<Strategy>();
            }
            if (!this.idArray_2[subscription_0.int_2].Contains(strategy__1))
            {
                this.idArray_2[subscription_0.int_2].Add(strategy__1);
            }
        }

        internal void method_2(Strategy strategy__1, Subscription subscription_0)
        {
            if (this.idArray_3[subscription_0.int_2] == null)
            {
                this.idArray_3[subscription_0.int_2] = new IdArray<List<Strategy>>();
            }
            if (this.idArray_3[subscription_0.int_2][subscription_0.int_3] == null)
            {
                this.idArray_3[subscription_0.int_2][subscription_0.int_3] = new List<Strateg_>();
            }
            if (this.idArray_3[subscription_0.int_2][subscription_0.int_3].Contains(strategy__1))
            {
                this.idArray_3[subscription_0.int_2][subscription_0.int_3].Remove(strategy__1);
            }
            if (this.idArray_2[subscription_0.int_2] == null)
            {
                this.idArray_2[subscription_0.int_2] = new List<Strategy_>();
            }
            if (this.idArray_2[subscription_0.int_2].Contains(strategy__1))
            {
                this.idArray_2[subscription_0.int_2].Remove(strategy__1);
            }
        }

        public int GetNextId()
        {
            int num;
            lock (this)
            {
                num = this.int_0;
                this.int_0 = num + 1;
                num = num;
            }
            return num;
        }

        public void Start(Strategy strategy, StrategyMode mode)
        {
            if (this.Status != StrategyStatus.Running)
            {
                this.strategy__0 = strategy;
                this.Mode = mode;
                this.method_39(StrategyStatusType.Started);
                this.method_40();
                if (this.Persistence != StrategyPersistence.Full)
                {
                    if (this.Persistence != StrategyPersistence.Save)
                    {
                        this.framework.orderManager_0.bool_0 = false;
                        goto IL_74;
                    }
                }
                this.framework.orderServer_0.SeriesName = strategy.Name;
                this.framework.orderManager_0.bool_0 = true;
                IL_74:
                if (this.Persistence == StrategyPersistence.Full || this.Persistence == StrategyPersistence.Load)
                {
                    this.framework.portfolioManager_0.Load(strategy.Name);
                    this.framework.orderManager_0.Load(strategy.Name, -1);
                }
                this.Status = StrategyStatus.Running;
                if (mode == StrategyMode.Backtest && !this.framework.bool_6)
                {
                    this.framework.providerManager_0.idataSimulator_0.RunOnSubscribe = false;
                }
                strategy.icojrGfcqNm();
                if ((this.Persistence == StrategyPersistence.Full || this.Persistence == StrategyPersistence.Save) && !strategy.Portfolio.iEcAijqtwI)
                {
                    this.framework.portfolioManager_0.Save(strategy.Portfolio);
                }
                strategy.vmethod_0(mode);
                if (mode == StrategyMode.Backtest && !this.framework.bool_6)
                {
                    this.framework.providerManager_0.idataSimulator_0.Run();
                    this.framework.providerManager_0.idataSimulator_0.RunOnSubscribe = true;
                }
            }
        }

        public void Stop()
        {
            if (this.Status != StrategyStatus.Stopped)
            {
                this.strategy__0.vmethod_1();
                if (this.Mode == StrategyMode.Backtest)
                {
                    this.framework.providerManager_0.idataSimulator_0.Disconnect();
                    this.framework.providerManager_0.iexecutionSimulator_0.Disconnect();
                }
                this.Status = StrategyStatus.Stopped;
                this.method_39(StrategyStatusType.Stopped);
            }
        }

        internal void method_3(string string_0, Event event_0, Exception exception_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                for (int i = 0; i < this.list_0.Count; i++)
                {
                    this.list_0[i].vmethod_2(string_0, event_0, exception_0);
                }
            }
        }

        internal void method_4(Provider provider_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                for (int i = 0; i < this.list_0.Count; i++)
                {
                    this.list_0[i].vmethod_3(provider_0);
                }
            }
        }

        internal void method_5(Provider provider_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                for (int i = 0; i < this.list_0.Count; i++)
                {
                    this.list_0[i].vmethod_4(provider_0);
                }
            }
        }

        internal void method_6(ProviderError providerError_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                for (int i = 0; i < this.list_0.Count; i++)
                {
                    this.list_0[i].vmethod_5(providerError_0);
                }
            }
        }

        internal void method_7(Bid bid_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy> list;
                if (this.Mode == StrategyMode.Backtest)
                {
                    list = this.idArray_2[bid_0.instrumentId];
                }
                else
                {
                    list = this.idArray_3[bid_0.instrumentId][(int)bid_0.providerId];
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_10(bid_0);
                }
            }
        }

        internal void method_8(Ask ask_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy> list;
                if (this.Mode == StrategyMode.Backtest)
                {
                    list = this.idArray_2[ask_0.instrumentId];
                }
                else
                {
                    list = this.idArray_3[ask_0.instrumentId][(int)ask_0.providerId];
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_11(ask_0);
                }
            }
        }

        internal void method_9(Trade trade_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy> list;
                if (this.Mode == StrategyMode.Backtest)
                {
                    list = this.idArray_2[trade_0.instrumentId];
                }
                else
                {
                    list = this.idArray_3[trade_0.instrumentId][(int)trade_0.providerId];
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_12(trade_0);
                }
            }
        }

        internal void method_10(Level2Snapshot level2Snapshot_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy> list;
                if (this.Mode == StrategyMode.Backtest)
                {
                    list = this.idArray_2[level2Snapshot_0.int_0];
                }
                else
                {
                    list = this.idArray_3[level2Snapshot_0.int_0][(int)level2Snapshot_0.byte_0];
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_13(level2Snapshot_0);
                }
            }
        }

        internal void method_11(Level2Update level2Update_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy> list;
                if (this.Mode == StrategyMode.Backtest)
                {
                    list = this.idArray_2[level2Update_0.int_0];
                }
                else
                {
                    list = this.idArray_3[level2Update_0.int_0][(int)level2Update_0.byte_0];
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_14(level2Update_0);
                }
            }
        }

        internal void method_12(Bar bar_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy> list = this.idArray_2[bar_0.int_1];
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_15(bar_0);
                }
            }
        }

        internal void method_13(Bar bar_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                List<Strategy_> list = this.idArray_2[bar_0.int_1];
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].vmethod_16(bar_0);
                }
            }
        }

        internal void method_14(BarSlice barSlice_0)
        {
            if (this.strategy__0 != null && this.Status == StrategyStatus.Running)
            {
                for (int i = 0; i < this.list_0.Count; i++)
                {
                    this.list_0[i].vmethod_17(barSlice_0);
                }
            }
        }

        internal void method_15(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_18(order_0);
            }
        }

        internal void method_16(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_19(order_0);
            }
        }

        internal void method_17(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_20(order_0);
            }
        }

        internal void method_18(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_21(order_0);
            }
        }

        internal void method_19(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_22(order_0);
            }
        }

        internal void method_20(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_23(order_0);
            }
        }

        internal void method_21(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_24(order_0);
            }
        }

        internal void method_22(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_25(order_0);
            }
        }

        internal void method_23(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_26(order_0);
            }
        }

        internal void method_24(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_27(order_0);
            }
        }

        internal void method_25(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_28(order_0);
            }
        }

        internal void method_26(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_29(order_0);
            }
        }

        internal void method_27(Order order_0)
        {
            if (order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[order_0.strategyId].vmethod_30(order_0);
            }
        }

        internal void method_28(ExecutionReport executionReport_0)
        {
            if (executionReport_0.order_0.strategyId == -1)
            {
                return;
            }
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_0[executionReport_0.order_0.strategyId].vmethod_31(executionReport_0);
            }
        }

        internal void method_29(OnFill onFill_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_1[onFill_0.portfolio_0.int_0].vmethod_32(onFill_0);
            }
        }

        internal void method_30(OnTransaction onTransaction_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_1[onTransaction_0.portfolio_0.int_0].vmethod_33(onTransaction_0);
            }
        }

        internal void method_31(Portfolio portfolio_0, Position position_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_1[portfolio_0.int_0].vmethod_34(position_0);
            }
        }

        internal void method_32(Portfolio portfolio_0, Position position_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_1[portfolio_0.int_0].vmethod_35(position_0);
            }
        }

        internal void method_33(Portfolio portfolio_0, Position position_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_1[portfolio_0.int_0].vmethod_36(position_0);
            }
        }

        internal void method_34(Portfolio portfolio_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running && (this.Persistence == StrategyPersistence.Full || this.Persistence == StrategyPersistence.Save) && !portfolio_0.iEcAijqtwI)
            {
                this.framework.portfolioManager_0.Save(this.strategy__0.Portfolio);
            }
        }

        internal void method_35(AccountReport accountReport_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.idArray_1[accountReport_0.int_4].vmethod_37(accountReport_0);
            }
        }

        internal void method_36(OnPropertyChanged onPropertyChanged_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                for (int i = 0; i < this.list_0.Count; i++)
                {
                    this.list_0[i].vmethod_38(onPropertyChanged_0);
                }
            }
        }

        internal StrategyStatusInfo method_37()
        {
            StrategyStatusType strategyStatusType_ = (this.Status == StrategyStatus.Running) ? StrategyStatusType.Started : StrategyStatusType.Stopped;
            return this.method_38(strategyStatusType_);
        }

        private StrategyStatusInfo method_38(StrategyStatusType strategyStatusType_0)
        {
            return new StrategyStatusInfo(this.method_41("SolutionStatus", strategyStatusType_0).dateTime, strategyStatusType_0)
            {
                Solution = ((this.strategy__0.Name == null) ? "Solution" : this.strategy__0.Name),
                Mode = this.Mode.ToString()
            };
        }

        private void method_39(StrategyStatusType strategyStatusType_0)
        {
            Group group = this.method_41("SolutionStatus", strategyStatusType_0);
            StrategyStatusInfo obj = this.method_38(strategyStatusType_0);
            this.framework.eventServer_0.OnLog(new GroupEvent(obj, group));
        }

        internal void method_40()
        {
            Group group_ = this.method_41("StrategyParameters", (this.strategy__0.Status == StrategyStatus.Running) ? StrategyStatusType.Started : StrategyStatusType.Stopped);
            this.method_42(this.strategy__0, "", group_);
        }

        private Group method_41(string string_0, StrategyStatusType strategyStatusType_0)
        {
            Group group = this.method_46(string_0);
            if (group == null)
            {
                group = new Group(string_0);
                if (this.framework.Mode == FrameworkMode.Realtime)
                {
                    group.DateTime = DateTime.Now;
                }
                else if (strategyStatusType_0 == StrategyStatusType.Stopped)
                {
                    group.DateTime = this.framework.Clock.DateTime;
                }
                this.framework.groupManager_0.Add(group);
            }
            else
            {
                if (this.framework.Mode == FrameworkMode.Realtime)
                {
                    group.DateTime = DateTime.Now;
                }
                else if (strategyStatusType_0 == StrategyStatusType.Stopped)
                {
                    group.DateTime = this.framework.Clock.DateTime;
                }
                this.framework.eventServer_0.OnLog(group);
            }
            return group;
        }

        internal void method_42(Strategy_ strategy__1, string string_0, Group group_0)
        {
            string_0 += ((string_0 == "") ? strategy__1.string_0 : ("\\" + strategy__1.string_0));
            ParameterList parameters = strategy__1.GetParameters();
            parameters.name = string_0;
            parameters.dateTime = group_0.dateTime;
            this.framework.eventServer_0.OnLog(new GroupEvent(parameters, group_0));
            foreach (Strategy_ current in strategy__1.strategyList__0)
            {
                this.method_42(current, string_0, group_0);
            }
        }

        private void method_43(Strategy_ strategy__1, string string_0, object object_0)
        {
            object parameter = strategy__1.GetParameter(string_0);
            if (parameter != null && !parameter.Equals(object_0))
            {
                strategy__1.SetParameter(string_0, object_0);
                Parameter oldParameter = new Parameter(string_0, parameter);
                Parameter newParameter = new Parameter(string_0, object_0);
                strategy__1.FfoznHaUc5(oldParameter, newParameter);
                Console.WriteLine(string.Concat(new object[]
                {
                    "OnUserCommand: Parameter[",
                    string_0,
                    "] changed from ",
                    parameter,
                    " to ",
                    object_0,
                    " at ",
                    strategy__1.string_0
                }));
            }
            foreach (Strategy_ current in strategy__1.strategyList__0)
            {
                if (strategy__1.GetType() == current.GetType())
                {
                    this.method_43(current, string_0, object_0);
                }
            }
        }

        private void method_44(Strategy_ strategy__1, string string_0)
        {
            if (!strategy__1.ExecuteMethod(string_0))
            {
                Console.WriteLine("OnUserCommand: Method: " + string_0 + " not found");
                return;
            }
            Console.WriteLine("OnUserCommand: Method[" + string_0 + "] executed at " + strategy__1.string_0);
            foreach (Strategy_ current in strategy__1.strategyList__0)
            {
                if (strategy__1.GetType() == current.GetType())
                {
                    this.method_44(current, string_0);
                }
            }
        }

        private Strategy_ method_45(Strategy_ strategy__1, string string_0, string string_1)
        {
            Strategy_ strategy_ = null;
            string_0 += ((string_0 == "") ? strategy__1.string_0 : ("\\" + strategy__1.string_0));
            if (string_0 == string_1)
            {
                return strategy__1;
            }
            foreach (Strategy_ current in strategy__1.strategyList__0)
            {
                strategy_ = this.method_45(current, string_0, string_1);
                if (strategy_ != null)
                {
                    break;
                }
            }
            return strategy_;
        }

        private Group method_46(string string_0)
        {
            foreach (Group current in this.framework.groupManager_0.Groups)
            {
                if (current.Name == string_0)
                {
                    return current;
                }
            }
            return null;
        }

        internal void CtJqelEsAB(Command_ command__0)
        {
        }

        internal void method_47(string string_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                if (string_0.Contains("Parameter"))
                {
                    string[] array = string_0.Split(new char[]
                    {
                        '\t'
                    });
                    if (!(array[0] != "Parameter"))
                    {
                        if (array.Length == 5)
                        {
                            string string_ = array[2];
                            Type type = Type.GetType(array[4]);
                            if (type == null)
                            {
                                Console.WriteLine("OnUserCommand: Unknown type: " + array[4]);
                                return;
                            }
                            object object_;
                            if (type == typeof(DateTime))
                            {
                                object_ = DateTime.Parse(array[3]);
                            }
                            else if (type == typeof(TimeSpan))
                            {
                                object_ = TimeSpan.Parse(array[3]);
                            }
                            else
                            {
                                object_ = Convert.ChangeType(array[3], type);
                            }
                            Strategy_ strategy_ = this.method_45(this.strategy__0, "", array[1]);
                            if (strategy_ != null)
                            {
                                this.method_43(strategy_, string_, object_);
                                strategy_.vmethod_40(string_0);
                            }
                            else
                            {
                                Console.WriteLine("OnUserCommand: Strategy: " + array[1] + " not found");
                            }
                            Group group = this.method_46("StrategyParameters");
                            if (group != null)
                            {
                                this.method_42(this.strategy__0, "", group);
                                return;
                            }
                            return;
                        }
                    }
                    return;
                }
                if (string_0.Contains("Method"))
                {
                    string[] array2 = string_0.Split(new char[]
                    {
                        '\t'
                    });
                    if (!(array2[0] != "Method"))
                    {
                        if (array2.Length == 3)
                        {
                            string string_2 = array2[2];
                            Strategy_ strategy_2 = this.method_45(this.strategy__0, "", array2[1]);
                            if (strategy_2 != null)
                            {
                                this.method_44(strategy_2, string_2);
                                strategy_2.vmethod_40(string_0);
                                return;
                            }
                            Console.WriteLine("OnUserCommand: Strategy: " + array2[1] + " not found");
                            return;
                        }
                    }
                    return;
                }
                this.strategy__0.vmethod_40(string_0);
            }
        }

        internal void method_48(Event event_0)
        {
            if (this.strategy__0 != null && this.strategy__0.Status == StrategyStatus.Running)
            {
                this.strategy__0.vmethod_41(event_0);
            }
        }

        public void Clear()
        {
            this.list_0.Clear();
            this.idArray_0.Clear();
            this.idArray_1.Clear();
            this.idArray_2.Clear();
            this.idArray_3.Clear();
            this.int_0 = 101;
        }

        internal Framework framework;

        internal Strategy strategy__0;

        private int int_0;

        internal List<Strategy> list_0;

        internal IdArray<Strategy> idArray_0;

        internal IdArray<Strategy> idArray_1;

        internal IdArray<List<Strategy>> idArray_2;

        internal IdArray<IdArray<List<Strategy>>> idArray_3;
    }
}