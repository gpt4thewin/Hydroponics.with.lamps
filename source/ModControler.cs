using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.AllInOnePonics
{
	internal static class Log
	{
		internal static void Message(string text)
		{
			Core.Log.Message(text);
		}
		internal static void Warning(string text)
		{
			Core.Log.Warning(text);
		}
		internal static void Error(string text)
		{
			Core.Log.Error(text);
		}
	}
	public class Core : HugsLib.ModBase
	{
		// ducktapestan
		private static Core running;

		private static WorldDataStore worldData;

		private static List<ThingComp> compsToTick; // can't tickrare from comp

		static Dictionary<ThingDef, ThingDef> allPonicsDefs = new Dictionary<ThingDef, ThingDef>();
		static Dictionary<ThingDef, ThingDef> allPonicsDefs_backup = new Dictionary<ThingDef, ThingDef>();

		internal static Dictionary<ThingDef, ThingDef> AllPonicsDefs
		{
			get
			{
				return allPonicsDefs;
			}

			set
			{
				allPonicsDefs = value;
			}
		}

		static String modname = "WM_PonicsWithLamps";

		public override String ModIdentifier
		{
			get
			{
				return modname;
			}
		}

		public Core()
		{
			running = this;
		}


		public override void DefsLoaded()
		{
			Config.InjectDefaultPlantSetter = Settings.GetHandle<bool>("InjectDefaultPlantSetter", "InjectDefaultPlantSetter".Translate(), "InjectDefaultPlantSetter_desc".Translate(), true);
			Config.InjectBuiltinSunlamps = Settings.GetHandle<bool>("InjectBuiltinSunlamps", "InjectBuiltinSunlamps".Translate(), "InjectBuiltinSunlamps_desc".Translate(), false);

			DoCompsInjection();
		}


		public override void WorldLoaded()
		{

			worldData = UtilityWorldObjectManager.GetUtilityWorldObject<WorldDataStore>();

			foreach (var item in worldData.defaultPonicsPlant.ToList())
			{
				item.Key.building.defaultPlantToGrow = item.Value;
			}
		}

		public override void SceneLoaded(UnityEngine.SceneManagement.Scene scene)
		{
			base.SceneLoaded(scene);

#if DEBUG
			Log.Message("SceneLoaded() " + scene.name);
#endif

			if (scene.name == "Play")
			{
				compsToTick = new List<ThingComp>();
			}

		}


		static void DoCompsInjection()
		{
			var PonicsSuperClass = typeof(Building_PlantGrower);
			var list = DefDatabase<ThingDef>.AllDefs.Where(arg => arg.thingClass != null && (arg.thingClass == PonicsSuperClass || arg.thingClass.IsSubclassOf(typeof(Building_PlantGrower))));

			foreach (var item in list)
			{
#if DEBUG
				Log.Message("Injected comp to " + item.defName);
#endif
				if (Config.InjectDefaultPlantSetter && !item.comps.Any(arg => arg is CompProperties_DefaultPlantSetter))
					item.comps.Add(new CompProperties_DefaultPlantSetter());

				//if (Config.InjectBuiltinSunlamps && !item.comps.Any(arg => arg is CompProperties_BuiltInSunlamps) &&
				//	item.comps.Any(arg => arg is CompProperties_Power))
				//{
				//	item.comps.Add(new CompProperties_BuiltInSunlamps());
				//}

				AllPonicsDefs.Add(item, item.building.defaultPlantToGrow);
			}
		}


		public override void Tick(int currentTick)
		{
			if (currentTick % 250 == 0)
				TickRare();
		}

		void TickRare()
		{
#if DEBUG
			Log.Message("TickRare comps: " + compsToTick.Count);
#endif
			foreach (var item in compsToTick)
			{
				item.CompTickRare();
			}
		}

		internal static void RecordCompToTick(ThingComp comp)
		{
			compsToTick.Add(comp);
		}
		internal static void UnrecordCompToTick(ThingComp comp)
		{
			compsToTick.Remove(comp);
		}

		internal static class Log
		{
			internal static void Message(string text)
			{
				running.Logger.Message(text);
			}
			internal static void Warning(string text)
			{
				running.Logger.Warning(text);
			}
			internal static void Error(string text)
			{
				running.Logger.Error(text);
			}
		}

		internal static ThingDef GetDefaultPonicsPlant(ThingDef forGrower)
		{
			ThingDef plantDef;

			if (!worldData.defaultPonicsPlant.TryGetValue(forGrower, out plantDef))
			{
				plantDef = forGrower.building.defaultPlantToGrow;
				worldData.defaultPonicsPlant.Add(forGrower, plantDef);
			}

			return plantDef;
			//return worldData.defaultPonicsPlant[forGrower];
		}

		internal static void SetDefaultPonicsPlant(ThingDef forGrower, ThingDef plantDef)
		{
			forGrower.building.defaultPlantToGrow = plantDef;
			worldData.defaultPonicsPlant[forGrower] = plantDef;

			Messages.Message(string.Format("MessageDefaultCropSet".Translate(), plantDef.label, forGrower.label), MessageSound.Benefit);
		}

		internal class WorldDataStore : UtilityWorldObject
		{
			internal Dictionary<ThingDef, ThingDef> defaultPonicsPlant = new Dictionary<ThingDef, ThingDef>(AllPonicsDefs);

			public override void PostAdd()
			{
				base.PostAdd();
			}

			public override void ExposeData()
			{
				base.ExposeData();
				Scribe_Collections.Look<ThingDef, ThingDef>(ref defaultPonicsPlant, "defaultPonicsPlant", LookMode.Def, LookMode.Def);
				//Scribe_Defs.LookDef<ThingDef>(ref defaultPonicsPlant, "defaultPonicsPlant");

				if (Scribe.mode == LoadSaveMode.PostLoadInit)
				{
					//if (defaultPonicsPlant == null)
					//	defaultPonicsPlant = ThingDef.Named("PlantRice");
				}
			}
		}
	}
}
