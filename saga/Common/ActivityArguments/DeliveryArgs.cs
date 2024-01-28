#nullable enable

namespace Common.ActivityArguments;

public class DeliveryArgs
{
  public Guid OrderId { get; set; }
  public string Area { get; set; }
  public string Address { get; set; }
}