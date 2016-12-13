using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using System.Linq;

using FastQuant.Quant;

namespace FastQuant.Optimization
{
    public delegate void OptimizationProgressEventHandler(object sender, OptimizationProgressEventArgs progress);

    public class GeneticOptimizer : Optimizer
    {
        public GeneticOptimizer(Framework framework) : base(framework)
        {
        }
    }

    public abstract class BaseStochasticOptimizer : Optimizer
    {
        protected class RunResult : IEquatable<RunResult>
        {

            public RunResult(Vector vector)
            {
                Vector = vector;
            }

            public RunResult(Vector vector, double objective) : this(vector)
            {
                Objective = objective;
            }

            public bool Equals(RunResult other)
            {
                if (other?.Vector == null || Vector == null)
                    return false;
                if (other.Vector.NRows != Vector.NRows)
                    return false;
                for (var i = 0; i < Vector.NRows; i++)
                {
                    if (Vector[i] != other.Vector[i])
                        return false;
                }
                return true;
            }

            public double Objective { get; set; }

            public Vector Vector { get; set; }
        }

        protected class ParameterValues
        {
            public string Name { get; set; }

            public List<object> Values { get; set; }=new List<object>();
        }

        protected List<ParameterValues> PVList { get; } = new List<ParameterValues>();

        protected List<RunResult> Results { get; } = new List<RunResult>();

        public BaseStochasticOptimizer(Framework framework):base(framework)
        {
        }

        protected void AddResult(RunResult result)
        {
            if (!Results.Any(n => n.Equals(result)))
                Results.Add(result);
        }

    }

    public class OptimizationManager
    {
        public OptimizationManager(Framework framework)
        {
            this.framework = framework;
            this.optimizer = new MulticoreOptimizer(framework);
            Add(this.optimizer);
            Add(new GeneticMulticoreOptimizer(framework));
        }

        public void Add(Optimizer optimizer)
        {
            Optimizers.Add(optimizer);
        }

        //private string CcQsHkys28()
        //{
        //    string optimizationManagerFileName = this.framework.Configuration.OptimizationManagerFileName;
        //    if (optimizationManagerFileName == null)
        //    {
        //        optimizationManagerFileName = Configuration.DefaultConfiguaration().OptimizationManagerFileName;
        //    }
        //    return optimizationManagerFileName;
        //}

        public virtual void OnOptimizationProgress()
        {
        }

        public virtual void OnOptimizationStart()
        {
        }

        public virtual void OnOptimizationStop()
        {
        }

        public OptimizationParameterSet Optimize(Strategy strategy, OptimizationUniverse universe = null)
        {
            return this.optimizer?.Optimize(strategy, universe);
        }

        public OptimizationParameterSet Optimize(Scenario scenario, OptimizationUniverse universe = null)
        {
            return this.optimizer?.Optimize(scenario);
        }

        public void Remove(Optimizer optimizer)
        {
            Optimizers.Remove(optimizer);
        }

        public void SaveSettings()
        {
            throw new NotImplementedException();
        }

        private OptimizerSettingsList Y28sGt3f4m()
        {
            OptimizerSettingsList optimizerSettingsList = new OptimizerSettingsList();
            foreach (var current in this.Optimizers)
            {
                var properties = current.Settings.GetProperties();
                properties.OptimizerName = current.Name;
                optimizerSettingsList.Items.Add(properties);
            }
            return optimizerSettingsList;
        }

        public Optimizer Optimizer
        {
            get
            {
                return this.optimizer;
            }
            set
            {
                this.optimizer = value;
                this.optimizer.Settings.IsDefaultOptimizer = true;
                this.optimizer.Framework = this.framework;
            }
        }

        public List<Optimizer> Optimizers { get; } = new List<Optimizer>();

        private Optimizer optimizer;

        private Framework framework;
    }


    public class GeneticMulticoreOptimizer : Optimizer
    {
        public GeneticMulticoreOptimizer(Framework framework) : base(framework)
        {
        }
    }
}