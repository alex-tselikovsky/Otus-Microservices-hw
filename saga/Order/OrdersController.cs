using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api;

[ApiController]
[Route("[controller]")]
public class OrdersController :  ControllerBase
{
  private readonly RoutingSlipCreator _routingSlipCreator;
  private List<Order> _orders;
  public OrdersController(RoutingSlipCreator routingSlipCreator, List<Order> orders)
  {
    _routingSlipCreator = routingSlipCreator;
    _orders = orders;
  }

  [HttpGet("{orderId}")]
  public IActionResult GetOrderInfo(string orderId)
  {
    var id = Guid.Parse(orderId);
    var order = _orders.FirstOrDefault(o => o.OrderId == id);
    if (order != default)
      return Ok(order);
    return NotFound("Order not found");
  }

  [HttpPost]
  public async Task<IActionResult> CreateOrder(ProcessOrder processOrder, [FromServices] IBus bus)
  {
    var order = new Order()
    {
      OrderId = NewId.NextGuid(),
      Skus = processOrder.Skus,
      Status = OrderStatus.Processing,
      Value = 5000
    };
    _orders.Add(order);
    var routingSlip = _routingSlipCreator.CreateRoutingSlip(order, processOrder);
    
    await bus.Execute(routingSlip).ConfigureAwait(false);
    return Ok(order.OrderId);
  }
}

