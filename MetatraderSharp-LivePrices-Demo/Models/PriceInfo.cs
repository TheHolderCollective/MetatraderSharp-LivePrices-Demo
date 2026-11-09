using LivePrices.Utilities;
using MetatraderSharp.MTsocketAPI.Responses.Common;

namespace LivePrices.Models;

public class PriceInfo
{
    public string Symbol { get; set; }
    public double Ask { get; set; }
    public double Bid { get; set; }
    public double Volume { get; set; }
    public double Spread { get; set; }

    public PriceInfo(string symbol, double ask, double bid, double volume)
    {
        Symbol = symbol;
        Ask = ask;
        Bid = bid;
        Volume = volume;
        Spread = SymbolUtilities.CalculateSpread(ask, bid, SymbolUtilities.GetSpreadFactor(symbol));
    }

    public PriceInfo(TrackPrices trackPrices)
    {
        Symbol = trackPrices.Symbol ?? "------";
        Ask = trackPrices.Ask;
        Bid = trackPrices.Bid;
        Volume = trackPrices.Volume;
        Spread = SymbolUtilities.CalculateSpread(Ask, Bid, SymbolUtilities.GetSpreadFactor(Symbol));
    }

    public override string ToString()
    {
        return $"Symbol: {Symbol} Ask: {Ask:N5} Bid: {Bid:N5} Volume: {Volume:N0} Spread: {Spread}";
    }
}
