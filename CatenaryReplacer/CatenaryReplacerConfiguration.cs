using CatenaryReplacer.OptionsFramework.Attibutes;

namespace CatenaryReplacer
{
    [Options("CatenaryReplacer.xml", "CatenaryReplacer.xml")]
    public class CatenaryReplacerConfiguration
    {
        [DropDown("Catenary Style", nameof(CatenaryStyle), null, nameof(ReplacementHandler), nameof(ReplacementHandler.Replace))]
        public int Style { get; set; }
    }
}