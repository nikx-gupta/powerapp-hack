using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracking.Clients
{
    public class OutputWriterFactory
    {
        private readonly IServiceProvider _svcProvider;
        private IServiceScope _scope;

        public OutputWriterFactory(IServiceProvider svcProvider)
        {
            _svcProvider = svcProvider;
        }

        public IOutputWriter Instance()
        {
            _scope = _svcProvider.CreateScope();
            return _scope.ServiceProvider.GetRequiredService<IOutputWriter>();
        }

        public void Flush()
        {
            _scope.Dispose();
        }
    }
}
