#nullable enable

namespace Api;

public class Order
{
  public Guid OrderId { get; set; }
  public string[] Skus { get; set; }
  public decimal Value { get; set; }
  public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
  Creating, //собирается пользователем
  Processing, //Оформляется
  Delivering, //Заказ оформлен. Запущен процесс отправки
  Delivered,//Доставлен
  Canceled// отменен
}