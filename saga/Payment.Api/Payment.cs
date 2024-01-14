namespace Payment.Api;

public class Payment
{
  public Guid PaymentId { get; set; }
  public Guid OrderId { get; set; }
  
  public decimal Value { get; set; }
  public Status Status { get; set; }
}
public enum Status
{
  Success,
  Returned
}