using System.ComponentModel;

namespace CatenaryReplacer
{
    public enum CatenaryStyle
    {
        [Description("None")] None = 0,
        [Description("Dutch Type A")] DutchTypeA = 1,
        [Description("Dutch Type B")] DutchTypeB = 2,
        [Description("German")] German = 3,
        [Description("PRR A")] PrrA = 4,
        [Description("PRR B")] PrrB = 5,
        [Description("Japan A")] JapanA = 6
    }
}