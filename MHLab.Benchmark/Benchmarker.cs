using System;
using System.Diagnostics;

namespace MHLab.Benchmark
{
    public class BenchmarkParameters
    {
        public long BenchmarkIterations;

        public bool   PerformWarmup;
        public long   WarmupIterations;
        public Action InitializeAction;
    }

    public class BenchmarkResult
    {
        public string Name;
        
        public TimeSpan Elapsed;
        public long     ElapsedMilliseconds;
        public long     ElapsedTicks;
        public long     AverageMilliseconds;
        public long     AverageTicks;
        public int      GarbageCollections0Count;
        public int      GarbageCollections1Count;
        public int      GarbageCollections2Count;
        
        public BenchmarkResult() {}

        public BenchmarkResult(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"--> {Name} <--\n"                +
                   $"Execution Time (TimeSpan): {Elapsed}\n"                +
                   $"Execution Time (ms): {ElapsedMilliseconds}\n"          +
                   $"Execution Ticks: {ElapsedTicks}\n"                     +
                   $"Average Execution Time (ms): {AverageMilliseconds}\n"  +
                   $"Average Execution Ticks: {AverageTicks}\n"             +
                   $"Garbage Collections (0): {GarbageCollections0Count}\n" +
                   $"Garbage Collections (1): {GarbageCollections1Count}\n" +
                   $"Garbage Collections (2): {GarbageCollections2Count}";
        }
    }

    public class BenchmarkComparison
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
            CompareTo   = to;

            ElapsedMillisecondsDifference           = to.ElapsedMilliseconds - from.ElapsedMilliseconds;
            if (from.ElapsedMilliseconds > 0)
                ElapsedMillisecondsDifferencePercentage = (ElapsedMillisecondsDifference / from.ElapsedMilliseconds) * 100;

            ElapsedTicksDifference           = to.ElapsedTicks - from.ElapsedTicks;
            if (from.ElapsedTicks > 0)
                ElapsedTicksDifferencePercentage = (ElapsedTicksDifference / from.ElapsedTicks) * 100;

            AverageMillisecondsDifference           = to.AverageMilliseconds - from.AverageMilliseconds;
            if (from.AverageMilliseconds > 0)
                AverageMillisecondsDifferencePercentage = (AverageMillisecondsDifference / from.AverageMilliseconds) * 100;

            AverageTicksDifference           = to.AverageTicks - from.AverageTicks;
            AverageTicksDifferencePercentage = (AverageTicksDifference / from.AverageTicks) * 100;

            GCCollections0Difference           = to.GarbageCollections0Count - from.GarbageCollections0Count;
            if (from.GarbageCollections0Count > 0)
                GCCollections0DifferencePercentage = (GCCollections0Difference / from.GarbageCollections0Count) * 100;

            GCCollections1Difference           = to.GarbageCollections1Count - from.GarbageCollections1Count;
            if (from.GarbageCollections1Count > 0)
                GCCollections1DifferencePercentage = (GCCollections1Difference / from.GarbageCollections1Count) * 100;

            GCCollections2Difference           = to.GarbageCollections2Count - from.GarbageCollections2Count;
            if (from.GarbageCollections2Count > 0)
                GCCollections2DifferencePercentage = (GCCollections2Difference / from.GarbageCollections2Count) * 100;
        }
    }

    public class Benchmarker : IDisposable
    {
        private readonly Stopwatch           _timer;
        private readonly BenchmarkResult     _result;

        private readonly int _initialCollection0Count;
        private readonly int _initialCollection1Count;
        private readonly int _initialCollection2Count;

        private Benchmarker(BenchmarkResult result)
        {
            _timer      = Stopwatch.StartNew();
            _result     = result;

            // Collect current collections counts, in order to calculate 
            // them correctly later.
            _initialCollection0Count = GC.CollectionCount(0);
            _initialCollection1Count = GC.CollectionCount(1);
            _initialCollection2Count = GC.CollectionCount(2);
        }

        public void Dispose()
        {
            _timer.Stop();

            // Collect results
            _result.GarbageCollections0Count = GC.CollectionCount(0) - _initialCollection0Count;
            _result.GarbageCollections1Count = GC.CollectionCount(0) - _initialCollection1Count;
            _result.GarbageCollections2Count = GC.CollectionCount(0) - _initialCollection2Count;
            _result.Elapsed                  = _timer.Elapsed;
            _result.ElapsedMilliseconds      = _timer.ElapsedMilliseconds;
            _result.ElapsedTicks             = _timer.ElapsedTicks;
            _result.AverageMilliseconds      = _timer.ElapsedMilliseconds;
            _result.AverageTicks             = _timer.ElapsedTicks;
        }

        public static Benchmarker StartScope(BenchmarkResult result)
        {
            return new Benchmarker(result);
        }

        private static void PerformWarmup(Action actionToProfile, BenchmarkParameters parameters)
        {
            // Do a warm-up spin if warmup is enabled
            if (!parameters.PerformWarmup) return;
            
            parameters.InitializeAction?.Invoke();
            
            for (long i = 0; i < parameters.WarmupIterations; i++)
            {
                actionToProfile.Invoke();
            }
        }

        public static BenchmarkResult Start(Action actionToProfile, BenchmarkParameters parameters)
        {
            PerformWarmup(actionToProfile, parameters);

            var result = new BenchmarkResult(actionToProfile.Method.Name);
            
            var iterations = parameters.BenchmarkIterations;

            using (var benchmark = StartScope(result))
            {
                for (var i = 0; i < iterations; i++)
                {
                    /* TODO:
                     I had to call Invoke on the passed action. This is a virtual call,
                     that is slower. Find a way to get rid of it. 
                     */
                    actionToProfile.Invoke();
                }
            }

            result.AverageMilliseconds /= iterations;
            result.AverageTicks        /= iterations;
            
            return result;
        }

        public static BenchmarkResult[] StartMultiple(Action[] actionsToProfile, BenchmarkParameters parameters)
        {
            var results = new BenchmarkResult[actionsToProfile.Length];

            for (var i = 0; i < actionsToProfile.Length; i++)
            {
                results[i] = Start(actionsToProfile[i], parameters);
            }

            return results;
        }

        public static BenchmarkComparison[] StartAndCompare(Action baseline, BenchmarkParameters parameters, params Action[] actions)
        {
            var mainResult   = Start(baseline, parameters);
            var otherResults = StartMultiple(actions, parameters);

            return Compare(mainResult, otherResults);
        }

        public static BenchmarkComparison[] Compare(BenchmarkResult baseline, params BenchmarkResult[] comparing)
        {
            var comparisons = new BenchmarkComparison[comparing.Length];
            for (int i = 0; i < comparing.Length; i++)
            {
                var comparison = new BenchmarkComparison();
                comparison.Compare(baseline, comparing[i]);
                comparisons[i] = comparison;
            }

            return comparisons;
        }
    }
}