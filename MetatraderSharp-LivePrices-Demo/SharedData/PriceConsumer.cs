namespace LivePrices.SharedData;

public delegate void PricesOutput<T>(T obj);

public class PriceConsumer<T>
{
    private volatile BlockingQueue<T> _inputQueue;
    private bool _keepConsuming;
    private PricesOutput<T> _outputMethod;

    public PriceConsumer(BlockingQueue<T> inputPipe, PricesOutput<T> pricesOutputMethod)
    {
        _inputQueue = inputPipe;
        _keepConsuming = true;
        _outputMethod = pricesOutputMethod;
    }

    public void Start()
    {
        Thread t = new Thread(new ThreadStart(Run));
        t.IsBackground = true;
        t.Start();
    }

    public void Stop()
    {
        _keepConsuming = false;
    }

    private void Run()
    {
        while (_keepConsuming)
        {
            T price = _inputQueue.Dequeue();
            _outputMethod(price);
        }
    }
}