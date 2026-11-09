using LivePrices.SharedData;
using MetatraderSharp;
using MetatraderSharp.Extensions;
using MetatraderSharp.MetatraderClient;
using MetatraderSharp.MTsocketAPI.Responses.Common;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace LivePrices.Models;

public class PriceTracker
{
    private MT4Client _mtClient;
    private TrackResponse _trackResponse;
    private PriceProducer _priceProducer;
    private PriceFilter _priceFilter;
    private PriceConsumer<PriceInfo> _priceConsumer;
    private BlockingCollection<TrackPrices> _producerOutputPipe;
    private BlockingCollection<PriceInfo> _filterOuputPipe;
    private PricesOutput<PriceInfo> _pricesOutputMethod;
    private ClientWebSocket _webSocket;
    private bool _trackingStarted;
    
    public bool TrackingStarted { get { return _trackingStarted; } }
    
    public PriceTracker(PricesOutput<PriceInfo> outputMethod)
    { 
        _mtClient = new();
        _webSocket = new ClientWebSocket();
        _producerOutputPipe = new();
        _filterOuputPipe = new();
        _pricesOutputMethod = outputMethod;
    }

    public async Task<string> StartTracking(string[] symbols)
    {
        string outputMessage = string.Empty;

        _trackResponse = await _mtClient.TrackPricesAsync(TrackingCommand.Start, symbols);

        if (_trackResponse.SuccessCount() == symbols.Length && _trackResponse.ErrorID == 0)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri($"ws://127.0.0.1:{_mtClient.WebSocketPort}"), CancellationToken.None);

            if (_webSocket.State == WebSocketState.Open)
            {
                _priceProducer = new(_producerOutputPipe, _webSocket);
                _priceFilter = new(_producerOutputPipe, _filterOuputPipe);
                _priceConsumer = new(_filterOuputPipe, _pricesOutputMethod);

                _priceProducer.Start();
                _priceFilter.Start();
                _priceConsumer.Start();

                _trackingStarted = true;
                outputMessage = "Tracking succesfully started.";
            }
            else
            {
                _trackResponse = await _mtClient.TrackPricesAsync(TrackingCommand.Stop);
                _trackingStarted = false;
                outputMessage = $"Websocket state: {_webSocket.State}. Websocket unable to connect.";
            }
        }
        else
        {
            _trackingStarted = false;
            outputMessage = $"An error occured: {_trackResponse.ErrorDescription}";
        }

        return outputMessage;
    }

    public async Task<string> StopTracking()
    {
        string outputMessage = string.Empty;

        if (_mtClient.ClientStatusIsError())
        {
            outputMessage = "Client is in error state. Unable to stop tracking.";
        }
        else if (_mtClient.ClientStatusIsOK() && !_trackingStarted)
        {
            outputMessage = "Tracking hasn't started.";
        }
        else if (_mtClient.ClientStatusIsOK() && _trackingStarted)
        {
            outputMessage = await StopTrackingPrices();
        }

        return outputMessage;
    }

    public string ResetClient()
    {
        string outputMessage = string.Empty;

        if (_trackingStarted)
        {
            outputMessage = "Unable to reset client while price tracking is in progress.";
        }
        else
        {
            _mtClient = new();
            _trackingStarted = false;

            outputMessage = "Client reset.\nStatus after reset: ";
            outputMessage += _mtClient.ClientStatusIsError() ? "Error" : "Ok";
        }

        return outputMessage;
    }

    public async Task<string> StopTrackingPrices()
    {
        string outputMessage = string.Empty;

        _priceProducer.Stop();
        _priceFilter.Stop();
        _priceConsumer.Stop();

        await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        _trackResponse = await _mtClient.TrackPricesAsync(TrackingCommand.Stop);

        if (_trackResponse.ErrorID == 0)
        {
            _trackingStarted = false;
            outputMessage = "Tracking succesfully stopped.";
        }
        else
        {
            _trackingStarted = false;
            outputMessage = $"An error occured: {_trackResponse.ErrorDescription}";
        }

        return outputMessage;
    }

    public async Task<SymbolList> GetAvailbleSymbols()
    {
        return await _mtClient.GetSymbolListAsync();
    }

    public bool IsTrackingAvailable()
    {
        return (_mtClient.ClientStatusIsOK() && !_trackingStarted);
    }

    public bool IsTrackingStarted()
    {
        return (_mtClient.ClientStatusIsOK() && _trackingStarted);
    }

    public bool IsTerminalClientInErrorState()
    {
        return _mtClient.ClientStatusIsError();
    }
}