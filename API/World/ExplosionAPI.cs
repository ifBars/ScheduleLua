﻿using System.Collections;
using FishNet;
using MoonSharp.Interpreter;
using ScheduleLua.API.Core;
using ScheduleOne.Combat;
using ScheduleOne.DevUtilities;
using UnityEngine;

namespace ScheduleLua.API.World
{
    public static class ExplosionAPI
    {
        /// <summary>
        /// Registers explosion-related API functions with the Lua engine
        /// </summary>
        public static void RegisterAPI(Script luaEngine)
        {
            if (luaEngine == null)
                throw new ArgumentNullException(nameof(luaEngine));

            LuaUtility.Log("✅ Registering Explosion API");

            luaEngine.Globals["TriggerExplosion"] = (Action<DynValue, float>)((pos, seconds) =>
            {
                if (pos.Type != DataType.Table)
                {
                    LuaUtility.LogError("TriggerExplosion expects a table with x, y, z");
                    return;
                }

                float x = (float)(pos.Table.Get("x").CastToNumber());
                float y = (float)(pos.Table.Get("y").CastToNumber());
                float z = (float)(pos.Table.Get("z").CastToNumber());

                Vector3 position = new Vector3(x, y, z);

                MelonLoader.MelonCoroutines.Start(DelayedExplosion(position, seconds));
            });

            LuaUtility.Log("✅ Explosion API registered.");
        }

        private static IEnumerator DelayedExplosion(Vector3 position, float seconds)
        {
            LuaUtility.Log($"⏳ Delayed explosion in {seconds} seconds at {position}");
            yield return new WaitForSeconds(seconds);

            if (InstanceFinder.IsServer)
            {
                LuaUtility.Log($"💥 Explosion triggered at {position}");
                NetworkSingleton<CombatManager>.Instance.CreateExplosion(position, ExplosionData.DefaultSmall);
            }
            else
            {
                LuaUtility.LogWarning("⚠️ Not on server — cannot trigger explosion");
            }
        }
    }
}
