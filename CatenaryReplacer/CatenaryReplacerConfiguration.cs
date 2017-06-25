using CatenaryReplacer.OptionsFramework.Attibutes;

namespace CatenaryReplacer
{
    [Options("CatenaryReplacer.xml", "CatenaryReplacer.xml")]
    public class CatenaryReplacerConfiguration
    {
        [DropDown("Catenary Style", nameof(CatenaryStyle))]
        public int Style { get; set; }
    }
}