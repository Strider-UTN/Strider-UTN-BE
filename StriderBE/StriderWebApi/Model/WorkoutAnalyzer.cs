namespace StriderWebApi.Model;

using Google.OrTools.Graph;

public class WorkoutAnalyzer
{

    private const double THRESHOLD_SPEED = 0.1;
    private const double INDEX_FACTOR = 20;

    public struct IntervalInfo
    {

        public double Duration { get; set; }
        public double Speed { get; set; }

    }

    public List<IntervalInfo> ParseSession(Athlete a, Session s)
    {

        List<IntervalInfo> intervals = new List<IntervalInfo>();

        for (int i = 0; i < s.Series.Count; i++)
        {
            for (int j = 0; j < s.Series[i].Intervals.Count; j++)
            {
                var l = s.Series[i].Intervals[j];
                for (int k = 0; k < l.Repetitions; k++)
                {
                    intervals.Add(new IntervalInfo { Duration = l.GetDuration(a), Speed = l.GetSpeed(a) });
                    if (j < s.Series[i].Intervals.Count - 1) intervals.Add(new IntervalInfo { Duration = l.Rest, Speed = 0 });
                }
            }
            if (i < s.Series.Count - 1) intervals.Add(new IntervalInfo { Duration = s.Series[i].Rest, Speed = 0 });
        }

        return intervals;
    }

    public List<IntervalInfo> ParseWorkout(Workout w)
    {

        List<IntervalInfo> intervals = new List<IntervalInfo>();

        foreach (var l in w.Laps)
        {
            intervals.Add(new IntervalInfo { Duration = l.Duration, Speed = l.Speed });
        }

        for (int i = 0; i < intervals.Count - 1; i++)
        {
            if (Math.Abs(intervals[i].Speed - intervals[i + 1].Speed) < THRESHOLD_SPEED * intervals[i].Speed)
            {
                var interval = intervals[i];
                interval.Duration += intervals[i + 1].Duration;
                interval.Speed = (intervals[i].Speed * interval.Duration + intervals[i + 1].Speed * intervals[i + 1].Duration) / (interval.Duration + intervals[i + 1].Duration);
                intervals[i] = interval;
                intervals.RemoveAt(i + 1);
                i--;
            }
        }

        return intervals;

    }

    public List<IntervalInfo> LinkSessionToWorkout(List<IntervalInfo> sessionIntervals, List<IntervalInfo> workoutIntervals)
    {

        var minCostFlow = new MinCostFlow();

        int SessionIndex(int i) => i + 1;
        int WorkoutIndex(int i) => i + 1 + sessionIntervals.Count;
        long Cost(int i, int j) => (long)Math.Round(Math.Pow(sessionIntervals[i].Duration * sessionIntervals[i].Speed - workoutIntervals[j].Duration * workoutIntervals[j].Speed, 2) + Math.Pow((i - j) * INDEX_FACTOR, 2));

        int n = sessionIntervals.Count;
        int m = workoutIntervals.Count;
        int max = n + m + 1;
        int sink = max + 1;

        var indices = new int[n, m];

        for (int i = 0; i < n; i++)
        {
            minCostFlow.AddArcWithCapacityAndUnitCost(0, SessionIndex(i), 1, 0);
            
        }

        if (m < n)
        {
            for (int i = 0; i < n; i++)
            {
                minCostFlow.AddArcWithCapacityAndUnitCost(SessionIndex(i), sink, 1, (long)Math.Round(Math.Pow(sessionIntervals[i].Duration * sessionIntervals[i].Speed, 2)));
            }
            minCostFlow.AddArcWithCapacityAndUnitCost(sink, max, n - m, 0);
        }


        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                indices[i, j] = minCostFlow.AddArcWithCapacityAndUnitCost(SessionIndex(i), WorkoutIndex(j), 1, Cost(i, j));
            }
        }

        for (int j = 0; j < m; j++)
        {
            minCostFlow.AddArcWithCapacityAndUnitCost(WorkoutIndex(j), max, 1, 0);
        }



        minCostFlow.SetNodeSupply(0, n);
        minCostFlow.SetNodeSupply(max, -n);

        var status = minCostFlow.Solve();

        if (status != MinCostFlow.Status.OPTIMAL)
        {
            throw new Exception("Optimization failed");
        }

        List<IntervalInfo> intervals = new List<IntervalInfo>();

        for (int i = 0; i < Math.Max(n, m); i++)
        {
            intervals.Add(new IntervalInfo { Duration = 0, Speed = 0});
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (minCostFlow.Flow(indices[i, j]) == 1)
                { 
                    intervals[j] = new IntervalInfo { Duration = sessionIntervals[i].Duration, Speed = sessionIntervals[i].Speed }; 
                }
            }
        }

        return intervals;
        
    }

    public List<AnalyzedInterval> Analyze(Athlete a, Workout w, Session s)
    {

        List<IntervalInfo> sessionIntervals = ParseSession(a, s);
        List<IntervalInfo> workoutIntervals = ParseWorkout(w);

        List<IntervalInfo> optimizedSessionIntervals = LinkSessionToWorkout(sessionIntervals, workoutIntervals);

        List<AnalyzedInterval> intervals = new List<AnalyzedInterval>();

        for (int i = 0; i < workoutIntervals.Count; i++)
        {

            intervals.Add(new AnalyzedInterval
            {
                ExpectedDistance = optimizedSessionIntervals[i].Duration * optimizedSessionIntervals[i].Speed,
                ExpectedDuration = optimizedSessionIntervals[i].Duration,
                ExpectedVelocity = optimizedSessionIntervals[i].Speed,
                ActualDistance = workoutIntervals[i].Duration * workoutIntervals[i].Speed,
                ActualDuration = workoutIntervals[i].Duration,
                ActualVelocity = workoutIntervals[i].Speed
            });

        }

        return intervals;
    }


}

public class AnalyzedInterval
{
    public double ExpectedDistance { get; set; }
    public double ExpectedDuration { get; set; }
    public double ExpectedVelocity { get; set; }
    public double ActualDistance { get; set; }
    public double ActualDuration { get; set; }
    public double ActualVelocity { get; set; }
}


