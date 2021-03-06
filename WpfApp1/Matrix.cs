using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [Serializable]
    public class Matrix
    {
        public float[,] elements;
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
            this.elements = new float[rows, rows];
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
            this.elements = new float[rows, cols];
        }
        public bool MakeIdentity()
        {
            if (rows != cols)
                return false;
            Matrix res = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
                elements[i, i] = 1.0f;
            return true;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {

            if ((a.Rows != b.Rows) || (a.Cols != b.Cols))
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = a.elements[i, j] + b.elements[i, j];

            return res;
        }

        public void AddWithMulLR(Matrix b, float lrRate)
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    elements[i, j] = elements[i, j] + b.elements[i, j] * lrRate;
        }


        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = a.elements[i, j] - b.elements[i, j];

            return res;
        }

        public static Matrix operator -(float c, Matrix a)
        {
            if (a == null)
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = c - a.elements[i, j];

            return res;
        }

        public static Matrix operator *(float c, Matrix a)
        {
            return a * c;
        }
        public static Matrix operator *(Matrix a, float c)
        {
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = c * a.elements[i, j];

            return res;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix res = new Matrix(a.Rows, b.Cols);
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

        /*public void mulAdamar(Matrix a)
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    elements[i, j] = a.elements[i, j] * elements[i, j];

        }

        public static void mulAdamar(Matrix a, Matrix b, ref Matrix res)
        {
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res.elements[i, j] = a.elements[i, j] * b.elements[i, j];

        }*/

        public Matrix Transpose()
        {
            Matrix res = new Matrix(cols, rows);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    res.elements[j, i] = this.elements[i, j];
            return res;
        }

        public void Sigmoid()
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    this.elements[i, j] = 1.0f / (1.0f + (float)Math.Exp((-1.0f) * this.elements[i, j]));
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
