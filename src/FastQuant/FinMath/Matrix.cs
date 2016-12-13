using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FastQuant.FinMath
{

    public class PopulationEventArgs : EventArgs
    {
        public List<Chromosome> Population { get; set; }

        public int GenerationNumber { get; set; }

        public PopulationEventArgs(List<Chromosome> population, int generationNumber)
        {
            Population = population;
            GenerationNumber = generationNumber;
        }
    }

    public class Chromosome : IComparable<Chromosome>, ICloneable
    {
        public Vector Vector { get; set; }

        public double Fitness
        {
            get
            {
                return this.fitness;
            }
            set
            {
                this.fitness = value;
                HasFitness = true;
            }
        }

        public bool HasFitness { get; set; }

        public Chromosome(Vector vector)
        {
            Vector = vector;
        }

        public void RoundVector()
        {
            for (var i = 0; i < Vector.NRows; i++)
            {
                Vector[i] = Math.Round(Vector[i]);
            }
        }

        public override bool Equals(object obj)
        {
            Chromosome chromosome = obj as Chromosome;
            if (chromosome == null)
            {
                return false;
            }
            if (this.Vector.NRows != chromosome.Vector.NRows)
            {
                return false;
            }
            for (int i = 0; i < this.Vector.NRows; i++)
            {
                if (this.Vector[i] != chromosome.Vector[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int num = 0;
            for (int i = 0; i < this.Vector.NRows; i++)
            {
                num ^= this.Vector[i].GetHashCode();
            }
            return num;
        }

        public override string ToString() => string.Join(" | ", Fitness, Vector);

        public object Clone()
        {
            return new Chromosome((Vector)Vector.Clone())
            {
                fitness = this.fitness,
                HasFitness = HasFitness
            };
        }

        public int CompareTo(Chromosome other)
        {
            if (other == null)
            {
                return 1;
            }
            if (this.HasFitness != other.HasFitness)
            {
                return this.HasFitness.CompareTo(other.HasFitness);
            }
            return this.fitness.CompareTo(other.fitness);
        }

        private double fitness;
    }

    public class Matrix
    {
        public int NRows { get; }

        public int NCols { get; }

        public Matrix(int nrows, int ncols)
        {
            this.NRows = nrows;
            this.NCols = ncols;
            this.hfJoFhQvyA = new double[nrows, ncols];
        }

        public double this[int row, int col]
        {
            get
            {
                return this.hfJoFhQvyA[row, col];
            }
            set
            {
                this.hfJoFhQvyA[row, col] = value;
            }
        }

        public Matrix Transpose()
        {
            Matrix matrix = new Matrix(this.NCols, this.NRows);
            for (int i = 0; i < this.NRows; i++)
            {
                for (int j = 0; j < this.NCols; j++)
                {
                    matrix[j, i] = this.hfJoFhQvyA[i, j];
                }
            }
            return matrix;
        }

        public static Matrix operator *(Matrix matrix, double x)
        {
            Matrix matrix2 = new Matrix(matrix.NRows, matrix.NCols);
            for (int i = 0; i < matrix.NRows; i++)
            {
                for (int j = 0; j < matrix.NCols; j++)
                {
                    matrix2[i, j] = matrix[i, j] * x;
                }
            }
            return matrix2;
        }

        public static Matrix operator *(double x, Matrix matrix)
        {
            return matrix * x;
        }

        public static Matrix operator +(Matrix matrix, double x)
        {
            Matrix matrix2 = new Matrix(matrix.NRows, matrix.NCols);
            for (int i = 0; i < matrix.NRows; i++)
            {
                for (int j = 0; j < matrix.NCols; j++)
                {
                    matrix2[i, j] = matrix[i, j] + x;
                }
            }
            return matrix2;
        }

        public static Matrix operator +(double x, Matrix matrix)
        {
            return matrix + x;
        }

        public static Matrix operator /(Matrix matrix, double x)
        {
            return matrix * (1.0 / x);
        }

        public static Vector operator *(Matrix matrix, Vector vector)
        {
            if (matrix.NCols != vector.NRows)
            {
                throw new Exception(string.Concat("Can not multiply matrix with ", matrix.NCols, " columns by vector with ", vector.NRows, " rows."));
            }
            Vector vector2 = new Vector(matrix.NRows);
            for (int i = 0; i < matrix.NRows; i++)
            {
                double num = 0.0;
                for (int j = 0; j < matrix.NCols; j++)
                {
                    num += matrix[i, j] * vector[j];
                }
                vector2[i] = num;
            }
            return vector2;
        }

        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.NRows != matrix2.NRows) throw new Exception("Can not add matrices with different sizes.");
            if (matrix1.NCols != matrix2.NCols) throw new Exception("Can not add matrices with different sizes.");

            Matrix matrix3 = new Matrix(matrix1.NRows, matrix1.NCols);
            for (int i = 0; i < matrix1.NRows; i++)
            {
                for (int j = 0; j < matrix1.NCols; j++)
                {
                    matrix3[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            }
            return matrix3;
        }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.NRows != matrix2.NRows) throw new Exception("Can not subtract matrices with different sizes.");
            if (matrix1.NCols != matrix2.NCols) throw new Exception("Can not subtract matrices with different sizes.");

            Matrix matrix3 = new Matrix(matrix1.NRows, matrix1.NCols);
            for (int i = 0; i < matrix1.NRows; i++)
            {
                for (int j = 0; j < matrix1.NCols; j++)
                {
                    matrix3[i, j] = matrix1[i, j] - matrix2[i, j];
                }
            }
            return matrix3;
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            Matrix matrix3 = new Matrix(matrix1.NRows, matrix2.NCols);
            if (matrix1.NCols != matrix2.NRows)
            {
                throw new Exception(string.Concat("Can not multiply matrix with ", matrix1.NCols, " columns by matrix with ", matrix2.NRows, " rows."));
            }
            for (int i = 0; i < matrix1.NRows; i++)
            {
                for (int j = 0; j < matrix2.NCols; j++)
                {
                    double num = 0.0;
                    for (int k = 0; k < matrix1.NCols; k++)
                    {
                        num += matrix1[i, k] * matrix2[k, j];
                    }
                    matrix3[i, j] = num;
                }
            }
            return matrix3;
        }

        public void Print(string title = null, string format = "0.00")
        {
            if (title != null)
            {
                Console.WriteLine("\n" + title + "\n");
            }
            for (int i = 0; i < this.NRows; i++)
            {
                for (int j = 0; j < this.NCols; j++)
                {
                    Console.Write(this.hfJoFhQvyA[i, j].ToString(format) + " ");
                }
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.NRows; i++)
            {
                for (int j = 0; j < this.NCols; j++)
                {
                    stringBuilder.Append(this.hfJoFhQvyA[i, j]);
                    if (j + 1 < this.NCols)
                    {
                        stringBuilder.Append(" ");
                    }
                }
                if (i + 1 < this.NRows)
                {
                    stringBuilder.Append("\r\n");
                }
            }
            return stringBuilder.ToString();
        }

        public static Matrix Rand(int nrows, int ncols)
        {
            Matrix matrix = new Matrix(nrows, ncols);
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    matrix[i, j] = Matrix.random.NextDouble();
                }
            }
            return matrix;
        }

        public static Matrix RandDiag(int nrows)
        {
            Matrix matrix = new Matrix(nrows, nrows);
            for (int i = 0; i < nrows; i++)
            {
                matrix[i, i] = Matrix.random.NextDouble();
            }
            return matrix;
        }

        public Vector Diag()
        {
            int num = Math.Min(this.NRows, this.NCols);
            Vector vector = new Vector(num);
            for (int i = 0; i < num; i++)
            {
                vector[i] = this[i, i];
            }
            return vector;
        }

        public Matrix CholeskyDecomposition()
        {
            if (this.NRows != this.NCols)
            {
                throw new Exception(string.Concat("Can not make Cholesky decomposition for matrix with ", this.NRows, " columns and ", this.NCols, " rows."));
            }
            Matrix matrix = new Matrix(this.NRows, this.NRows);
            for (int i = 0; i < this.NRows; i++)
            {
                double num;
                for (int j = 0; j < i; j++)
                {
                    num = 0.0;
                    for (int k = 0; k < j; k++)
                    {
                        num += matrix[i, k] * matrix[j, k];
                    }
                    matrix[i, j] = (this[i, j] - num) / matrix[j, j];
                }
                num = this[i, i];
                for (int l = 0; l < i; l++)
                {
                    num -= matrix[i, l] * matrix[i, l];
                }
                matrix[i, i] = Math.Sqrt(num);
            }
            return matrix;
        }

        internal double[,] hfJoFhQvyA;

        private static readonly Random random = new Random();
    }
}