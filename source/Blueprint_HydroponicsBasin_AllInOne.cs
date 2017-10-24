using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WM.AllInOnePonics
{
	public class Blueprint_HydroponicsBasin_AllInOne : Blueprint_Build
	{
		public Blueprint_HydroponicsBasin_AllInOne()
		{
		}

		internal ThingDef plantDefToGrow;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			var list = new List<Gizmo>();

			list.AddRange(base.GetGizmos());

			var gizmo = new Command_SetPlantToGrow_Blueprint(this);

			list.Add(gizmo);

			return list;
		}
	}
}
