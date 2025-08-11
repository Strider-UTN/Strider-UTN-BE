namespace StriderWebApi.Model;

public class WorkoutAnalyzer
{

    private const double THRESHOLD_SPEED = 0.1;
    private const double DT = 20;

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

    public double L2Distance(List<IntervalInfo> a, List<IntervalInfo> b, int shift)
    {
        double timeShift = shift >= 0 ? a.Take(shift).Sum(i => i.Duration) : b.Take(-shift).Sum(i => i.Duration);

        double dt = DT;
        double time = Math.Min(timeShift, b.Sum(i => i.Duration));
        double max = Math.Max(a.Sum(i => i.Duration), b.Sum(i => i.Duration) + timeShift);
        double diff = 0;

        while (time < max)
        {

            double aSpeed = GetSpeedAt(a, time);
            double bSpeed = GetSpeedAt(b, time - timeShift);

            diff += (aSpeed - bSpeed) * (aSpeed - bSpeed) * dt;
            time += dt;

        }

        return Math.Sqrt(diff);
        
    }

    private static double GetSpeedAt(List<IntervalInfo> intervals, double time) {
        if (time < 0) return 0;
        if (time > intervals.Sum(i => i.Duration)) return 0;

        double acum = 0;
        for (int i = 0; i < intervals.Count; i++)
        {
            acum += intervals[i].Duration;
            if (acum > time) return intervals[i].Speed;
        }
        return 0;
    }


    public int FindOptimalShift(List<IntervalInfo> a, List<IntervalInfo> b) {
        double minDiff = double.MaxValue;
        int minShift = 0;
        int c = Math.Max(a.Count, b.Count); 
        for (int shift = -c; shift < c; shift++)
        {
            double diff = L2Distance(a, b, shift);
            if (diff < minDiff)
            {
                minDiff = diff;
                minShift = shift;
            }
        }
        return minShift;
    }

    public List<AnalyzedInterval> Analyze(Athlete a, Workout w, Session s)
    {

        List<IntervalInfo> sessionIntervals = ParseSession(a, s);
        List<IntervalInfo> workoutIntervals = ParseWorkout(w);

        int optimalShift = FindOptimalShift(sessionIntervals, workoutIntervals);

        if (optimalShift < 0)
        {
            for (int i = 0; i < -optimalShift; i++)
            {
                workoutIntervals.Insert(0, new IntervalInfo { Duration = 0, Speed = 0 });
            }
        }
        else
        {
            for (int i = 0; i < optimalShift; i++)
            {
                sessionIntervals.Insert(0, new IntervalInfo { Duration = 0, Speed = 0 });
            }
        }

        if (workoutIntervals.Count < sessionIntervals.Count)
        {
            for (int i = 0; i <= sessionIntervals.Count - workoutIntervals.Count; i++)
            {
                workoutIntervals.Add(new IntervalInfo { Duration = sessionIntervals[i].Duration, Speed = 0 });
            }
        }
        else if (workoutIntervals.Count > sessionIntervals.Count)
        {
            for (int i = 0; i <= workoutIntervals.Count - sessionIntervals.Count; i++)
            {
                sessionIntervals.Add(new IntervalInfo { Duration = workoutIntervals[i].Duration, Speed = 0 });
            }
        }

        List<AnalyzedInterval> intervals = new List<AnalyzedInterval>();

        for (int i = 0; i < sessionIntervals.Count; i++)
        {

            intervals.Add(new AnalyzedInterval
            {
                ExpectedDistance = sessionIntervals[i].Duration * sessionIntervals[i].Speed,
                ExpectedDuration = sessionIntervals[i].Duration,
                ExpectedVelocity = sessionIntervals[i].Speed,
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


