using FluentAssertions;
using MlHost.Tools;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MlHost.Test.Tools
{
    public class RangeLimitTests
    {
        [Fact]
        public void GivenRangeLimit_WhenConstructed_Verifies()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);

            range.Current.Should().Be(min);
            range.MinRange.Should().Be(min);
            range.MaxRange.Should().Be(max);

            range.Current = min + 1;
            range.Current.Should().Be(min + 1);
            range.Increment().Should().BeTrue();
            range.Current.Should().Be(min + 2);
            range.Increment(-1).Should().BeTrue();
            range.Current.Should().Be(min + 1);

            range.Current = min - 1;
            range.Current.Should().Be(min);
            range.Increment(-1).Should().BeFalse();
            range.Current.Should().Be(min);

            range.Current = max + 1;
            range.Current.Should().Be(max);
            range.Increment().Should().BeFalse();
            range.Current.Should().Be(max);

            range.Current = max - 1;
            range.Current.Should().Be(max - 1);
            range.Increment().Should().BeTrue();
            range.Current.Should().Be(max);
            range.Increment().Should().BeFalse();
            range.Current.Should().Be(max);
        }

        [Fact]
        public void GivenRangeLimit_WhenIncrementedAndDecremented_ShouldPass()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);
            range.Current.Should().Be(min);

            range.Increment().Should().BeTrue();
            range.Current.Should().Be(min + 1);

            range.Increment(-1).Should().BeTrue();
            range.Current.Should().Be(min);
        }

        [Fact]
        public void GivenSmallRange_WhenIncrementedPassRange_ShouldBeInRange()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);
            range.Current = max;
            range.Increment().Should().BeFalse();

            range.Current.Should().Be(max);
        }

        [Fact]
        public void GivenSmallRange_WhenDecrementPassRange_ShouldBeInRange()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);
            range.Increment(-1).Should().BeFalse();

            range.Current.Should().Be(min);
        }

        [Fact]
        public void GivenSmallRange_WhenIncrementedFullRange_ShouldSignalMaxRange()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);

            Enumerable.Range(min, max)
                .Select(x => (index: x, pass: range.Increment()))
                .All(x => x.index == range.Current - 1 && x.pass == true)
                .Should().BeTrue();

            range.Increment().Should().BeFalse();
            range.Current.Should().Be(max);
        }

        [Fact]
        public void GivenSmallRange_WhenDecrementedFullRange_ShouldSignalMinRange()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);
            range.Current = range.MaxRange;

            Enumerable.Range(min, max)
                .Select(x => range.Increment(-1))
                .All(x => x == true)
                .Should().BeTrue();

            range.Increment(-1).Should().BeFalse();
            range.Current.Should().Be(min);
        }

        [Fact]
        public void GivenSmallRange_WhenIncrementedByTwoFullRange_ShouldSignalMaxRange()
        {
            const int min = 0;
            const int max = 10;

            var range = new RangeLimit(min, max);

            Enumerable.Range(min, max / 2)
                .Select(x => range.Increment(2))
                .All(x => x == true)
                .Should().BeTrue();

            range.Increment(1).Should().BeFalse();
            range.Current.Should().Be(max);
        }
    }
}
