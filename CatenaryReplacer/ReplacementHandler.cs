using UnityEngine;

namespace CatenaryReplacer
{
    public static class ReplacementHandler
    {
        private const string GameObjectName = "CatenaryReplacer";

        public static void Replace(int style) //arg not used
        {
            Revert();
            new GameObject(GameObjectName).AddComponent<Replacer>();
        }

        public static void Revert()
        {
            if (!LoadingExtension.Ingame)
            {
                return;
            }

            var go = GameObject.Find(GameObjectName);

            if (go != null)
            {
                Object.Destroy(go);
            }
        }
    }
}