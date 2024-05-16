using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Coursework
{
    internal class MatrixController
    {
        private Matrix matrix;

        public MatrixController(int size)
        {
            matrix = new Matrix(size);
        }

        public void GenerateMatrixTextBoxes(Grid matrixGrid)
        {
            matrixGrid.Children.Clear();
            matrixGrid.RowDefinitions.Clear();
            matrixGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < matrix.matrix[0].Count; i++)
            {
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < matrix.matrix[0].Count; i++)
            {
                matrixGrid.RowDefinitions.Add(new RowDefinition());

                for (int j = 0; j < matrix.matrix[0].Count; j++)
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
                    textBox.MaxLength = 5;
                    textBox.Width = 50;
                    textBox.Height = 23;
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.Margin = new Thickness(5);

                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j * 2 + 1);
                    matrixGrid.Children.Add(textBox);

                    TrackTextBoxChanges(textBox, i, j);
                }
            }
        }
        public bool ValidateMatrixData(Grid matrixGrid)
        {
            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox)
                {
                    TextBox textBox = element as TextBox;
                    string input = textBox.Text.Trim();
                    if (string.IsNullOrEmpty(input) || !double.TryParse(input, out _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void TrackTextBoxChanges(TextBox textBox, int rowIndex, int columnIndex)
        {
            textBox.LostFocus += (sender, e) =>
            {
                string input = textBox.Text;
                if (!input.Contains(" ") && double.TryParse(input, out double value))
                {
                    matrix.matrix[rowIndex][columnIndex] = value;
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
                else
                {
                    textBox.BorderBrush = Brushes.Red;
                    MessageBox.Show("Invalid input. Please enter a valid decimal value.");
                }
            };
        }
        public Matrix GetMatrix()
        {
            return matrix;
        }

        public void ClearMatrixData(Grid matrixGrid)
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
