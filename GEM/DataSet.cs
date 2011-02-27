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
        #region fields & properties

        private double fitness = 0;

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
                if (fitness == 0)
                {
                    //calculate it
                    throw new System.NotImplementedException();
                }
                else return fitness;
            }
            set
            {
                fitness = value;
            }
        }

        #endregion

        #region methods

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
            //TODO: before doing this,
            //I'll have to know how I pass the DS to weka (file/param., etc.)
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
