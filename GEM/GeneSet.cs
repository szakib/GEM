using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixLibrary;

namespace GEM
{
    [Serializable()]
    /// <summary>
    /// The set of genes representing a dataset
    /// </summary>
    public class GeneSet
    {
        #region fields & properties

        private Guid    guid;

        /// <summary>
        /// total number of instances (both training and test)
        /// </summary>
        private int     dataSetSize;
        
        /// <summary>
        /// number of attributes (including target attribute)
        /// </summary>
        private int     numAttribs;

        /// <summary>
        /// number of classes
        /// </summary>
        private int     numClasses;

        /// <summary>
        /// percentage of nominal attributes
        /// </summary>
        private double  nominalAttribRatio;

        /// <summary>
        /// percentage of discrete attributes
        /// </summary>
        private double  discreteAttribRatio;

        /// <summary>
        /// percentage of missing values
        /// </summary>
        private double  missingValueRatio;

        /// <summary>
        /// percentage of irrelevant attribs
        /// </summary>
        private double  irrelevantAttribRatio;

        //Matrices for statistical generation of data (p45 in Anton's thesis)

        /// <summary>
        /// Column vector of mean values of attribs
        /// </summary>
        private Matrix  meanMatrix;

        /// <summary>
        /// Column vector of stddev values of attribs.
        /// </summary>
        private Matrix  stdDevMatrix;

        /// <summary>
        /// Correlation matrix between the attributes
        /// </summary>
        private Matrix  correlationMatrix;

        /// <summary>
        /// Column vector of number of classes in the nominal attributes
        /// </summary>
        private Matrix nominalClassesMatrix;

        //properties

        /// <summary>
        /// Ratio of continuous attributes
        /// </summary>
        public double ContinuousAttribRatio
        {
            get
            {
                return 1 - nominalAttribRatio - discreteAttribRatio;
            }
        }

        /// <summary>
        /// number of nominal attributes
        /// </summary>
        public int NumNominalAttribs
        {
            get
            {
                return (int)Math.Round(numAttribs * nominalAttribRatio);
            }
        }

        /// <summary>
        /// Number of discrete attributes
        /// </summary>
        public int NumDiscreteAttribs
        {
            get
            {
                return (int)Math.Round(numAttribs * discreteAttribRatio);
            }
        }

        /// <summary>
        /// Number of continuous attributes
        /// </summary>
        public int NumContinuousAttribs
        {
            get
            {
                return numAttribs - NumNominalAttribs - NumDiscreteAttribs;
            }
        }

        #endregion

        #region methods

        public GeneSet()
        {
            throw new Exception("Use GeneSet(bool fillRandom) constructor instead.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneSet"/> class
        /// </summary>
        /// <param name="fillRandom">if set to <c>true</c> fills random values,
        /// otherwise returns empty gene set.</param>
        public GeneSet(bool fillRandom)
        {
            guid = new Guid();

            if (fillRandom)
            {
                Random rnd = new Random();

                dataSetSize = rnd.Next(GeneConstants.minDSSize, GeneConstants.maxDSSize);
                numAttribs = rnd.Next(GeneConstants.minNumAttribs, GeneConstants.maxNumAttribs);
                numClasses = rnd.Next(GeneConstants.minNumClasses, GeneConstants.maxNumClasses);

                nominalAttribRatio = rnd.NextDouble();
                discreteAttribRatio = rnd.NextDouble();
                missingValueRatio = rnd.NextDouble();
                irrelevantAttribRatio = rnd.NextDouble();
            }
            else
            {
                numAttribs = 1;
            }

            meanMatrix = new Matrix(numAttribs, 1);
            stdDevMatrix = new Matrix(numAttribs, 1);
            correlationMatrix = new Matrix(numAttribs, numAttribs);
            nominalClassesMatrix = new Matrix(NumNominalAttribs, 1);

            if (fillRandom)
                RandomiseMatrices();
        }

        /// <summary>
        /// Randomises the matrices according to the values of the scalar genes.
        /// </summary>
        public void RandomiseMatrices()
        {
            Random rnd = new Random();

            for (int row = 0; row < numAttribs; row++)
            {
                //nominal attribute
                if (row < NumNominalAttribs)
                {
                    //has to be between min and max
                    nominalClassesMatrix[row, 0] = Math.Round((rnd.NextDouble()
                                            * (GeneConstants.maxNominal - GeneConstants.minNominal)
                                            ) + GeneConstants.minNominal);

                    //has to be between 0 and max class
                    meanMatrix[row, 0] = rnd.NextDouble() * (nominalClassesMatrix[row, 0] - 1);
                    //has to be between 0 and max class
                    stdDevMatrix[row, 0] = rnd.NextDouble() * (nominalClassesMatrix[row, 0] - 1);
                }
                //discrete attribute
                else if (row < NumNominalAttribs + NumDiscreteAttribs)
                {
                    //has to be between min and max
                    meanMatrix[row, 0] = (rnd.NextDouble()
                                            * (GeneConstants.maxDiscrete - GeneConstants.minDiscrete)
                                         ) + GeneConstants.minDiscrete;
                    //has to be between 0 and max
                    stdDevMatrix[row, 0] = rnd.NextDouble() * GeneConstants.maxDiscrete;
                }
                //continuous attribute
                else if (row < numAttribs - 1)
                {
                    //has to be between min and max
                    meanMatrix[row, 0] = (rnd.NextDouble()
                                            * (GeneConstants.maxContinuous - GeneConstants.minContinuous)
                                         ) + GeneConstants.minContinuous;
                    //has to be between 0 and max
                    stdDevMatrix[row, 0] = rnd.NextDouble() * GeneConstants.maxContinuous;
                }
                //class
                else
                {
                    //both the mean and stddev need to be
                    //between 0 and the number of the last class
                    meanMatrix[row, 0] = rnd.NextDouble() * (numClasses - 1);
                    stdDevMatrix[row, 0] = rnd.NextDouble() * (numClasses - 1);
                }

                //Correlation matrix
                for (int column = 0; column < numAttribs; column++)
                {
                    //initialise one half
                    if (column > row)
                        //correlationMatrix elements between -1 and 1
                        correlationMatrix[row, column] = (rnd.NextDouble() * 2) - 1;
                    //centerline is filled with 1's
                    else if (column == row)
                        correlationMatrix[row, column] = 1;
                    //other half copied
                    else
                        correlationMatrix[row, column] = correlationMatrix[column, row];
                }
            }
        }

        /// <summary>
        /// Mutates the gene set according to the given mutation coefficient
        /// </summary>
        /// <param name="mutationCoefficient">The mutation coefficient</param>
        public void Mutate(double mutationCoefficient)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
