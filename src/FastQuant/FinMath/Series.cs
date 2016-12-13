using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FastQuant.FinMath
{
    public class SeriesItem : IComparable
    {
        public DateTime DateTime { get; }

        public double[] Data
        {
            get
            {
                return this.double_0;
            }
            set
            {
                for (int i = 0; i < this.double_0.Length; i++)
                {
                    this.double_0[i] = value[i];
                }
            }
        }

        public SeriesItem(DateTime dateTime, int n = 1, double[] data = null)
        {
            DateTime = dateTime;
            this.double_0 = new double[n];
            if (data == null)
            {
                for (int i = 0; i < n; i++)
                {
                    this.double_0[i] = double.NaN;
                }
                return;
            }
            Data = data;
        }

        public bool IsComplete()
        {
            return this.double_0.All(t => !double.IsNaN(t));
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

        public int CompareTo(object obj)
        {
            SeriesItem seriesItem = (SeriesItem)obj;
            if (seriesItem.DateTime == this.DateTime)
            {
                return 0;
            }
            if (seriesItem.DateTime > this.DateTime)
            {
                return 1;
            }
            return -1;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.DateTime.ToString(SeriesItem.DateFormat) + " ");
            for (int i = 0; i < this.double_0.Length; i++)
            {
                stringBuilder.Append(this.double_0[i]);
                if (i + 1 < this.double_0.Length)
                {
                    stringBuilder.Append(" ");
                }
            }
            return stringBuilder.ToString();
        }

        public Vector ToVector()
        {
            return new Vector(this.double_0);
        }

        public static SeriesItem FromString(string data)
        {
            string[] array = data.Split(' ');
            DateTime dateTime = DateTime.ParseExact(array[0], SeriesItem.DateFormat, null);
            double[] array2 = new double[array.Length - 1];
            for (int i = 0; i < array.Length - 1; i++)
            {
                array2[i] = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
            }
            return new SeriesItem(dateTime, array.Length - 1, array2);
        }
        public static string DateFormat = "yyyy-MM-dd";

        internal double[] double_0;
    }

    public class Series
    {
        public int Count => this.sortedList_0.Count;

        public int Nrows { get; }

        public Series(int nrows = 1)
        {
            Nrows = nrows;
        }

        private SeriesItem method_0(SeriesItem seriesItem_0)
        {
            this.sortedList_0.Add(seriesItem_0.DateTime, seriesItem_0);
            return seriesItem_0;
        }

        public SeriesItem Add(DateTime dateTime)
        {
            SeriesItem seriesItem = this[dateTime];
            if (seriesItem == null)
            {
                return this.method_0(new SeriesItem(dateTime, this.Nrows, null));
            }
            return seriesItem;
        }

        public SeriesItem Add(DateTime dateTime, double data, int row = 0)
        {
            SeriesItem expr_07 = this.Add(dateTime);
            expr_07[row] = data;
            return expr_07;
        }

        public SeriesItem Add(DateTime dateTime, double[] data)
        {
            SeriesItem seriesItem;
            this.sortedList_0.TryGetValue(dateTime, out seriesItem);
            if (seriesItem == null)
            {
                seriesItem = this.method_0(new SeriesItem(dateTime, this.Nrows, data));
            }
            else
            {
                seriesItem.Data = data;
            }
            return seriesItem;
        }

        public SeriesItem this[DateTime dateTime]
        {
            get
            {
                SeriesItem result;
                this.sortedList_0.TryGetValue(dateTime, out result);
                return result;
            }
        }

        public SeriesItem this[int i] => this.sortedList_0.Values[i];

        public Series Complete()
        {
            Series series = new Series(this.Nrows);
            int count = this.sortedList_0.Count;
            for (int i = 0; i < count; i++)
            {
                if (this[i].IsComplete())
                {
                    series.method_0(this[i]);
                }
            }
            return series;
        }

        public Series Copy(int index1, int index2)
        {
            Series series = new Series(this.Nrows);
            int arg_17_0 = this.sortedList_0.Count;
            for (int i = index1; i <= index2; i++)
            {
                series.method_0(this[i]);
            }
            return series;
        }

        public Series Return(int index1, int index2)
        {
            Series series = new Series(this.Nrows);
            double[] array = new double[this.Nrows];
            for (int i = index1 + 1; i <= index2; i++)
            {
                for (int j = 0; j < this.Nrows; j++)
                {
                    array[j] = (this.sortedList_0.Values[i][j] - this.sortedList_0.Values[i - 1][j]) / this.sortedList_0.Values[i - 1][j];
                }
                series.Add(this.sortedList_0.Values[i].DateTime, array);
            }
            return series;
        }

        public Series Return()
        {
            return this.Return(0, this.sortedList_0.Count - 1);
        }

        public Series LogReturn(int index1, int index2)
        {
            Series series = new Series(this.Nrows);
            double[] array = new double[this.Nrows];
            for (int i = index1 + 1; i <= index2; i++)
            {
                for (int j = 0; j < this.Nrows; j++)
                {
                    array[j] = Math.Log(this.sortedList_0.Values[i][j] / this.sortedList_0.Values[i - 1][j]);
                }
                series.Add(this.sortedList_0.Values[i].DateTime, array);
            }
            return series;
        }

        public Series LogReturn()
        {
            return this.LogReturn(0, this.sortedList_0.Count - 1);
        }

        public double Min(int row, int index1, int index2)
        {
            double num = 1.7976931348623157E+308;
            for (int i = index1; i <= index2; i++)
            {
                if (this.sortedList_0.Values[i][row] < num)
                {
                    num = this.sortedList_0.Values[i][row];
                }
            }
            return num;
        }

        public double Min(int row)
        {
            return this.Min(row, 0, this.sortedList_0.Count - 1);
        }

        public Vector Min(int index1, int index2)
        {
            Vector vector = new Vector(this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                vector[i] = this.Min(i, index1, index2);
            }
            return vector;
        }

        public Vector Min()
        {
            return this.Min(0, this.sortedList_0.Count - 1);
        }

        public double Max(int row, int index1, int index2)
        {
            double num = -1.7976931348623157E+308;
            for (int i = index1; i <= index2; i++)
            {
                if (this.sortedList_0.Values[i][row] > num)
                {
                    num = this.sortedList_0.Values[i][row];
                }
            }
            return num;
        }

        public double Max(int row)
        {
            return this.Max(row, 0, this.sortedList_0.Count - 1);
        }

        public Vector Max(int index1, int index2)
        {
            Vector vector = new Vector(this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                vector[i] = this.Max(i, index1, index2);
            }
            return vector;
        }

        public Vector Max()
        {
            return this.Max(0, this.sortedList_0.Count - 1);
        }

        public double Mean(int row, int index1, int index2)
        {
            double num = 0.0;
            for (int i = index1; i <= index2; i++)
            {
                num += this.sortedList_0.Values[i][row];
            }
            return num / (double)(index2 - index1 + 1);
        }

        public double Mean(int row)
        {
            return this.Mean(row, 0, this.sortedList_0.Count - 1);
        }

        public Vector Mean(int index1, int index2)
        {
            Vector vector = new Vector(this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                vector[i] = this.Mean(i, index1, index2);
            }
            return vector;
        }

        public Vector Mean()
        {
            return this.Mean(0, this.sortedList_0.Count - 1);
        }

        public double Variance(int row, int index1, int index2)
        {
            double num = 0.0;
            double num2 = this.Mean(row, index1, index2);
            for (int i = index1; i <= index2; i++)
            {
                num += Math.Pow(this.sortedList_0.Values[i][row] - num2, 2.0);
            }
            return num / (double)(index2 - index1 + 1);
        }

        public double Variance(int row)
        {
            return this.Variance(row, 0, this.sortedList_0.Count - 1);
        }

        public Vector Variance(int index1, int index2)
        {
            Vector vector = new Vector(this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                vector[i] = this.Variance(i, index1, index2);
            }
            return vector;
        }

        public Vector Variance()
        {
            return this.Variance(0, this.sortedList_0.Count);
        }

        public double StdDev(int row, int index1, int index2)
        {
            return Math.Sqrt(this.Variance(row, index1, index2));
        }

        public double StdDev(int row)
        {
            return this.StdDev(row, 0, this.sortedList_0.Count - 1);
        }

        public Vector StdDev(int index1, int index2)
        {
            Vector vector = new Vector(this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                vector[i] = this.StdDev(i, index1, index2);
            }
            return vector;
        }

        public Vector StdDev()
        {
            return this.StdDev(0, this.sortedList_0.Count - 1);
        }

        public double Covariance(int row1, int row2, int index1, int index2)
        {
            double num = 0.0;
            double num2 = this.Mean(row1, index1, index2);
            double num3 = this.Mean(row2, index1, index2);
            for (int i = index1; i <= index2; i++)
            {
                num += (this.sortedList_0.Values[i][row1] - num2) * (this.sortedList_0.Values[i][row2] - num3);
            }
            return num / (double)(index2 - index1 + 1);
        }

        public double Covariance(int row1, int row2)
        {
            return this.Covariance(row1, row2, 0, this.sortedList_0.Count - 1);
        }

        public Matrix CovarianceMatrix(int index1, int index2)
        {
            Matrix matrix = new Matrix(this.Nrows, this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                for (int j = 0; j < this.Nrows; j++)
                {
                    matrix[i, j] = this.Covariance(i, j, index1, index2);
                }
            }
            return matrix;
        }

        public Matrix CovarianceMatrix()
        {
            return this.CovarianceMatrix(0, this.sortedList_0.Count - 1);
        }

        public double Correlation(int row1, int row2, int index1, int index2)
        {
            return this.Covariance(row1, row2, index1, index2) / (this.StdDev(row1, index1, index2) * this.StdDev(row2, index1, index2));
        }

        public double Correlation(int row1, int row2)
        {
            return this.Correlation(row1, row2, 0, this.sortedList_0.Count - 1);
        }

        public Matrix CorrelationMatrix(int index1, int index2)
        {
            Matrix matrix = new Matrix(this.Nrows, this.Nrows);
            for (int i = 0; i < this.Nrows; i++)
            {
                for (int j = 0; j < this.Nrows; j++)
                {
                    matrix[i, j] = this.Correlation(i, j, index1, index2);
                }
            }
            return matrix;
        }

        public Matrix CorrelationMatrix()
        {
            return this.CorrelationMatrix(0, this.sortedList_0.Count - 1);
        }

        public void Print(string title = null, string format = "0.00")
        {
            if (title != null)
            {
                Console.WriteLine("\n" + title + "\n");
            }
            for (int i = 0; i < this.sortedList_0.Count; i++)
            {
                Console.Write(this.sortedList_0.Values[i].DateTime + " ");
                for (int j = 0; j < this.Nrows; j++)
                {
                    Console.Write(this.sortedList_0.Values[i][j].ToString(format) + " ");
                }
                Console.WriteLine();
            }
        }

        private SortedList<DateTime, SeriesItem> sortedList_0 = new SortedList<DateTime, SeriesItem>();
    }
}