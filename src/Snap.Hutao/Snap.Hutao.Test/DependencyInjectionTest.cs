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

    private interface IService
    {
    }

    private sealed class ServiceA : IService
    {
    }

    private sealed class ServiceB : IService
    {
    }
}