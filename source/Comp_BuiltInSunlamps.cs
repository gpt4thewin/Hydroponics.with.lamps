using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.AllInOnePonics
{
	public class Comp_BuiltInSunlamps : ThingComp
	{
		public Comp_BuiltInSunlamps()
		{
		}

		//static readonly ThingDef def = ThingDef.Named("HydroponicsBasin_AllInOne");

		IEnumerable<IntVec3> Cells
		{
			get
			{
				return parent.OccupiedRect().Cells;
			}
		}
		int CellsCount
		{
			get
			{
				return parent.def.size.x * parent.def.size.z;
			}
		}

		private CompProperties_BuiltInSunlamps Props
		{
			get
			{
				return (CompProperties_BuiltInSunlamps)this.props;
			}
		}

		List<ThingWithComps> Glowers;

		bool glowOnInt = false;

		private CompPowerTrader CompPowerTrader
		{
			get
			{
				return parent.GetComp<CompPowerTrader>();
			}
		}
		private CompFlickable CompFlickable
		{
			get
			{
				return parent.GetComp<CompFlickable>();
			}
		}

		private bool ShouldBrightNow
		{
			get
			{
				return Utils.DayTime(this.parent.Map);
			}
		}
		private bool CanBrightNow
		{
			get
			{
				return CompPowerTrader.PowerOn && CompFlickable.SwitchIsOn && ShouldBrightNow;
			}
		}

		public override void CompTickRare()
		{
#if DEBUG
			Log.Message("CompTickRare() BuildintLamps. ShouldBrightNow=" + ShouldBrightNow + " CanBrightNow=" + CanBrightNow);
#endif

			if (ShouldBrightNow)
				CompPowerTrader.PowerOutput = -CompPowerTrader.Props.basePowerConsumption + -Props.powerConsumptionPerTile * CellsCount;
			else
				//CompPowerTrader.PowerOutput -= LampPowerConsumptionPerTile * CellsCount;
				CompPowerTrader.PowerOutput = -CompPowerTrader.Props.basePowerConsumption;

			if (this.glowOnInt != CanBrightNow)
			{
				UpdateLit();

				this.glowOnInt = CanBrightNow;
			}
		}

		public void UpdateLit()
		{
			if (this.glowOnInt == CanBrightNow)
			{
				return;
			}
			foreach (var item in Glowers)
			{
				item.TryGetComp<CompFlickable>().SwitchIsOn = CanBrightNow;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();

			Scribe_Values.Look<bool>(ref glowOnInt, "glowOnInt");
			Scribe_Collections.Look<ThingWithComps>(ref Glowers, "glowers", LookMode.Reference);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			Core.RecordCompToTick(this);

			if (Glowers != null)
				return;

			Glowers = new List<ThingWithComps>(CellsCount);

			foreach (var item in this.parent.OccupiedRect().Cells)
			{
				var t = GenSpawn.Spawn(Utils.Def_MinimumSunLamp, item, this.parent.Map);
				t.Rotation = this.parent.Rotation;
				t.TryGetComp<CompFlickable>().SwitchIsOn = false;
				//t.SetFaction(this.parent.Faction);

				Glowers.Add((ThingWithComps)t);
			}
		}

		//public override void PostDestroy(DestroyMode mode, Map previousMap)
		//{
		//	base.PostDestroy(mode, previousMap);
		//	ClearGlowers();
		//	Core.UnrecordCompToTick(this);
		//}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);
			ClearGlowers();
			Core.UnrecordCompToTick(this);
		}

		//internal void PostCompMake(ThingWithComps argParent) // ducktape argument
		//{
		//	Glowers = new List<ThingWithComps>(CellsCount);

		//	foreach (var item in argParent.OccupiedRect().Cells)
		//	{
		//		var t = GenSpawn.Spawn(Utils.Def_MinimumSunLamp, item, argParent.Map);
		//		t.Rotation = argParent.Rotation;
		//		t.TryGetComp<CompFlickable>().SwitchIsOn = false;

		//		Glowers.Add((ThingWithComps)t);
		//	}
		//}

		//public override void PostDestroy(DestroyMode mode, Map previousMap)
		//{
		//	base.PostDestroy(mode, previousMap);
		//	ClearGlowers();
		//}

		void ClearGlowers()
		{
			foreach (var item in Glowers)
			{
				if (!item.DestroyedOrNull())
					item.Destroy();
			}
			Glowers = null;
		}
	}
}
