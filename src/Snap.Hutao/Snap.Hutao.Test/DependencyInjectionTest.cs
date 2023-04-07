using Microsoft.Extensions.DependencyInjection;
using System;

namespace Snap.Hutao.Test;

[TestClass]
public class DependencyInjectionTest
{
    [TestMethod]
    public void OriginalTypeNotDiscoverable()
    {
        IServiceProvider services = new ServiceCollection()
            .AddSingleton<IService, ServiceA>()
            .AddSingleton<IService, ServiceB>()
            .BuildServiceProvider();

        Assert.IsNull(services.GetService<ServiceA>());
        Assert.IsNull(services.GetService<ServiceB>());
    }

    [TestMethod]
    public void ScopedServiceInitializeMultipleTimesInScope()
    {
        IServiceProvider services = new ServiceCollection()
            .AddScoped<IService, ServiceA>()
            .BuildServiceProvider();

        IServiceScopeFactory scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            IService service1 = scope.ServiceProvider.GetRequiredService<IService>();
            IService service2 = scope.ServiceProvider.GetRequiredService<IService>();
            Assert.AreNotEqual(service1.Id, service2.Id);
        }
    }

    private interface IService
    {
        Guid Id { get; }
    }

    private sealed class ServiceA : IService
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
}