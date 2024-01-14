#nullable enable

using MassTransit;

namespace Common;

public class EndpointNameFormatterByArgs:DefaultEndpointNameFormatter
{
  public override string ExecuteActivity<T, TArguments>()
  {
    return ExecuteActivityByArgs<TArguments>();
  }

  public string ExecuteActivityByArgs<TArguments>()
  {
    var argsName = SanitizeName(FormatName(typeof(TArguments)));
    return $"{argsName}_execute";
  }
}