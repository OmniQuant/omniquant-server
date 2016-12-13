using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FastQuant.Optimization;

namespace FastQuant.Experimental.Strategy
{
    public class StrategyList : IEnumerable<Strategy>
    {
        public int Count => this.strategiesByIndex.Count;

        public bool Contains(Strategy strategy) => !Contains(strategy.Id);

        public bool Contains(int id) => this.strategiesById[id] == null;

        public void Add(Strategy strategy)
        {
            if (!Contains(strategy.Id))
                Console.WriteLine($"StrategyList::Add strategy with Id = {strategy.Id} is already in the list");
            else
            {
                this.strategiesByIndex.Add(strategy);
                this.strategiesById.Add(strategy.Id, strategy);
            }
        }

        public void Remove(Strategy strategy)
        {
            this.strategiesByIndex.Remove(strategy);
            this.strategiesById.Remove(strategy.Id);
        }

        public Strategy GetByIndex(int index) => this.strategiesByIndex[index];

        public Strategy GetById(int id) => this.strategiesById[id];

        public Strategy this[int index]
        {
            get
            {
                return this.strategiesByIndex[index];
            }
            set
            {
                this.strategiesByIndex[index] = value;
            }
        }

        public void Clear()
        {
            this.strategiesByIndex.Clear();
            this.strategiesById.Clear();
        }

        public IEnumerator<Strategy> GetEnumerator() => this.strategiesByIndex.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly IdArray<Strategy> strategiesById = new IdArray<Strategy>();

        private readonly List<Strategy> strategiesByIndex = new List<Strategy>();
    }

    public class ScenarioManager
    {
        public Scenario Scenario
        {
            get
            {
                return this.scenario;
            }
            set
            {
                this.scenario = value;
            }
        }

        public ScenarioManager(Framework framework)
        {
            this.framework = framework;
        }

        public void Start()
        {
            if (this.scenario == null) return;
            this.framework.Clear();
            this.thread = new Thread(() => this.scenario.Run())
            {
                Name = "Scenario Manager Thread",
                IsBackground = true
            };
            this.thread.Start();
        }

        public void Stop()
        {
            this.framework.ExperimentalStrategyManager.Stop();
        }

        private Framework framework;

        private Scenario scenario;

        private Thread thread;
    }

    public class Scenario
    {
        public Clock Clock => this.framework.Clock;

        public InstrumentManager InstrumentManager => this.framework.InstrumentManager;

        public DataManager DataManager => this.framework.DataManager;

        public ProviderManager ProviderManager => this.framework.ProviderManager;

        public OrderManager OrderManager => this.framework.OrderManager;

        public IDataSimulator DataSimulator => this.framework.ProviderManager.DataSimulator;

        public IExecutionSimulator ExecutionSimulator => this.framework.ProviderManager.ExecutionSimulator;

        public BarFactory BarFactory => this.framework.EventManager.BarFactory;

        public EventManager EventManager => this.framework.EventManager;

        public StrategyManager StrategyManager => this.framework.ExperimentalStrategyManager;

        public StatisticsManager StatisticsManager => this.framework.StatisticsManager;

        public GroupManager GroupManager => this.framework.GroupManager;

        public AccountDataManager AccountDataManager => this.framework.AccountDataManager;

        public DataFileManager DataFileManager => this.framework.DataFileManager;

        public OptimizationManager OptimizatioManager => this.framework.OptimizationManager;

        public Optimizer Optimizer => this.framework.OptimizationManager.Optimizer;

        public Scenario(Framework framework)
        {
            this.framework = framework;
        }

        public void Start(Strategy strategy, StrategyMode mode)
        {
            Console.WriteLine($"{DateTime.Now} Scenario::StartStrategy {strategy.Name} in {mode}");
            this.framework.ExperimentalStrategyManager.Start(strategy, mode);
            while (this.framework.ExperimentalStrategyManager.Status != StrategyStatus.Stopped)
                Thread.Sleep(10);
            Console.WriteLine($"{DateTime.Now} Scenario::StartStrategy Done");
        }

        public void StartBacktest(Strategy strategy) => Start(strategy, StrategyMode.Backtest);

        public void StartPaper(Strategy strategy) => Start(strategy, StrategyMode.Paper);

        public void StartLive(Strategy strategy) => Start(strategy, StrategyMode.Live);

        public virtual void Run()
        {
        }

        public OptimizationParameterSet Optimize(FastQuant.Strategy strategy, OptimizationUniverse universe = null)
        {
            return this.framework.OptimizationManager.Optimize(strategy, universe);
        }

        protected internal Framework framework;
    }
}