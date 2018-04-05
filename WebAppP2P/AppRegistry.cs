using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;
using WebAppP2P.Jobs;
using WebAppP2P.Services;

namespace WebAppP2P
{

    public class AppRegistry : Registry
    {
        private static object _tracerLock = new object();
        private static object _blockchainLock = new object();

        public AppRegistry(IServiceProvider applicationServices, IOptionsSnapshot<NodeOptions> nodeOptions)
        {
            NonReentrantAsDefault();
            Schedule(
                    TracerJob(applicationServices)
                ).NonReentrant().ToRunEvery(nodeOptions.Value.Tracer.JobInterval).Seconds();

            Schedule(
                    TracerJob(applicationServices)
                ).NonReentrant().ToRunNow();
            
            Schedule(
                    BlockchainJob(applicationServices)
                ).NonReentrant().ToRunEvery(nodeOptions.Value.Blockchain.JobInterval).Seconds(); 
        }

        private static Action TracerJob(IServiceProvider applicationServices)
        {
            return () =>
            {
                lock (_tracerLock)
                {
                    var scope = applicationServices.CreateScope();
                    Console.WriteLine("Start Tracer {0}",DateTime.Now);
                    var job = scope.ServiceProvider.GetService<TracerJob>();
                    job.Execute();
                    Console.WriteLine("Stop Tracer {0}", DateTime.Now);
                }

            };
        }

        private static Action BlockchainJob(IServiceProvider applicationServices)
        {
            return () =>
            {
                lock (_blockchainLock)
                {
                    var scope = applicationServices.CreateScope();
                    Console.WriteLine("Start Blockchain {0}", DateTime.Now);
                    var job = scope.ServiceProvider.GetService<BlockchainJob>();
                    job.Execute();
                    Console.WriteLine("Stop Blockchain {0}", DateTime.Now);
                }
            };
        }
    }
}
