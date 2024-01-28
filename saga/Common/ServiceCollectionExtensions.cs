#nullable enable

using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class ServiceCollectionExtensions
{
  /// <summary>
  ///Устанавливаем форматтер для имен очередей через аргументы активити.
  ///Стандартный форматтер использует имя класса активити, что связывает процесс создания маршрута с реализацией всех активити.
  /// Кастомный форматтер в качестве имени очереди использует имя класс аргументов, что сильно уменьшает связность сервисов.
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection SetEndpointNameFormatterByArgs(this IServiceCollection services)
  {
    services.AddSingleton<EndpointNameFormatterByArgs>();
    services.AddSingleton<IEndpointNameFormatter>(sp =>
      sp.GetRequiredService<EndpointNameFormatterByArgs>());
    services.AddSingleton<IEndpointAddressProvider, RabbitMqEndpointAddressProvider>();
    return services;
  }
}