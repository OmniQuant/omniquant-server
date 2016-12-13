using System;
using System.Globalization;
using System.Text;

namespace FastQuant.FinMath
{
    public class Vector : ICloneable
    {
        public int NRows { get; }

        public Vector(int nrows)
        {
            this.NRows = nrows;
            this.double_0 = new double[nrows];
        }

        public Vector(double v0, double v1)
        {
            this.NRows = 2;
            this.double_0 = new double[this.NRows];
            this.double_0[0] = v0;
            this.double_0[1] = v1;
        }

        public Vector(double v0, double v1, double v2)
        {
            this.NRows = 3;
            this.double_0 = new double[this.NRows];
            this.double_0[0] = v0;
            this.double_0[1] = v1;
            this.double_0[2] = v2;
        }

        public Vector(double v0, double v1, double v2, double v3)
        {
            this.NRows = 4;
            this.double_0 = new double[this.NRows];
            this.double_0[0] = v0;
            this.double_0[1] = v1;
            this.double_0[2] = v2;
            this.double_0[3] = v3;
        }

        public Vector(double v0, double v1, double v2, double v3, double v4)
        {
            this.NRows = 5;
            this.double_0 = new double[this.NRows];
            this.double_0[0] = v0;
            this.double_0[1] = v1;
            this.double_0[2] = v2;
            this.double_0[3] = v3;
            this.double_0[4] = v4;
        }

        public Vector(double[] values)
        {
            this.NRows = values.Length;
            this.double_0 = values;
        }

        public double Length()
        {
            double d = 0.0;
            for (int i = 0; i < this.NRows; i++)
            {
                d = this.double_0[i] * this.double_0[i];
            }
            return Math.Sqrt(d);
        }

        public Vector Normalize()
        {
            Vector vector = new Vector(this.NRows);
            double num = this.Length();
            for (int i = 0; i < this.NRows; i++)
            {
                vector[i] = this.double_0[i] / num;
            }
            return vector;
        }

        public double this[int row]
        {
            get
            {
                return this.double_0[row];
            }
            set
            {
                this.double_0[row] = value;
            }
        }

        public double Min()
        {
            double num = 1.7976931348623157E+308;
            for (int i = 0; i < this.NRows; i++)
            {
                if (this.double_0[i] < num)
                {
                    num = this.double_0[i];
                }
            }
            return num;
        }

        public double Max()
        {
            double num = -1.7976931348623157E+308;
            for (int i = 0; i < this.NRows; i++)
            {
                if (this.double_0[i] > num)
                {
                    num = this.double_0[i];
                }
            }
            return num;
        }

        public double Mean()
        {
            double num = 0.0;
            for (int i = 0; i < this.NRows; i++)
            {
                num += this.double_0[i];
            }
            return num / (double)this.NRows;
        }

        public void Round()
        {
            for (int i = 0; i < this.NRows; i++)
            {
                this.double_0[i] = Math.Round(this.double_0[i]);
            }
        }

        public double Variance()
        {
            double num = 0.0;
            double num2 = this.Mean();
            for (int i = 0; i < this.NRows; i++)
            {
                num += Math.Pow(this.double_0[i] - num2, 2.0);
            }
            return num / (double)this.NRows;
        }

        public double StdDev()
        {
            return Math.Sqrt(this.Variance());
        }

        public void Assign(double x)
        {
            for (int i = 0; i < this.NRows; i++)
            {
                this.double_0[i] = x;
            }
        }

        public static Vector operator *(Vector vector, double x)
        {
            Vector vector2 = new Vector(vector.NRows);
            for (int i = 0; i < vector.NRows; i++)
            {
                vector2[i] = vector[i] * x;
            }
            return vector2;
        }

        public static Vector operator *(double x, Vector vector)
        {
            return vector * x;
        }

        public static Vector operator +(Vector vector, double x)
        {
            Vector vector2 = new Vector(vector.NRows);
            for (int i = 0; i < vector.NRows; i++)
            {
                vector2[i] = vector[i] + x;
            }
            return vector2;
        }

