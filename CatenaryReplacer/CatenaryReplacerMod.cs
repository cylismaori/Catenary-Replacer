using ICities;
using CatenaryReplacer.OptionsFramework.Extensions;

namespace CatenaryReplacer
{
    public class CatenaryReplacerMod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Catenary Replacer";

        public string Description => "Replaces the catenaries on railroads with those of your choice";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<CatenaryReplacerConfiguration>();
        }
    }
}