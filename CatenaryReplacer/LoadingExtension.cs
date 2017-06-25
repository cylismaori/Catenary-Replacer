using ICities;

namespace CatenaryReplacer
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static bool Ingame;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Ingame = true;
            ReplacementHandler.Replace();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Ingame = false;
            ReplacementHandler.Revert();
        }
    }
}