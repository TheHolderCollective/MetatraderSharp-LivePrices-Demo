using LivePrices.Models;
using MetatraderSharp.MTsocketAPI.Responses.Common;
using System.Collections.Concurrent;

namespace LivePrices.SharedData;

public class PriceFilter
{
    private volatile BlockingCollection<TrackPrices> _inputQueue;
    private volatile BlockingCollection<PriceInfo> _outputQueue;
    private bool _keepFiltering;

    public PriceFilter(BlockingCollection<TrackPrices> inputPipe, BlockingCollection<PriceInfo> outputPipe)
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
            TrackPrices price = _inputQueue.Take();
            _outputQueue.Add(new PriceInfo(price));
        }
    }
}
