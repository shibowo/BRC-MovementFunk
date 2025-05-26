using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MovementPlus
{
    public static class MPAnimation
    {
        private static bool isBoEInstalled = false;
        private static object boEPluginInstance = null;
        private static FieldInfo customAnimsField = null;
        private static Dictionary<string, int> gameAnimationByCustomAnimation = new Dictionary<string, int>();
        private static bool isCached = false;

        public static void Init()
        {
            if (Chainloader.PluginInfos.TryGetValue("com.Dragsun.BunchOfEmotes", out var pluginInfo))
            {
                isBoEInstalled = true;
                boEPluginInstance = pluginInfo.Instance;
                var boEPluginType = GetTypeByName("BunchOfEmotes.BunchOfEmotesPlugin");
                customAnimsField = boEPluginType?.GetField("myCustomAnims2");
            }
        }

        private static void CacheAnimationsIfNecessary()
        {
            if (isCached || !isBoEInstalled) return;

            var boeDictionary = customAnimsField?.GetValue(boEPluginInstance) as Dictionary<int, string>;
            if (boeDictionary == null || boeDictionary.Count == 0) return;

            foreach (var kvp in boeDictionary)
            {
                gameAnimationByCustomAnimation[kvp.Value] = kvp.Key;
            }

            isCached = true;
        }

        public static int GetAnimationByName(string name)
        {
            if (isBoEInstalled)
            {
                CacheAnimationsIfNecessary();
                if (gameAnimationByCustomAnimation.TryGetValue(name, out int animation))
                {
                    return animation;
                }
            }
            return Animator.StringToHash(name);
        }

        private static Type GetTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }
            return null;
        }
    }
}