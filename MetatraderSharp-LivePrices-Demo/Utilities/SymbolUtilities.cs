namespace LivePrices.Utilities;

public static class SymbolUtilities
{

    public static double CalculateSpread(double ask, double bid, double factor)
    {
        return Math.Round((ask - bid) * factor, 0);
    }

    // Only accurate for forex symbols
    public static double GetSpreadFactor(string symbol)
    {
        if (symbol.Contains("JPY"))
        {
            return 1000;
        }
        else
        {
            return 100000;
        }
    }

    // Only accurate for forex symbols
    public static string GetFormatSpecifier(string symbol)
    {
        if (symbol.Contains("JPY"))
        {
            return "N3";
        }
        else
        {
            return "N5";
        }
    }
}
