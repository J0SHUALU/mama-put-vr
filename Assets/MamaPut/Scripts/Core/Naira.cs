using TMPro;

namespace MamaPut
{
    public static class Naira
    {
        static string symbol;

        // Falls back to "N" if the font can't draw the naira sign, which is how most people write it anyway.
        public static string Symbol
        {
            get
            {
                if (symbol == null)
                {
                    var font = TMP_Settings.defaultFontAsset;
                    symbol = font != null && font.HasCharacter('₦', true, true) ? "₦" : "N";
                }
                return symbol;
            }
        }

        public static string Format(int amount) => Symbol + amount.ToString("N0");
    }
}
