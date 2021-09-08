using UnityEngine;
using System;


public class Matrix
{
    private int rows;
    private int cols;
    private float[,] matrix;

    public Matrix(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        matrix = new float[rows, cols];
    }

    public Matrix(float[,] mat)
    {
        this.rows = mat.GetLength(0);
        this.cols = mat.GetLength(1);
        this.matrix = mat;
    }

    public float this[int r, int c]
    {
        get
        {
            return matrix[r, c];
        }
        set
        {
            matrix[r, c] = value;
        }
    }

    public static Matrix operator +(Matrix m1, Matrix m2)
    {
        if (m1.ColCount() != m2.ColCount() || m1.RowCount() != m2.RowCount())
        {
            throw new ArithmeticException("Impossible Operation: " + m1.shapeStr() + " + " + m2.shapeStr());
        }

        Matrix m = new Matrix(m1.rows, m1.cols);
        for (int r = 0; r < m1.rows; r++)
        {
            for (int c = 0; c < m1.cols; c++)
            {
                m[r, c] = m1[r, c] + m2[r, c];
            }
        }
        return m;
    }

    public static Matrix operator -(Matrix m1, Matrix m2)
    {
        if (m1.ColCount() != m2.ColCount() || m1.RowCount() != m2.RowCount())
        {
            throw new ArithmeticException("Impossible Operation: " + m1.shapeStr() + " - " + m2.shapeStr());
        }

        Matrix m = new Matrix(m1.rows, m1.cols);
        for (int r = 0; r < m1.rows; r++)
        {
            for (int c = 0; c < m1.cols; c++)
            {
                m[r, c] = m1[r, c] - m2[r, c];
            }
        }
        return m;
    }

    public static Matrix operator /(Matrix m1, Matrix m2)
    {
        if (m1.ColCount() != m2.ColCount() || m1.RowCount() != m2.RowCount())
        {
            throw new ArithmeticException("Impossible Operation: " + m1.shapeStr() + " / " + m2.shapeStr());
        }


        Matrix m = new Matrix(m1.rows, m1.cols);
        for (int r = 0; r < m1.rows; r++)
        {
            for (int c = 0; c < m1.cols; c++)
            {
                m[r, c] = m1[r, c] / m2[r, c];
            }
        }
        return m;
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        if (m1.ColCount() == m2.RowCount()) { return _MatMul(m1, m2); }
        if (m1.ColCount() != m2.ColCount() || m1.RowCount() != m2.RowCount())
        {
            throw new ArithmeticException("Impossible Operation: " + m1.shapeStr() + " * " + m2.shapeStr());
        }

        Matrix m = new Matrix(m1.rows, m1.cols);
        for (int r = 0; r < m1.rows; r++)
        {
            for (int c = 0; c < m1.cols; c++)
            {
                m[r, c] = m1[r, c] - m2[r, c];
            }
        }
        return m;
    }

    private static Matrix _MatMul(Matrix m1, Matrix m2)
    {
        Matrix result = new Matrix(m1.RowCount(), m2.ColCount());
        for (int i = 0; i < m1.RowCount(); i++)
        {
            for (int j = 0; j < m2.ColCount(); j++)
            {
                for (int k = 0; k < m2.RowCount(); k++)
                {
                    result[i, j] += m1[i, k] * m2[k, j];
                }
            }
        }
        return result;
    }

    public static Matrix random(int rows, int cols, float rangeMin, float rangeMax)
    {
        Matrix mat = new Matrix(rows, cols);
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                mat[r, c] = UnityEngine.Random.Range(rangeMin, rangeMax); } }
        return mat;
    }

    public static Matrix random(int rows, int cols)
    {
        return random(rows, cols, -2f, 2f);
    }
    
    public int RowCount() { return rows; }

    public int ColCount() { return cols; }

    public string shapeStr() { return "(" + rows + ", " + cols + ")"; }

    public string toString()
    {
        string str = "[[";
        for (int r = 0; r < this.rows; r++)
        {
            string ta = "";
            for (int c = 0; c < this.cols-1; c++)
            {
                ta += this[r, c] + ", ";
            }
            str += ta + this[r, this.cols-1] + "]\n [";
        }
        return str.Substring(0, str.Length - 3) + "]";
    }

    public Matrix copy() {
        Matrix mat = new Matrix(this.rows, this.cols);

        for (int r = 0; r < this.rows; r++) {
            for (int c = 0; c < this.cols; c++) {
                mat[r, c] = this[r, c]; } }
        return mat;
    }

    public void mutate(float bias)
    {
        int count = rows * cols;
        count = Mathf.Clamp(UnityEngine.Random.Range(0, (int) bias*count), 1, count);
        for (int r = 0; r < count; r++) {
            this[UnityEngine.Random.Range(0, rows), UnityEngine.Random.Range(0, cols)] = UnityEngine.Random.Range(-2f, 2f);
        }
    }

    public Matrix mutatedCopy(float mutationRate)
    {
        Matrix mat = new Matrix(this.rows, this.cols);
        for (int r = 0; r < this.rows; r++) {
            for (int c = 0; c < this.cols; c++) {
                if (UnityEngine.Random.Range(0f, 1f) < mutationRate) { mat[r, c] = UnityEngine.Random.Range(-2f, 2f); }
                else { mat[r, c] = this.matrix[r, c]; } 
            } }
        return mat;
    }

    public Matrix T()
    {
        Matrix mat = new Matrix(cols, rows);
        for (int r = 0; r < this.rows; r++) {
            for (int c = 0; c < this.cols; c++) {
                mat[c, r] = this[r, c];
            } }
        return mat;
    }

    public void Normalize() {
        float maximum = float.MinValue;
        float minimum = float.MaxValue;

        for(int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                maximum = Mathf.Max(maximum, matrix[r, c]);
                minimum = Mathf.Min(minimum, matrix[r, c]);
            }
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                matrix[r, c] = (matrix[r, c] - minimum) / (maximum- minimum);
            }
        }
    }

    public Matrix NormalizedCopy()
    {
        Matrix copyMatrix = this.copy();
        
        float maximum = float.MinValue;
        float minimum = float.MaxValue; 
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                maximum = Mathf.Max(maximum, matrix[r, c]);
                minimum = Mathf.Min(minimum, matrix[r, c]);
            }
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                copyMatrix[r, c] = (matrix[r, c] - minimum) / (maximum - minimum);
            }
        }
        return copyMatrix;
    }

    public int[] GetMaxLoc() {
        float maximum = float.MinValue;
        int[] maxLoc = new int[] {0, 0};
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                if (matrix[r, c] > maximum) {
                    maximum = matrix[r, c];
                    maxLoc[0] = r;
                    maxLoc[1] = c;
                } } }
        return maxLoc;
    }
}
