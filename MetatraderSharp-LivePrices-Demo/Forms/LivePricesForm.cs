using LivePrices.Models;
using LivePrices.SharedData;
using LivePrices.Utilities;
using MetatraderSharp;
using MetatraderSharp.Extensions;
using MetatraderSharp.MetatraderClient;
using MetatraderSharp.MTsocketAPI.Responses.Common;
using System.Globalization;
using System.Net.WebSockets;

namespace LivePrices;

public partial class LivePrices : Form
{
    private MT4Client _mtClient;
    private TrackResponse _trackResponse;
    private PriceProducer _priceProducer;
    private PriceFilter _priceFilter;
    private PriceConsumer<PriceInfo> _priceConsumer;
    private BlockingQueue<TrackPrices> _producerOutputPipe;
    private BlockingQueue<PriceInfo> _filterOuputPipe;
    private ClientWebSocket _webSocket;
    private bool _trackingStarted;
    private string[] _symbols;

    public LivePrices()
    {
        if (!CultureInfo.CurrentCulture.Name.Equals("en-US"))
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

        InitializeComponent();
        SetLabelsToDefaults();

        _mtClient = new();
        _webSocket = new ClientWebSocket();
        _producerOutputPipe = new();
        _filterOuputPipe = new();
        _symbols = new string[2];
    }

    private async void LivePrices_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_trackingStarted)
            await StopTrackingPrices();
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        await StartTracking();
    }

    private async void button2_Click(object sender, EventArgs e)
    {
        await StopTracking();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        ResetClient();
    }

    private async Task StartTracking()
    {
        string outputMessage = string.Empty;

        if (_mtClient.ClientStatusIsError())
        {
            outputMessage = "Unable to start tracking. Please check that terminal is running and MTsocketAPI is loaded.";
        }
        else if (_mtClient.ClientStatusIsOK() && _trackingStarted)
        {
            outputMessage = "Tracking already started.";
        }
        else if (_mtClient.ClientStatusIsOK() && !_trackingStarted)
        {
            SymbolList symbols = await _mtClient.GetSymbolListAsync();

            if (symbols.ErrorID == 0)
            {
                var symbolSelectForm = new SymbolSelectForm(symbols.GetSymbolNames());
                symbolSelectForm.ShowDialog();

                if (symbolSelectForm.OkClicked)
                {
                    if (symbolSelectForm.Symbol1 is not null && symbolSelectForm.Symbol2 is not null)
                    {
                        _symbols[0] = symbolSelectForm.Symbol1;
                        _symbols[1] = symbolSelectForm.Symbol2;

                        outputMessage = await StartTrackingPrices(_symbols);
                    }
                    else
                    {
                        outputMessage = "Invalid symbol selections.Unable to start tracking.";
                    }
                }
                else if (symbolSelectForm.CancelClicked)
                {
                    outputMessage = "Start tracking cancelled.";
                }
            }
            else
            {
                outputMessage = "Unable to start tracking. Symbol loading from client failed.";
            }
        }

        MessageBox.Show(outputMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task StopTracking()
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

        MessageBox.Show(outputMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ResetClient()
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

        MessageBox.Show(outputMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task<string> StartTrackingPrices(string[] symbols)
    {
        string outputMessage = string.Empty;

        _trackResponse = await _mtClient.TrackPricesAsync(TrackingCommand.Start, symbols);

        if (_trackResponse.SuccessCount() == _symbols.Length && _trackResponse.ErrorID == 0)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri($"ws://127.0.0.1:{_mtClient.WebSocketPort}"), CancellationToken.None);

            if (_webSocket.State == WebSocketState.Open)
            {
                _priceProducer = new(_producerOutputPipe, _webSocket);
                _priceFilter = new(_producerOutputPipe, _filterOuputPipe);
                _priceConsumer = new(_filterOuputPipe, OutputToLabels);

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

    private async Task<string> StopTrackingPrices()
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

        SetLabelsToDefaults();
        return outputMessage;
    }

    private void OutputToLabels(PriceInfo output)
    {
        if (output.Symbol == _symbols[0])
        {
            string specifier = SymbolUtilities.GetFormatSpecifier(_symbols[0]);

            lbl_gb1_Symbol.Text = _symbols[0];
            lbl_gb1_Bid.Text = output.Bid.ToString(specifier);
            lbl_gb1_Ask.Text = output.Ask.ToString(specifier);
            lbl_gb1_Spread.Text = output.Spread.ToString();
            lbl_gb1_Volume.Text = output.Volume.ToString();
        }
        if (output.Symbol == _symbols[1])
        {
            string specifier = SymbolUtilities.GetFormatSpecifier(_symbols[1]);

            lbl_gb2_Symbol.Text = _symbols[1];
            lbl_gb2_Bid.Text = output.Bid.ToString(specifier);
            lbl_gb2_Ask.Text = output.Ask.ToString(specifier);
            lbl_gb2_Spread.Text = output.Spread.ToString();
            lbl_gb2_Volume.Text = output.Volume.ToString();
        }
    }

    private void SetLabelsToDefaults()
    {
        // Labels in Groupbox 1
        lbl_gb1_Symbol.Text = "------";
        lbl_gb1_Bid.Text = "0.00000";
        lbl_gb1_Ask.Text = "0.00000";
        lbl_gb1_Spread.Text = "0";
        lbl_gb1_Volume.Text = "0.00";

        // Labels in Groupbox 2
        lbl_gb2_Symbol.Text = "------";
        lbl_gb2_Bid.Text = "0.00000";
        lbl_gb2_Ask.Text = "0.00000";
        lbl_gb2_Spread.Text = "0";
        lbl_gb2_Volume.Text = "0.00";
    }
}