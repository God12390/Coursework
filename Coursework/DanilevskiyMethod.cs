using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
namespace Coursework
{
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
            if(eigenValues.Count == 0)
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
    }
}
