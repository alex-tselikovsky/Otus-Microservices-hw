#nullable enable

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth;

public class UserDto
{
  public string? Login { get; set; }
  public string? Email { get; set; }
  public string? Name { get; set; }
}

public class UserPasswordDto:UserDto
{
  public string? Password { get; set; }
}

public class UserDb
{
  public UserDb()
  {
    Id = ObjectId.GenerateNewId().ToString();
  }
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; }
  public string? Login { get; set; }
  public string? Email { get; set; }
  public string? Name { get; set; }
  public string PassHash { get; set; }
}

public class UserLogin
{
  public string? Login { get; set; }
  public string? Password { get; set; }
}