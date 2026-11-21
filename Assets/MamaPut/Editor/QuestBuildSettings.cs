using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

namespace MamaPut.EditorTools
{
    public static class QuestBuildSettings
    {
        [MenuItem("Mama Put/Apply Quest 2 Build Settings")]
        static void Apply()
        {
            var android = NamedBuildTarget.Android;

            PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetManagedStrippingLevel(android, ManagedStrippingLevel.Medium);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            Debug.Log("Mama Put: IL2CPP, ARM64, ASTC, Vulkan, linear colour and API 29+ applied. " +
                      "Set MSAA 4x, HDR off and shadows off on the URP asset by hand.");
        }

        [MenuItem("Mama Put/Mark Selection Batching Static")]
        static void MarkStatic()
        {
            int count = 0;
            foreach (var go in Selection.gameObjects)
            {
                foreach (var t in go.GetComponentsInChildren<Transform>(true))
                {
                    var flags = GameObjectUtility.GetStaticEditorFlags(t.gameObject);
                    GameObjectUtility.SetStaticEditorFlags(t.gameObject,
                        flags | StaticEditorFlags.BatchingStatic | StaticEditorFlags.ContributeGI);
                    count++;
                }
            }
            Debug.Log($"Mama Put: marked {count} objects as batching static.");
        }
    }
}
