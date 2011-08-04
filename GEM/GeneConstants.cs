using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEM
{
    static class GeneConstants
    {
        /// <summary>
        /// Minimum dataset size
        /// </summary>
        public const int minDSSize      = 10;

        /// <summary>
        /// Maximum dataset size
        /// </summary>
        public const int maxDSSize      = 1000;

        /// <summary>
        /// Minimum number of attributes
        /// </summary>
        public const int minNumAttribs  = 2;

        /// <summary>
        /// Maximum number of attributes
        /// </summary>
        public const int maxNumAttribs  = 30;

        /// <summary>
        /// Minimum number of classes
        /// </summary>
        public const int minNumClasses  = 2;

        /// <summary>
        /// Maximum number of classes
        /// </summary>
        public const int maxNumClasses  = 30;
        
        /// <summary>
        /// Minimum number of classes in nominal attributes
        /// </summary>
        public const int minNominal     = 2;

        /// <summary>
        /// Maximum number of classes in nominal attributes
        /// </summary>
        public const int maxNominal     = 1000;

        /// <summary>
        /// Minimum value of discrete attributes
        /// </summary>
        public const int minDiscrete    = -100000;  //-100k

        /// <summary>
        /// Maximum value of discrete attributes
        /// </summary>
        public const int maxDiscrete    = 100000;   //100k

        /// <summary>
        /// Minimum value of continuous attributes
        /// </summary>
        public const int minContinuous  = -1000000; //-1M

        /// <summary>
        /// Maximum value of continuous attributes
        /// </summary>
        public const int maxContinuous  = 1000000;  //1M

        /// <summary>
        /// Minimum of missingValueRatio
        /// </summary>
        public const double minMissing = 0.01;

        /// <summary>
        /// Maximum of missingValueRatio
        /// </summary>
        public const double maxMissing = 0.5;

        /// <summary>
        /// The base chance of mutation
        /// The effective chance is this * the coefficient
        /// </summary>
        public const double baseMutationChance = 0.001;
    }
}
