using System;
using Horarium.Interfaces;
using Newtonsoft.Json;

namespace Horarium
{
    public class HorariumSettings
    {
        public TimeSpan IntervalStartJob { get; set; } = TimeSpan.FromMilliseconds(100);

        public TimeSpan ObsoleteExecutingJob { get; set; } = TimeSpan.FromMinutes(5);

        public IJobScopeFactory JobScopeFactory { get; set; } = new DefaultJobScopeFactory();

        public IHorariumLogger Logger { get; set; } = new EmptyLogger();

        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
    }
}