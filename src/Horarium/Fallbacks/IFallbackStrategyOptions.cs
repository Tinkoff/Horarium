﻿using System;
using Horarium.Builders.Fallback;
using Horarium.Interfaces;

namespace Horarium.Fallbacks
{
    public interface IFallbackStrategyOptions
    {
        void CreateFallbackJob<TJob, TJobParam>(TJobParam parameters, Action<IFallbackJobBuilder> fallbackJobConfigure)
            where TJob : IJob<TJobParam>;

        void StopExecution();

        void GoNext();
    }
}