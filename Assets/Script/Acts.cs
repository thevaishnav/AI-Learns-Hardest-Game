using UnityEngine;
using System.Collections.Generic;

public static class Acts
{
    public static Dictionary<bool, int> B2I = new Dictionary<bool, int>() { { false, 0 }, { true, 1} };

    public static float _Sigmoid(float X) { return 1 / (1 + Mathf.Exp(-X)); }
    public static float _ReLU(float X) { return Mathf.Max(0, X); }
    public static float _LeakyReLU(float X) { return (X * B2I[X > 0]) + (0.1f * X * B2I[X < 0]); }
    public static float _Tanh(float X) { return (2 / (1 + Mathf.Exp(-2 * X))) - 1; ; }
    public static float _Step(float X) { return B2I[X > 0.5f]; }

    public static Matrix Sigmoid(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());

        for (int r = 0; r < matrix.RowCount(); r++) {
            for (int c = 0; c < matrix.ColCount(); c++) {
                mat[r, c] = _Sigmoid(matrix[r, c]); } }
        return mat;
    }

    public static Matrix ReLU(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());
        for (int r = 0; r < matrix.RowCount(); r++) {
            for (int c = 0; c < matrix.ColCount(); c++) {
                mat[r, c] = _ReLU(matrix[r, c]); } }
        return mat;
    }

    public static Matrix LeakyReLU(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());
        for (int r = 0; r < matrix.RowCount(); r++)
        {
            for (int c = 0; c < matrix.ColCount(); c++)
            {
                mat[r, c] = _LeakyReLU(matrix[r, c]);
            }
        }
        return mat;
    }

    public static Matrix Tanh(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());
        for (int r = 0; r < matrix.RowCount(); r++)
        {
            for (int c = 0; c < matrix.ColCount(); c++)
            {
                mat[r, c] = _Tanh(matrix[r, c]);
            }
        }
        return mat;
    }

    public static Matrix Exp(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());
        for (int r = 0; r < matrix.RowCount(); r++)
        {
            for (int c = 0; c < matrix.ColCount(); c++)
            {
                mat[r, c] = Mathf.Exp(matrix[r, c]);
            }
        }
        return mat;
    }

    public static Matrix Step(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());

        for (int r = 0; r < matrix.RowCount(); r++)
        {
            for (int c = 0; c < matrix.ColCount(); c++)
            {
                mat[r, c] = _Step(matrix[r, c]);
            }
        }
        return mat;
    }

    public static Matrix SoftMaxRow(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());

        for (int i = 0; i < matrix.RowCount(); i++) {
            float sm = 0f;
            for (int j = 0; j < matrix.ColCount(); j++) {
                float exp = Mathf.Exp(matrix[i, j]);
                sm += exp;
                mat[i, j] = exp; }

            for (int j = 0; j < matrix.ColCount(); j++) { mat[i, j] /= sm; }
        }
        return mat;
    }

    public static Matrix SoftMax(Matrix matrix)
    {
        Matrix mat = new Matrix(matrix.RowCount(), matrix.ColCount());
        float sm = 0f;

        for (int i = 0; i < matrix.RowCount(); i++) {
            for (int j = 0; j < matrix.ColCount(); j++) {
                float exp = Mathf.Exp(matrix[i, j]);
                sm += exp;
                mat[i, j] = exp;
            } }

        for (int i = 0; i < matrix.RowCount(); i++) {
            for (int j = 0; j < matrix.ColCount(); j++) {
                mat[i, j] /= sm;
            } }
        return mat;
    }
}
