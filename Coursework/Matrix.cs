using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class Matrix
    {
        public List<List<double>> matrix;
        public int iterations=0;
        public Matrix(List<List<double>> matrix)
        {
            this.matrix = matrix;
        }
        public Matrix(int size)
        {
            this.matrix = GetEmptyMatrix(size, size);
        }
        public List<List<double>> getEmptyMatrix()
        {
            List<List<double>> emptyMatrix = new List<List<double>>();
            for (int i = 0; i < matrix[0].Count; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    row.Add(0);
                    iterations++;
                }
                emptyMatrix.Add(row);
            }
            return emptyMatrix;
        }
        public List<List<double>> GetEmptyMatrix(int rows, int cols)
        {
            List<List<double>> emptyMatrix = new List<List<double>>();
            for (int i = 0; i < rows; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    row.Add(0);
                    iterations++;
                }
                emptyMatrix.Add(row);
            }
            return emptyMatrix;
        }
        public List<List<double>> GetUnitMatrix()
        {
            List<List<double>> unitMatrix = getEmptyMatrix();
            for (int i = 0; i < matrix[0].Count; i++)
            {
                unitMatrix[i][i] = 1;
                iterations++;
            }
            return unitMatrix;
        }
        public bool IsSymmetrical()
        {
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    iterations++;
                    if (matrix[i][j] != matrix[j][i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool IsIdentity()
        {
            int num = 0;
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    iterations++;
                    if (i != j && matrix[i][j] == 0)
                    {
                        num++;
                    }
                }
            }
            return num == matrix[0].Count * matrix[0].Count - matrix[0].Count;
        }
        public List<List<double>> GetTransposedMatrix()
        {
            List<List<double>> transposedMatrix = GetEmptyMatrix(matrix[0].Count, matrix.Count);
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    iterations++;
                    transposedMatrix[j][i] = matrix[i][j];
                }
            }
            return transposedMatrix;
        }
        public Matrix MatrixMultiplication(List<List<double>> matrixA)
        {
            int size = matrixA[0].Count;
            List<List<double>> resultMatrix = getEmptyMatrix();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        resultMatrix[i][j] += matrix[i][k] * matrixA[k][j];
                        iterations++;
                    }
                }
            }
            return new Matrix(resultMatrix);
        }
        public List<double> MatrixMultiplication(List<double> matrixA)
        {
            List<double> resultVector = new List<double>(matrix.Count);
            for (int i = 0; i < matrix.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    sum += matrix[i][j] * matrixA[j];
                    iterations++;
                }
                resultVector.Add(sum);
            }
            return resultVector;
        }
    }
}
