using LivePrices.Models;
using LivePrices.Utilities;
using MetatraderSharp.Extensions;
using System.Globalization;

namespace LivePrices;

public partial class LivePrices : Form
{
    private PriceTracker _priceTracker;
    private string _outputMessage;
    private string[] _symbols;

    public LivePrices()
    {
        InitializeComponent();
        SetLabelsToDefaults();
        SetCultureToUS();

        _priceTracker = new(OutputToLabels);
        _symbols = new string[2];
        _outputMessage = string.Empty;
    }

    private async void LivePrices_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_priceTracker.TrackingStarted)
            await _priceTracker.StopTrackingPrices();
    }

    private async void btn_StartTracking_Click(object sender, EventArgs e)
    {
        if (_priceTracker.IsTrackingAvailable())
        {
            var availableSymbols = await _priceTracker.GetAvailbleSymbols();

            var symbolSelectForm = new SymbolSelectForm(availableSymbols.GetSymbolNames());
            symbolSelectForm.StartPosition = FormStartPosition.CenterParent;
            symbolSelectForm.ShowDialog();

            if (symbolSelectForm.OkClicked && symbolSelectForm.SymbolSelectionsAreOk)
            {
                _symbols[0] = symbolSelectForm.Symbol1;
                _symbols[1] = symbolSelectForm.Symbol2;

                _outputMessage = await _priceTracker.StartTracking(_symbols);
            }
            else if (symbolSelectForm.OkClicked && !symbolSelectForm.SymbolSelectionsAreOk)
            {
                _outputMessage = "Invalid symbol selections.Unable to start tracking.";
            }
            else if (symbolSelectForm.CancelClicked)
            {
                _outputMessage = "Start tracking cancelled.";
            }
        }
        else if (_priceTracker.IsTrackingStarted())
        {
            _outputMessage = "Tracking already started.";
        }
        else if (_priceTracker.IsTerminalClientInErrorState())
        {
            _outputMessage = "Unable to start tracking. Please check that terminal is running and MTsocketAPI is loaded.";
        }

        MessageBox.Show(_outputMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void btn_StopTracking_Click(object sender, EventArgs e)
    {
        _outputMessage = await _priceTracker.StopTracking();
        MessageBox.Show(_outputMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        SetLabelsToDefaults();
    }

    private void btn_ResetClient_Click(object sender, EventArgs e)
    {
        _outputMessage = _priceTracker.ResetClient();
        MessageBox.Show(_outputMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    private void SetCultureToUS()
    {
        if (!CultureInfo.CurrentCulture.Name.Equals("en-US"))
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
    }
}