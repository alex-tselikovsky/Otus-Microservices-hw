#nullable enable

using MassTransit;
using MassTransit.Courier.Contracts;

namespace Api;

public class RoutingSlipActivityCompletedConsumer:IConsumer<RoutingSlipActivityCompleted>
{
  private IList<Order> _orders;
  private ILogger<RoutingSlipActivityCompletedConsumer> _logger;

  public RoutingSlipActivityCompletedConsumer(List<Order> orders, ILogger<RoutingSlipActivityCompletedConsumer> logger)
  {
    _orders = orders;
    _logger = logger;
  }

  public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
  {
    var activity = context.Message.ActivityName;
    _logger.LogError($"Activity {activity} success. Order is being deliveried");
    
    var orderId = Guid.Parse((string)context.Message.Variables["OrderId"]);
    var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
    if (order != default)
      order.Status = OrderStatus.Delivering;
    return Task.CompletedTask;  }
}