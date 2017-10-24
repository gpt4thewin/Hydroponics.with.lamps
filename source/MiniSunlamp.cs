using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.AllInOnePonics
{
	public class MiniSunlamp : ThingWithComps
	{
		public override void PostMapInit()
		{
			base.PostMapInit();

			if (this.DestroyedOrNull())
				return;

			var list = this.Position.GetThingList(this.Map);

#if DEBUG
			Log.Message(string.Format("things at {0}: {1}", this.Position, string.Join(" ; ", list.Select(arg => arg.ToString()).ToArray())));
#endif

			if (!list.Any((obj) => obj is Building_PlantGrower))
			{
				Log.Warning("Deleted orphan " + this);
				this.Destroy();
			}
			else
			{
				var stackedLamps = list.Where((arg) => arg.def == this.def && arg != this);
				if (stackedLamps.Any())
				{
					foreach (var item in stackedLamps.ToList())
					{
						Log.Warning("Deleted stacked " + item);
						item.Destroy();
					}
				}
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
		}
	}
}

