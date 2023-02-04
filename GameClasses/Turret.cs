//using BepInEx.Logging;
//using BepInEx;
//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace AutomaticFuel.GameClasses
//{

//    internal class TurretFuelMain
//    {

//        [HarmonyPatch(typeof(Turret), "HasAmmo")]
//        private static class Turret_Reload_Patch
//        {
//            private static void Postfix(Turret __instance, ZNetView ___m_nview)
//            {
               
//                    //if (!___m_nview.isActiveAndEnabled || Player.m_localPlayer == null || Player.m_localPlayer.IsTeleporting())
//                    //    return;


                 
//                        Refuel(___m_nview);
//                return true;
                
//            }
//        }
//        public static async void Refuel(ZNetView znview)
//        {
//            await Task.Delay(33);
//            znview.InvokeRPC("AddAmmo", "$ammo_turretbolt");

//        }

//    }

    

//}



