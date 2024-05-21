using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Snap.Hutao.Test.PlatformExtensions;

[TestClass]
public sealed class DependencyInjectionTest
{
    private readonly IServiceProvider services = new ServiceCollection()
        .AddSingleton<IService, ServiceA>()
        .AddSingleton<IService, ServiceB>()
        .AddScoped<IScopedService, ServiceA>()
        .AddKeyedTransient<IKeyedService, KeyedServiceA>("A")
        .AddKeyedTransient<IKeyedService, KeyedServiceB>("B")
        .AddTransient(typeof(IGenericService<>), typeof(GenericService<>))
        .AddLogging(builder => builder.AddConsole())
        .BuildServiceProvider();

    [TestMethod]
    public void OriginalTypeCannotResolved()
    {
        Assert.IsNull(services.GetService<ServiceA>());
        Assert.IsNull(services.GetService<ServiceB>());
    }

    [TestMethod]
    public void GenericServicesCanBeResolved()
    {
        IServiceProvider services = new ServiceCollection()
            .AddTransient(typeof(IGenericService<>), typeof(GenericService<>))
            .BuildServiceProvider();

        Assert.IsNotNull(services.GetService<IGenericService<int>>());
    }

    [TestMethod]
    public void ScopedServiceInitializeMultipleTimesInScope()
    {
        using (IServiceScope scope = services.CreateScope())
        {
            IScopedService service1 = scope.ServiceProvider.GetRequiredService<IScopedService>();
            IScopedService service2 = scope.ServiceProvider.GetRequiredService<IScopedService>();
            Assert.AreNotEqual(service1.Id, service2.Id);
        }
    }

    [TestMethod]
    public void LoggerWithInterfaceTypeCanBeResolved()
    {
        Assert.IsNotNull(services.GetService<ILogger<IScopedService>>());
        Assert.IsNotNull(services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IScopedService)));
    }

    [TestMethod]
    public void KeyedServicesCanBeResolvedAsEnumerable()
    {
        Assert.IsNotNull(services.GetRequiredKeyedService<IKeyedService>("A"));
        Assert.IsNotNull(services.GetRequiredKeyedService<IKeyedService>("B"));

        Assert.AreEqual(0, services.GetServices<IKeyedService>().Count());
    }

    private interface IService
    {
        Guid Id { get; }
    }

    private interface IScopedService
    {
        Guid Id { get; }
    }

    private sealed class ServiceA : IService, IScopedService
    {
        public Guid Id
        {
            get => Guid.NewGuid();
        }
    }

    private sealed class ServiceB : IService
    {
        public Guid Id
        {
            get => throw new NotImplementedException();
        }
    }

    private interface IGenericService<T>
    {
    }

    private sealed class GenericService<T> : IGenericService<T>
    {
    }

    private sealed class NonInjectedServiceA
    {
    }

    private sealed class NonInjectedServiceB
    {
        [ActivatorUtilitiesConstructor]
        public NonInjectedServiceB(NonInjectedServiceA? serviceA)
        {
        }
    }

    private interface IKeyedService;

    private sealed class KeyedServiceA : IKeyedService
    {
    }

    private sealed class KeyedServiceB : IKeyedService
    {
    }
}