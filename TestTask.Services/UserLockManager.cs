using System.Collections.Concurrent;

namespace TestTask.Services;

public class UserLockManager
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _userLocks = new();

    public async Task ExecuteWithLockAsync(int userId, Func<Task> action)
    {
        var userLock = _userLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));

        await userLock.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            userLock.Release();
            
            if (userLock.CurrentCount == 1)
                _userLocks.TryRemove(userId, out _);
        }
    }
}