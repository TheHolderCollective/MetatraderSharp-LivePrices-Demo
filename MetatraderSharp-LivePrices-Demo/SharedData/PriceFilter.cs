using LivePrices.Models;
using MetatraderSharp.MTsocketAPI.Responses.Common;

namespace LivePrices.SharedData;

public class PriceFilter
{
    private volatile BlockingQueue<TrackPrices> _inputQueue;
    private volatile BlockingQueue<PriceInfo> _outputQueue;
    private bool _keepFiltering;

    public PriceFilter(BlockingQueue<TrackPrices> inputPipe, BlockingQueue<PriceInfo> outputPipe)
    {
        _inputQueue = inputPipe;
        _outputQueue = outputPipe;
        _keepFiltering = true;
    }

    public void Start()
    {
        Thread t = new Thread(new ThreadStart(Run));
        t.IsBackground = true;
        t.Start();
    }

    public void Stop()
    {
        _keepFiltering = false;
    }

    private void Run()
    {
        while (_keepFiltering)
        {
            TrackPrices price = _inputQueue.Dequeue();
            _outputQueue.Enqueue(new PriceInfo(price));
        }
    }
}
