using System.ComponentModel;

namespace CatenaryReplacer
{
    public enum CatenaryStyle
    {
        [Description("None")] None = 0,
        [Description("Vanilla")] Vanilla = 1,
        [Description("Dutch Type A")] DutchTypeA = 2,
        [Description("Dutch Type B")] DutchTypeB = 3,
        [Description("German")] German = 4,
        [Description("PRR A")] PrrA = 5,
        [Description("PRR B")] PrrB = 6,
        [Description("Japan A")] JapanA = 7,
        [Description("Expo Line")] ExpoA = 8
    }
}