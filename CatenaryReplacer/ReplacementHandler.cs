using UnityEngine;

namespace CatenaryReplacer
{
    public static class ReplacementHandler
    {
        private const string GameObjectName = "CatenaryReplacer";

        public static void ReplaceInt(int style) //arg not used
        {
            Replace();
        }

        public static void ReplaceBool(bool style) //arg not used
        {
            Replace();
        }

        public static void Replace()
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
                Object.DestroyImmediate(go);
            }
        }
    }
}