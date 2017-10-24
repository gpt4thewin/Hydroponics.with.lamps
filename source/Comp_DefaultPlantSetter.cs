using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WM.AllInOnePonics
{
	

	public class Comp_DefaultPlantSetter : ThingComp
	{
		public Comp_DefaultPlantSetter()
		{
		}

		public Building_PlantGrower ParentPlantGrower
		{
			get
			{
				return (this.parent as Building_PlantGrower);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			var list = new List<Gizmo>();

			var setDefaultGizmo = new Command_Action();

			var plantDef = (ParentPlantGrower).GetPlantDefToGrow();

			setDefaultGizmo.action = delegate
			{
				Core.SetDefaultPonicsPlant(this.parent.def,plantDef);
			};

			var thingDef = Core.GetDefaultPonicsPlant(this.parent.def);
			setDefaultGizmo.icon = thingDef.uiIcon;

			setDefaultGizmo.defaultLabel = string.Format("SetDefaultPonicsPlant".Translate());

			list.Add(setDefaultGizmo);

			return list;
		}
	}
}
