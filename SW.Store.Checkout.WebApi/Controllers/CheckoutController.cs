﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SW.Store.Checkout.Domain.Orders.Commands;
using SW.Store.Checkout.Read.Extensibility;
using SW.Store.Checkout.Read.ReadView;
using SW.Store.Checkout.WebApi.Models;
using SW.Store.Core.Messages;
using SW.Store.Core.Queues.ProcessOrder;
using System;

namespace SW.Store.Checkout.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProcessOrderQueueCommandBus commandBus;
        private readonly IOrderReadRepository orderReadRepository;

        public CheckoutController(
            IMessageSender messageSender,
            IMapper mapper,
            IProcessOrderQueueCommandBus commandBus,
            IOrderReadRepository orderReadRepository)
        {
            this.mapper = mapper;
            this.commandBus = commandBus;
            this.orderReadRepository = orderReadRepository;
        }

        // POST api/orders
        [HttpPost]
        public IActionResult Post([FromBody] CreateOrderModel createOrder)
        {
            var orderId = Guid.NewGuid();
            commandBus.Send(new CreateOrder
            {
                OrderId = orderId,
                CustomerId = createOrder.CustomerId,
                Lines = createOrder.Lines
            });
            return Created("status", orderId);
        }

        [HttpPut]
        [Route("add-line")]
        public void AddLine([FromBody] AddOrderLine addOrderLine)
        {
            commandBus.Send(addOrderLine);
        }

        [HttpPut]
        [Route("remove-line")]
        public void RemoveLine([FromBody] RemoveOrderLine removeOrderLine)
        {
            commandBus.Send(removeOrderLine);
        }

        ///GET api/orders/status
        [HttpGet]
        [Route("status/{id}")]
        public IActionResult Status([FromRoute] Guid id)
        {
            OrderReadView order = orderReadRepository.GetById(id);
            if (order != null)
            {
                return Ok(mapper.Map<OrderReadModel>(order));
            }
            return NotFound();
        }
    }
}
