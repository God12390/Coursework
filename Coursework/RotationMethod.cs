using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class RotationMethod
    {
        public Matrix matrix { get; set; }
        public RotationMethod(Matrix matrix)
        {
            this.matrix = matrix;
        }
        public double GetRotationAngle(List<List<double>> matrix, int i, int j)
        {
            if (matrix[i][i] - matrix[j][j] == 0)
            {
                return Math.PI / 4.0;
            }
            return 0.5 * Math.Atan(2 * matrix[i][j] / (matrix[i][i] - matrix[j][j]));
        }
        public List<List<double>> GetRotationMatrix(int size, int i, int j, double angle)
        {
            List<List<double>> unitMatrix = matrix.GetUnitMatrix();
            unitMatrix[i][i] = Math.Cos(angle);
            unitMatrix[i][j] = -Math.Sin(angle);
            unitMatrix[j][i] = Math.Sin(angle);
            unitMatrix[j][j] = Math.Cos(angle);
            return unitMatrix;
        }

        public (int, int) GetLargestNonDiagonalElement(List<List<double>> matrix)
        {
            double maxAbs = Math.Abs(matrix[0][1]);
            int rowIndex = 0, columnIndex = 1;
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    this.matrix.iterations += 1;
                    if (i != j && Math.Abs(matrix[i][j]) > maxAbs)
                    {
                        maxAbs = Math.Abs(matrix[i][j]);
                        rowIndex = i;
                        columnIndex = j;
                    }
                }
            }
            return (rowIndex, columnIndex);
        }
        public double SumOfSquaresOfNotDiagonalElements(List<List<double>> matrix)
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
        public (List<double>, List<Matrix>) GetEigenvalues(double epsilon)
        {
            if(!matrix.IsSymmetrical())
            {
                throw new ArgumentException("Matrix should be symmetrical");
            }
            if(matrix.IsIdentity())
            {
                throw new ArgumentException("Matrix cannot be identity");
            }
            List<Matrix> rotationMatrices = new List<Matrix>();
            Matrix currentMatrix = new Matrix(matrix.matrix);
            double sumOfSquares = SumOfSquaresOfNotDiagonalElements(currentMatrix.matrix);
            while (sumOfSquares > epsilon)
            {
                (int i, int j) = GetLargestNonDiagonalElement(currentMatrix.matrix);
                double angle = GetRotationAngle(currentMatrix.matrix, i, j);
                Matrix rotationMatrix = new Matrix(GetRotationMatrix(currentMatrix.matrix[0].Count, i, j, angle));
                rotationMatrices.Add(rotationMatrix);
                Matrix transposedMatrix = new Matrix(rotationMatrix.GetTransposedMatrix());
                currentMatrix = transposedMatrix.MatrixMultiplication(currentMatrix.matrix).MatrixMultiplication(rotationMatrix.matrix);
                sumOfSquares = SumOfSquaresOfNotDiagonalElements(currentMatrix.matrix);
            }
            List<double> eigenvalues = ExtractEigenvalues(currentMatrix.matrix, epsilon);
            return (eigenvalues, rotationMatrices);
        }

        private List<double> ExtractEigenvalues(List<List<double>> matrix, double epsilon)
        {
            List<double> eigenvalues = new List<double>();
            int decimalPlaces = (epsilon < 1) ? (int)Math.Ceiling(-Math.Log10(epsilon)) : 2;
            for (int i = 0; i < matrix.Count; i++)
            {
                this.matrix.iterations++;
                eigenvalues.Add(Math.Round(matrix[i][i], decimalPlaces));
            }
            return eigenvalues;
        }
        private List<List<double>> ExtractEigenVectors(List<List<double>> matrix, double epsilon)
        {
            int decimalPlaces = (epsilon < 1) ? (int)Math.Ceiling(-Math.Log10(epsilon)) : 2;
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix.Count; j++)
                {
                    this.matrix.iterations++;
                    matrix[i][j] = Math.Round(matrix[i][j], decimalPlaces);
                }
            }
            return matrix;
        }

        public List<List<double>> GetEigenVectors(List<Matrix> rotationMatrixes, double epsilon)
        {
            Matrix resultMatrix = rotationMatrixes[0];

            foreach (Matrix matrix in rotationMatrixes.Skip(1))
            {
                this.matrix.iterations++;
                resultMatrix = resultMatrix.MatrixMultiplication(matrix.matrix);
            }
            Matrix eigenVectors = new Matrix(matrix.getEmptyMatrix());
            eigenVectors.matrix = ExtractEigenVectors(resultMatrix.matrix, epsilon);
            eigenVectors.matrix = eigenVectors.GetTransposedMatrix();
            return eigenVectors.matrix;
        }
    }
}
