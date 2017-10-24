using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.AllInOnePonics
{
	public class Comp_DayNightSwitch : ThingComp
	{
		public Comp_DayNightSwitch()
		{
		}

		private CompFlickable CompFlickable
		{
			get
			{
				return this.parent.GetComp<CompFlickable>();
			}
		}

		private CompPowerTrader CompPowerTrader
		{
			get
			{
				return this.parent.GetComp<CompPowerTrader>();
			}
		}

		public CompGlower CompGlower
		{
			get
			{
				return this.parent.GetComp<CompGlower>();
			}
		}

		private CompGlower compGlower_backup;

		private CompProperties_DayNightSwitch Props
		{
			get
			{
				return (CompProperties_DayNightSwitch)this.props;
			}
		}

		private bool ShouldBeSwitchedOn
		{
			get
			{
				return (Props.diurnal == Utils.DayTime(this.parent.Map));
				//return true;
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			if (respawningAfterLoad)
				return;

			Core.RecordCompToTick(this);
		}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);

			Core.UnrecordCompToTick(this);
		}

		public override void CompTickRare()
		{
#if DEBUG
			Log.Message("CompTickRare() ShouldBeSwitchedOn=" + ShouldBeSwitchedOn + " CompFlickable.SwitchIsOn=" + this.CompFlickable.SwitchIsOn);
#endif
			if (ShouldBeSwitchedOn != CompFlickable.SwitchIsOn)
			{
				CompFlickable.SwitchIsOn = ShouldBeSwitchedOn;
				CompGlower.UpdateLit(this.parent.Map);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
		}
	}
}
