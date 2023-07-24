using System;
using Xunit;

namespace Horarium.Test
{
    public class AssemblyQualifiedNameWithoutVersionTests
    {
        [Fact]
        public void GetName_ForNonGenericJob_ShouldRemoveVersion()
        {
            var jobName = new SecondNonGenericJob().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<SecondNonGenericJob>(instance);
            Assert.Equal("Horarium.Test.SecondNonGenericJob, Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithTwoArguments_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithTwoArguments<SecondNonGenericJob, FirstNonGenericJob>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithTwoArguments<SecondNonGenericJob, FirstNonGenericJob>>(instance);
            Assert.Equal("Horarium.Test.GenericJobWithTwoArguments`2[[Horarium.Test.SecondNonGenericJob, Horarium.Test], [Horarium.Test.FirstNonGenericJob, Horarium.Test]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithTwoNestedGenericArguments_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithTwoArguments<GenericJob<FirstNonGenericJob>, GenericJob<SecondNonGenericJob>>()
                .GetType()
                .AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithTwoArguments<GenericJob<FirstNonGenericJob>, GenericJob<SecondNonGenericJob>>>(instance);
            Assert.Equal("Horarium.Test.GenericJobWithTwoArguments`2[[Horarium.Test.GenericJob`1[[Horarium.Test.FirstNonGenericJob, Horarium.Test]], Horarium.Test], [Horarium.Test.GenericJob`1[[Horarium.Test.SecondNonGenericJob, Horarium.Test]], Horarium.Test]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithSingleGenericArgument_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJob<SecondNonGenericJob>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJob<SecondNonGenericJob>>(instance);
            Assert.Equal("Horarium.Test.GenericJob`1[[Horarium.Test.SecondNonGenericJob, Horarium.Test]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithSingleNestedGenericArgument_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJob<GenericJob<SecondNonGenericJob>>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJob<GenericJob<SecondNonGenericJob>>>(instance);
            Assert.Equal("Horarium.Test.GenericJob`1[[Horarium.Test.GenericJob`1[[Horarium.Test.SecondNonGenericJob, Horarium.Test]], Horarium.Test]], Horarium.Test", jobName);
        }
    }
}