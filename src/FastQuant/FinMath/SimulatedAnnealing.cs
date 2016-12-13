using System;

namespace FastQuant.FinMath
{
    public class SimulatedAnnealing
    {
        public int NT { get; set; }

        public int NC { get; set; }

        public int DR { get; set; }

        public bool OutputEnabled { get; set; }

        public SimulatedAnnealing()
        {
            NC = 50;
            NT = 50;
            DR = 1;
        }

        private void method_0(Func<Vector, double> func_1, Vector vector_6, Vector vector_7, Vector vector_8)
        {
            this.func_0 = func_1;
            this.vector_0 = vector_6;
            this.vector_1 = vector_7;
            if (!(vector_7 >= vector_6))
            {
                throw new Exception("Combination of bounding vectors is not valid.");
            }
            if (vector_8 != null)
            {
                this.vector_3 = vector_8;
            }
            else
            {
                this.vector_3 = (vector_7 - vector_6) / 2.0;
            }
            this.double_8 = func_1(this.vector_3);
            this.vector_2 = this.vector_3;
            this.vector_4 = this.vector_3;
            this.vector_5 = this.vector_3;
            this.double_7 = this.double_8;
            this.double_9 = this.double_8;
            this.double_10 = this.double_8;
            this.double_11 = 0.0;
            this.double_12 = 0.0;
            this.int_2 = 1;
            this.int_3 = 1;
            this.int_4 = 0;
            this.int_5 = 0;
            this.double_0 = 0.8;
            this.double_1 = 0.001;
            this.double_3 = -1.0 / Math.Log(this.double_0);
            this.double_4 = -1.0 / Math.Log(this.double_1);
            this.double_5 = this.double_3;
            this.double_6 = Math.Pow(this.double_4 / this.double_3, 1.0 / ((double)this.NC - 1.0));
            this.random_0 = new Random();
        }

        public Vector Run(Func<Vector, double> func, Vector xl, Vector xu, Vector x0 = null)
        {
            this.method_0(func, xl, xu, x0);
            for (int i = 0; i < this.NC; i++)
            {
                for (int j = 0; j < this.NT; j++)
                {
                    this.method_1();
                }
                this.double_5 *= this.double_6;
            }
            if (this.OutputEnabled)
            {
                Console.WriteLine();
                Console.WriteLine("Best function : " + this.double_10);
                this.vector_5.Print("Best coordiantes : ", "0.00");
                Console.WriteLine();
                Console.WriteLine("Accepted : " + this.int_2);
                Console.WriteLine("Rejected : " + this.int_3);
                Console.WriteLine("Accepted uphills : " + this.int_4);
                Console.WriteLine("Rejected uphills : " + this.int_5);
            }
            return this.vector_5;
        }

        private void method_1()
        {
            bool flag = true;
            this.vector_2 = this.vector_4 + (double)this.DR * (Vector.Rand(this.vector_2.NRows) - 0.5);
            this.vector_2 = Vector.Min(this.vector_2, this.vector_1);
            this.vector_2 = Vector.Max(this.vector_2, this.vector_0);
            this.double_7 = this.func_0(this.vector_2);
            this.double_11 = Math.Abs(this.double_7 - this.double_9);
            if (this.double_7 > this.double_9)
            {
                if (this.double_12 == 0.0)
                {
                    this.double_12 = this.double_11;
                }
                this.double_2 = Math.Exp(-this.double_11 / (this.double_5 * this.double_12));
                if (this.double_2 > this.random_0.NextDouble())
                {
                    flag = true;
                    this.int_4++;
                }
                else
                {
                    flag = false;
                    this.int_5++;
                }
            }
            if (flag)
            {
                this.vector_4 = this.vector_2;
                this.double_9 = this.double_7;
                if (this.double_9 < this.double_10)
                {
                    this.double_10 = this.double_9;
                    this.vector_5 = this.vector_4;
                }
                this.int_2++;
                this.double_12 = (this.double_12 * ((double)this.int_2 - 1.0) + this.double_11) / (double)this.int_2;
                return;
            }
            this.int_3++;
        }

        private Func<Vector, double> func_0;

        private Vector vector_0;

        private Vector vector_1;

        private Vector vector_2;

        private Vector vector_3;

        private Vector vector_4;

        private Vector vector_5;

        private int int_2;

        private int int_3;

        private int int_4;

        private int int_5;

        private double double_0;

        private double double_1;

        private double double_2;

        private double double_3;

        private double double_4;

        private double double_5;

        private double double_6;

        private double double_7;

        private double double_8;

        private double double_9;

        private double double_10;

        private double double_11;

        private double double_12;

        private Random random_0;
    }
}