        public static Vector operator +(double x, Vector vector)
        {
            return vector + x;
        }

        public static Vector operator -(Vector vector, double x)
        {
            Vector vector2 = new Vector(vector.NRows);
            for (int i = 0; i < vector.NRows; i++)
            {
                vector2[i] = vector[i] - x;
            }
            return vector2;
        }

        public static Vector operator -(double x, Vector vector)
        {
            Vector vector2 = new Vector(vector.NRows);
            for (int i = 0; i < vector.NRows; i++)
            {
                vector2[i] = x - vector[i];
            }
            return vector2;
        }

        public static Vector operator /(Vector vector, double x)
        {
            return vector * (1.0 / x);
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not add vectors with different sizes.");
            }
            Vector vector3 = new Vector(vector1.NRows);
            for (int i = 0; i < vector1.NRows; i++)
            {
                vector3[i] = vector1[i] + vector2[i];
            }
            return vector3;
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not subtract vectors with different sizes.");
            }
            Vector vector3 = new Vector(vector1.NRows);
            for (int i = 0; i < vector1.NRows; i++)
            {
                vector3[i] = vector1[i] - vector2[i];
            }
            return vector3;
        }

        public static double operator *(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not multiply vectors with different sizes.");
            }
            double num = 0.0;
            for (int i = 0; i < vector1.NRows; i++)
            {
                num += vector1[i] * vector2[i];
            }
            return num;
        }

        public static bool operator <(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not compare vectors with different sizes.");
            }
            for (int i = 0; i < vector1.NRows; i++)
            {
                if (vector1[i] >= vector2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator >(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not compare vectors with different sizes.");
            }
            for (int i = 0; i < vector1.NRows; i++)
            {
                if (vector1[i] <= vector2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator <=(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not compare vectors with different sizes.");
            }
            for (int i = 0; i < vector1.NRows; i++)
            {
                if (vector1[i] > vector2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator >=(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not compare vectors with different sizes.");
            }
            for (int i = 0; i < vector1.NRows; i++)
            {
                if (vector1[i] < vector2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void Print(string title = null, string format = "0.00")
        {
            if (title != null)
            {
                Console.WriteLine("\n" + title + "\n");
            }
            for (int i = 0; i < this.NRows; i++)
            {
                Console.WriteLine(this.double_0[i].ToString(format));
            }
        }

        public static Vector Rand(int nrows)
        {
            Vector vector = new Vector(nrows);
            for (int i = 0; i < nrows; i++)
            {
                vector[i] = Vector.random.NextDouble();
            }
            return vector;
        }

        public static Vector Max(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not perform max operation for vectors with different sizes.");
            }
            Vector vector3 = new Vector(vector1.NRows);
            for (int i = 0; i < vector1.NRows; i++)
            {
                vector3[i] = Math.Max(vector1[i], vector2[i]);
            }
            return vector3;
        }

        public static Vector Min(Vector vector1, Vector vector2)
        {
            if (vector1.NRows != vector2.NRows)
            {
                throw new Exception("Can not perform min operation for vectors with different sizes.");
            }
            Vector vector3 = new Vector(vector1.NRows);
            for (int i = 0; i < vector1.NRows; i++)
            {
                vector3[i] = Math.Min(vector1[i], vector2[i]);
            }
            return vector3;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.NRows; i++)
            {
                stringBuilder.Append(this.double_0[i]);
                if (i + 1 < this.NRows)
                {
                    stringBuilder.Append(" ");
                }
            }
            return stringBuilder.ToString();
        }

        public static Vector FromString(string data)
        {
            string[] array = data.Split(new char[]
            {
                ' '
            });
            double[] array2 = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array2[i] = double.Parse(array[i], CultureInfo.InvariantCulture);
            }
            return new Vector(array2);
        }

        public object Clone()
        {
            Vector vector = new Vector(this.NRows);
            for (int i = 0; i < this.NRows; i++)
            {
                vector[i] = this[i];
            }
            return vector;
        }

        internal double[] double_0;

        private static readonly Random random = new Random();
    }
}