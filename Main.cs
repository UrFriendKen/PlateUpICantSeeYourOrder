using HarmonyLib;
using KitchenMods;
using PreferenceSystem;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenICantSeeYourOrder
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "I Can't See Your Order!";
        public const string MOD_VERSION = "0.1.1";

        internal const string TRANSITION_TIME_ID = "transitionTime";

        internal const string EXPANDED_SIZE_ID = "expandedSize";
        internal const string MINIMIZED_SIZE_ID = "minimizedSize";

        internal static PreferenceSystemManager PrefManager;

        public Main()
        {
            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Transition Time (seconds)")
                .AddOption<float>(
                    TRANSITION_TIME_ID,
                    0.2f,
                    Enumerable.Range(1, 10).Select(x => x / 10f).ToArray(),
                    Enumerable.Range(1, 10).Select(x => $"{x/10f:0.0}").ToArray())
                .AddLabel("Expanded Size")
                .AddOption<float>(
                    EXPANDED_SIZE_ID,
                    2f,
                    Enumerable.Range(1, 25).Select(x => x / 10f).ToArray(),
                    Enumerable.Range(1, 25).Select(x => $"{x * 10}%").ToArray())
                .AddLabel("Minimized Size")
                .AddOption<float>(
                    MINIMIZED_SIZE_ID,
                    1f,
                    Enumerable.Range(1, 25).Select(x => x / 10f).ToArray(),
                    Enumerable.Range(1, 25).Select(x => $"{x * 10}%").ToArray())
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        public void PreInject()
        {
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
