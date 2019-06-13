using System;
using Horarium.AspNetCore;
using Horarium.Interfaces;
using Horarium.Repository;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Horarium.Test.AspNetCore
{
    public class RegistrationHorariumExtensionTest
    {
        [Fact]
        public void AddHorariumServer_DefaultSettings_ReplaceForAspNetCore()
        {
            var serviceMock = new Mock<IServiceCollection>();

            var service = serviceMock.Object;

            ServiceDescriptor descriptor = null;

            var settings = new HorariumSettings();

            serviceMock.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => descriptor = x);

            service.AddHorariumServer(Mock.Of<IJobRepository>(),
                provider => settings);

            var horarium = descriptor.ImplementationFactory(Mock.Of<IServiceProvider>());

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(IHorarium), descriptor.ServiceType);
            Assert.Equal(typeof(JobScopeFactory), settings.JobScopeFactory.GetType());
            Assert.Equal(typeof(HorariumLogger), settings.Logger.GetType());
            Assert.Equal(typeof(HorariumServer), horarium.GetType());
        }

        [Fact]
        public void AddHorariumClient_DefaultSettings_ReplaceForAspNetCore()
        {
            var serviceMock = new Mock<IServiceCollection>();

            var service = serviceMock.Object;

            ServiceDescriptor descriptor = null;

            var settings = new HorariumSettings();

            serviceMock.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => descriptor = x);

            service.AddHorariumClient(Mock.Of<IJobRepository>(),
                provider => settings);

            var horarium = descriptor.ImplementationFactory(Mock.Of<IServiceProvider>());

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(IHorarium), descriptor.ServiceType);
            Assert.Equal(typeof(JobScopeFactory), settings.JobScopeFactory.GetType());
            Assert.Equal(typeof(HorariumLogger), settings.Logger.GetType());
            Assert.Equal(typeof(HorariumClient), horarium.GetType());
        }

        [Fact]
        public void AddHorariumClient_CustomSettings_DontReplaceForAspNetCore()
        {
            var serviceMock = new Mock<IServiceCollection>();

            var service = serviceMock.Object;

            var settings = new HorariumSettings
            {
                JobScopeFactory = new JobScopeFactoryTest(),
                Logger = new LoggerTest()
            };

            serviceMock.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => { });

            service.AddHorariumClient(Mock.Of<IJobRepository>(),
                provider => settings);

            Assert.Equal(typeof(JobScopeFactoryTest), settings.JobScopeFactory.GetType());
            Assert.Equal(typeof(LoggerTest), settings.Logger.GetType());
        }
        
        [Fact]
        public void AddHorariumServer_CustomSettings_DontReplaceForAspNetCore()
        {
            var serviceMock = new Mock<IServiceCollection>();

            var service = serviceMock.Object;

            var settings = new HorariumSettings
            {
                JobScopeFactory = new JobScopeFactoryTest(),
                Logger = new LoggerTest()
            };

            serviceMock.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => { });

            service.AddHorariumServer(Mock.Of<IJobRepository>(),
                provider => settings);

            Assert.Equal(typeof(JobScopeFactoryTest), settings.JobScopeFactory.GetType());
            Assert.Equal(typeof(LoggerTest), settings.Logger.GetType());
        }

        class JobScopeFactoryTest : IJobScopeFactory
        {
            public IJobScope Create()
            {
                throw new NotImplementedException();
            }
        }

        class LoggerTest : IHorariumLogger
        {
            public void Debug(string msg)
            {
                throw new NotImplementedException();
            }

            public void Debug(Exception ex)
            {
                throw new NotImplementedException();
            }

            public void Error(Exception ex)
            {
                throw new NotImplementedException();
            }

            public void Error(string message, Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}