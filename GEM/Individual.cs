using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEM
{
    /// <summary>
    /// An individual in the population of the genetic algorithm
    /// </summary>
    public class Individual
    {
        #region fields & properties

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
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
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
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Fitness value of the individual
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


        #endregion
    }
}
