﻿// InjectorBills.cs created by Iron Wolf for Pawnmorph on 08/29/2020 3:27 PM
// last updated 08/29/2020  3:27 PM

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pawnmorph.Chambers;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Pawnmorph.ITabs
{
    /// <summary>
    /// Itab for injector bills 
    /// </summary>
    /// <seealso cref="RimWorld.ITab_Bills" />
    public class InjectorBills : ITab_Bills
    {
        private float viewHeight = 1000f;

        private Vector2 scrollPosition;

        private Bill mouseoverBill;

        private static readonly Vector2 WinSize = new Vector2(420f, 480f);

        [CanBeNull]
        MorphDef MorphFromSyringeRecipe([NotNull] RecipeDef rDef)
        {
            var ingestible = rDef.ProducedThingDef?.ingestible; //get the morph required to be tagged by checking the hediff the ingestion outcome doer gives a pawn and comparing that with the 
            if (ingestible == null) return null; //full and/or partial tf hediff stored in MorphDef
            var fOutcomeDoer = ingestible.outcomeDoers?.OfType<IngestionOutcomeDoer_GiveHediff>().FirstOrDefault(); //this is hacky and need to figure out a more elegant solution that doesn't 
            var morphHDef = fOutcomeDoer?.hediffDef; //required all injector or morph xml files to be updated 
            if (morphHDef == null) return null;
            return MorphDef.AllDefs.FirstOrDefault(m => m.fullTransformation == morphHDef
                                                     || m.partialTransformation == morphHDef); 
        }


        bool CanUseRecipe(RecipeDef recipe)
        {
            if(!(recipe.AvailableNow && recipe.AvailableOnNow(SelTable))) return false;
            if (!LoadedModManager.GetMod<PawnmorpherMod>().GetSettings<PawnmorpherSettings>().injectorsRequireTagging)
                return true; 
            
            
            var mDef = MorphFromSyringeRecipe(recipe);
            return mDef?.IsTagged() != false; 
        }

        [TweakValue("PMInterface", 0f, 128f)]
        private static float PasteX = 48f;

        [TweakValue("PMInterface", 0f, 128f)]
        private static float PasteY = 3f;

        [TweakValue("PMInterface", 0f, 32f)]
        private static float PasteSize = 24f;
        /// <summary>
        /// Fills the tab.
        /// </summary>
        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            Rect rect2 = new Rect(WinSize.x - PasteX, PasteY, PasteSize, PasteSize);
            if (BillUtility.Clipboard == null)
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect2, PMTexButton.Paste, 1f);
                GUI.color = Color.white;
                TooltipHandler.TipRegionByKey(rect2, "PasteBillTip");
            }
            else if (!SelTable.def.AllRecipes.Contains(BillUtility.Clipboard.recipe) || !BillUtility.Clipboard.recipe.AvailableNow || !BillUtility.Clipboard.recipe.AvailableOnNow(SelTable))
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect2, PMTexButton.Paste, 1f);
                GUI.color = Color.white;
                TooltipHandler.TipRegionByKey(rect2, "ClipboardBillNotAvailableHere");
            }
            else if (SelTable.billStack.Count >= 15)
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect2, PMTexButton.Paste, 1f);
                GUI.color = Color.white;
                if (Mouse.IsOver(rect2))
                {
                    TooltipHandler.TipRegion(rect2, "PasteBillTip".Translate() + " (" + "PasteBillTip_LimitReached".Translate() + ")");
                }
            }
            else
            {
                if (Widgets.ButtonImageFitted(rect2, PMTexButton.Paste, Color.white))
                {
                    Bill bill = BillUtility.Clipboard.Clone();
                    bill.InitializeAfterClone();
                    SelTable.billStack.AddBill(bill);
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegionByKey(rect2, "PasteBillTip");
            }
            Rect rect3 = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                
                for (int i = 0; i < SelTable.def.AllRecipes.Count; i++)
                {
                    RecipeDef recipe = SelTable.def.AllRecipes[i];
                    if (CanUseRecipe(recipe))
                    {
                        var tempRecipe = recipe;
                        list.Add(new FloatMenuOption(recipe.LabelCap, delegate
                        {
                            if (!SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
                            {
                                Bill.CreateNoPawnsWithSkillDialog(recipe);
                            }
                            Bill bill2 = recipe.MakeNewBill();
                            SelTable.billStack.AddBill(bill2);
                            if (recipe.conceptLearned != null)
                            {
                                PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
                            }
                            if (TutorSystem.TutorialMode)
                            {
                                TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap.Resolve());
                            }
                        }, recipe.UIIconThing, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, recipe)));
                    }
                }
                if (!list.Any())
                {
                    list.Add(new FloatMenuOption("NoneBrackets".Translate(), null));
                }
                return list;
            };
            mouseoverBill = SelTable.billStack.DoListing(rect3, recipeOptionsMaker, ref scrollPosition, ref viewHeight);
        }

    }
}