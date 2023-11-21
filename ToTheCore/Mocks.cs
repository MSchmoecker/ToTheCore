using BelowTheStone.Crafting;
using BelowTheStone.InventorySystem;
using HarmonyLib;
using UnityEngine;

namespace ToTheCore {
    public static class Mocks {
        public static void FixMocks(ItemStack itemStack) {
            if (itemStack != null && itemStack.ItemType && itemStack.ItemType.name.StartsWith("Mock_")) {
                string itemName = itemStack.ItemType.name.Substring(5);
                AccessTools.FieldRefAccess<Item, ItemType>(itemStack.Item, "itemType") = Patches.vanillaItems[itemName];
            }
        }

        public static void FixMocks(Ingredient ingredient) {
            if (ingredient != null && ingredient.ItemType && ingredient.ItemType.name.StartsWith("Mock_")) {
                string itemName = ingredient.ItemType.name.Substring(5);
                AccessTools.FieldRefAccess<Ingredient, ItemType>(ingredient, "itemType") = Patches.vanillaItems[itemName];
            }
        }

        public static void FixMocks(ItemType itemType) {
            if (itemType != null && itemType.EquipItemPrefab) {
                FixMocks(itemType.EquipItemPrefab);
            }
        }

        public static void FixMocks(GameObject gameObject) {
            if (gameObject && gameObject.TryGetComponent(out EquippedItem equippedItem)) {
                FixMocks(equippedItem);
            }
        }

        public static void FixMocks(EquippedItem equippedItem) {
            if (equippedItem) {
                if (equippedItem.ItemTypeData && equippedItem.ItemTypeData.name.StartsWith("Mock_")) {
                    string itemName = equippedItem.ItemTypeData.name.Substring(5);
                    AccessTools.FieldRefAccess<EquippedItem, ItemType>(equippedItem, "itemTypeData") = Patches.vanillaItems[itemName];
                }

                if (equippedItem.HandAnimations && equippedItem.HandAnimations.name.StartsWith("Mock_")) {
                    string controllerName = equippedItem.HandAnimations.name.Substring(5);
                    AccessTools.FieldRefAccess<EquippedItem, RuntimeAnimatorController>(equippedItem, "handAnimations") = Patches.vanillaAnimations[controllerName];
                }
            }
        }
    }
}
