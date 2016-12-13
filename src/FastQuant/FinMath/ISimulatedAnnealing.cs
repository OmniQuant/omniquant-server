using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace FastQuant.FinMath
{
    public class RandomGenerator
    {
        public static double NextNormal(double mean, double stdDev)
        {
            double num2 = Math.Sqrt(-2.0*Math.Log(random.NextDouble()))*Math.Sin(2*Math.PI*random.NextDouble());
            return mean + stdDev*num2;
        }

        public static double NextNormal2(double mean, double stdDev)
        {
            double num;
            double num2;
            double num3;
            while (true)
            {
                num = random.NextDouble();
                if (num > 0.516058551)
                {
                    goto Block_12;
                }
                if (num < 0.107981933)
                {
                    goto IL_230;
                }
                double num4;
                if (num < 0.483941449)
                {
                    num2 = random.NextDouble();
                    num2 = num2 - 1.0 + num2;
                    num3 = ((num2 > 0.0) ? (2.0 - num2) : (-2.0 - num2));
                    if ((1.448242853 - num)*(1.46754004 + Math.Abs(num3)) < 3.307147487)
                    {
                        goto IL_26D;
                    }
                    num4 = num2*num2;
                    if ((num + 1.036467755)*(3.631288474 + num4) < 5.295844968)
                    {
                        goto IL_272;
                    }
                    if (0.591923442 - num < Math.Exp(-(num3*num3 + 0.4515827053)/2.0))
                    {
                        goto IL_276;
                    }
                    if (num + 0.375959516 < Math.Exp(-(num4 + 0.4515827053)/2.0))
                    {
                        break;
                    }
                }
                do
                {
                    num4 = RandomGenerator.random.NextDouble();
                    num = 0.187308492*RandomGenerator.random.NextDouble();
                    num3 = 0.4571828819 - 0.7270572718*num4 - num;
                    if (num3 > 0.0)
                    {
                        num2 = 2.0 + num/num4;
                    }
                    else
                    {
                        num4 = 1.0 - num4;
                        num = 0.187308492 - num;
                        num2 = -(2.0 + num/num4);
                    }
                    if ((num - 0.8853395638 + num4)*(0.2770276848 + num4) + 0.2452635696 < 0.0)
                    {
                        break;
                    }
                } while (num >= num4 + 0.03895759111 || num2*num2 >= 4.0*(0.5029324303 - Math.Log(num4)));
            }
            double num5 = num2;
            goto IL_27D;
            Block_12:
            num5 = 4.132731354*num - 3.132731354;
            goto IL_27D;
            IL_230:
            num2 = 18.52161694*num - 1.0;
            num5 = ((num2 > 0.0) ? (1.0 + num2) : (-1.0 + num2));
            goto IL_27D;
            IL_26D:
            num5 = num3;
            goto IL_27D;
            IL_272:
            num5 = num2;
            goto IL_27D;
            IL_276:
            num5 = num3;
            IL_27D:
            return mean + stdDev*num5;
        }

        private static readonly Random random = new Random();
    }

    public interface IGeneticFunction
    {
        string Name { get; }
    }
    public class MutationTypeConverter : GeneticFunctionTypeConverter
    {
        protected override List<IGeneticFunction> Collection => new List<IGeneticFunction>{new FullMutation()};
    }

    public class Bounds
    {
        public Vector Lower { get; set; }

        public Vector Upper { get; set; }

        public Bounds(Vector lower, Vector upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }

    public abstract class GeneticFunctionTypeConverter : ExpandableObjectConverter
    {
        protected abstract List<IGeneticFunction> Collection { get; }

        public GeneticFunctionTypeConverter()
        {
            this.list_0 = Collection;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(this.list_0);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string) && value is IGeneticFunction)
            {
                return (value as IGeneticFunction).Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value.GetType() == typeof(string)
                ? this.list_0.Where(f => f.Name == this.value).Single()
                : base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        private List<IGeneticFunction> list_0;

        public object value;
    }

    public class CrossoverTypeConverter : GeneticFunctionTypeConverter
    {
        protected override List<IGeneticFunction> Collection => new List<IGeneticFunction> {new BLXCrossover()};
    }

    public interface IGeneticSelection : IGeneticFunction
    {
        List<Chromosome> SelectChromosomes(List<Chromosome> population);
    }

    public class TournamentSelection : IGeneticSelection
    {
        public string Name => "Tournament Selection";

        public List<Chromosome> SelectChromosomes(List<Chromosome> population)
        {
            List<Chromosome> list = new List<Chromosome>();
            for (int i = 0; i < population.Count; i++)
            {
                Chromosome chromosome = population[GeneticAlgorithm.Random.Next(population.Count - 1)];
                Chromosome chromosome2 = population[GeneticAlgorithm.Random.Next(population.Count - 1)];
                list.Add((chromosome.Fitness > chromosome2.Fitness) ? chromosome : chromosome2);
            }
            return list;
        }
    }

    public class GeneticAlgorithmParameters
    {
        public IGeneticSelection Selection { get; set; }

        public IGeneticCrossover Crossover { get; set; }

        public IGeneticMutation Mutation { get; set; }

        public int PopulationCount { get; set; }

        public int GenerationCount { get; set; }

        public int EliteCount { get; set; }

        public double MutationProbability { get; set; }

        public GeneticAlgorithmParameters()
        {
            Selection = new TournamentSelection();
            Crossover = new BLXCrossover();
            Mutation = new FullMutation();
            PopulationCount = 30;
            GenerationCount = 30;
            EliteCount = 1;
            MutationProbability = 0.05;
        }
    }

    public class BLXCrossover : IGeneticCrossover
    {
        public string Name => "BLX Crossover";

        public Chromosome Cross(Chromosome chromosome1, Chromosome chromosome2, Bounds bounds)
        {
            int nRows = chromosome1.Vector.NRows;
            Vector vector = new Vector(nRows);
            for (int i = 0; i < nRows; i++)
            {
                double num = Math.Min(chromosome1.Vector[i], chromosome2.Vector[i]);
                double expr_55 = Math.Max(chromosome1.Vector[i], chromosome2.Vector[i]);
                double num2 = (expr_55 - num)*this.Alpha;
                double num3 = Math.Max(num - num2, bounds.Lower[i]);
                double num4 = Math.Min(expr_55 + num2, bounds.Upper[i]);
                double num5 = GeneticAlgorithm.Random.NextDouble();
                vector[i] = num5*(num4 - num3) + num3;
                if (this.IsPorfolioOptimization)
                {
                    if (num5 < 0.1)
                    {
                        vector[i] = bounds.Lower[i];
                    }
                    else if (num5 < 0.2)
                    {
                        vector[i] = bounds.Upper[i];
                    }
                }
            }
            return new Chromosome(vector);
        }

        public double Alpha = 0.5;

        public bool IsPorfolioOptimization;
    }

    public class FullMutation : IGeneticMutation
    {
        public string Name => "Full Mutation";

        public Chromosome Mutate(Chromosome chromosome, Bounds bounds)
        {
            Vector vector = new Vector(bounds.Lower.NRows);
            for (int i = 0; i < vector.NRows; i++)
            {
                vector[i] = GeneticAlgorithm.Random.NextDouble()*(bounds.Upper[i] - bounds.Lower[i]) + bounds.Lower[i];
            }
            return new Chromosome(vector);
        }
    }

    public class GeneticAlgorithm
    {
        public static Random Random { get; set; }
    }

    public interface IGeneticMutation : IGeneticFunction
    {
        Chromosome Mutate(Chromosome chromosome, Bounds bounds);
    }

    public interface IGeneticCrossover : IGeneticFunction
    {
        Chromosome Cross(Chromosome chromosome1, Chromosome chromosome2, Bounds bounds);
    }

    public interface ISimulatedAnnealing
    {
        int ObjectiveCallCount { get; set; }

        double BestObjective { get; }

        Vector CurrentCandidate { get; }

        void Init(Func<Vector, double> objectiveFunc, Vector lowerBounds, Vector upperBounds);

        double Progress();

        Vector NextCandidate();

        void NextStep(double objective);

        Vector Run();
    }

    public class RouletteWheelSelection : IGeneticSelection
    {
        public string Name => "Roulette Wheel Selection";

        public List<Chromosome> SelectChromosomes(List<Chromosome> population)
        {
            double num = population.Min(p => p.Fitness);
            if (num < 0.0)
            {
                this.double_0 = -num*1.1;
            }

            double double_ = population.Sum(p => p.Fitness);
            this.list_0 = new List<Chromosome>();
            for (int i = 0; i < population.Count; i++)
            {
                this.list_0.Add(this.method_0(double_, population));
            }
            return this.list_0;
        }

        private Chromosome method_0(double double_1, List<Chromosome> list_1)
        {
            double num = GeneticAlgorithm.Random.NextDouble();
            double num2 = 0.0;
            foreach (Chromosome current in list_1)
            {
                num2 += (current.Fitness + this.double_0)/double_1;
                if (num < num2)
                {
                    return current;
                }
            }
            throw new Exception("Selection -> chromosome not found");
        }

        private List<Chromosome> list_0;

        private double double_0;
    }
}