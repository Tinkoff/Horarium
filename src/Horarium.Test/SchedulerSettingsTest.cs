using System;
using Xunit;

namespace Horarium.Test
{
    public class SchedulerSettingsTest
    {
        [Fact]
        public void IntervalStartJob_IsValid()
        {
            var defaultIntervalStartJob = TimeSpan.FromMilliseconds(100);
            
            var settings = new HorariumSettings();
            
            Assert.Equal(defaultIntervalStartJob, settings.IntervalStartJob);
        }
    }
}