#nullable enable

using Common.ActivityArguments;
using MassTransit;

namespace Delivery;

public class DeliveryActivity:IExecuteActivity<DeliveryArgs>
{
  private readonly ILogger<DeliveryActivity> _logger;
  private List<Delivery> _collection;
  public DeliveryActivity(ILogger<DeliveryActivity> logger, List<Delivery> collection)
  {
    _logger = logger;
    _collection = collection;
  }

  public async Task<ExecutionResult> Execute(ExecuteContext<DeliveryArgs> context)
  {
    _logger.LogInformation($"Searching courier for order {context.Arguments.OrderId} processing");
    await Task.Delay(100);

    var delivery = new Delivery()
    {
      OrderId = context.Arguments.OrderId,
      Address = context.Arguments.Address,
      Area = context.Arguments.Area
    };
    if (context.Arguments.Area == "Moscow")
    {
      delivery.Status = DeliveryStatus.CourierNotFound;
      _collection.Add(delivery);
      throw new RoutingSlipException("Can't deliver to this region");
    }

    delivery.Status = DeliveryStatus.Shipped;
    _logger.LogInformation($"Order {context.Arguments.OrderId} success delivered");
    _collection.Add(delivery);
    
    return context.Completed();
  }
}