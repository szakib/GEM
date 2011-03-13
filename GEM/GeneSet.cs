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
        /// Mean value of class attribute
        /// </summary>
        private double meanClass;

        /// <summary>
        /// Standard deviation of class attribute
        /// </summary>
        private double stdDevClass;

        /// <summary>
        /// Column vector of mean values of nominal attribs
        /// </summary>
        private Matrix meanMatrixNominal;

        /// <summary>
        /// Column vector of mean values of discrete attribs
        /// </summary>
        private Matrix meanMatrixDiscrete;

        /// <summary>
        /// Column vector of mean values of continuous attribs
        /// </summary>
        private Matrix meanMatrixContinuous;

        /// <summary>
        /// Column vector of stddev values of nominal attribs.
        /// </summary>
        private Matrix stdDevMatrixNominal;

        /// <summary>
        /// Column vector of stddev values of discrete attribs.
        /// </summary>
        private Matrix stdDevMatrixDiscrete;

        /// <summary>
        /// Column vector of stddev values of continuous attribs.
        /// </summary>
        private Matrix stdDevMatrixContinuous;

        /// <summary>
        /// Column vector of number of classes in the nominal attributes
        /// </summary>
        private Matrix  nominalClassesMatrix;

        /// <summary>
        /// Correlation matrix among the attributes
        /// </summary>
        private Matrix  correlationMatrix;

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

        /*The order of the attributes is the following:
         Class
         Nominal ones
         Discrete ones
         Continuous ones*/

        /// <summary>
        /// Column vector of mean values of attribs
        /// </summary>
        private Matrix meanMatrix
        {
            get
            {
                Matrix ret = new Matrix(numAttribs, 1);

                ret[0, 0] = meanClass;

                for (int row = 1; row < numAttribs; row++)
                    //nominal
                    if (row < NumNominalAttribs + 1)
                        ret[row, 0] = meanMatrixNominal[row - 1, 0];
                    //discrete
                    else if (row < NumNominalAttribs + NumDiscreteAttribs + 1)
                        ret[row, 0] = meanMatrixDiscrete[row - 1 - NumNominalAttribs, 0];
                    //continuous
                    else
                    {
                        int index = row - 1 - NumNominalAttribs - NumDiscreteAttribs;
                        ret[row, 0] = meanMatrixContinuous[index, 0];
                    }

                return ret;
            }
        }

        /// <summary>
        /// Column vector of stddev values of attribs.
        /// </summary>
        private Matrix stdDevMatrix
        {
            get
            {
                Matrix ret = new Matrix(numAttribs, 1);

                ret[0, 0] = meanClass;

                for (int row = 1; row < numAttribs; row++)
                    //nominal
                    if (row < NumNominalAttribs + 1)
                        ret[row, 0] = stdDevMatrixNominal[row - 1, 0];
                    //discrete
                    else if (row < NumNominalAttribs + NumDiscreteAttribs + 1)
                        ret[row, 0] = stdDevMatrixDiscrete[row - 1 - NumNominalAttribs, 0];
                    //continuous
                    else
                    {
                        int index = row - 1 - NumNominalAttribs - NumDiscreteAttribs;
                        ret[row, 0] = stdDevMatrixContinuous[index, 0];
                    }

                return ret;
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

                meanClass = RandomDouble(rnd, 0, numClasses - 1);
                stdDevClass = RandomDouble(rnd, 0, numClasses - 1);
            }
            else
            {
                numAttribs = 1;
                nominalAttribRatio = 0;
                discreteAttribRatio = 0;
            }

            InitMatrices();

            if (fillRandom)
                RandomiseMatrices();
        }

        /// <summary>
        /// Initialises the matrices
        /// </summary>
        public InitMatrices()
        {
            meanMatrixNominal = new Matrix(NumNominalAttribs, 1);
            stdDevMatrixNominal = new Matrix(NumNominalAttribs, 1);
            meanMatrixDiscrete = new Matrix(NumDiscreteAttribs, 1);
            stdDevMatrixDiscrete = new Matrix(NumDiscreteAttribs, 1);
            meanMatrixContinuous = new Matrix(NumContinuousAttribs, 1);
            stdDevMatrixContinuous = new Matrix(NumContinuousAttribs, 1);
            nominalClassesMatrix = new Matrix(NumNominalAttribs, 1);
            correlationMatrix = new Matrix(numAttribs, numAttribs);
        }

        /// <summary>
        /// Randomises the matrices according to the values of the scalar genes
        /// </summary>
        public void RandomiseMatrices()
        {
            Random rnd = new Random();

            //class is already initialised by the time we get here

            //nominal
            for (int row = 0; row < NumNominalAttribs; row++)
            {
                //has to be between min and max, needs to be an int stored as double
                nominalClassesMatrix[row, 0]
                    = Math.Round(RandomDouble(rnd, GeneConstants.minNominal, GeneConstants.maxNominal));

                //has to be between 0 and max class
                meanMatrixNominal[row, 0] = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                //has to be between 0 and max class
                stdDevMatrixNominal[row, 0] = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
            }

            //discrete
            for (int row = 0; row < NumDiscreteAttribs; row++)
            {
                //has to be between min and max
                meanMatrixDiscrete[row, 0]
                    = RandomDouble(rnd, GeneConstants.minDiscrete, GeneConstants.maxDiscrete);
                //has to be between 0 and max
                stdDevMatrixDiscrete[row, 0] = RandomDouble(rnd, 0, GeneConstants.maxDiscrete);
            }

            //continuous
            for (int row = 0; row < NumContinuousAttribs; row++)
            {
                //has to be between min and max
                meanMatrixContinuous[row, 0]
                    = RandomDouble(rnd, GeneConstants.minContinuous, GeneConstants.maxContinuous);
                //has to be between 0 and max
                stdDevMatrixContinuous[row, 0] = RandomDouble(rnd, 0, GeneConstants.maxContinuous);
            }

            //Correlation matrix
            for (int row = 0; row < numAttribs; row++)
                for (int column = 0; column < numAttribs; column++)
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

        /// <summary>
        /// Mutates the gene set according to the given mutation coefficient
        /// </summary>
        /// <param name="mutationCoefficient">The mutation coefficient</param>
        public void Mutate(double mutationCoefficient)
        {
            Random rnd = new Random();

            double chance = mutationCoefficient * GeneConstants.baseMutationChance;

            //dataSetSize
            if (GetsMutated(rnd, chance))
                dataSetSize = RandomInt(rnd, GeneConstants.minDSSize, GeneConstants.maxDSSize);
            //numAttribs
            if (GetsMutated(rnd, chance))
                numAttribs = RandomInt(rnd, GeneConstants.minNumAttribs, GeneConstants.maxNumAttribs);
            //numClasses
            if (GetsMutated(rnd, chance))
                numClasses = RandomInt(rnd, GeneConstants.minNumClasses, GeneConstants.maxNumClasses);

            //nominalAttribRatio
            if (GetsMutated(rnd, chance))
                nominalAttribRatio = rnd.NextDouble();
            //discreteAttribRatio
            if (GetsMutated(rnd, chance))
                discreteAttribRatio = rnd.NextDouble();
            //missingValueRatio
            if (GetsMutated(rnd, chance))
                missingValueRatio = rnd.NextDouble();
            //irrelevantAttribRatio
            if (GetsMutated(rnd, chance))
                irrelevantAttribRatio = rnd.NextDouble();

            //meanClass
            if (GetsMutated(rnd, chance))
                meanClass = RandomDouble(rnd, 0, numClasses - 1);
            //stdDevClass
            if (GetsMutated(rnd, chance))
                stdDevClass = RandomDouble(rnd, 0, numClasses - 1);

            //The matrices are mutated first and then adjusted
            //to avoid replacing new random values with even newer random values
            
            //correlationMatrix
            correlationMatrix = MutateMatrix(correlationMatrix, rnd, chance, -1, 1);
            correlationMatrix = AdjustMatrixSize(correlationMatrix, numAttribs, numAttribs, -1, 1, rnd);

            //nominalClassesMatrix
            nominalClassesMatrix = MutateMatrix(nominalClassesMatrix, rnd, chance,
                GeneConstants.minNominal, GeneConstants.maxNominal);
            nominalClassesMatrix = AdjustMatrixSize(nominalClassesMatrix, NumNominalAttribs, 1,
                GeneConstants.minNominal, GeneConstants.maxNominal, rnd);
            nominalClassesMatrix = RoundMatrixValues(nominalClassesMatrix);
            
            # region nominal mean&stdDev

            Matrix newMeanMatrixNominal = new Matrix(NumNominalAttribs, 1);
            Matrix newStdDevMatrixNominal = new Matrix(NumNominalAttribs, 1);
                        
            for (int row = 0; row < NumNominalAttribs; row++)
            {
                //while still inside the original matrix, either copy or mutate
                if (row < stdDevMatrixNominal.NoRows)
                {
                    if (GetsMutated(rnd, chance))
                        newStdDevMatrixNominal[row, 0]
                            = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                    else
                        newStdDevMatrixNominal[row, 0] = stdDevMatrixNominal[row, 0];
                    if (GetsMutated(rnd, chance))
                        newMeanMatrixNominal[row, 0]
                            = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                    else
                        newMeanMatrixNominal[row, 0] = meanMatrixNominal[row, 0];
                }
                //beyond the original matrix, value has to be a new random number
                else
                {
                    newStdDevMatrixNominal[row, 0] = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                    newMeanMatrixNominal[row, 0] = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                }
            }

            stdDevMatrixNominal = newStdDevMatrixNominal;
            meanMatrixNominal = newMeanMatrixNominal;

            #endregion

            //discrete mean&stdDev
            stdDevMatrixDiscrete = MutateMatrix(stdDevMatrixDiscrete, rnd, chance,
                0, GeneConstants.maxDiscrete);
            stdDevMatrixDiscrete = AdjustMatrixSize(stdDevMatrixDiscrete, NumDiscreteAttribs, 1,
                0, GeneConstants.maxDiscrete, rnd);
            meanMatrixDiscrete = MutateMatrix(meanMatrixDiscrete, rnd, chance,
                GeneConstants.minDiscrete, GeneConstants.maxDiscrete);
            meanMatrixDiscrete = AdjustMatrixSize(meanMatrixDiscrete, NumDiscreteAttribs, 1,
                GeneConstants.minDiscrete, GeneConstants.maxDiscrete, rnd);

            //continuous mean&stdDev
            stdDevMatrixContinuous = MutateMatrix(stdDevMatrixContinuous, rnd, chance,
                0, GeneConstants.maxContinuous);
            stdDevMatrixContinuous = AdjustMatrixSize(stdDevMatrixContinuous, NumContinuousAttribs, 1,
                0, GeneConstants.maxContinuous, rnd);
            meanMatrixContinuous = MutateMatrix(meanMatrixContinuous, rnd, chance,
                GeneConstants.minContinuous, GeneConstants.maxContinuous);
            meanMatrixContinuous = AdjustMatrixSize(meanMatrixContinuous, NumContinuousAttribs, 1,
                GeneConstants.minContinuous, GeneConstants.maxContinuous, rnd); 
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
        /// Produces offspring with another <see cref="GeneSet"/>
        /// </summary>
        /// <param name="other">The other parent</param>
        /// <returns>List of children</returns>
        public List<GeneSet> Breed(GeneSet other)
        {
            //init
            //currently 2 parents produce 2 offspring, but this can be changed if needed
            List<GeneSet> ret = new List<GeneSet>(2);
            GeneSet child1 = new GeneSet(false);
            GeneSet child2 = new GeneSet(false);
            ret.Add(child1);
            ret.Add(child2);
            Random rnd = new Random();

            //pass simple values on to children randomly
            if (rnd.NextDouble() > 0.5)
            {
                child1.dataSetSize = this.dataSetSize;
                child2.dataSetSize = other.dataSetSize;
            }
            else
            {
                child2.dataSetSize = this.dataSetSize;
                child1.dataSetSize = other.dataSetSize;
            }

            if (rnd.NextDouble() > 0.5)
            {
                child1.numAttribs = this.numAttribs;
                child2.numAttribs = other.numAttribs;
            }
            else
            {
                child2.numAttribs = this.numAttribs;
                child1.numAttribs = other.numAttribs;
            }

            //the class attribute is inherited as a whole
            if (rnd.NextDouble() > 0.5)
            {
                child1.numClasses = this.numClasses;
                child1.meanClass = this.meanClass;
                child1.stdDevClass = this.stdDevClass;
                child2.numClasses = other.numClasses;
                child2.meanClass = other.meanClass;
                child2.stdDevClass = other.stdDevClass;
            }
            else
            {
                child2.numClasses = this.numClasses;
                child2.meanClass = this.meanClass;
                child2.stdDevClass = this.stdDevClass;
                child1.numClasses = other.numClasses;
                child1.meanClass = other.meanClass;
                child1.stdDevClass = other.stdDevClass;
            }

            if (rnd.NextDouble() > 0.5)
            {
                child1.nominalAttribRatio = this.nominalAttribRatio;
                child2.nominalAttribRatio = other.nominalAttribRatio;
            }
            else
            {
                child2.nominalAttribRatio = this.nominalAttribRatio;
                child1.nominalAttribRatio = other.nominalAttribRatio;
            }

            if (rnd.NextDouble() > 0.5)
            {
                child1.discreteAttribRatio = this.discreteAttribRatio;
                child2.discreteAttribRatio = other.discreteAttribRatio;
            }
            else
            {
                child2.discreteAttribRatio = this.discreteAttribRatio;
                child1.discreteAttribRatio = other.discreteAttribRatio;
            }

            if (rnd.NextDouble() > 0.5)
            {
                child1.missingValueRatio = this.missingValueRatio;
                child2.missingValueRatio = other.missingValueRatio;
            }
            else
            {
                child2.missingValueRatio = this.missingValueRatio;
                child1.missingValueRatio = other.missingValueRatio;
            }

            if (rnd.NextDouble() > 0.5)
            {
                child1.irrelevantAttribRatio = this.irrelevantAttribRatio;
                child2.irrelevantAttribRatio = other.irrelevantAttribRatio;
            }
            else
            {
                child2.irrelevantAttribRatio = this.irrelevantAttribRatio;
                child1.irrelevantAttribRatio = other.irrelevantAttribRatio;
            }

            child1.InitMatrices();
            child2.InitMatrices();

            //create combined vectors of attribute parametres
            Matrix meanNomCombined = CombineVectors(this.meanMatrixNominal, other.meanMatrixNominal);
            Matrix meanDisCombined = CombineVectors(this.meanMatrixDiscrete, other.meanMatrixDiscrete);
            Matrix meanConCombined = CombineVectors(this.meanMatrixContinuous, other.meanMatrixContinuous);
            Matrix sdNomCombined = CombineVectors(this.stdDevMatrixNominal, other.stdDevMatrixNominal);
            Matrix sdDisCombined = CombineVectors(this.stdDevMatrixDiscrete, other.stdDevMatrixDiscrete);
            Matrix sdConCombined = CombineVectors(this.stdDevMatrixContinuous, other.stdDevMatrixContinuous);
            Matrix nomClassCombined = CombineVectors(this.nominalClassesMatrix, other.nominalClassesMatrix);

            //TODO create combined correlationMatrix

            #region pass attributes on to children randomly
            //all parametres of an attribute are passed to the same child
            
            int indexCombined;
            int indexChild;
            
            //Nominal attribs
            List<int> takenAttribsNom = new List<int>();
            //Child1 gets the right to select attribs randomly
            indexChild = 0;
            while (indexChild < child1.NumNominalAttribs)
            {
                indexCombined = RandomInt(rnd, 0, meanNomCombined.NoRows - 1);
                if (!takenAttribsNom.Contains(indexCombined))
                {
                    takenAttribsNom.Add(indexCombined);
                    child1.meanMatrixNominal[indexChild, 0] = meanNomCombined[indexCombined, 0];
                    child1.stdDevMatrixNominal[indexChild, 0] = sdNomCombined[indexCombined, 0];
                    child1.nominalClassesMatrix[indexChild, 0] = nomClassCombined[indexCombined, 0];
                    indexChild++;
                }
            }
            //child2 takes the rest
            indexChild = 0;
            for(indexCombined = 0; indexCombined < child2.NumNominalAttribs; indexCombined++)
                if (!takenAttribsNom.Contains(indexCombined))
                {
                    child2.meanMatrixNominal[indexChild, 0] = meanNomCombined[indexCombined, 0];
                    child2.stdDevMatrixNominal[indexChild, 0] = sdNomCombined[indexCombined, 0];
                    child2.nominalClassesMatrix[indexChild, 0] = nomClassCombined[indexCombined, 0];
                    indexChild++;
                }
            
            //Discrete attribs
            List<int> takenAttribsDis = new List<int>();
            //Child1 gets the right to select attribs randomly
            indexChild = 0;
            while (indexChild < child1.NumDiscreteAttribs)
            {
                indexCombined = RandomInt(rnd, 0, meanDisCombined.NoRows - 1);
                if (!takenAttribsDis.Contains(indexCombined))
                {
                    takenAttribsDis.Add(indexCombined);
                    child1.meanMatrixDiscrete[indexChild, 0] = meanDisCombined[indexCombined, 0];
                    child1.stdDevMatrixDiscrete[indexChild, 0] = sdDisCombined[indexCombined, 0];
                    indexChild++;
                }
            }
            //child2 takes the rest
            indexChild = 0;
            for(indexCombined = 0; indexCombined < child2.NumDiscreteAttribs; indexCombined++)
                if (!takenAttribsDis.Contains(indexCombined))
                {
                    child2.meanMatrixDiscrete[indexChild, 0] = meanDisCombined[indexCombined, 0];
                    child2.stdDevMatrixDiscrete[indexChild, 0] = sdDisCombined[indexCombined, 0];
                    indexChild++;
                }

            //Continuous attribs
            List<int> takenAttribsCon = new List<int>();
            //Child1 gets the right to select attribs randomly
            indexChild = 0;
            while (indexChild < child1.NumContinuousAttribs)
            {
                indexCombined = RandomInt(rnd, 0, meanConCombined.NoRows - 1);
                if (!takenAttribsCon.Contains(indexCombined))
                {
                    takenAttribsCon.Add(indexCombined);
                    child1.meanMatrixContinuous[indexChild, 0] = meanConCombined[indexCombined, 0];
                    child1.stdDevMatrixContinuous[indexChild, 0] = sdConCombined[indexCombined, 0];
                    indexChild++;
                }
            }
            //child2 takes the rest
            indexChild = 0;
            for(indexCombined = 0; indexCombined < child2.NumNominalAttribs; indexCombined++)
                if (!takenAttribsNom.Contains(indexCombined))
                {
                    child2.meanMatrixContinuous[indexChild, 0] = meanConCombined[indexCombined, 0];
                    child2.stdDevMatrixContinuous[indexChild, 0] = sdConCombined[indexCombined, 0];
                    indexChild++;
                }

            #endregion

            return ret;
        }

        #region general-purpose matrix and vector operations

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

        /// <summary>
        /// Rounds the values of the target matrix
        /// </summary>
        /// <param name="target">The target matrix</param>
        /// <returns>The rounded matrix</returns>
        private Matrix RoundMatrixValues(Matrix target)
        {
            for (int row = 0; row < target.NoRows; row++)
                for (int col = 0; col < target.NoCols; col++)
                    target[row, col] = Math.Round(target[row, col]);

            return target;
        }

        /// <summary>
        /// Combines two column vectors into one
        /// </summary>
        /// <param name="v1">The 1st vector</param>
        /// <param name="v2">The 2nd vector</param>
        /// <returns>The combined vector</returns>
        private Matrix CombineVectors(Matrix v1, Matrix v2)
        {
            Matrix ret = new Matrix(v1.NoRows + v2.NoRows, 1);

            for (int row = 0; row < v1.NoRows; row++)
                ret[row, 0] = v1[row, 0];
            for (int row = 0; row < v2.NoRows; row++)
                ret[row + v1.NoRows, 0] = v2[row, 0];

            return ret;
        }

        #endregion

        #endregion
    }
}
