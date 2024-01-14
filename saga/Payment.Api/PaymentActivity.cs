#nullable enable

using Common.ActivityArguments;
using MassTransit;

namespace Payment.Api;

public class PaymentActivity:IActivity<PaymentArgs, PaymentLog>
{
  private readonly ILogger<PaymentActivity> _logger;
  private List<Payment> _collection;
  public PaymentActivity(ILogger<PaymentActivity> logger, List<Payment> collection)
  {
    _logger = logger;
    _collection = collection;
  }

  public async Task<ExecutionResult> Execute(ExecuteContext<PaymentArgs> context)
  {
    _logger.LogInformation($"Payment for order {context.Arguments.OrderId} processing");
    await Task.Delay(100);
    var payment = new Payment()
    {
      PaymentId = NewId.NextGuid(),
      OrderId = context.Arguments.OrderId,
      Status = Status.Success,
      Value = context.Arguments.Value
    };
    _logger.LogInformation($"Payment for order {context.Arguments.OrderId} success");
    _collection.Add(payment);
    return context.CompletedWithVariables(new PaymentLog(){PaymentId = payment.PaymentId }, new {payment.PaymentId});
  }

  public async Task<CompensationResult> Compensate(CompensateContext<PaymentLog> context)
  {
    _logger.LogInformation($"Payment for order {context.GetVariable<string>("OrderId")} refunding");
    var paymentId = context.Log.PaymentId;
    var payment = _collection.FirstOrDefault(p=>p.PaymentId == paymentId);
    if (payment == default)
    {
      _logger.LogWarning($"Payment {paymentId} not found");
      return context.Compensated();
    }
    await Task.Delay(10);
    payment.Status = Status.Returned;
    return context.Compensated();
  }
}

public class PaymentLog
{
  public Guid PaymentId { get; set; }
}