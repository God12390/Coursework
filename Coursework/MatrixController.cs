using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
namespace Coursework
{
    internal class MatrixController
    {
        private Matrix matrix;
        private int iterations;
        private List<double> eigenValues;
        private List<List<double>> eigenVectors;
        private double[] polynomialCoefficients;
        private long calculationTime;


        public Matrix Matrix { get => matrix; set=> matrix = value; }
        public int Iterations { get => iterations; set => iterations = value; }
        public List<double> EigenValues { get => eigenValues; set => eigenValues = value; }
        public List<List<double>> EigenVectors { get => eigenVectors; set => eigenVectors = value; }
        public double[] PolynomialCoefficients { get => polynomialCoefficients; set => polynomialCoefficients = value; }
        public long CalculationTime { get => calculationTime; set => calculationTime = value; }


        public MatrixController(int size)
        {
            matrix = new Matrix(size);
        }
        public void ResetIterations() => matrix.Iterations = 0;
        public void generateMatrixTextBoxes(Grid matrixGrid)
        {
            matrixGrid.Children.Clear();
            matrixGrid.RowDefinitions.Clear();
            matrixGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < matrix.MatrixData[0].Count; i++)
            {
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < matrix.MatrixData[0].Count; i++)
            {
                matrixGrid.RowDefinitions.Add(new RowDefinition());

                for (int j = 0; j < matrix.MatrixData[0].Count; j++)
                {
                    Label label = new Label();
                    label.Content = $"A{i + 1}{j + 1}";
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.FontWeight = FontWeights.Bold;
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j * 2);
                    matrixGrid.Children.Add(label);

                    TextBox textBox = new TextBox();
                    textBox.Text = "0";
                    textBox.TextAlignment = TextAlignment.Center;
                    textBox.MaxLength = 14;
                    textBox.Width = 50;
                    textBox.Height = 23;
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.Margin = new Thickness(5);

                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j * 2 + 1);
                    matrixGrid.Children.Add(textBox);

                    trackTextBoxChanges(textBox, i, j);
                }
            }
        }
        public void  CalculateDanilevskiy()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                ResetIterations();
                DanilevskiyMethod danilevskiyMethod = new DanilevskiyMethod(matrix);
                (eigenValues, List<Matrix> similarityMatrices, double[] polyCoeffs) = danilevskiyMethod.GetEigenValues();
                polynomialCoefficients = polyCoeffs;
                eigenVectors = danilevskiyMethod.GetEigenVectors(eigenValues, similarityMatrices);
                iterations = danilevskiyMethod.Matrix.Iterations;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Danilevskiy method calculation: {ex.Message}");
            }
            stopwatch.Stop();
            calculationTime = stopwatch.ElapsedMilliseconds;
        }
        public void CalculateRotation(double tolerance)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                ResetIterations();
                RotationMethod rotationMethod = new RotationMethod(matrix);
                (eigenValues, List<Matrix> rotationMatrices) = rotationMethod.GetEigenvalues(tolerance);
                eigenVectors = rotationMethod.GetEigenVectors(rotationMatrices, tolerance);
                iterations = rotationMethod.Matrix.Iterations;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Rotation method calculation: {ex.Message}");
            }
            stopwatch.Stop();
            calculationTime = stopwatch.ElapsedMilliseconds;
        }
        public void generateRandomMatrix(Grid matrixGrid)
        {
            Random random = new Random();
            int rowCount = matrix.MatrixData.Count;
            int columnCount = matrix.MatrixData[0].Count;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    double value =  (random.NextDouble() * 10000);
                    matrix.MatrixData[i][j] = value;
                    TextBox textBox = matrixGrid.Children.OfType<TextBox>().FirstOrDefault(tb => Grid.GetRow(tb) == i && Grid.GetColumn(tb) == j * 2 + 1);
                    textBox.Text = $"{value:F2}";
                }
            }
        }
        public bool validateMatrixData(Grid matrixGrid)
        {
            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox)
                {
                    TextBox currentTextBox = element as TextBox;
                    string input = currentTextBox.Text.Trim();
                    if (string.IsNullOrEmpty(input) || !IsValidDouble(input))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool IsValidDouble(string input)
        {
            double min = -10000, max = 10000;
            int decimalPart = 5;
            if (double.TryParse(input, out double value))
            {
                if (double.IsInfinity(value))
                {
                    MessageBox.Show($"Value should be in range[{min}, {max}]");
                    return false;
                }
                string[] parts = input.Split('.');
                if (parts.Length > 1 && parts[1].Length > decimalPart)
                {
                    MessageBox.Show($"Max decimal part is {decimalPart} symbols");
                    return false;
                }
                if (value >= min && value <= max)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show($"Value should be in range[{min}, {max}]");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Enter a real number");
                return false;
            }
        }
        private void trackTextBoxChanges(TextBox textBox, int rowIndex, int columnIndex)
        {
            textBox.LostFocus += (sender, e) =>
            {
                string input = textBox.Text;
                if (!input.Contains(" ") && IsValidDouble(input))
                {
                    matrix.MatrixData[rowIndex][columnIndex] = double.Parse(input);
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
                else
                {
                    textBox.BorderBrush = Brushes.Red;
                }
            };
        }
        public Matrix getMatrix()
        {
            return matrix;
        }
        public void clearMatrixData(Grid matrixGrid)
        {
            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    textBox.Text = "0";
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
            }
        }
    }
}
