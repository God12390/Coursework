using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Coursework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class EigenPair
    {
        public double EigenValue { get; set; }
        public double[] EigenVector { get; set; }
        public string EigenVectorString => string.Join(", ", EigenVector);
    }
    public partial class MainWindow : Window
    {
        private Matrix matrix;
        private string selectedFileName;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MatrixSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int size))
                {
                    GenerateMatrixTextBoxes(size);
                }
                else
                {
                    MessageBox.Show("Invalid matrix size");
                }
            }
        }

        private void GenerateMatrixTextBoxes(int size)
        {
            MatrixGrid.Children.Clear();
            MatrixGrid.RowDefinitions.Clear();
            MatrixGrid.ColumnDefinitions.Clear();
            this.matrix = new Matrix(size);

            for (int i = 0; i < size; i++)
            {
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < size; i++)
            {
                MatrixGrid.RowDefinitions.Add(new RowDefinition());

                for (int j = 0; j < size; j++)
                {
                    Label label = new Label();
                    label.Content = $"A{i + 1}{j + 1}";
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.FontWeight = FontWeights.Bold;
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j * 2);
                    MatrixGrid.Children.Add(label);

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
                    MatrixGrid.Children.Add(textBox);

                    TrackTextBoxChanges(textBox, i, j);
                }
            }
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

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CalculateButton(object sender, RoutedEventArgs e)
        {
            List<double> eigenValues = new List<double>();
            List<List<double>> eigenVectors = new List<List<double>>();
            List<EigenPair> eigenPairs = new List<EigenPair>(); // Визначення змінної

            int method = SelectedMethod.SelectedIndex;
            if (method == 0)
            {
                MessageBox.Show("You should select the method");
                return;
            }
            else if (method == 1)
            {
                DanilevskiyMethod danilevskiyMethod = new DanilevskiyMethod(matrix);
                (eigenValues, List<Matrix> similarityMatrices) = danilevskiyMethod.GetEigenValues();
                eigenVectors = danilevskiyMethod.GetEigenVectors(eigenValues, similarityMatrices);
                eigenPairs = new List<EigenPair>();
                for (int i = 0; i < eigenValues.Count; i++)
                {
                    eigenPairs.Add(new EigenPair { EigenValue = eigenValues[i], EigenVector = eigenVectors[i].ToArray() });
                }
                EigenDataGrid.ItemsSource = eigenPairs;
            }
            else if (method == 2)
            {
                if (SelectedTolerance.SelectedItem != null)
                {
                    string toleranceValue = (SelectedTolerance.SelectedItem as ComboBoxItem).Content.ToString();
                    if (double.TryParse(toleranceValue, out double tolerance))
                    {
                        RotationMethod rotationMethod = new RotationMethod(matrix);
                        (eigenValues, List<Matrix> rotationMatrixes) = rotationMethod.findEigenvalues(tolerance);
                        eigenVectors = rotationMethod.findEigenVectors(rotationMatrixes, tolerance);
                        eigenPairs = new List<EigenPair>(); // Присвоєння нового значення
                        for (int i = 0; i < eigenValues.Count; i++)
                        {
                            eigenPairs.Add(new EigenPair { EigenValue = eigenValues[i], EigenVector = eigenVectors[i].ToArray() });
                        }
                        EigenDataGrid.ItemsSource = eigenPairs;
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while selecting tolerance");
                    }
                }
                else
                {
                    MessageBox.Show("An error occurred while selecting tolerance");
                }
            }
            ButtonSaveIntoFile.Visibility = eigenPairs.Any() ? Visibility.Visible : Visibility.Collapsed;
        }
        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            int size = matrix.matrix.Count;
            this.matrix = new Matrix(size);
            foreach (UIElement element in MatrixGrid.Children)
            {
                if (element is TextBox)
                {
                    TextBox textBox = element as TextBox;
                    textBox.Text = "0";
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
            }
        }
        private void ButtonSave(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = SelectedFile.Text;
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                List<EigenPair> eigenPairs = EigenDataGrid.ItemsSource as List<EigenPair>;
                if (eigenPairs != null && eigenPairs.Any())
                {
                    using (StreamWriter writer = new StreamWriter(selectedFilePath, true))
                    {
                        foreach (EigenPair pair in eigenPairs)
                        {
                            writer.WriteLine($"{pair.EigenValue} | [{pair.EigenVectorString}]");
                        }
                    }
                    MessageBox.Show("Data saved successfully.");
                }
                else
                {
                    MessageBox.Show("No data to save.");
                }
            }
            else
            {
                MessageBox.Show("Please select a file to save.");
            }
        }
        private void ButtonSelectFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = saveFileDialog.FileName;
                SelectedFile.Text = selectedFilePath;
            }
        }
    }
  
}
