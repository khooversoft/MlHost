using System;

namespace Toolbox.Tools
{
    public class RangeLimit
    {
        private int _current;

        public RangeLimit(int minRange, int maxRange)
        {
            _current = minRange;
            MinRange = minRange;
            MaxRange = maxRange;
        }

        public int MinRange { get; }

        public int MaxRange { get; }

        public int Current { get => ConstrainToRange(_current); set => _current = ConstrainToRange(value); }

        public void Reset() => _current = MinRange;

        public bool Increment(int value = 1) => (_current + value)
            .Action(x => _current = ConstrainToRange(x))
            .Func(x => InRange(x));

        public bool InRange(int value) => value >= MinRange && value <= MaxRange;

        private int ConstrainToRange(int value) => Math.Max(Math.Min(value, MaxRange), MinRange);
    }
}
