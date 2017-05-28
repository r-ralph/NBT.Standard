using BenchmarkDotNet.Running;

namespace Benchmark
{
    public class Program
    {
        #region Static Methods

        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TagWriterBenchmark>();
        }

        #endregion
    }
}