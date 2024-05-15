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
        public Matrix(List<List<double>> matrix)
        {
            this.matrix = matrix;
        }
        public Matrix(int size)
        {
            this.matrix = getEmptyMatrix(size);
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
                }
                emptyMatrix.Add(row);
            }
            return emptyMatrix;
        }
        public List<List<double>> getEmptyMatrix(int size)
        {
            List<List<double>> emptyMatrix = new List<List<double>>();
            for (int i = 0; i < size; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < size; j++)
                {
                    row.Add(0);
                }
                emptyMatrix.Add(row);
            }
            return emptyMatrix;
        }
        public List<List<double>> getUnitMatrix()
        {
            List<List<double>> unitMatrix = getEmptyMatrix();
            for (int i = 0; i < matrix[0].Count; i++)
            {
                unitMatrix[i][i] = 1;
            }
            return unitMatrix;
        }
        public void isSymmetrical()
        {
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    if (matrix[i][j] != matrix[j][i])
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }
        public List<List<double>> getTransposedMatrix()
        {
            List<List<double>> transposedMatrix = getEmptyMatrix();
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    transposedMatrix[j][i] = matrix[i][j];
                }
            }
            return transposedMatrix;
        }
        public Matrix matrixMultiplication(List<List<double>> matrixA)
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
                }
                resultVector.Add(sum);
            }
            return resultVector;
        }
    }
}
