﻿// MutationCategoryDef.cs created by Iron Wolf for Pawnmorph on 09/15/2019 9:00 PM
// last updated 09/15/2019  9:00 PM

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pawnmorph.Hediffs;
using Verse;

namespace Pawnmorph
{
    /// <summary>
    /// def representing a 'category' of mutations 
    /// </summary>
    public class MutationCategoryDef : Def
    {


        [NotNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        private List<MutationDef> mutations = new List<MutationDef>();

        /// <summary>
        /// if true, this category will be used to generate a 'genome' that will give a random mutation within this category 
        /// </summary>
        public bool genomeProvider; 

        /// <summary>if mutations in this category should be restricted to special PawnGroupKinds</summary>
        public bool restricted;

        /// <summary>
        /// a custom description for the genome object generated by this category 
        /// </summary>
        public string customGenomeDescription;

        /// <summary>
        /// The explicit genome definition
        /// </summary>
        public ThingDef explicitGenomeDef;


        /// <summary>
        /// if the associated genome is consumed on use 
        /// </summary>
        public bool genomeConsumedOnUse = true; 

        internal ThingDef implicitGenomeDef;

        /// <summary>
        /// Gets the explicit MutationCategoryDef if it exists, otherwise retrieves the implict MutationCategoryDef.
        /// </summary>
        [CanBeNull]
        public ThingDef GenomeDef => explicitGenomeDef ?? implicitGenomeDef; 

        [Unsaved] private List<MutationDef> _allMutations;

        /// <summary>
        /// Gets the number of mutations in the category.
        /// </summary>
        /// <value>
        /// The mutations in category.
        /// </value>
        public int MutationsInCategory => AllMutations.Count();

        /// <summary>
        /// Gets all mutations in this category 
        /// </summary>
        /// <value>
        /// All mutations.
        /// </value>
        public IEnumerable<MutationDef> AllMutations
        {
            get
            {
                if (_allMutations == null)
                {
                    _allMutations = new List<MutationDef>(mutations);

                    foreach (MutationDef mutation in MutationDef.AllMutations.Where(m => m.categories.Contains(this)))
                    {
                        if (!_allMutations.Contains(mutation))
                            _allMutations.Add(mutation); 
                    }

                }

                return _allMutations; 
            }
        }

        /// <summary>
        /// gets all configuration errors with this instance .
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string configError in base.ConfigErrors())
            {
                yield return configError;
            }

            if (genomeProvider && string.IsNullOrEmpty(label))
            {
                yield return "is genome provide but has no label!";
            }
        }
    }
}