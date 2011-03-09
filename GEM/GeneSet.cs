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

        /// <summary>
        /// Unique ID linking the gene set to the dataset it represents
        /// </summary>
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

        /*The order of the attributes is the following:
         Class
         Nominal ones
         Discrete ones
         Continuous ones*/

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

                dataSetSize = RandomInt(rnd, GeneConstants.minDSSize, GeneConstants.maxDSSize);
                numAttribs = RandomInt(rnd, GeneConstants.minNumAttribs, GeneConstants.maxNumAttribs);
                numClasses = RandomInt(rnd, GeneConstants.minNumClasses, GeneConstants.maxNumClasses);

                //values between 0 and 1, no need for RandomDouble()
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
                //class
                if (0 == row)
                {
                    //both the mean and stddev need to be
                    //between 0 and the number of the last class
                    meanMatrix[row, 0] = RandomDouble(rnd, 0, numClasses - 1);
                    stdDevMatrix[row, 0] = RandomDouble(rnd, 0, numClasses - 1);
                }
                //nominal attribute
                else if (row < NumNominalAttribs + 1)
                {
                    //has to be between min and max, needs to be an int stored as double
                    nominalClassesMatrix[row, 0]
                        = Math.Round(RandomDouble(rnd, GeneConstants.minNominal, GeneConstants.maxNominal));

                    //has to be between 0 and max class
                    meanMatrix[row, 0] = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                    //has to be between 0 and max class
                    stdDevMatrix[row, 0] = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                }
                //discrete attribute
                else if (row < NumNominalAttribs + NumDiscreteAttribs + 1)
                {
                    //has to be between min and max
                    meanMatrix[row, 0]
                        = RandomDouble(rnd, GeneConstants.minDiscrete, GeneConstants.maxDiscrete);
                    //has to be between 0 and max
                    stdDevMatrix[row, 0] = RandomDouble(rnd, 0, GeneConstants.maxDiscrete);
                }
                //continuous attribute
                else
                {
                    //has to be between min and max
                    meanMatrix[row, 0]
                        = RandomDouble(rnd, GeneConstants.minContinuous, GeneConstants.maxContinuous);
                    //has to be between 0 and max
                    stdDevMatrix[row, 0] = RandomDouble(rnd, 0, GeneConstants.maxContinuous);
                }

                //Correlation matrix
                for (int column = 0; column < numAttribs; column++)
                {
                    //initialise one half
                    if (column > row)
                        //correlationMatrix elements between -1 and 1
                        correlationMatrix[row, column] = RandomDouble(rnd, -1, 1);
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
            Random rnd = new Random();

            double chance = mutationCoefficient * GeneConstants.baseMutationChance;

            //dataSetSize;
            if (GetsMutated(rnd, chance))
                dataSetSize = RandomInt(rnd, GeneConstants.minDSSize, GeneConstants.maxDSSize);
            //numAttribs;
            if (GetsMutated(rnd, chance))
                numAttribs = RandomInt(rnd, GeneConstants.minNumAttribs, GeneConstants.maxNumAttribs);
            //numClasses;
            if (GetsMutated(rnd, chance))
                numClasses = RandomInt(rnd, GeneConstants.minNumClasses, GeneConstants.maxNumClasses);

            //nominalAttribRatio;
            if (GetsMutated(rnd, chance))
                nominalAttribRatio = rnd.NextDouble();
            //discreteAttribRatio;
            if (GetsMutated(rnd, chance))
                discreteAttribRatio = rnd.NextDouble();
            //missingValueRatio;
            if (GetsMutated(rnd, chance))
                missingValueRatio = rnd.NextDouble();
            //irrelevantAttribRatio;
            if (GetsMutated(rnd, chance))
                irrelevantAttribRatio = rnd.NextDouble();

            //The matrices are mutated first and then adjusted
            //to avoid replacing new random values with even newer random values
            
            /*//meanMatrix;
            MutateMatrix(meanMatrix, rnd, chance);
            AdjustMatrixSize(meanMatrix);
            //stdDevMatrix;
            MutateMatrix(meanMatrix);
            AdjustMatrixSize(meanMatrix);
            //correlationMatrix;
            MutateMatrix(meanMatrix);
            AdjustMatrixSize(meanMatrix);
            //nominalClassesMatrix;
            MutateMatrix(meanMatrix);
            AdjustMatrixSize(meanMatrix);*/
        }

        /// <summary>
        /// Binary decision on whether a particular value gets mutated
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="chance">Mutation chance</param>
        /// <returns><c>true</c> if it does get mutated, <c>false</c> otherwise</returns>
        private bool GetsMutated(Random rnd, double chance)
        {
            return rnd.NextDouble() < chance;
        }

        /// <summary>
        /// Random int between min and max
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns>The random int</returns>
        private int RandomInt(Random rnd, int min, int max)
        {
            if (max >= min)
                return rnd.Next(min, max);
            else
                return rnd.Next(max, min);
        }

        /// <summary>
        /// Random double between min and max
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns>The random double</returns>
        private double RandomDouble(Random rnd, double min, double max)
        {
            if (max >= min)
                return (rnd.NextDouble() * (max - min)) + min;
            else
                return (rnd.NextDouble() * (min - max)) + max;
        }

        /// <summary>
        /// Mutates the target matrix.
        /// </summary>
        /// <param name="target">The target matrix</param>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="chance">The mutation chance</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns>The mutated matrix</returns>
        private Matrix MutateMatrix(Matrix target,
                                    Random rnd,
                                    double chance,
                                    double min,
                                    double max)
        {
            for (int row = 0; row < target.NoRows; row++)
                for (int col = 0; col < target.NoCols; col++)
                    if (GetsMutated(rnd, chance))
                        target[row, col] = RandomDouble(rnd, min, max);

            return target;
        }

        /// <summary>
        /// Adjusts the size of a matrix
        /// and fills any newly created elements with appropriate random values
        /// </summary>
        /// <param name="target">The matrix to adjust</param>
        /// <param name="newRows">The new number of rows</param>
        /// <param name="newCols">The new number of cols</param>
        /// <param name="minValue">The minimum allowed value</param>
        /// <param name="maxValue">The maximum allowed value</param>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <returns>The adjusted matrix</returns>
        private Matrix AdjustMatrixSize(Matrix target,
                                        int newRows,
                                        int newCols,
                                        double minValue,
                                        double maxValue,
                                        Random rnd)
        {
            if (target.NoCols == newCols && target.NoRows == newRows)
                return target;
            else
            {
                Matrix ret = new Matrix(newRows, newCols);

                //copy available values and fill any new ones with random numbers
                for (int row = 0; row < newRows; row++)
                    //there is a potentially a value to be copied
                    if (row < target.NoRows)
                        for (int col = 0; col < newRows; col++)
                            //there is a value to be copied
                            if (col < target.NoCols)
                                ret[row, col] = target[row, col];
                            else
                                ret[row, col] = RandomDouble(rnd, minValue, maxValue);
                    else
                        for (int col = 0; col < newRows; col++)
                            ret[row, col] = RandomDouble(rnd, minValue, maxValue);

                return ret;
            }
        }

        #endregion
    }
}
