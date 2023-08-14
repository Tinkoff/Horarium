using System;
using System.ComponentModel;
using Xunit;

namespace Horarium.Test.TypeName
{
    public class AssemblyQualifiedNameWithoutVersionTests
    {
        [Fact]
        [Category("OldBehavior")]
        public void GetName_ForNonGenericJob_ShouldRemoveVersion()
        {
            var jobName = new FirstNonGenericJob().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<FirstNonGenericJob>(instance);
            Assert.Equal("Horarium.Test.TypeName.FirstNonGenericJob, Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForNonGenericJobWithAttribute_ShouldRemoveVersion()
        {
            var jobName = new FirstNonGenericJobWithAttribute().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<FirstNonGenericJobWithAttribute>(instance);
            Assert.Equal("Horarium.Test.TypeName.FirstNonGenericJobWithAttribute, Horarium.Test", jobName);
        }

        [Fact]
        [Category("OldBehavior")]
        public void GetName_ForGenericJobWithTwoArguments_ShouldNotRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithTwoArguments<FirstNonGenericJob, SecondNonGenericJob>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithTwoArguments<FirstNonGenericJob, SecondNonGenericJob>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJobWithTwoArguments`2[[Horarium.Test.TypeName.FirstNonGenericJob, Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithTwoArgumentsWithAttribute_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithTwoArgumentsWithAttribute<FirstNonGenericJob, SecondNonGenericJob>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithTwoArgumentsWithAttribute<FirstNonGenericJob, SecondNonGenericJob>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJobWithTwoArgumentsWithAttribute`2[[Horarium.Test.TypeName.FirstNonGenericJob, Horarium.Test], [Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test]], Horarium.Test", jobName);
        }

        [Fact]
        [Category("OldBehavior")]
        public void GetName_ForGenericJobWithTwoNestedGenericArguments_ShouldNotRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithTwoArguments<GenericJob<FirstNonGenericJob>, GenericJob<SecondNonGenericJob>>()
                .GetType()
                .AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithTwoArguments<GenericJob<FirstNonGenericJob>, GenericJob<SecondNonGenericJob>>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJobWithTwoArguments`2[[Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.FirstNonGenericJob, Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithTwoNestedGenericArgumentsWithAttribute_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithTwoArgumentsWithAttribute<GenericJob<FirstNonGenericJob>, GenericJob<SecondNonGenericJob>>()
                .GetType()
                .AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithTwoArgumentsWithAttribute<GenericJob<FirstNonGenericJob>, GenericJob<SecondNonGenericJob>>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJobWithTwoArgumentsWithAttribute`2[[Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.FirstNonGenericJob, Horarium.Test]], Horarium.Test], [Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test]], Horarium.Test]], Horarium.Test", jobName);
        }

        [Fact]
        [Category("OldBehavior")]
        public void GetName_ForGenericJobWithSingleGenericArgument_ShouldNotRemoveVersionForAllTypes()
        {
            var jobName = new GenericJob<SecondNonGenericJob>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJob<SecondNonGenericJob>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithSingleGenericArgumentWithAttribute_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithAttribute<SecondNonGenericJob>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithAttribute<SecondNonGenericJob>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJobWithAttribute`1[[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test]], Horarium.Test", jobName);
        }

        [Fact]
        [Category("OldBehavior")]
        public void GetName_ForGenericJobWithSingleNestedGenericArgument_ShouldNotRemoveVersionForAllTypes()
        {
            var jobName = new GenericJob<GenericJob<SecondNonGenericJob>>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJob<GenericJob<SecondNonGenericJob>>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Horarium.Test", jobName);
        }

        [Fact]
        public void GetName_ForGenericJobWithSingleNestedGenericArgumentWithAttribute_ShouldRemoveVersionForAllTypes()
        {
            var jobName = new GenericJobWithAttribute<GenericJob<SecondNonGenericJob>>().GetType().AssemblyQualifiedNameWithoutVersion();

            var type = Type.GetType(jobName, true);
            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);
            Assert.IsType<GenericJobWithAttribute<GenericJob<SecondNonGenericJob>>>(instance);
            Assert.Equal("Horarium.Test.TypeName.GenericJobWithAttribute`1[[Horarium.Test.TypeName.GenericJob`1[[Horarium.Test.TypeName.SecondNonGenericJob, Horarium.Test]], Horarium.Test]], Horarium.Test", jobName);
        }
    }
}