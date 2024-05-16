using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace Coursework
{
    internal class Graph
    {
        private double minX;
        private double maxX;
        private PlotModel plotModel;
        private double[] polynomialCoefficients;

        public Graph(double[] coefficients, double leftBoundary, double rightBoundary)
        {
            minX = leftBoundary;
            maxX = rightBoundary;
            polynomialCoefficients = coefficients;
        }

        public PlotModel BuildGraph()
        {
            InitializePlotModel();
            AddPolynomialSeries();
            AddAnnotations();
            return plotModel;
        }

        private void InitializePlotModel()
        {
            plotModel = new PlotModel { Title = "Polynomial Graph" };

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "x",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,
                AxisTitleDistance = 20,
                AxisTickToLabelDistance = 0,
                MinimumRange = 1,
                AbsoluteMaximum = 500,
                StartPosition = 0,
                AbsoluteMinimum = -500
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "y",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,
                AxisTitleDistance = 20,
                AxisTickToLabelDistance = 0,
                MinimumRange = 1,
                StartPosition = 0,
                AbsoluteMaximum = 500,
                AbsoluteMinimum = -500
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
        }

        private void AddPolynomialSeries()
        {
            LineSeries series = new LineSeries { Color = OxyColors.Blue };

            for (double x = minX - 10; x <= maxX + 10; x += 0.01)
            {
                double y = CalculatePolynomialValue(x);
                if (!double.IsNaN(y))
                {
                    series.Points.Add(new DataPoint(x, y));
                }
            }

            plotModel.Series.Add(series);
        }

        private double CalculatePolynomialValue(double x)
        {
            double y = 0;
            for (int i = 0; i < polynomialCoefficients.Length; i++)
            {
                y += polynomialCoefficients[i] * Math.Pow(x, polynomialCoefficients.Length - 1 - i);
            }
            return y;
        }

        private void AddAnnotations()
        {
            var verticalLine = new LineAnnotation
            {
                Color = OxyColors.Black,
                Type = LineAnnotationType.Vertical,
                X = 0,
                MinimumY = -1000000,
                MaximumY = 1000000,
                LineStyle = LineStyle.Solid
            };

            var horizontalLine = new LineAnnotation
            {
                Color = OxyColors.Black,
                Type = LineAnnotationType.Horizontal,
                Y = 0,
                MinimumX = -1000000,
                MaximumX = 1000000,
                LineStyle = LineStyle.Solid
            };

            plotModel.Annotations.Add(verticalLine);
            plotModel.Annotations.Add(horizontalLine);
        }
    }
}