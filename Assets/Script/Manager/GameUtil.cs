using System;
using System.Threading;

public static class GameUtil
{
    private static long _lastId = 0;

    public static long GenerateUniqueId()
    {
        long newId = DateTime.UtcNow.Ticks;
        while (true)
        {
            long lastId = Volatile.Read(ref _lastId);
            long idToAssign = (newId <= lastId) ? lastId + 1 : newId;
            if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
            {
                return idToAssign;
            }
        }
    }
}
