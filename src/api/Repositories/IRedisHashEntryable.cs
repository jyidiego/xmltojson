using StackExchange.Redis;

namespace APIService.Repository
{
  public interface IRedisHashEntryable
  {
    HashEntry[] ToHashEntries();
  }
}