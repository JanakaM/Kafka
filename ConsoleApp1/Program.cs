using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, collection) =>
                {
                    collection.AddHostedService<KafkaConsumerHostedService>();
                    collection.AddHostedService<KafkaProducerHostingService>();
                });
    }

    public class KafkaConsumerHostedService : IHostedService
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly ClusterClient _cluster;
        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger)
        {
            _logger = logger;
            _cluster = new ClusterClient(new Configuration()
            {
                Seeds = "localhost:9092"

            }, new ConsoleLogger());
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("demo");
            _cluster.MessageReceived += record =>
            {
                _logger.LogInformation($"Received {Encoding.UTF8.GetString(record.Value as byte[])}");
            };

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            return Task.CompletedTask;
        }
    }

    public class KafkaProducerHostingService : IHostedService
    {
        private readonly ILogger<KafkaProducerHostingService> _log;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerHostingService(ILogger<KafkaProducerHostingService> logger)
        {
            _log = logger;

            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
            };
            
            _producer = new ProducerBuilder<Null, string>(config)
                .Build();
        }

        private void LogHandler(IProducer<Null, string> arg1, LogMessage arg2)
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 1; i < 100; i++)
            {
                var value = $"Hello Minecraft world {i}";
                _log.LogInformation(value);

                await _producer.ProduceAsync("demo", new Message<Null, string>()
                {
                    Value = value,
                }, cancellationToken);
            }

            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }
}