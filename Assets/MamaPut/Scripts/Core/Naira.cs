using TMPro;

namespace MamaPut
{
    public static class Naira
    {
        static string symbol;

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
