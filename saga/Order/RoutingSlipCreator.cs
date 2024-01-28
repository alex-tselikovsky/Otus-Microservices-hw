#nullable enable

using Common;
using Common.ActivityArguments;
using MassTransit;
using MassTransit.Courier.Contracts;
using Payment.Api;

namespace Api;

public class RoutingSlipCreator
{
  private readonly IEndpointAddressProvider _endpointAddressProvider;

  public RoutingSlipCreator(IEndpointAddressProvider endpointAddressProvider)
  {
    _endpointAddressProvider = endpointAddressProvider;
  }

  internal RoutingSlip CreateRoutingSlip(Order order, ProcessOrder processOrder)
  {
    var builder = new RoutingSlipBuilder(NewId.NextGuid());

    builder.SetVariables(new
    {
      order.OrderId,
    });

    builder.AddActivity("ProcessPayment", _endpointAddressProvider.GetExecuteEndpoint<PaymentArgs>(), new PaymentArgs { OrderId = order.OrderId});
    builder.AddActivity(
      "ProcessDelivery",
      _endpointAddressProvider.GetExecuteEndpoint<DeliveryArgs>(),
      new DeliveryArgs
      {
        OrderId = order.OrderId,
        Area = processOrder.DeliveryArea,
        Address = processOrder.DeliveryAddress
      });
    // builder.AddActivity("ProcessDelivery", deliveryServiceUri);
    //
    // builder.AddSubscription(RoutingSlipEvents.ActivityFaulted, RoutingSlipEventContents.None, "ProcessPayment",
    //   x => x.Send<OrderPaymentFailed>(new { context.Message.OrderId }));
    
    // builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Completed,
    //   x => x.Send<RegistrationCompleted>(new { context.Message.OrderId }));

    return builder.Build();
  }
}


public record ProcessOrder
{
  public string DeliveryAddress { get; set; }
  public string DeliveryArea { get; set; }
  public string[] Skus { get; set; }
}

