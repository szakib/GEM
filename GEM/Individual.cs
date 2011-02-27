using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GEM
{
    [Serializable()]
    /// <summary>
    /// An individual in the population of the genetic algorithm
    /// </summary>
    public class Individual
    {
        #region fields & properties

        private GeneSet genes;
        private DataSet dataSet;
        
        /// <summary>
        /// The gene set of the individual
        /// </summary>
        /// <value>
        /// The genes
        /// </value>
        public GeneSet Genes
        {
            get
            {
                return genes;
            }
            set
            {
                genes = value;
            }
        }

        /// <summary>
        /// The dataset that the individual represents
        /// </summary>
        /// <value>
        /// The dataset
        /// </value>
        public DataSet DataSet
        {
            get
            {
                return dataSet;
            }
            set
            {
                dataSet = value;
            }
        }

        /// <summary>
        /// Fitness value of the individual.
        /// Same as fitness of its dataset.
        /// </summary>
        /// <value>
        /// The fitness
        /// </value>
        public double Fitness
        {
            get
            {
                return this.DataSet.Fitness;
            }
            set
            {
                this.DataSet.Fitness = value;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Individual"/> class
        /// </summary>
        public Individual()
        {
            //fill genes with random values
            genes = new GeneSet(true);
            dataSet = new DataSet(genes);
        }

        #endregion
    }
}
