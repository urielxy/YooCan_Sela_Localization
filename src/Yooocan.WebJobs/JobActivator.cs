using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace Yooocan.WebJobs
{
    internal class JobActivator : IJobActivator
    {
        private readonly IServiceProvider _container;

        public JobActivator(IServiceProvider container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.GetService<T>();
        }
    }
}
