using Coursework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

public enum Method
{
    Danilevskiy,
    Rotation
}
namespace Coursework
{
    public partial class MainWindow : Window
    {
        private MatrixController matrixController;
        private GraphController graphController;
        private FileController fileController = new FileController();
        private List<EigenPair> eigenPairs;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void matrixSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int size))
                {
                    matrixController = new MatrixController(size);
                    matrixController.generateMatrixTextBoxes(MatrixGrid);
                }
                else
                {
                    MessageBox.Show("Invalid matrix size");
                }
            }
        }

        private void exitButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void calculateButton(object sender, RoutedEventArgs e)
        {
            Method method = SelectedMethod.SelectedIndex == 0 ? Method.Danilevskiy : Method.Rotation;
            if (matrixController == null || !matrixController.validateMatrixData(MatrixGrid))
            {
                MessageBox.Show("Please enter valid decimal values in all matrix cells.");
                return;
            }
            if (SelectedMethod.SelectedIndex == -1)
            {
                MessageBox.Show("You should select the method");
                return;
            }
            try
            {
                switch (method)
                {
                    case Method.Danilevskiy:
                        matrixController.CalculateDanilevskiy();
                        graphController = new GraphController(matrixController.polynomialCoefficients, matrixController.eigenValues.Min(), 
                                                              matrixController.eigenValues.Max(), matrixController.eigenValues.ToArray());
                        break;
                    case Method.Rotation:
                        double tolerance = double.Parse((SelectedTolerance.SelectedItem as ComboBoxItem)?.Content.ToString());
                        matrixController.CalculateRotation(tolerance);
                        break;
                    default:
                        break;
                }
                eigenPairs = matrixController.eigenValues.Select((value, index) => new EigenPair {
                    EigenValue = value,
                    EigenVector = matrixController.eigenVectors[index].ToArray()
                }).ToList();

                EigenDataGrid.ItemsSource = eigenPairs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonClear(object sender, RoutedEventArgs e)
        {
            matrixController.matrix = new Matrix(matrixController.matrix.matrix.Count);
            matrixController?.clearMatrixData(MatrixGrid);
            plotView.Visibility = Visibility.Collapsed;
            EigenDataGrid.ItemsSource = null;
        }

        private void buttonSave(object sender, RoutedEventArgs e)
        {
            var selectedFilePath = SelectedFile.Text;
            fileController.saveToFile(selectedFilePath, eigenPairs, matrixController.matrix.matrix);
        }
        private void GenerateMatrixButton(object sender, RoutedEventArgs e)
        {
            matrixController.generateRandomMatrix(MatrixGrid);
        }
        private void ButtonShowComplexity(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Number of iterations: {matrixController.iterations}");
        }
        private void buttonSelectFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                SelectedFile.Text = saveFileDialog.FileName;
            }
        }

        private void buildGraphButton(object sender, RoutedEventArgs e)
        {
            plotView.Visibility = Visibility.Visible;
            try
            {
            plotView.Model = graphController?.buildGraph();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}