namespace StriderWebApi.Model;

using Google.OrTools.Graph;

public class WorkoutAnalyzer
{

    private const double THRESHOLD_SPEED = 0.1;
    private const double INDEX_DIFFERENCE_WEIGHT = 2.5;

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

    private double CalculateMaxArea(List<IntervalInfo> baseIntervals, List<IntervalInfo> toMatchIntervals)
    {

        double area = 0;
        for (int i = 0; i < baseIntervals.Count; i++)
        {
            for (int j = 0; j < toMatchIntervals.Count; j++)
            {
                area = Math.Max(area, Math.Pow(baseIntervals[i].Duration * baseIntervals[i].Speed - toMatchIntervals[j].Duration * toMatchIntervals[j].Speed, 2));
            }
        }
        return area;
    }

    public List<IntervalInfo> AnalyzeIntervals(List<IntervalInfo> baseIntervals, List<IntervalInfo> toMatchIntervals)
    {

        var minCostFlow = new MinCostFlow();

        double areaNormalization = CalculateMaxArea(baseIntervals, toMatchIntervals);
        int n = baseIntervals.Count;
        int m = toMatchIntervals.Count;

        int SessionIndex(int i) => i + 1;
        int WorkoutIndex(int i) => i + 1 + baseIntervals.Count;
        long Cost(int i, int j) => (long)Math.Round(Math.Pow(baseIntervals[i].Duration * baseIntervals[i].Speed - toMatchIntervals[j].Duration * toMatchIntervals[j].Speed,2) / areaNormalization + INDEX_DIFFERENCE_WEIGHT * Math.Pow(i-j,2) / Math.Pow(Math.Max(n-1,m-1),2));

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
                minCostFlow.AddArcWithCapacityAndUnitCost(SessionIndex(i), sink, 1, (long)Math.Round(Math.Pow(baseIntervals[i].Duration * baseIntervals[i].Speed,2) / areaNormalization));
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

        if (status != MinCostFlowBase.Status.OPTIMAL)
        {
            throw new Exception("Unable to match session intervals with workout intervals");
        }

        List<IntervalInfo> intervals = new List<IntervalInfo>();

        for (int i = 0; i < Math.Max(n, m); i++)
        {
            intervals.Add(new IntervalInfo { Duration = 0, Speed = 0 });
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (minCostFlow.Flow(indices[i, j]) == 1)
                {
                    intervals[j] = new IntervalInfo { Duration = baseIntervals[i].Duration, Speed = baseIntervals[i].Speed };
                }
            }
        }

        return intervals;

    }

    public Analysis Analyze(Athlete a, Workout w, Session s)
    {

        List<IntervalInfo> sessionIntervals = ParseSession(a, s);
        List<IntervalInfo> workoutIntervals = ParseWorkout(w);

        List<IntervalInfo> optimizedSessionIntervals = AnalyzeIntervals(sessionIntervals, workoutIntervals);

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
                ActualVelocity = workoutIntervals[i].Speed,
                HR = w.Laps[i].HR
            });

        }

        return new Analysis { Intervals = intervals };
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
    public double HR { get; set; }
    
}

public class Analysis
{
    public List<AnalyzedInterval> Intervals { get; set; } = [];


    public int AveragePaceDifferencePerThousandMetersInSeconds()
    {
        double difference = 0;
        foreach (var interval in Intervals)
        {
            if (interval.ActualVelocity == 0) continue;
            double actualPace = 1 / interval.ActualVelocity * 1000 / 60;
            double expectedPace = 1 / interval.ExpectedVelocity * 1000 / 60;
            difference += Math.Abs(actualPace - expectedPace) * interval.ActualDistance / 1000;
        }
        return (int) (difference / Intervals.Count);
    }

    public int MaxPaceDifferencePerThousandMetersInSeconds()
    {
        double difference = 0;
        foreach (var interval in Intervals)
        {
            if (interval.ActualVelocity == 0) continue;
            double actualPace = 1 / interval.ActualVelocity * 1000 / 60;
            double expectedPace = 1 / interval.ExpectedVelocity * 1000 / 60;
            difference = Math.Max(difference, Math.Abs(actualPace - expectedPace) * interval.ActualDistance / 1000);
        }
        return (int) difference;
    }
}


