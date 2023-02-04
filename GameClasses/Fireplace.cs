﻿using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomaticFuel.GameClasses
{
    internal class Fireplace_Patches
    {
        [HarmonyPatch(typeof(Fireplace), "UpdateFireplace")]
        private static class Fireplace_UpdateFireplace_Patch
        {
            private static void Postfix(Fireplace __instance, ZNetView ___m_nview)
            {
                if (!Player.m_localPlayer || !AutomaticFuelPlugin.isOn.Value || !___m_nview.IsOwner() ||
                    (__instance.name.Contains("groundtorch") && !AutomaticFuelPlugin.refuelStandingTorches.Value) ||
                    (__instance.name.Contains("walltorch") && !AutomaticFuelPlugin.refuelWallTorches.Value) ||
                    (__instance.name.Contains("fire_pit") && !AutomaticFuelPlugin.refuelFirePits.Value) ||
                    (__instance.name.Contains("brazier") && !AutomaticFuelPlugin.refuelBraziers.Value) ||
                    (__instance.name.Contains("hearth") && !AutomaticFuelPlugin.refuelHearth.Value) ||
                    (__instance.name.Contains("bathtub") && !AutomaticFuelPlugin.refuelHotTub.Value))

                    return;

                if (Time.time - AutomaticFuelPlugin.lastFuel < 0.1)
                {
                    AutomaticFuelPlugin.fuelCount++;
                    RefuelTorch(__instance, ___m_nview, AutomaticFuelPlugin.fuelCount * 33);
                }
                else
                {
                    AutomaticFuelPlugin.fuelCount = 0;
                    AutomaticFuelPlugin.lastFuel = Time.time;
                    RefuelTorch(__instance, ___m_nview, 0);
                }
            }
        }

        public static async void RefuelTorch(Fireplace fireplace, ZNetView znview, int delay)
        {
            try
            {
                await Task.Delay(delay);

                if (!fireplace || !znview || !znview.IsValid() || !AutomaticFuelPlugin.modEnabled.Value)
                    return;

                int maxFuel = (int)(fireplace.m_maxFuel - Mathf.Ceil(znview.GetZDO().GetFloat("fuel", 0f)));

                List<Container> nearbyContainers = TastyUtils.GetNearbyContainers
                    (fireplace.transform.position, AutomaticFuelPlugin.fireplaceRange.Value);

                Vector3 position = fireplace.transform.position + Vector3.up;
                foreach (Collider collider in Physics.OverlapSphere(position, AutomaticFuelPlugin.dropRange.Value
                    , LayerMask.GetMask(new string[] { "item" })))
                {
                    if (collider?.attachedRigidbody)
                    {
                        ItemDrop item = collider.attachedRigidbody.GetComponent<ItemDrop>();
                        //Dbgl($"nearby item name: {item.m_itemData.m_dropPrefab.name}");

                        if (item?.GetComponent<ZNetView>()?.IsValid() != true)
                            continue;

                        string name = TastyUtils.GetPrefabName(item.gameObject.name);

                        if (item.m_itemData.m_shared.m_name == fireplace.m_fuelItem.m_itemData.m_shared.m_name && maxFuel > 0)
                        {
                            if (AutomaticFuelPlugin.fuelDisallowTypes.Value.Split(',').Contains(name))
                            {
                                //Dbgl($"ground has {item.m_itemData.m_dropPrefab.name} but it's forbidden by config");
                                continue;
                            }

                            AutomaticFuelPlugin.Dbgl($"auto adding fuel {name} from ground");

                            int amount = Mathf.Min(item.m_itemData.m_stack, maxFuel);
                            maxFuel -= amount;

                            for (int i = 0; i < amount; i++)
                            {
                                if (item.m_itemData.m_stack <= 1)
                                {
                                    if (znview.GetZDO() == null)
                                        AutomaticFuelPlugin.Destroy(item.gameObject);
                                    else
                                        ZNetScene.instance.Destroy(item.gameObject);
                                    znview.InvokeRPC("AddFuel", new object[] { });
                                    if (AutomaticFuelPlugin.distributedFilling.Value)
                                        return;
                                    break;
                                }

                                item.m_itemData.m_stack--;
                                znview.InvokeRPC("AddFuel", new object[] { });
                                Traverse.Create(item).Method("Save").GetValue();
                                if (AutomaticFuelPlugin.distributedFilling.Value)
                                    return;
                            }
                        }
                    }
                } 
                foreach (Container c in nearbyContainers)
                {
                    if (fireplace.m_fuelItem && maxFuel > 0)
                    {
                        List<ItemDrop.ItemData> itemList = new List<ItemDrop.ItemData>();
                        c.GetInventory().GetAllItems(fireplace.m_fuelItem.m_itemData.m_shared.m_name, itemList);

                        foreach (var fuelItem in itemList)
                        {
                            if (fuelItem != null && (!AutomaticFuelPlugin.leaveLastItem.Value || fuelItem.m_stack > 1))
                            {
                                if (AutomaticFuelPlugin.fuelDisallowTypes.Value.Split(',').Contains(fuelItem.m_dropPrefab.name))
                                {
                                    //Dbgl($"container at {c.transform.position} has {item.m_stack} {item.m_dropPrefab.name} but it's forbidden by config");
                                    continue;
                                }
                                maxFuel--;

                                AutomaticFuelPlugin.Dbgl($"container at {c.transform.position} has {fuelItem.m_stack} {fuelItem.m_dropPrefab.name}, taking one");

                                znview.InvokeRPC("AddFuel", new object[] { });

                                c.GetInventory().RemoveItem(fireplace.m_fuelItem.m_itemData.m_shared.m_name, 1);
                                typeof(Container).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(c, new object[] { });
                                typeof(Inventory).GetMethod("Changed", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(c.GetInventory(), new object[] { });
                                if (AutomaticFuelPlugin.distributedFilling.Value)
                                    return;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        
    }
}