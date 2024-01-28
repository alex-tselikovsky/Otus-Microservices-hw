#nullable enable

using MassTransit;
using MassTransit.Courier.Contracts;

namespace Api;

public class RoutingSlipActivityFaultedConsumer:IConsumer<RoutingSlipActivityFaulted>
{
  private IList<Order> _orders;
  private ILogger<RoutingSlipActivityFaultedConsumer> _logger;

  public RoutingSlipActivityFaultedConsumer(ILogger<RoutingSlipActivityFaultedConsumer> logger, List<Order> orders)
  {
    _logger = logger;
    _orders = orders;
  }

  public Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
  {
    var activity = context.Message.ActivityName;
    _logger.LogError($"Activity {activity} faulted. Order is being cancelled");
    
    var orderId = Guid.Parse((string)context.Message.Variables["OrderId"]);
    var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
    if (order != default)
      order.Status = OrderStatus.Canceled;
    return Task.CompletedTask;
  }
}