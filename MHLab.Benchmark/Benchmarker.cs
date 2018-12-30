using System;
using System.Diagnostics;

namespace MHLab.Benchmark
{
    public struct BenchmarkParameters
    {
        public long BenchmarkIterations;

        public bool PerformWarmup;
        public long WarmupIterations;
        public Action WarmupAction;
    }

    public struct BenchmarkResult
    {
        public TimeSpan Elapsed;
        public long ElapsedMilliseconds;
        public long ElapsedTicks;
        public long AverageMilliseconds;
        public long AverageTicks;
        public int GarbageCollections0Count;
        public int GarbageCollections1Count;
        public int GarbageCollections2Count;
        public long Iterations;
    }

    public struct BenchmarkComparison
    {
        public BenchmarkResult CompareFrom;
        public BenchmarkResult CompareTo;

        public float ElapsedMillisecondsDifference;
        public float ElapsedMillisecondsDifferencePercentage;

        public float ElapsedTicksDifference;
        public float ElapsedTicksDifferencePercentage;

        public float AverageMillisecondsDifference;
        public float AverageMillisecondsDifferencePercentage;

        public float AverageTicksDifference;
        public float AverageTicksDifferencePercentage;

        public float GCCollections0Difference;
        public float GCCollections0DifferencePercentage;

        public float GCCollections1Difference;
        public float GCCollections1DifferencePercentage;

        public float GCCollections2Difference;
        public float GCCollections2DifferencePercentage;

        internal void Compare(BenchmarkResult from, BenchmarkResult to)
        {
            CompareFrom = from;
            CompareTo = to;

            ElapsedMillisecondsDifference = to.ElapsedMilliseconds - from.ElapsedMilliseconds;
            ElapsedMillisecondsDifferencePercentage = (ElapsedMillisecondsDifference / from.ElapsedMilliseconds) * 100;

            ElapsedTicksDifference = to.ElapsedTicks - from.ElapsedTicks;
            ElapsedTicksDifferencePercentage = (ElapsedTicksDifference / from.ElapsedTicks) * 100;

            AverageMillisecondsDifference = to.AverageMilliseconds - from.AverageMilliseconds;
            AverageMillisecondsDifferencePercentage = (AverageMillisecondsDifference / from.AverageMilliseconds) * 100;

            AverageTicksDifference = to.AverageTicks - from.AverageTicks;
            AverageTicksDifferencePercentage = (AverageTicksDifference / from.AverageTicks) * 100;

            GCCollections0Difference = to.GarbageCollections0Count - from.GarbageCollections0Count;
            GCCollections0DifferencePercentage = (GCCollections0Difference / from.GarbageCollections0Count) * 100;

            GCCollections1Difference = to.GarbageCollections1Count - from.GarbageCollections1Count;
            GCCollections1DifferencePercentage = (GCCollections1Difference / from.GarbageCollections1Count) * 100;

            GCCollections2Difference = to.GarbageCollections2Count - from.GarbageCollections2Count;
            GCCollections2DifferencePercentage = (GCCollections2Difference / from.GarbageCollections2Count) * 100;
        }
    }

    public class Benchmarker
    {
        public static BenchmarkResult Start(Action actionToProfile, BenchmarkParameters parameters)
        {
            // Do a warm-up spin if warmup is enabled
            if (parameters.PerformWarmup)
            {
                parameters.WarmupAction?.Invoke();
                for (long i = 0; i < parameters.WarmupIterations; i++)
                {
                    actionToProfile.Invoke();
                }
            }

            // Collect current collections counts, in order to calculate 
            // them correctly later.
            var initialCollection0Count = GC.CollectionCount(0);
            var initialCollection1Count = GC.CollectionCount(1);
            var initialCollection2Count = GC.CollectionCount(2);

            var iterations = parameters.BenchmarkIterations;

            var stopwatch = Stopwatch.StartNew();
            for (long i = 0; i < iterations; i++)
            {
                // TODO: For generic purpose, I had to call Invoke on the
                // TODO: passed action. This generates a virtual call,
                // TODO: that is slower. Find a way to get rid of it.
                actionToProfile.Invoke();
            }
            stopwatch.Stop();

            // Calculate collections counts correctly.
            var collection0Count = GC.CollectionCount(0) - initialCollection0Count;
            var collection1Count = GC.CollectionCount(1) - initialCollection1Count;
            var collection2Count = GC.CollectionCount(2) - initialCollection2Count;

            // Generate the benchmark results.
            return new BenchmarkResult()
            {
                GarbageCollections0Count = collection0Count,
                GarbageCollections1Count = collection1Count,
                GarbageCollections2Count = collection2Count,
                Elapsed = stopwatch.Elapsed,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                ElapsedTicks = stopwatch.ElapsedTicks,
                AverageMilliseconds = stopwatch.ElapsedMilliseconds / iterations,
                AverageTicks = stopwatch.ElapsedTicks / iterations,
                Iterations = iterations
            };
        }

        public static BenchmarkResult[] Start(Action[] actionsToProfile, BenchmarkParameters parameters)
        {
            var results = new BenchmarkResult[actionsToProfile.Length];

            for (var i = 0; i < actionsToProfile.Length; i++)
            {
                results[i] = Start(actionsToProfile[i], parameters);
            }

            return results;
        }

        public static BenchmarkComparison[] Compare(Action mainAction, BenchmarkParameters parameters, params Action[] actions)
        {
            var mainResult = Start(mainAction, parameters);
            var otherResults = Start(actions, parameters);

            var comparisons = new BenchmarkComparison[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                var comparison = new BenchmarkComparison();
                comparison.Compare(mainResult, otherResults[i]);
                comparisons[i] = comparison;
            }

            return comparisons;
        }
    }
}
