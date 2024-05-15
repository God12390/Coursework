using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class RotationMethod
    {
        private Matrix matrix { get; set; }

        public RotationMethod(Matrix matrix)
        {
            this.matrix = matrix;
        }
        public double getRotationAngle(List<List<double>> matrix, int i, int j)
        {
            if (matrix[i][i] == matrix[j][j])
            {
                return Math.PI / 4.0;
            }
            return 0.5 * Math.Atan(2 * matrix[i][j] / (matrix[i][i] - matrix[j][j]));
        }
        public List<List<double>> calculateRotationMatrix(int size, int i, int j, double angle)
        {
            List<List<double>> unitMatrix = matrix.getUnitMatrix();
            unitMatrix[i][i] = Math.Cos(angle);
            unitMatrix[i][j] = -Math.Sin(angle);
            unitMatrix[j][i] = Math.Sin(angle);
            unitMatrix[j][j] = Math.Cos(angle);
            return unitMatrix;
        }

        public (double, int, int) getLargestNonDiagonalElement(List<List<double>> matrix)
        {
            double maxAbs = Math.Abs(matrix[0][1]);
            int rowIndex = 0, columnIndex = 1;
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    if (i != j && Math.Abs(matrix[i][j]) > maxAbs)
                    {
                        maxAbs = Math.Abs(matrix[i][j]);
                        rowIndex = i;
                        columnIndex = j;
                    }
                }
            }
            return (maxAbs, rowIndex, columnIndex);
        }
        public double sumOfSquaresOfNotDiagonalElements(List<List<double>> matrix)
        {
            double sum = 0;
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    if (i != j)
                    {
                        sum += matrix[i][j] * matrix[i][j];
                    }
                }
            }
            return sum;
        }
        public (List<double>, List<Matrix>) findEigenvalues(double epsilon)
        {
            matrix.isSymmetrical();
            List<Matrix> rotationMatrices = new List<Matrix>();
            Matrix currentMatrix = new Matrix(matrix.matrix);
            double sumOfSquares = sumOfSquaresOfNotDiagonalElements(currentMatrix.matrix);
            while (sumOfSquares > epsilon)
            {
                (double maxValue, int i, int j) = getLargestNonDiagonalElement(currentMatrix.matrix);
                double angle = getRotationAngle(currentMatrix.matrix, i, j);
                Matrix rotationMatrix = new Matrix(calculateRotationMatrix(currentMatrix.matrix[0].Count, i, j, angle));
                rotationMatrices.Add(rotationMatrix);
                Matrix transposedMatrix = new Matrix(rotationMatrix.getTransposedMatrix());
                currentMatrix = transposedMatrix.matrixMultiplication(currentMatrix.matrix).matrixMultiplication(rotationMatrix.matrix);
                sumOfSquares = sumOfSquaresOfNotDiagonalElements(currentMatrix.matrix);
            }
            List<double> eigenvalues = extractEigenvalues(currentMatrix.matrix, epsilon);
            return (eigenvalues, rotationMatrices);
        }

        private List<double> extractEigenvalues(List<List<double>> matrix, double epsilon)
        {
            List<double> eigenvalues = new List<double>();
            int decimalPlaces = (epsilon < 1) ? (int)Math.Ceiling(-Math.Log10(epsilon)) : 2;
            for (int i = 0; i < matrix.Count; i++)
            {
                eigenvalues.Add(Math.Round(matrix[i][i], decimalPlaces));
            }
            return eigenvalues;
        }
        private List<List<double>> extractEigenVectors(List<List<double>> matrix, double epsilon)
        {
            int decimalPlaces = (epsilon < 1) ? (int)Math.Ceiling(-Math.Log10(epsilon)) : 2;
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix.Count; j++)
                {
                    matrix[i][j] = Math.Round(matrix[i][j], decimalPlaces);
                }
            }
            return matrix;
        }

        public List<List<double>> findEigenVectors(List<Matrix> rotationMatrixes, double epsilon)
        {
            Matrix resultMatrix = rotationMatrixes[0];

            foreach (Matrix matrix in rotationMatrixes.Skip(1))
            {
                resultMatrix = resultMatrix.matrixMultiplication(matrix.matrix);
            }
            int digitsAfterComma = (epsilon < 1) ? (int)Math.Ceiling(-Math.Log10(epsilon)) : 2;
            Matrix eigenVectors = new Matrix(matrix.getEmptyMatrix());
            eigenVectors.matrix = extractEigenVectors(resultMatrix.matrix, epsilon);
            eigenVectors.matrix = eigenVectors.getTransposedMatrix();
            return eigenVectors.matrix;
        }
    }
}
