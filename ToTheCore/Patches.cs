﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BelowTheStone.Crafting;
using BelowTheStone.NewDatabase;
using HarmonyLib;
using UnityEngine;

namespace ToTheCore {
    [HarmonyPatch]
    public static class Patches {
        public static Dictionary<string, ItemType> vanillaItems = new Dictionary<string, ItemType>();
        public static Dictionary<string, RuntimeAnimatorController> vanillaAnimations = new Dictionary<string, RuntimeAnimatorController>();

        [HarmonyPatch(typeof(SODatabase), nameof(SODatabase.Init), new Type[0]), HarmonyPrefix]
        public static void SODatabaseInit(SODatabase __instance) {
            foreach (DatabaseElement databaseElement in __instance.MasterList) {
                if (databaseElement is ItemType itemType) {
                    vanillaItems[itemType.NameID] = itemType;

                    if (itemType && itemType.EquipItemPrefab && itemType.EquipItemPrefab.TryGetComponent(out EquippedItem equippedItem) &&
                        equippedItem.HandAnimations) {
                        vanillaAnimations[equippedItem.HandAnimations.name] = equippedItem.HandAnimations;
                    }
                }
            }

            AssetBundle assetBundle = LoadAssetBundleFromResources("tothecore");
            ItemType toTheCoreDevice = assetBundle.LoadAsset<ItemType>("MS_ToTheCoreRadar");
            Mocks.FixMocks(toTheCoreDevice);

            __instance.MasterList.Add(toTheCoreDevice);
            __instance.MasterList.Add(assetBundle.LoadAsset<CraftingRecipe>("MS_ToTheCoreRadar_Recipe"));
        }

        [HarmonyPatch(typeof(CraftingRecipe), "OnEnable"), HarmonyPrefix]
        public static void CraftingRecipeOnEnable(CraftingRecipe __instance) {
            Mocks.FixMocks(__instance.OutputItem);

            foreach (Ingredient ingredient in __instance.Ingredients) {
                Mocks.FixMocks(ingredient);
            }
        }

        public static AssetBundle LoadAssetBundleFromResources(string bundleName) {
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();
            string resourceName = resourceAssembly.GetManifestResourceNames().Single(str => str.EndsWith(bundleName));

            using (Stream stream = resourceAssembly.GetManifestResourceStream(resourceName)) {
                return AssetBundle.LoadFromStream(stream);
            }
        }
    }
}
