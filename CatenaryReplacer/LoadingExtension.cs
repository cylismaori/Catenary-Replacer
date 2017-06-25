using System.Collections.Generic;
using CatenaryReplacer.OptionsFramework;
using ICities;
using UnityEngine;

namespace CatenaryReplacer
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private const string GameObjectName = "CatenaryReplacer";

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            new GameObject(GameObjectName).AddComponent<Replacer>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            var go = GameObject.Find(GameObjectName);

            if (go != null)
            {
                Object.Destroy(go);
            }
        }
    }
}