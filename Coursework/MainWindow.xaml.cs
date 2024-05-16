using Coursework;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Coursework
{
    public partial class MainWindow : Window
    {
        private MatrixController matrixController;
        private GraphController graphController;
        private FileController fileController;
        private List<EigenPair> eigenPairs;

        public MainWindow()
        {
            InitializeComponent();
            fileController = new FileController();
        }

        private void MatrixSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int size))
                {
                    matrixController = new MatrixController(size);
                    matrixController.GenerateMatrixTextBoxes(MatrixGrid);
                }
                else
                {
                    MessageBox.Show("Invalid matrix size");
                }
            }
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CalculateButton(object sender, RoutedEventArgs e)
        {
            if (matrixController == null || !matrixController.ValidateMatrixData(MatrixGrid))
            {
                MessageBox.Show("Please enter valid decimal values in all matrix cells.");
                return;
            }

            int method = SelectedMethod.SelectedIndex;
            if (method == 0)
            {
                MessageBox.Show("You should select the method");
                return;
            }

            var matrix = matrixController.GetMatrix();
            var eigenValues = new List<double>();
            var eigenVectors = new List<List<double>>();

            if (method == 1)
            {
                var danilevskiyMethod = new DanilevskiyMethod(matrix);
                (eigenValues, var similarityMatrices, var polynomialCoefficients) = danilevskiyMethod.GetEigenValues();
                graphController = new GraphController(polynomialCoefficients, eigenValues.Min(), eigenValues.Max());
                eigenVectors = danilevskiyMethod.GetEigenVectors(eigenValues, similarityMatrices);
            }
            else if (method == 2)
            {
                var toleranceValue = (SelectedTolerance.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (double.TryParse(toleranceValue, out var tolerance))
                {
                    var rotationMethod = new RotationMethod(matrix);
                    (eigenValues, var rotationMatrices) = rotationMethod.findEigenvalues(tolerance);
                    eigenVectors = rotationMethod.findEigenVectors(rotationMatrices, tolerance);
                }
            }

            eigenPairs = eigenValues.Select((value, index) => new EigenPair
            {
                EigenValue = value,
                EigenVector = eigenVectors[index].ToArray()
            }).ToList();

            EigenDataGrid.ItemsSource = eigenPairs;
            ButtonSaveIntoFile.Visibility = eigenPairs.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            matrixController?.ClearMatrixData(MatrixGrid);
            plotView.Visibility = Visibility.Collapsed;
            EigenDataGrid.ItemsSource = null;
        }

        private void ButtonSave(object sender, RoutedEventArgs e)
        {
            var selectedFilePath = SelectedFile.Text;
            fileController.SaveEigenPairsToFile(selectedFilePath, eigenPairs);
        }

        private void ButtonSelectFile(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SelectedFile.Text = saveFileDialog.FileName;
            }
        }

        private void BuildGraphButton(object sender, RoutedEventArgs e)
        {
            plotView.Visibility = Visibility.Visible;
            plotView.Model = graphController?.BuildGraph();
        }
    }
}