using System.Threading;
using System.Threading.Tasks;
using Horarium.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Horarium.AspNetCore
{
    public class HorariumServerHostedService : IHostedService
    {
        private readonly HorariumServer _horariumServer;

        public HorariumServerHostedService(IHorarium horarium)
        {
            _horariumServer = (HorariumServer) horarium;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _horariumServer.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _horariumServer.Stop(cancellationToken);
        }
    }
}