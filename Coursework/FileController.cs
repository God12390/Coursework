using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Coursework
{
    internal class FileController
    {
        public void SaveEigenPairsToFile(string filePath, List<EigenPair> eigenPairs)
        {
            if (!string.IsNullOrEmpty(filePath) && eigenPairs != null && eigenPairs.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    foreach (EigenPair pair in eigenPairs)
                    {
                        writer.WriteLine($"Eigen value: {pair.EigenValue} | Eigen vector: [{pair.EigenVectorString}]");
                    }
                    writer.WriteLine(new string('-', 35));
                }
                MessageBox.Show("Data saved successfully.");
            }
            else
            {
                MessageBox.Show("No data to save or invalid file path.");
            }
        }
    }
}