﻿using System;
using System.Collections.Generic;
using SW.Store.Checkout.Extensibility.Dto;

namespace SW.Store.Checkout.Extensibility.Client
{
    public class CreateOrderRequestModel
    {
        public Guid OrderId { get; set; }

        public int CustomerId { get; set; }

        public IEnumerable<OrderLineDto> Lines { get; set; }
    }
}
