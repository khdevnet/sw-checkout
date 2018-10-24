﻿using System;
using SW.Store.Core.Commands;

namespace SW.Store.Checkout.Domain.Accounts.Commands
{
    public class RemoveOrderLine : ICommand
    {
        public Guid OrderId { get; set; }

        public int ProductNumber { get; set; }
    }
}
