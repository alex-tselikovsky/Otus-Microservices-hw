#nullable enable

namespace Delivery;

public class Delivery
{
  public Guid OrderId { get; set; }
  public string Address { get; set; }
  public DeliveryStatus Status { get; set; }
  public string Area { get; set; }
}

public enum DeliveryStatus
{
  CourierNotFound,
  Shipped,
  Delivered
}