# Horarium

Horarium is the .Net library to manage background jobs and its compatibility with .Net Framework and .Net Core.
Horarium fully based on asynchronous work model, it allows you to run hundreds of parallels jobs on one instance. It supports distributed system for management jobs, that use for it MongoDB.

## Getting started

Add nuget-package Horarium

```bash
dotnet add package Horarium
```

Add job implementation interface ```IJob<T>```

```csharp
    public class TestJob : IJob<int>
    {
        public async Task Execute(int param)
        {
            Console.WriteLine(param);
            await Task.Run(() => { });
        }
    }
```

Create ```HorariumServer``` and schedule ```TestJob```

```csharp
var horarium = new HorariumServer("mongodb://localhost:27017/horarium");
await horarium.Create<TestJob, int>(666)
        .Schedule();
```

## Add to ```Asp.Net core``` application

Create ```JobFactory```  for creating job use DI in ```Asp.Net core```

```csharp
public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object CreateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }

        public IDisposable BeginScope()
        {
            return _serviceProvider.CreateScope();
        }
    }
```

Registration Horarium in DI

```csharp
service.AddSingleton<IHorarium>(serviceProvider =>
                new HorariumServer("mongodb://localhost:27017/horarium",
                    new HorariumSettings()
                    {
                        JobFactory = new JobFactory(serviceProvider)
                    });)
```

Use interface ```IHorarium``` in Controller

```csharp

    private readonly IHorarium _horarium;

    public HomeController(IHorarium horarium)
    {
        _horarium = horarium;
    }

    [Route("api")]
    public class HomeController : Controller
    {
        [HttpPost]
        public Task Run(int count)
        {
            await horarium.Create<TestJob, int>(count)
                          .Schedule();
        }
    }
```

## Create Recurrent Job

Add job implementation interface ```IJobRecurrent```

```csharp
public class TestRecurrentJob : IJobRecurrent
    {
        public Task Execute()
        {
            Console.WriteLine("Run -" + DateTime.Now);
            return Task.CompletedTask;
        }
    }
```

Schedule ```TestRecurrentJob``` that run every 15 seconds

```csharp
await horarium.CreateRecurrent<TestRecurrentJob>(Cron.SecondInterval(15))
                .Schedule();
```

## Create sequence jobs

Sometimes you need to create sequence jobs when first job success second job will run. If the job failed next jobs won't start

```csharp
await horarium
    .Create<TestJob, int>(1) // 1-st job
    .Next<TestJob, int>(2) // 2-nd job
    .Next<TestJob, int>(3) // 3-rd job
    .Schedule();
```

## Distributed Horarium

![Distributed Scheme](DistributedScheme.png)

Horarium has two types of class: server and client. A client can only add new jobs in DB and server can add and run jobs. Horarium guarantees that job run **only one time.**