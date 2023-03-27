using Microsoft.Extensions.DependencyInjection;
using System;

namespace Snap.Hutao.Test;

[TestClass]
public class DependencyInjectionTest
{
    [TestMethod]
    public void OriginalTypeDiscoverable()
    {
        IServiceProvider services = new ServiceCollection()
            .AddSingleton<IService, ServiceA>()
            .AddSingleton<IService, ServiceB>()
            .BuildServiceProvider();

        Assert.IsNotNull(services.GetService<ServiceA>());
        Assert.IsNotNull(services.GetService<ServiceB>());
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