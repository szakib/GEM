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

        /// <summary>
        /// Fitness value of this dataset
        /// </summary>
        private double fitness = 0;

        /// <summary>
        /// Gets or sets the fitness
        /// </summary>
        /// <value>
        /// The fitness value
        /// </value>
        public double Fitness
        {
            get
            {
                if (fitness == 0)
                {
                    //TODO calculate it, meaning:
                    //run the learning with the ARFF file
                    //and actually calculate the fitness value from the results of the learning
                    throw new System.NotImplementedException();
                }
                
                return fitness;
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
            throw new Exception("Use one of the other constructors instead of the default.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class
        /// from a gene set
        /// </summary>
        /// <param name="geneSet">The gene set.</param>
        public DataSet(GeneSet geneSet)
        {
            //DS will be passed to weka as an ARFF file
            //TODO steps to take here:
            //generate dataset from the data
            //make ARFF file
            //save ARFF using the GUID of the gene set as name
            throw new System.NotImplementedException();
        }

        //TODO make constructor to load dataset from file
        /*        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class
        /// from an ARFF file
        /// </summary>
        public DataSet()
        {
            throw new System.NotImplementedException();
        }*/

        #endregion
    }
}
