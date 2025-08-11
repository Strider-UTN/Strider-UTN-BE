using Xunit;
using StriderWebApi.Model;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Test
{
    public class WorkoutAnalyzerTests
    {
        private readonly WorkoutAnalyzer _analyzer;
        private readonly Athlete _athlete;

        public WorkoutAnalyzerTests()
        {
            _analyzer = new WorkoutAnalyzer();
            _athlete = new Athlete
            {
                Id = 1,
                Username = "testathlete",
                Name = "Test Athlete",
                Email = "test@example.com",
                Address = "123 Test St",
                CreatedBy = "system",
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MALE,
                VO2Max = 50.0, // 50 km/h max speed
                Height = 175.0,
                Weight = 70.0,
                DateStartedRunning = DateTime.Now.AddYears(-5)
            };
        }

        [Fact]
        public void ParseSession_SimpleSession_ReturnsCorrectIntervals()
        {
            // Arrange
            var session = CreateSimpleSession();

            // Act
            var result = _analyzer.ParseSession(_athlete, session);

            // Assert
            Assert.Equal(3, result.Count); // 2 intervals (no series rest since only 1 series)
            Assert.Equal(60.0, result[0].Duration); // First interval: 60 units of time
            Assert.Equal(60.0, result[0].Speed);
            Assert.Equal(30.0, result[1].Duration); // Second interval
            Assert.Equal(0.0, result[1].Speed);
        }

        [Fact]
        public void ParseWorkout_SimpleWorkout_ReturnsCorrectIntervals()
        {
            // Arrange
            var workout = CreateSimpleWorkout();

            // Act
            var result = _analyzer.ParseWorkout(workout);

            // Assert
            Assert.Single(result); // Should merge all consecutive similar speeds (60 km/h)
            Assert.Equal(180.0, result[0].Duration); // Combined duration of all three laps
            Assert.Equal(60.0, result[0].Speed);
        }

        [Fact]
        public void ParseWorkout_WorkoutWithSpeedVariations_DoesNotMergeDifferentSpeeds()
        {
            // Arrange
            var workout = CreateWorkoutWithSpeedVariations();

            // Act
            var result = _analyzer.ParseWorkout(workout);

            // Assert
            Assert.Equal(3, result.Count); // Should not merge different speeds
            Assert.Equal(60.0, result[0].Duration);
            Assert.Equal(60.0, result[0].Speed);
            Assert.Equal(60.0, result[1].Duration);
            Assert.Equal(30.0, result[1].Speed);
            Assert.Equal(60.0, result[2].Duration);
            Assert.Equal(60.0, result[2].Speed);
        }

        [Fact]
        public void Analyze_WorkoutMatchesSession_ReturnsCorrectAnalysis()
        {
            // Arrange
            var session = CreateSimpleSession();
            var workout = CreateMatchingWorkout();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Equal(3, result.Count); // Session has 2 intervals, workout has 4 intervals after parsing
            
            // First interval (session interval 1 vs merged workout laps 1+2)
            Assert.Equal(3600.0, result[0].ExpectedDistance); // 60 × 60 = 3600 units of distance
            Assert.Equal(60.0, result[0].ExpectedDuration); // 60 units of time
            Assert.Equal(60.0, result[0].ExpectedVelocity);
            Assert.Equal(3600.0, result[0].ActualDistance); // 60 × 60 = 3600 units of distance (merged laps)
            Assert.Equal(60.0, result[0].ActualDuration); // 60 units of time (merged laps)
            Assert.Equal(60.0, result[0].ActualVelocity); // Average speed of merged laps
        }

        [Fact]
        public void Analyze_WorkoutWithExtraLaps_HandlesExtraLapsCorrectly()
        {
            // Arrange
            var session = CreateSimpleSession();
            var workout = CreateWorkoutWithExtraLaps();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Equal(5, result.Count); // Session has 3 intervals, workout has 5 intervals after parsing
            
            // Extra laps should be handled by padding
            // The last interval should have zero values for expected
            Assert.Equal(0.0, result[4].ExpectedDistance);
            Assert.Equal(0, result[4].ExpectedDuration); // This should be padding from workout
            Assert.Equal(0.0, result[4].ExpectedVelocity);
        }

        [Fact]
        public void Analyze_WorkoutWithSplitIntervals_HandlesSplitIntervalsCorrectly()
        {
            // Arrange
            var session = CreateSessionWithLongInterval();
            var workout = CreateWorkoutWithSplitInterval();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Single(result); // Session has 1 interval, workout has 1 interval as it should be merged
            
            // The long interval should be matched with the combined workout laps
            Assert.Equal(7200.0, result[0].ExpectedDistance); // 120 × 60 = 7200 units of distance
            Assert.Equal(120.0, result[0].ExpectedDuration); // 120 units of time
            Assert.Equal(60.0, result[0].ExpectedVelocity);
            
            // The workout has this split into two 1km laps
            Assert.Equal(7200.0, result[0].ActualDistance); // 120 × 60 = 7200 units of distance total
            Assert.Equal(120.0, result[0].ActualDuration); // 120 units of time total
            Assert.Equal(60.0, result[0].ActualVelocity); // Average speed
        }

        [Fact]
        public void Analyze_WorkoutCloseToSession_FindsOptimalAlignment()
        {
            // Arrange
            var session = CreateSimpleSession();
            var workout = CreateWorkoutCloseToSession();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Equal(4, result.Count); // Session has 3 intervals, workout has 4 intervals after parsing (some laps merged)

            // Should find optimal shift to align workouts
            // The workout starts with a warmup lap, so the first interval should be padded
            Assert.True(result[0].ExpectedDistance == 0.0);// First interval should be padded
        }

        [Fact]
        public void Analyze_WorkoutWithLongerLaps_HandlesDurationOverrun()
        {
            // Arrange
            var session = CreateSimpleSession();
            var workout = CreateWorkoutWithLongerLaps();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Equal(3, result.Count); // Session has 3 intervals, workout has 3 intervals
            
            // First interval: planned 60s, actual 75s (25% longer)
            Assert.Equal(3600.0, result[0].ExpectedDistance); // 60 × 60 = 3600 units
            Assert.Equal(60.0, result[0].ExpectedDuration);   // 60 units of time
            Assert.Equal(60.0, result[0].ExpectedVelocity);
            Assert.Equal(4500.0, result[0].ActualDistance);   // 75 × 60 = 4500 units
            Assert.Equal(75.0, result[0].ActualDuration);     // 75 units of time (25% longer)
            Assert.Equal(60.0, result[0].ActualVelocity);     // Same speed, just longer duration
            
            // Second interval: planned 30s rest, actual 40s rest (33% longer)
            Assert.Equal(0.0, result[1].ExpectedDistance);    // Rest period
            Assert.Equal(30.0, result[1].ExpectedDuration);   // 30 units of time
            Assert.Equal(0.0, result[1].ExpectedVelocity);
            Assert.Equal(0.0, result[1].ActualDistance);      // Rest period
            Assert.Equal(40.0, result[1].ActualDuration);     // 40 units of time (33% longer)
            Assert.Equal(0.0, result[1].ActualVelocity);
        }

        [Fact]
        public void Analyze_WorkoutWithLongerDistance_HandlesDistanceOverrun()
        {
            // Arrange
            var session = CreateSessionWithLongInterval();
            var workout = CreateWorkoutWithLongerDistance();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Single(result); // Session has 1 interval, workout has 1 interval
            
            // Planned: 120s at 60 units/s = 7200 units
            // Actual: 150s at 60 units/s = 9000 units (25% longer)
            Assert.Equal(7200.0, result[0].ExpectedDistance); // 120 × 60 = 7200 units
            Assert.Equal(120.0, result[0].ExpectedDuration);  // 120 units of time
            Assert.Equal(60.0, result[0].ExpectedVelocity);
            Assert.Equal(9000.0, result[0].ActualDistance);   // 150 × 60 = 9000 units
            Assert.Equal(150.0, result[0].ActualDuration);    // 150 units of time (25% longer)
            Assert.Equal(60.0, result[0].ActualVelocity);     // Same speed, longer duration
        }

        [Fact]
        public void Analyze_WorkoutWithMixedOverruns_HandlesVariedOverruns()
        {
            // Arrange
            var session = CreateSessionWithMultipleIntervals();
            var workout = CreateWorkoutWithMixedOverruns();

            // Act
            var result = _analyzer.Analyze(_athlete, workout, session);

            // Assert
            Assert.Equal(5, result.Count); // Session has 3 intervals, workout has 4 intervals after parsing
            
            // First interval: planned 60s, actual 70s (17% longer)
            Assert.Equal(3600.0, result[0].ExpectedDistance); // 60 × 60 = 3600 units
            Assert.Equal(60.0, result[0].ExpectedDuration);   // 60 units of time
            Assert.Equal(4200.0, result[0].ActualDistance);   // 70 × 60 = 4200 units
            Assert.Equal(70.0, result[0].ActualDuration);     // 70 units of time
            
            // Second interval: planned 30s rest, actual 35s rest (17% longer)
            Assert.Equal(0.0, result[1].ExpectedDistance);    // Rest period
            Assert.Equal(30.0, result[1].ExpectedDuration);   // 30 units of time
            Assert.Equal(0.0, result[1].ActualDistance);      // Rest period
            Assert.Equal(35.0, result[1].ActualDuration);     // 35 units of time
            
            // Third interval: planned 60s, actual 65s (8% longer)
            Assert.Equal(3600.0, result[2].ExpectedDistance); // 60 × 60 = 3600 units
            Assert.Equal(60.0, result[2].ExpectedDuration);   // 60 units of time
            Assert.Equal(3900.0, result[2].ActualDistance);   // 65 × 60 = 3900 units
            Assert.Equal(65.0, result[2].ActualDuration);     // 65 units of time

        }

        #region Helper Methods

        private Session CreateSimpleSession()
        {
            var interval1 = new Interval
            {
                Id = 1,
                Distance = null,
                Duration = 60.0, // 60 units of time
                Speed = 60.0, // 60 units of distance per unit of time
                Repetitions = 1,
                Rest = 30.0, // 30 units of time rest
                Type = new FixedDuration(),
                SpeedType = new FixedSpeed()
            };

            var interval2 = new Interval
            {
                Id = 2,
                Distance = null,
                Duration = 60.0, // 60 units of time
                Speed = 60.0, // 60 units of distance per unit of time
                Repetitions = 1,
                Rest = 30.0, // 30 units of time rest
                Type = new FixedDuration(),
                SpeedType = new FixedSpeed()
            };

            var serie = new Serie
            {
                Intervals = new List<Interval> { interval1, interval2 },
                Repetitions = 1,
                Rest = 0 // 120 units of time rest between series
            };

            return new Session
            {
                Id = 1,
                Name = "Simple Session",
                Series = new List<Serie> { serie }
            };
        }

        private Workout CreateSimpleWorkout()
        {
            return new Workout
            {
                Id = 1,
                Name = "Simple Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 },
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 },
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }
                }
            };
        }

        private Workout CreateWorkoutWithSpeedVariations()
        {
            return new Workout
            {
                Id = 2,
                Name = "Speed Variations Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 },
                    new() { Duration = 60.0, Speed = 30.0, Distance = 1800.0 },
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }
                }
            };
        }

        private Workout CreateMatchingWorkout()
        {
            return new Workout
            {
                Id = 3,
                Name = "Matching Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 },
                    new() { Duration = 30.0, Speed = 0.0, Distance = 0.0 },
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }
                }
            };
        }

        private Workout CreateWorkoutWithExtraLaps()
        {
            return new Workout
            {
                Id = 4,
                Name = "Extra Laps Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 },
                    new() { Duration = 30.0, Speed = 0.0, Distance = 0.0 },
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 },
                    new() { Duration = 45.0, Speed = 45.0, Distance = 45 * 45 }, // Extra lap
                    new() { Duration = 30.0, Speed = 0.0, Distance = 0.0 }  // Extra rest
                }
            };
        }

        private Session CreateSessionWithLongInterval()
        {
            var longInterval = new Interval
            {
                Id = 1,
                Distance = null,
                Duration = 120.0, // 120 units of time
                Speed = 60.0, // 60 units of distance per unit of time
                Repetitions = 1,
                Rest = 60.0, // 60 units of time rest
                Type = new FixedDuration(),
                SpeedType = new FixedSpeed()
            };

            var serie = new Serie
            {
                Intervals = new List<Interval> { longInterval },
                Repetitions = 1,
                Rest = 0.0
            };

            return new Session
            {
                Id = 2,
                Name = "Long Interval Session",
                Series = new List<Serie> { serie }
            };
        }

        private Workout CreateWorkoutWithSplitInterval()
        {
            return new Workout
            {
                Id = 5,
                Name = "Split Interval Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }, // First half of long interval
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }, // Second half of long interval
                }
            };
        }

        private Workout CreateWorkoutCloseToSession()
        {
            return new Workout
            {
                Id = 6,
                Name = "Close to Session Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 30.0, Speed = 30.0, Distance = 900.0 },  // 30 × 30 = 900 units (Warmup lap)
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }, // 60 × 60 = 3600 units
                    new() { Duration = 30.0, Speed = 0.0, Distance = 0.0 },     // 30 × 0 = 0 units
                    new() { Duration = 60.0, Speed = 60.0, Distance = 3600.0 }  // 60 × 60 = 3600 units
                }
            };
        }

        private Workout CreateWorkoutWithLongerLaps()
        {
            return new Workout
            {
                Id = 7,
                Name = "Longer Laps Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 75.0, Speed = 60.0, Distance = 4500.0 }, // 75 × 60 = 4500 units (25% longer than planned)
                    new() { Duration = 40.0, Speed = 0.0, Distance = 0.0 },     // 40 × 0 = 0 units (33% longer rest than planned)
                    new() { Duration = 75.0, Speed = 60.0, Distance = 4500.0 }  // 75 × 60 = 4500 units (25% longer than planned)
                }
            };
        }

        private Workout CreateWorkoutWithLongerDistance()
        {
            return new Workout
            {
                Id = 8,
                Name = "Longer Distance Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 150.0, Speed = 60.0, Distance = 9000.0 } // 150 × 60 = 9000 units (25% longer than planned)
                }
            };
        }

        private Session CreateSessionWithMultipleIntervals()
        {
            var interval1 = new Interval
            {
                Id = 1,
                Distance = null,
                Duration = 60.0, // 60 units of time
                Speed = 60.0, // 60 units of distance per unit of time
                Repetitions = 1,
                Rest = 30.0, // 30 units of time rest between repetitions
                Type = new FixedDuration(),
                SpeedType = new FixedSpeed()
            };

            var interval2 = new Interval
            {
                Id = 2,
                Distance = null,
                Duration = 60.0, // 60 units of time
                Speed = 60.0, // 60 units of distance per unit of time
                Repetitions = 1,
                Rest = 30.0, // 30 units of time rest between repetitions
                Type = new FixedDuration(),
                SpeedType = new FixedSpeed()
            };

            var serie = new Serie
            {
                Intervals = new List<Interval> { interval1, interval2 },
                Repetitions = 1,
                Rest = 0.0 // No rest between series (only 1 series)
            };

            return new Session
            {
                Id = 3,
                Name = "Multiple Intervals Session",
                Series = new List<Serie> { serie }
            };
        }

        private Workout CreateWorkoutWithMixedOverruns()
        {
            return new Workout
            {
                Id = 9,
                Name = "Mixed Overruns Workout",
                Laps = new List<Lap>
                {
                    new() { Duration = 70.0, Speed = 60.0, Distance = 4200.0 }, // 70 × 60 = 4200 units (17% longer than planned)
                    new() { Duration = 35.0, Speed = 0.0, Distance = 0.0 },     // 35 × 0 = 0 units (17% longer rest than planned)
                    new() { Duration = 65.0, Speed = 60.0, Distance = 3900.0 }, // 65 × 60 = 3900 units (8% longer than planned)
                    new() { Duration = 40.0, Speed = 0.0, Distance = 0.0 },     // 40 × 0 = 0 units (33% longer rest than planned)
                    new() { Duration = 70.0, Speed = 60.0, Distance = 4200.0 }  // 70 × 60 = 4200 units (17% longer than planned)
                }
            };
        }

        #endregion
    }
} 
