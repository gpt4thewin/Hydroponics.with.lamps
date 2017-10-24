using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.AllInOnePonics
{
	public class Command_SetPlantToGrow_Blueprint : Command_Action
	{
		public override void ProcessInput(Event ev)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();

			foreach (ThingDef current in Utils.AvailablePonicsCrops)
			{
				if (this.IsPlantAvailable(current))
				{
					ThingDef localPlantDef = current;
					string text = current.LabelCap;
					if (current.plant.sowMinSkill > 0)
					{
						string text2 = text;
						text = string.Concat(new object[]
						{
					text2,
					" (",
					"MinSkill".Translate(),
					": ",
					current.plant.sowMinSkill,
					")"
						});
					}
					List<FloatMenuOption> arg_121_0 = list;
					Func<Rect, bool> extraPartOnGUI = (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, localPlantDef);
					arg_121_0.Add(new FloatMenuOption(text, delegate
					{
						// ----------- MOD -----------

						blueprint.plantDefToGrow = localPlantDef;

						// ----------- /MOD -----------

						this.WarnAsAppropriate(localPlantDef, blueprint.Map);
					}, MenuOptionPriority.Default, null, null, 29f, extraPartOnGUI, null));
				}
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}


		// Verse.Command_SetPlantToGrow
		private bool IsPlantAvailable(ThingDef plantDef)
		{
			List<ResearchProjectDef> sowResearchPrerequisites = plantDef.plant.sowResearchPrerequisites;
			if (sowResearchPrerequisites == null)
			{
				return true;
			}
			for (int i = 0; i < sowResearchPrerequisites.Count; i++)
			{
				if (!sowResearchPrerequisites[i].IsFinished)
				{
					return false;
				}
			}
			return true;
		}

		// Verse.Command_SetPlantToGrow
		private void WarnAsAppropriate(ThingDef plantDef, Map map)
		{
			if (plantDef.plant.sowMinSkill > 0)
			{
				foreach (Pawn current in map.mapPawns.FreeColonistsSpawned)
				{
					if (current.skills.GetSkill(SkillDefOf.Growing).Level >= plantDef.plant.sowMinSkill && !current.Downed && current.workSettings.WorkIsActive(WorkTypeDefOf.Growing))
					{
						return;
					}
				}
				Find.WindowStack.Add(new Dialog_MessageBox("NoGrowerCanPlant".Translate(new object[]
				{
			plantDef.label,
			plantDef.plant.sowMinSkill
				}).CapitalizeFirst(), null, null, null, null, null, false));
			}
		}
	}
}