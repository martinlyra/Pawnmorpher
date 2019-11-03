﻿// TransformationRequest.cs modified by Iron Wolf for Pawnmorph on 08/18/2019 8:55 AM
// last updated 08/18/2019  8:55 AM

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Pawnmorph.TfSys
{
    /// <summary>
    /// struct representing the request to transform pawns 
    /// </summary>
    public struct TransformationRequest
    {
        /// <summary>
        /// Returns true if this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => originals != null && originals.Length > 0 && outputDef != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationRequest"/> struct.
        /// </summary>
        /// <param name="outputDef">The output definition.</param>
        /// <param name="original">The original.</param>
        /// <param name="maxSeverity">the maximum severity of the former human hediff</param>
        public TransformationRequest(PawnKindDef outputDef, Pawn original, float maxSeverity=1)
        {
            originals = new [] {original};
            this.outputDef = outputDef;
            forcedGender = TFGender.Original;
            forcedGenderChance = 50; 
            cause = null;
            tale = null;
            this.maxSeverity = maxSeverity;
            minSeverity = 0;
            forcedFaction = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationRequest"/> struct.
        /// </summary>
        /// <param name="outputDef">The output definition.</param>
        /// <param name="originals">The originals.</param>
        public TransformationRequest(PawnKindDef outputDef, params Pawn[] originals)
        {
            this.originals = originals;
            this.outputDef = outputDef;
            forcedGender = TFGender.Original;
            forcedGenderChance = 50;
            cause = null;
            tale = null;
            this.maxSeverity = 1;
            minSeverity = 0;
            forcedFaction = default; 
        }
        /// <summary>The pawns to be transformed</summary>
        public Pawn[] originals;
        /// <summary>The output pawn kind</summary>
        public PawnKindDef outputDef;
        /// <summary>The forced gender option</summary>
        public TFGender forcedGender;
        /// <summary>
        /// if forcedGender is None, the chance to switch genders 
        /// </summary>
        public float forcedGenderChance;
        /// <summary>The cause of the transformation</summary>
        public Hediff cause;
        /// <summary>The tale to record</summary>
        public TaleDef tale;

        /// <summary>
        /// the faction to put the resultant pawn into 
        /// </summary>
        public Faction forcedFaction; 

        /// <summary>
        /// The minimum severity of the former human hediff
        /// </summary>
        public float minSeverity; 

        /// <summary>
        /// The minimum severity of the former human hediff
        /// </summary>
        public float maxSeverity; 

    }
}