#nullable enable

namespace Common;

/// <summary>
/// Провайдер для получения названия очереди из типов аргумента (по умолчанию из типа активити)
/// </summary>
public class RabbitMqEndpointAddressProvider :
  IEndpointAddressProvider
{
  private readonly EndpointNameFormatterByArgs _formatter;

  public RabbitMqEndpointAddressProvider(EndpointNameFormatterByArgs formatter)
  {
    _formatter = formatter;
  }

  public Uri GetExecuteEndpoint<TActivityArguments>()
    where TActivityArguments : class
  {
    return new Uri($"exchange:{_formatter.ExecuteActivityByArgs<TActivityArguments>()}");
  }
}

public interface IEndpointAddressProvider
{
  Uri GetExecuteEndpoint<TActivityArguments>()
    where TActivityArguments : class;
}