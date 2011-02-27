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
        public const int minDSSize      = 100;

        /// <summary>
        /// Maximum dataset size
        /// </summary>
        public const int maxDSSize      = 1000;

        /// <summary>
        /// Minimum number of attributes
        /// </summary>
        public const int minNumAttribs  = 10;

        /// <summary>
        /// Maximum number of attributes
        /// </summary>
        public const int maxNumAttribs  = 1000;

        /// <summary>
        /// Minimum number of classes
        /// </summary>
        public const int minNumClasses  = 2;

        /// <summary>
        /// Maximum number of classes
        /// </summary>
        public const int maxNumClasses  = 1000;
        
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
    }
}
