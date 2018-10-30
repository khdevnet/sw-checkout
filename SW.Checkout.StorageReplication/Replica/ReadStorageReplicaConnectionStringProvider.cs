﻿using Microsoft.Extensions.Configuration;
using SW.Checkout.Core.Replication;
using SW.Checkout.Core.Settings;

namespace SW.Checkout.StorageReplication.Replica
{
    internal class ReadStorageReplicaConnectionStringProvider : ConnectionStringProviderBase, IReadStorageReplicaConnectionStringProvider
    {
        public ReadStorageReplicaConnectionStringProvider(IConfiguration configuration) : base(configuration, "ReadStorageReplica")
        {

        }
    }
}
