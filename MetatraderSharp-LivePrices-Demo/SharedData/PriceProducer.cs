using MetatraderSharp.MTsocketAPI.Responses.Common;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

namespace LivePrices.SharedData;

public class PriceProducer
{
    private volatile BlockingCollection<TrackPrices> _outputQueue;
    private ClientWebSocket _webSocket;
    private bool keepProducing;

    public PriceProducer(BlockingCollection<TrackPrices> outputPipe, ClientWebSocket socket)
    {
        _outputQueue = outputPipe;
        _webSocket = socket;
        keepProducing = true;
    }

    public void Start()
    {
        Thread t = new Thread(new ThreadStart(Run));
        t.IsBackground = true;
        t.Start();
    }

    public void Stop()
    {
        keepProducing = false;
    }

    private async void Run()
    {
        try
        {
            byte[] buffer = new byte[4096];

            while (keepProducing)
            {
                // Read streamed data into the buffer
                var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType != WebSocketMessageType.Close)
                {
                    // process received and add it to the output queue
                    string jsonData = Encoding.ASCII.GetString(buffer, 0, result.Count);
                    var output = JsonConvert.DeserializeObject<TrackPrices>(jsonData);

                    if (output is not null)
                        _outputQueue.Add((TrackPrices)output);
                }
            }
        }
        catch (WebSocketException ex)
        {
            // This exception occurs even though websocket is closed using correct method.

            //string exceptionMessage = $"Exception: {ex.GetType().ToString()}\n" + $"Message: {ex.Message}";
            //MessageBox.Show(exceptionMessage, "Error", MessageBoxButtons.OK);
        }
       
    }
}
