using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Coursework
{
    internal class Graph
    {
        private double minX;
        private double maxX;
        private double[] roots;
        private PlotModel plotModel;
        private double[] polynomialCoefficients;
        public Graph(double[] coefficients, double leftBoundary, double rightBoundary, double[] roots)
        {
            minX = leftBoundary - 5;
            maxX = rightBoundary + 5;
            polynomialCoefficients = coefficients;
            this.roots = roots;
        }

        public PlotModel buildGraph()
        {
            InitializePlotModel();
            AddPolynomialSeries();
            AddIntersectionPoints();
            AddAnnotations();
            AdjustAspectRatio();
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
                AxisTitleDistance = 5,
                AxisTickToLabelDistance = 0,
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "y",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,
                AxisTitleDistance = 5,
                AxisTickToLabelDistance = 0,
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
        }
        private void AddPolynomialSeries()
        {
            LineSeries series = new LineSeries { Color = OxyColors.Blue };
            int iterations = 0;

            for (double x = minX; x <= maxX; x += 0.1)
            {
                iterations++;
                if (iterations > 1e6)
                {
                    throw new Exception("Graph is too complex to build");
                }
                double y = GetPolynomialValue(x);
                if (!double.IsNaN(y) && Math.Abs(y) < 1e5)
                {
                    series.Points.Add(new DataPoint(x, y));
                }
            }
            plotModel.Series.Add(series);
        }
        private void AddIntersectionPoints()
        {
            foreach (var root in roots)
            {
                double y = GetPolynomialValue(root);
                double epsilon = Math.Pow(10,  -polynomialCoefficients.Length);
                double yLeft = GetPolynomialValue(root - epsilon);
                double yRight = GetPolynomialValue(root + epsilon);
                ScatterSeries intersectionPoint = new ScatterSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerFill = OxyColors.Red,
                    MarkerSize = 3
                };
                intersectionPoint.Points.Add(new ScatterPoint(root, y));
                LineSeries intersectionLine = new LineSeries
                {
                    Color = OxyColors.Blue,
                    StrokeThickness = 1
                };
                intersectionLine.Points.Add(new DataPoint(root - epsilon, yLeft));
                intersectionLine.Points.Add(new DataPoint(root, 0));
                intersectionLine.Points.Add(new DataPoint(root + epsilon, yRight));
                plotModel.Series.Add(intersectionPoint);
                plotModel.Series.Add(intersectionLine);
            }
        }
        private double GetPolynomialValue(double x)
        {
            double y = 0;
            for (int i = 0; i < polynomialCoefficients.Length; i++)
            {
                y += polynomialCoefficients[i] * Math.Pow(x, polynomialCoefficients.Length - 1 - i);
            }
            return y;
        }
        private void AdjustAspectRatio()
        {
            var xAxis = plotModel.Axes[0];
            var yAxis = plotModel.Axes[1];
            double yMin = double.MaxValue;
            double yMax = double.MinValue;
            for (double x = minX; x <= maxX; x += 0.01)
            {
                double y = GetPolynomialValue(x);
                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }
            double xRange = maxX - minX;
            double yRange = yMax - yMin;
            if (xRange > yRange)
            {
                double yMid = (yMax + yMin) / 2;
                double newYRange = xRange;
                yAxis.Zoom(yMid - newYRange / 2, yMid + newYRange / 2);
            }
            else
            {
                double xMid = (maxX + minX) / 2;
                double newXRange = yRange;
                xAxis.Zoom(xMid - newXRange / 2, xMid + newXRange / 2);
            }
        }
        private void AddAnnotations()
        {
            var verticalLine = new LineAnnotation
            {
                Color = OxyColors.Black,
                Type = LineAnnotationType.Vertical,
                X = 0,
                MinimumY = -100000000,
                MaximumY = 100000000,
                LineStyle = LineStyle.Solid
            };

            var horizontalLine = new LineAnnotation
            {
                Color = OxyColors.Black,
                Type = LineAnnotationType.Horizontal,
                Y = 0,
                MinimumX = -100000000,
                MaximumX = 100000000,
                LineStyle = LineStyle.Solid
            };

            plotModel.Annotations.Add(verticalLine);
            plotModel.Annotations.Add(horizontalLine);
        }
    }
}
