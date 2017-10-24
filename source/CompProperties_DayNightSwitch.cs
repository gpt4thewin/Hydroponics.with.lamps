using System;
using Verse;

namespace WM.AllInOnePonics
{
	public class CompProperties_DayNightSwitch : CompProperties
	{
		public bool diurnal = true;

		public CompProperties_DayNightSwitch()
		{
			this.compClass = typeof(Comp_DayNightSwitch);
		}
	}
}
