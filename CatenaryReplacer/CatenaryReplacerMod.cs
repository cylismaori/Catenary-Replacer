using System.Collections;
using ICities;
using System.Collections.Generic;
using CatenaryReplacer.OptionsFramework;
using CatenaryReplacer.OptionsFramework.Extensions;
using UnityEngine;

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