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
    public class Individual: ISerializable
    {
        #region fields & properties

        /// <summary>
        /// The gene set of the individual
        /// </summary>
        private GeneSet genes;

        //The dataset does not get fully serialised into the save file,
        //because it is saved in a separate ARFF file for use with weka.
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
        /// </summary>
        public Individual()
        {
            //fill genes with random values
            genes = new GeneSet(true);
            dataSet = new DataSet(genes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Individual"/> class
        /// as a deserialised object from a save file.
        /// </summary>
        /// <param name="info">The info</param>
        /// <param name="ctxt">The CTXT</param>
        public Individual(SerializationInfo info, StreamingContext ctxt)
        {
            genes = (GeneSet)info.GetValue("GeneSet", typeof(GeneSet));
            //TODO:
            //dataSet = new DataSet(something to ID file);
            Fitness = (double)info.GetValue("Fitness", typeof(double));
        }

        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/>
        /// with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/>
        /// to populate with data.</param>
        /// <param name="context">The destination (see
        /// <see cref="T:System.Runtime.Serialization.StreamingContext"/>)
        /// for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GeneSet", genes);
            info.AddValue("Fitness", Fitness);
        }

        #endregion //ISerializable Members

        #endregion //methods
    }
}
