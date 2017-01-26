
using System.Threading;

public static class EventIdentifier
{
    private static int _eventIndex = 0;

    public static void Reset()
    {
        _eventIndex = 0;
    }

    public static int NextId()
    {
        return Interlocked.Increment(ref _eventIndex);
    }
}