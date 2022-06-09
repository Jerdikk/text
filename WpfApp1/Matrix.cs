using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Matrix
    {
        public double[,] elements;
        private int rows;
        private int cols;
        public int Rows { get { return rows; } }
        public int Cols { get { return cols; } }

        public Matrix(int rows)
        {
            if (rows <= 0)
            {
                this.rows = 0;
                this.cols = 0;
                this.elements = null;
                return;
            }
            this.rows = rows;
            this.cols = rows;
            this.elements = new double[rows, rows];
        }

        public Matrix(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
            {
                this.rows = 0;
                this.cols = 0;
                this.elements = null;
                return;
            }
            this.rows = rows;
            this.cols = cols;
            this.elements = new double[rows, cols];
        }
        public bool MakeIdentity()
        {
            if (rows != cols)
                return false;
            Matrix res = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
                elements[i, i] = 1.0;
            return true;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {

            if ((a == null) || (b == null) || (a.Rows != b.Rows) || (a.Cols != b.Cols))
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = a.elements[i, j] + b.elements[i, j];

            return res;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if ((a == null) || (b == null) || (a.Rows != b.Rows) || (a.Cols != b.Cols))
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = a.elements[i, j] - b.elements[i, j];

            return res;
        }

        public static Matrix operator *(Matrix a, double c)
        {
            if (a == null)
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = c * a.elements[i, j];

            return res;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a == null)
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < b.Cols; j++)
                    for (int k = 0; k < b.Rows; k++)
                        res.elements[i, j] += a.elements[i, k] * b.elements[k, j];

            return res;
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            try
            {
                if ((a is null) && (b is null))
                    return true;
                if ((a is null) || (b is null))
                    return false;

                if ((a.Rows != b.Rows) || (a.Cols != b.Cols))
                    return false;

                for (int i = 0; i < a.Rows; i++)
                    for (int j = 0; j < a.Cols; j++)
                        if (a.elements[i, j] != b.elements[i, j])
                            return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            try
            {
                if ((a is null) && (b is null))
                    return false;
                if ((a is null) && !(b is null))
                    return true;
                if (!(a is null) && (b is null))
                    return true;


                if ((a.Rows != b.Rows) || (a.Cols != b.Cols))
                    return false;
                for (int i = 0; i < a.Rows; i++)
                    for (int j = 0; j < a.Cols; j++)
                        if (a.elements[i, j] != b.elements[i, j])
                            return true;

                return false;
            }
            catch
            {
                return true;
            }
        }


        public static Matrix mulAdamar(Matrix a, Matrix b)
        {
            Matrix res = null;
            if ((a == null) || (b == null) || (a.Rows != b.Rows) || (a.Cols != b.Cols))
                return res;
            res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = a.elements[i, j] * b.elements[i, j];

            return res;
        }
    }

}
