using HarmonyLib;
using Kitchen;
using System.Reflection;
using UnityEngine;

namespace KitchenICantSeeYourOrder.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        static FieldInfo f_Container = typeof(ItemCollectionView).GetField("Container", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPostfix]
        static void GetPrefab_Postfix(ViewType view_type, ref GameObject __result)
        {
            if (view_type == ViewType.ItemCollectionView &&
                __result != null &&
                __result.GetComponent<ItemCollectionExpanderView>() == null)
            {
                ItemCollectionExpanderView expanderView = __result.AddComponent<ItemCollectionExpanderView>();

                expanderView.ItemCollectionView = __result.GetComponent<ItemCollectionView>();
                expanderView.DefaultScale = Vector3.one;
            }
        }
    }
}
