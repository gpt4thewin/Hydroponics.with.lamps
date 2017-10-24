using System;
using Verse;

namespace WM.AllInOnePonics
{
	public class CompProperties_BuiltInSunlamps : CompProperties
	{
		public CompProperties_BuiltInSunlamps()
		{
			compClass = typeof(Comp_BuiltInSunlamps);
		}

		public float powerConsumptionPerTile = 13f;
	}
}
