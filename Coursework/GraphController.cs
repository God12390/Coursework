using OxyPlot;

namespace Coursework
{

    internal class GraphController
    {
        private Graph graph;

        public GraphController(double[] polynomialCoefficients, double minX, double maxX)
        {
            graph = new Graph(polynomialCoefficients, minX, maxX);
        }

        public PlotModel BuildGraph()
        {
            return graph.BuildGraph();
        }
    }
  
}
