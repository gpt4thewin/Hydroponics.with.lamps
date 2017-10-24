using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace WM.AllInOnePonics
{
	public static class Utils
	{
		public static readonly ThingDef Def_MinimumSunLamp = ThingDef.Named("MinimumSunLamp");

		internal static bool DayTime(Map map)
		{
			return (GenLocalDate.DayPercent(map) >= 0.25f && GenLocalDate.DayPercent(map) <= 0.8f);
		}

		//public static IEnumerable<ThingDef> AvailablePonicsCrops
		//{
		//	get
		//	{
		//		return DefDatabase<ThingDef>.AllDefs.Where(arg => arg.plant != null && arg.plant.sowTags.Contains("Hydroponic") );
		//	}
		//}

		public static ModContentPack GetMod(this Def def)
		{
			foreach (var mod in LoadedModManager.RunningMods.Reverse())
			{
				var AllThingDefs = mod.AllDefs.Where((Def arg) => arg is ThingDef);

				bool result = AllThingDefs.Any(delegate (Def arg)
				{
					if (!(arg is ThingDef))
						return false;

					if (arg == def)
						return true;

					if (((ThingDef)arg).race != null)
					{
						if (((ThingDef)arg).race.meatDef == def)
							return true;
						if (((ThingDef)arg).race.corpseDef == def)
							return true;
					}

					return false;
				});

				if (result)
					return mod;
			}

			Log.Warning("Could not figure out the mod of Def " + def);
			return null;
		}
	}
}
