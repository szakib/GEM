using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;

namespace GEM
{
    [Serializable()]
    /// <summary>
    /// An individual in the population of the genetic algorithm
    /// </summary>
    public class Individual //: ISerializable
    {
        #region fields & properties

        /// <summary>
        /// True if this individual is immune to mutation
        /// </summary>
        private bool immuneToMutation = false;

        /// <summary>
        /// Accumulated fitness value for selection
        /// </summary>
        public double AccumFitness = 0;
        
        /// <summary>
        /// The gene set of the individual
        /// </summary>
        private GeneSet genes;

        //Only the fitness value is saved.
        //[NonSerialized]
        /// <summary>
        /// The dataset of the individual
        /// </summary>
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
                //the DS only gets generated when it is actually needed
                if (null == dataSet)
                    dataSet = new DataSet(genes);

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
                return dataSet.Fitness;
            }
            set
            {
                dataSet.Fitness = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Individual"/> has mutated in the last round.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mutated; otherwise, <c>false</c>.
        /// </value>
        public bool Mutated
        {
            get
            {
                return genes.Mutated;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Individual"/> class
        /// with random values
        /// </summary>
        public Individual()
        {
            //fill genes with random values
            genes = new GeneSet(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Individual"/> class
        /// from a given gene set
        /// </summary>
        /// <param name="initGenes">The <see cref="GeneSet"/> specifying the individual</param>
        public Individual(GeneSet initGenes)
        {
            genes = initGenes;
        }

        /// <summary>
        /// Immunises the individual against future mutations
        /// </summary>
        public void ImmuniseAgainstMutations()
        {
            immuneToMutation = true;
            genes.Mutated = false;
        }

        /// <summary>
        /// Mutates the gene set according to the specified mutation coefficient
        /// </summary>
        /// <param name="mutationCoefficient">The mutation coefficient</param>
        public void Mutate(double mutationCoefficient)
        {
            if (!immuneToMutation)
                genes.Mutate(mutationCoefficient);
            //if there was a mutation, the DS is invalid
            if (genes.Mutated)
                dataSet = null;
        }

        /// <summary>
        /// Saves the dataset of this individual into an ARFF file
        /// </summary>
        /// <param name="path">The path to save to, excluding the filename</param>
        public void SaveArff(string path)
        {
            dataSet.SaveArff(Path.Combine(path, genes.guid.ToString() + ".arff"));
        }

        #endregion //methods
    } //public class Individual

    /// <summary>
    /// Class for the (descending) sorting of Individuals by their fitness
    /// </summary>
    public class IndividualComparer : IComparer<Individual>
    {
        #region IComparer<Individual> Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first individual to compare.</param>
        /// <param name="y">The second individual to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(Individual x, Individual y)
        {
            if (null != x && null != y)
            {
                //this results in a descending order
                return y.Fitness.CompareTo(x.Fitness);
            }
            else if (null == x && null != y)
                return 1;
            else if (null == y && null != x)
                return -1;
            else //both are null
                return 0;
        }

        #endregion
    } //public class IndividualComparer : IComparer<Individual>
}
