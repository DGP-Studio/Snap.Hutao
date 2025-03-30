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
        .AddTransient<IKeyedService, NoKeyService>()
        .AddKeyedTransient<IKeyedService, KeyedServiceA>("A")
        .AddKeyedTransient<IKeyedService, KeyedServiceB>("B")
        .AddTransient(typeof(IGenericService<>), typeof(GenericService<>))
        .AddTransient(typeof(IGenericService<int>), typeof(CloseGenericService))
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
        Assert.IsNotNull(services.GetService<IGenericService<double>>());
    }

    [TestMethod]
    public void CloseGenericSeriveCanBeResolved()
    {
        IGenericService<int> service = services.GetRequiredService<IGenericService<int>>();
        Assert.IsTrue(service is CloseGenericService);
    }

    [TestMethod]
    public void ScopedServiceInitializeOneTimeInScope()
    {
        using (IServiceScope scope = services.CreateScope())
        {
            IScopedService service1 = scope.ServiceProvider.GetRequiredService<IScopedService>();
            IScopedService service2 = scope.ServiceProvider.GetRequiredService<IScopedService>();
            Assert.AreEqual(service1.Id, service2.Id);
        }
    }

    [TestMethod]
    public void LoggerWithInterfaceTypeCanBeResolved()
    {
        Assert.IsNotNull(services.GetService<ILogger<IScopedService>>());
        Assert.IsNotNull(services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IScopedService)));
    }

    [TestMethod]
    public void KeyedServicesCanNotBeResolvedAsEnumerable()
    {
        Assert.IsNotNull(services.GetRequiredKeyedService<IKeyedService>("A"));
        Assert.IsNotNull(services.GetRequiredKeyedService<IKeyedService>("B"));

        Assert.AreEqual(1, services.GetServices<IKeyedService>().Count());
    }

    [TestMethod]
    public void NoKeyServiceCanBeResolved()
    {
        Assert.IsNotNull(services.GetRequiredKeyedService<IKeyedService>("A"));
        Assert.IsNotNull(services.GetRequiredService<IKeyedService>());
    }

    [TestMethod]
    public void TransientServiceIsDisposedInScope()
    {
        IServiceProvider serviceProvider = new ServiceCollection().AddTransient<DisposableService>().BuildServiceProvider();

        DisposableService service;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            service = scope.ServiceProvider.GetRequiredService<DisposableService>();
        }

        Assert.IsTrue(service.IsDisposed);
        GC.KeepAlive(service);
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
        public Guid Id { get; } =  Guid.NewGuid();
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

    private sealed class CloseGenericService : IGenericService<int>
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

    private sealed class NoKeyService : IKeyedService;

    private sealed class KeyedServiceA : IKeyedService;

    private sealed class KeyedServiceB : IKeyedService;

    private sealed class DisposableService : IDisposable
    {
        private bool isDisposed;

        public bool IsDisposed { get => isDisposed; }

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}