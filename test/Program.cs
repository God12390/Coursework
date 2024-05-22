using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
namespace MyApp
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
            this.matrix = getEmptyMatrix(size, size);
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
        public List<List<double>> getEmptyMatrix(int rows, int cols)
        {
            List<List<double>> emptyMatrix = new List<List<double>>();
            for (int i = 0; i < rows; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < cols; j++)
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
        public bool isSymmetrical()
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
            return true;
        }
        public List<List<double>> getTransposedMatrix()
        {
            List<List<double>> transposedMatrix = getEmptyMatrix(matrix[0].Count, matrix.Count);
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
    internal class DanilevskiyMethod
    {
        private readonly Matrix matrix;

        public DanilevskiyMethod(Matrix matrix)
        {
            this.matrix = matrix;
        }
        public List<List<double>> FindInverseMatrix(List<List<double>> matrixToInvert)
        {
            Matrix<double> convertedMatrix = Matrix<double>.Build.DenseOfRows(matrixToInvert);
            Matrix<double> invertedMatrix = convertedMatrix.Inverse();
            List<List<double>> invertedMatrixList = new List<List<double>>();
            for (int i = 0; i < invertedMatrix.RowCount; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < invertedMatrix.ColumnCount; j++)
                {
                    row.Add(invertedMatrix[i, j]);
                }
                invertedMatrixList.Add(row);
            }
            return invertedMatrixList;
        }

        public List<List<double>> GetB(List<List<double>> matrixA, int row)
        {
            List<List<double>> B = matrix.getUnitMatrix();
            for (int x = 0; x < matrixA.Count; x++)
            {
                if (row - 1 != x)
                    B[row - 1][x] = -matrixA[row][x] / matrixA[row][row - 1];
                else
                    B[row - 1][row - 1] = 1 / matrixA[row][row - 1];
            }
            return B;
        }

        public (Matrix, List<Matrix>) GetNormalForm()
        {
            Matrix A = new Matrix(matrix.matrix);
            List<Matrix> arrayB = new List<Matrix>();
            for (int i = A.matrix.Count - 1; i > 0; i--)
            {
                Matrix B = new Matrix(GetB(A.matrix, i));
                arrayB.Add(B);
                Matrix BReverse = new Matrix(FindInverseMatrix(B.matrix));
                A = (BReverse.matrixMultiplication(A.matrix)).matrixMultiplication(B.matrix);
            }
            return (A, arrayB);
        }
        public (List<double>, List<Matrix>, double[]) GetEigenValues()
        {
            var (coefficientsMatrix, arrayB) = GetNormalForm();
            List<double> polynomialCoefficients = coefficientsMatrix.matrix[0];
            double[] coefficients = new double[polynomialCoefficients.Count + 1];
            coefficients[0] = 1;
            for (int i = 0; i < polynomialCoefficients.Count; i++)
            {
                coefficients[i + 1] = -polynomialCoefficients[i];
            }
            Array.Reverse(coefficients);
            Polynomial poly = new Polynomial(coefficients);

            Complex[] roots = poly.Roots();
            List<double> eigenValues = new List<double>();
            foreach (var root in roots)
            {
                if (root.Imaginary == 0)
                {
                    eigenValues.Add(root.Real);
                }
            }
            Array.Reverse(coefficients);
            return (eigenValues, arrayB, coefficients);
        }
        public List<List<double>> GetEigenVectors(List<double> ownValues, List<Matrix> similarityMatrices)
        {
            Matrix similarityMatrix = similarityMatrices[0];
            for (int i = 1; i < similarityMatrices.Count; i++)
            {
                similarityMatrix = similarityMatrix.matrixMultiplication(similarityMatrices[i].matrix);
            }
            Matrix ownVectors = new Matrix(Enumerable.Range(0, matrix.matrix.Count).Select(k => ownValues.Select(val => Math.Pow(val, matrix.matrix.Count - k - 1)).ToList()).ToList());
            List<List<double>> transposedVectors = ownVectors.getTransposedMatrix();

            for (int i = 0; i < transposedVectors.Count; i++)
            {
                transposedVectors[i] = similarityMatrix.MatrixMultiplication(transposedVectors[i]);
            }
            return transposedVectors;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string input;
            while(true)
            {
                input = Console.ReadLine();
                if (Double.TryParse(input, out double c))
                {
                    if (double.IsPositiveInfinity(c))
                    {
                        Console.WriteLine("Переповнення: значення дорівнює PositiveInfinity");
                    }
                    else if (double.IsNegativeInfinity(c))
                    {
                        Console.WriteLine("Переповнення: значення дорівнює NegativeInfinity");
                    }
                    else if (double.IsNaN(c))
                    {
                        Console.WriteLine("Помилка: значення дорівнює NaN (Not a Number)");
                    }
                    else
                    {
                        Console.WriteLine(c);
                    }
                }
                else
                {
                    Console.WriteLine("Не вдалося розпарсити значення.");
                }


            }

        }
    }
}