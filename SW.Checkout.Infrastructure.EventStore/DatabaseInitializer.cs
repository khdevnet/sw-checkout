﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SW.Checkout.Core.Aggregates;
using SW.Checkout.Core.Events;
using SW.Checkout.Core.Initializers;
using SW.Checkout.Core.Queues.ProcessOrder;
using SW.Checkout.Domain.Warehouses;

namespace SW.Checkout.Infrastructure.EventStore
{
    internal class DatabaseInitializer : IInitializer
    {
        private readonly IAggregationRepository repository;
        private readonly IReadStorageSyncEventBus readStorageSyncEventBus;

        public int Order { get; } = 2;

        public DatabaseInitializer(IAggregationRepository repository, IReadStorageSyncEventBus readStorageSyncEventBus)
        {
            this.repository = repository;
            this.readStorageSyncEventBus = readStorageSyncEventBus;
        }

        public void Init()
        {
            WarehouseAggregate[] aggregates = new[] {
                CreateWarehouseAggregate(Guid.Parse("6df8744a-d464-4826-91d1-08095ab49d93"), "Naboo"),
                CreateWarehouseAggregate(Guid.Parse("6df8744a-d464-4826-91d1-08095ab49d94"), "Tatooine")
                };

            var events = aggregates.ToDictionary(agg => agg.Id, agg => agg.PendingEvents.ToList());

            Func<Dictionary<Guid, List<IEvent>>> transactionFunc = () => events;
            Action transactionPostProcessFunc = () => events.SelectMany(agg => agg.Value).ToList().ForEach(@event => readStorageSyncEventBus.Send(@event));

            int attempts_count = 0;
            while (attempts_count <= 20)
            {
                try
                {
                    repository.Transaction(transactionFunc, transactionPostProcessFunc);
                    attempts_count = 21;
                }
                catch (Exception)
                {
                    attempts_count += 1;
                    Console.WriteLine($"### Retry connect to Rabbit MQ attempt {attempts_count}");
                }
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        private static WarehouseAggregate CreateWarehouseAggregate(Guid warehouseId, string name)
        {
            return new WarehouseAggregate(warehouseId, name, Enumerable.Range(1, 5).Select(productId => new WarehouseItem
            {
                ProductId = productId,
                Quantity = 5000
            }));
        }
    }
}
