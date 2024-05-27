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
        public int iterations = 0;
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
    internal class DanilevskiyMethod
    {
        public Matrix matrix;

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
                    matrix.iterations++;
                }
                invertedMatrixList.Add(row);
            }
            return invertedMatrixList;
        }

        public List<List<double>> GetB(List<List<double>> matrixA, int row)
        {
            List<List<double>> B = matrix.GetUnitMatrix();
            for (int x = 0; x < matrixA.Count; x++)
            {
                if (row - 1 != x)
                    B[row - 1][x] = -matrixA[row][x] / matrixA[row][row - 1];
                else
                    B[row - 1][row - 1] = 1 / matrixA[row][row - 1];
                matrix.iterations++;
            }
            return B;
        }

        public (Matrix, List<Matrix>) GetNormalForm()
        {
            Matrix A = new Matrix(matrix.matrix);
            List<Matrix> arrayB = new List<Matrix>();
            for (int i = A.matrix.Count - 1; i > 0; i--)
            {
                matrix.iterations++;
                if (A.matrix[i][i - 1] == 0)
                {
                    for (int j = 0; j < A.matrix.Count; j++)
                    {
                        double temp = A.matrix[j][i];
                        A.matrix[j][i] = A.matrix[j][i - 1];
                        A.matrix[j][i - 1] = temp;
                    }
                    List<double> tempRow = new List<double>(A.matrix[i]);
                    A.matrix[i] = new List<double>(A.matrix[i - 1]);
                    A.matrix[i - 1] = tempRow;
                }

                Matrix B = new Matrix(GetB(A.matrix, i));
                arrayB.Add(B);
                Matrix BReverse = new Matrix(FindInverseMatrix(B.matrix));
                A = (BReverse.MatrixMultiplication(A.matrix)).MatrixMultiplication(B.matrix);
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
                matrix.iterations++;
                coefficients[i + 1] = -polynomialCoefficients[i];
            }
            foreach (var item in coefficients)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();
            Array.Reverse(coefficients);
            Polynomial poly = new Polynomial(coefficients);

            Complex[] roots = poly.Roots();
            List<double> eigenValues = new List<double>();
            foreach (var root in roots)
            {
                matrix.iterations++;
                if (root.Imaginary == 0)
                {
                    eigenValues.Add(root.Real);
                }
            }
            if (eigenValues.Count == 0)
            {
                throw new Exception("There are no real solution");
            }
            Array.Reverse(coefficients);
            return (eigenValues, arrayB, coefficients);
        }
        public List<List<double>> GetEigenVectors(List<double> ownValues, List<Matrix> similarityMatrices)
        {
            Matrix similarityMatrix = similarityMatrices[0];
            for (int i = 1; i < similarityMatrices.Count; i++)
            {
                similarityMatrix = similarityMatrix.MatrixMultiplication(similarityMatrices[i].matrix);
            }
            Matrix ownVectors = new Matrix(Enumerable.Range(0, matrix.matrix.Count).Select(k => ownValues.Select(val => Math.Pow(val, matrix.matrix.Count - k - 1)).ToList()).ToList());
            List<List<double>> transposedVectors = ownVectors.GetTransposedMatrix();

            for (int i = 0; i < transposedVectors.Count; i++)
            {
                transposedVectors[i] = similarityMatrix.MatrixMultiplication(transposedVectors[i]);
            }

            return transposedVectors;
        }
        public bool CheckEigenVectors(List<double> eigenValues, List<List<double>> eigenVectors)
        {
            double epsilon = 1e-6; // Точність перевірки

            for (int i = 0; i < eigenValues.Count; i++)
            {
                List<double> Ax = matrix.MatrixMultiplication(eigenVectors[i]); // Обчислення A * x_i
                List<double> lambda_x = eigenVectors[i].Select(val => val * eigenValues[i]).ToList(); // Обчислення λ_i * x_i

                // Порівняння векторів Ax і λx
                for (int j = 0; j < Ax.Count; j++)
                {
                    if (Math.Abs(Ax[j] - lambda_x[j]) > epsilon)
                    {
                        return false; // Власний вектор не є правильним для вказаного власного значення
                    }
                }
            }

            return true; // Власні вектори є правильними
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Matrix matrix = new Matrix(new List<List<double>> { new List<double> { 4, 25, 12} ,
                                                                new List<double> { 0, 5, 12},
                                                                new List<double> { 2, 15, 2 }
                                                               });
            DanilevskiyMethod danilevskiyMethod = new DanilevskiyMethod(matrix);
            var(a, b, c) =  danilevskiyMethod.GetEigenValues();
            var d = danilevskiyMethod.GetEigenVectors(a, b);
            foreach (var item in a)
            {
                Console.WriteLine(item);
            }
            foreach (var item in d)
            {
                Console.WriteLine(String.Join(" ", item));
            }


            Console.WriteLine(danilevskiyMethod.CheckEigenVectors(a, d));
        }
    }
}