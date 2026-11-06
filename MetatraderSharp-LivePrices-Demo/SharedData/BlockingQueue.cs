namespace LivePrices.SharedData;

public class BlockingQueue<T>
{
    object syncLock = new object();

    Queue<T> queue = new Queue<T>();

    public int Count { get { return queue.Count; } }

    public void Enqueue(T t)
    {
        lock (syncLock)
        {
            queue.Enqueue(t);
            Monitor.Pulse(syncLock);
        }
    }

    public T Dequeue()
    {
        lock (syncLock)
        {
            while (queue.Count == 0)
            {
                Monitor.Wait(syncLock);
            }
            return queue.Dequeue();
        }
    }
}
