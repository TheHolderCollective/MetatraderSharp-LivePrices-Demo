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
        Spread = CalculateSpread(ask, bid, GetSpreadFactor(symbol));
    }

    public PriceInfo(TrackPrices trackPrices)
    {
        Symbol = trackPrices.Symbol ?? "------";
        Ask = trackPrices.Ask;
        Bid = trackPrices.Bid;
        Volume = trackPrices.Volume;
        Spread = CalculateSpread(Ask, Bid, GetSpreadFactor(Symbol));
    }

    public override string ToString()
    {
        return $"Symbol: {Symbol} Ask: {Ask:N5} Bid: {Bid:N5} Volume: {Volume:N0} Spread: {Spread}";
    }

    // not exhaustive/accurate for all symbols 
    private static double GetSpreadFactor(string symbol)
    {
        if (symbol.Contains("JPY"))
        {
            return 1000;
        }
        else if (symbol.Contains("BTCUSD"))
        {
            return 100;
        }
        else if (symbol.Contains("BTC-JPY"))
        {
            return 1 / 1000;
        }
        else
        {
            return 100000;
        }
    }

    private static double CalculateSpread(double ask, double bid, double factor)
    {
        return Math.Round((ask - bid) * factor, 0);
    }
}
