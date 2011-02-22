using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEM
{
    /// <summary>
    /// A dataset
    /// </summary>
    public class DataSet
    {
        #region methods & properties

        /// <summary>
        /// Gets or sets the fitness
        /// </summary>
        /// <value>
        /// The fitness
        /// </value>
        public double Fitness
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class
        /// </summary>
        public DataSet()
        {
            throw new Exception("Use the DataSet(GeneSet geneSet) constructor instead of the default.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class
        /// from a gene set
        /// </summary>
        /// <param name="geneSet">The gene set.</param>
        public DataSet(GeneSet geneSet)
        {
        }

    }
}
