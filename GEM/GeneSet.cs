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
        public Guid    guid;

        /// <summary>
        /// total number of instances (both training and test)
        /// </summary>
        public int     dataSetSize;
        
        /// <summary>
        /// number of attributes (_including_ target attribute)
        /// </summary>
        public int     numAttribs;

        /// <summary>
        /// number of classes
        /// </summary>
        public int     numClasses;

        /// <summary>
        /// percentage of nominal attributes
        /// </summary>
        private double  nominalAttribRatio;

        /// <summary>
        /// percentage of discrete attributes
        /// </summary>
        private double  discreteAttribRatio;

        //This could be a vector, so each attrib can have a different ratio
        /// <summary>
        /// ratio of missing values
        /// </summary>
        public double  missingValueRatio;

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
        public Matrix nominalClassesMatrix;

        /// <summary>
        /// Correlation matrix among the attributes
        /// </summary>
        public Matrix correlationMatrix;

        /// <summary>
        /// True if this gene set has mutated in the last round
        /// </summary>
        private bool mutated = false;

        //properties

        public MaNet.Matrix CholeskyMatrix
        {
            get
            {
                return CholeskyOfCorrelations(correlationMatrix);
            }
        }

        /// <summary>
        /// True if this gene set has mutated in the last round
        /// </summary>
        public bool Mutated
        {
            get
            {
                return mutated;
            }
        }

        /// <summary>
        /// Ratio of continuous attributes
        /// </summary>
        private double ContinuousAttribRatio
        {
            get
            {
                return ((numAttribs - 1) / numAttribs) - nominalAttribRatio - discreteAttribRatio;
            }
        }

        //The behaviour (which one is a function of which)
        //of the ratios / counts might need to be swapped.
        //This way it is _theoretically_ possible that
        //the rounding of the attribs produces a mismatch in the counts
        //i.e. the nom+cont+discr don't add up to the total.
        
        /// <summary>
        /// number of nominal attributes
        /// </summary>
        public int NumNominalAttribs
        {
            get
            {
                return (int)Math.Round(numAttribs * nominalAttribRatio);
            }
            set
            {
                int cont = NumContinuousAttribs;
                int disc = NumDiscreteAttribs;
                numAttribs = 1 + cont + disc + value;
                nominalAttribRatio = ((double)value) / ((double)numAttribs);
                discreteAttribRatio = ((double)disc) / ((double)numAttribs);
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
            set
            {
                int cont = NumContinuousAttribs;
                int nom = NumNominalAttribs;
                numAttribs = 1 + cont + nom + value;
                discreteAttribRatio = ((double)value) / ((double)numAttribs);
                nominalAttribRatio = ((double)nom) / ((double)numAttribs);
            }
        }

        /// <summary>
        /// Number of continuous attributes
        /// </summary>
        public int NumContinuousAttribs
        {
            //the +-1 is the target attribute
            get
            {
                return numAttribs - NumNominalAttribs - NumDiscreteAttribs - 1;
            }
            set
            {
                int nom = NumNominalAttribs;
                int disc = NumDiscreteAttribs;
                numAttribs = 1 + nom + disc + value;
                nominalAttribRatio = ((double)nom) / ((double)numAttribs);
                discreteAttribRatio = ((double)disc) / ((double)numAttribs);
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
        public Matrix meanMatrix
        {
            get
            {
                Matrix ret = new Matrix(numAttribs, 1);

                ret[0, 0] = meanClass;

                //index starts from 1 because class is already done
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
        public Matrix stdDevMatrix
        {
            get
            {
                Matrix ret = new Matrix(numAttribs, 1);

                ret[0, 0] = stdDevClass;

                //index starts from 1 because class is already done
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
            guid = Guid.NewGuid();

            if (fillRandom)
            {
                Random rnd = new Random();

                dataSetSize = RandomInt(rnd, GeneConstants.minDSSize, GeneConstants.maxDSSize);
                numAttribs = RandomInt(rnd, GeneConstants.minNumAttribs, GeneConstants.maxNumAttribs);
                numClasses = RandomInt(rnd, GeneConstants.minNumClasses, GeneConstants.maxNumClasses);

                //without the explicit casts, the 1st part is 0 because it's an int.
                double maxNAR = ((double)numAttribs - 1) / (double)numAttribs;
                nominalAttribRatio = RandomDouble(rnd, 0, maxNAR);
                discreteAttribRatio = RandomDouble(rnd, 0, maxNAR - nominalAttribRatio);
                missingValueRatio = RandomDouble(
                    rnd, GeneConstants.minMissing, GeneConstants.maxMissing);

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
        public void InitMatrices()
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

            //class (not matrices) is already initialised by the time we get here

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
            correlationMatrix = RandomCorrelMatrix(rnd, numAttribs);
        } //public void RandomiseMatrices()

        /// <summary>
        /// Binary decision on whether a particular value gets mutated
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="chance">Mutation chance</param>
        /// <returns><c>true</c> if it does get mutated, <c>false</c> otherwise</returns>
        private bool GetsMutated(Random rnd, double chance)
        {
            if (rnd.NextDouble() < chance)
            {
                mutated = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Random int between min and max, _including_ both the bounds
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns>The random int</returns>
        private int RandomInt(Random rnd, int min, int max)
        {
            if (max >= min)
                return rnd.Next(min, max + 1);
            else
                return rnd.Next(max, min + 1);
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
        /// Random float between min and max
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns>The random float</returns>
        private float RandomFloat(Random rnd, double min, double max)
        {
            if (max >= min)
                return (float)((rnd.NextDouble() * (max - min)) + min);
            else
                return (float)((rnd.NextDouble() * (min - max)) + max);
        }

        /// <summary>
        /// Returns the minimum value of the given attribute
        /// </summary>
        /// <param name="index">Index of the attribute</param>
        /// <returns>The minimum value of the given attribute</returns>
        public double MinOfAttrib(int index)
        {
            //class or nominal
            if (index < NumNominalAttribs + 1)
                return 0;
            //discrete
            else if (index < NumNominalAttribs + NumDiscreteAttribs + 1)
                return GeneConstants.minDiscrete;
            //continuous
            else
                return GeneConstants.minContinuous;
        }

        /// <summary>
        /// Returns the maximum value of the given attribute
        /// </summary>
        /// <param name="index">Index of the attribute</param>
        /// <returns>The maximum value of the given attribute</returns>
        public double MaxOfAttrib(int index)
        {
            //class
            if (0 == index)
                return numClasses - 1;
            //nominal
            else if (index < NumNominalAttribs + 1)
                return nominalClassesMatrix[index - 1, 0] - 1;
            //discrete
            else if (index < NumNominalAttribs + NumDiscreteAttribs + 1)
                return GeneConstants.maxDiscrete;
            //continuous
            else
                return GeneConstants.maxContinuous;
        }

        #region Inheritance operators

        /// <summary>
        /// Mutates the gene set according to the given mutation coefficient
        /// </summary>
        /// <param name="chance">The mutation coefficient</param>
        /// <returns>true if mutation happened</returns>
        public void Mutate(double chance)
        {
            mutated = false;
            Random rnd = new Random();

            int oldNumNom = NumNominalAttribs;
            int oldNumDisc = NumDiscreteAttribs;
            int oldNumCont = NumContinuousAttribs;

            //dataSetSize
            if (GetsMutated(rnd, chance))
            {
                dataSetSize = RandomInt(rnd, GeneConstants.minDSSize, GeneConstants.maxDSSize);
            }
            //numAttribs
            if (GetsMutated(rnd, chance))
            {
                numAttribs = RandomInt(rnd, GeneConstants.minNumAttribs, GeneConstants.maxNumAttribs);
            }
            //numClasses
            if (GetsMutated(rnd, chance))
            {
                numClasses = RandomInt(rnd, GeneConstants.minNumClasses, GeneConstants.maxNumClasses);
            }
            //nominalAttribRatio
            if (GetsMutated(rnd, chance))
            {
                nominalAttribRatio = rnd.NextDouble();
            }
            //discreteAttribRatio
            if (GetsMutated(rnd, chance))
            {
                double maxDAR = ((numAttribs - 1) / numAttribs) - nominalAttribRatio;
                discreteAttribRatio = RandomDouble(rnd, 0, maxDAR);
            }
            //missingValueRatio
            if (GetsMutated(rnd, chance))
            {
                missingValueRatio = RandomDouble(
                    rnd, GeneConstants.minMissing, GeneConstants.maxMissing);
            }
            //meanClass
            if (GetsMutated(rnd, chance))
            {
                meanClass = RandomDouble(rnd, 0, numClasses - 1);
            }
            //stdDevClass
            if (GetsMutated(rnd, chance))
            {
                stdDevClass = RandomDouble(rnd, 0, numClasses - 1);
            }

            //The matrices are mutated first and then adjusted
            //to avoid replacing new random values with even newer random values

            //correlationMatrix
            //mutate
            correlationMatrix = MutateCorrelMatrix(correlationMatrix, rnd, chance);
            //resize
            if (correlationMatrix.NoRows < numAttribs)
            {
                Matrix newPart = RandomCorrelMatrix(rnd, numAttribs - correlationMatrix.NoRows);
                correlationMatrix = CombineCorrelMatrices(correlationMatrix, newPart, rnd);
            }
            else if (correlationMatrix.NoRows > numAttribs)
            {
                Matrix newC = new Matrix(Matrix.Identity(numAttribs));

                for (int x = 0; x < numAttribs; x++)
                    for (int y = 0; y < x; y++)
                    {
                        newC[x, y] = correlationMatrix[x, y];
                        newC[y, x] = correlationMatrix[x, y];
                    }
            }

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
                    {
                        newStdDevMatrixNominal[row, 0]
                            = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                    }
                    else
                        newStdDevMatrixNominal[row, 0] = stdDevMatrixNominal[row, 0];

                    if (GetsMutated(rnd, chance)
                        //it might be that the old value is above the current maximum
                        || meanMatrixNominal[row, 0] > nominalClassesMatrix[row, 0] - 1)
                    {
                        newMeanMatrixNominal[row, 0]
                            = RandomDouble(rnd, 0, nominalClassesMatrix[row, 0] - 1);
                    }
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
        } //public bool Mutate(double mutationCoefficient)

        /// <summary>
        /// Produces offspring with another <see cref="GeneSet"/>
        /// </summary>
        /// <param name="other">The other parent</param>
        /// <returns>List of children</returns>
        public List<GeneSet> Breed(GeneSet other)
        {
            //currently 2 parents produce 2 offspring, but this can be changed if needed
            //"inherit entire attributes" i.e. not the ratios, but the matrix elements
            
            //init
            List<GeneSet> ret = new List<GeneSet>(2);
            GeneSet child1 = new GeneSet(false);
            GeneSet child2 = new GeneSet(false);
            ret.Add(child1);
            ret.Add(child2);
            Random rnd = new Random();

            //pass simple values on to children randomly
            //dataSetSize is inherited from one of the parents
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

            //NumNominalAttribs is inherited from one of the parents
            if (rnd.NextDouble() > 0.5)
            {
                child1.NumNominalAttribs = this.NumNominalAttribs;
                child2.NumNominalAttribs = other.NumNominalAttribs;
            }
            else
            {
                child2.NumNominalAttribs = this.NumNominalAttribs;
                child1.NumNominalAttribs = other.NumNominalAttribs;
            }
            //NumDiscreteAttribs is inherited from one of the parents
            if (rnd.NextDouble() > 0.5)
            {
                child1.NumDiscreteAttribs = this.NumDiscreteAttribs;
                child2.NumDiscreteAttribs = other.NumDiscreteAttribs;
            }
            else
            {
                child2.NumDiscreteAttribs = this.NumDiscreteAttribs;
                child1.NumDiscreteAttribs = other.NumDiscreteAttribs;
            }
            //NumContinuousAttribs is inherited from one of the parents
            if (rnd.NextDouble() > 0.5)
            {
                child1.NumContinuousAttribs = this.NumContinuousAttribs;
                child2.NumContinuousAttribs = other.NumContinuousAttribs;
            }
            else
            {
                child2.NumContinuousAttribs = this.NumContinuousAttribs;
                child1.NumContinuousAttribs = other.NumContinuousAttribs;
            }

            //helper lists to know which attrib of the children is which parent's which attrib
            List<GeneSet> child1AttribParents = new List<GeneSet>(child1.numAttribs);
            List<int> child1AttribIndices = new List<int>(child1.numAttribs);
            List<GeneSet> child2AttribParents = new List<GeneSet>(child2.numAttribs);
            List<int> child2AttribIndices = new List<int>(child2.numAttribs);
            //the class attribute is inherited as a whole
            if (rnd.NextDouble() > 0.5)
            {
                child1.numClasses = this.numClasses;
                child1.meanClass = this.meanClass;
                child1.stdDevClass = this.stdDevClass;
                child1AttribParents.Add(this);
                child2AttribParents.Add(other);
                child2.numClasses = other.numClasses;
                child2.meanClass = other.meanClass;
                child2.stdDevClass = other.stdDevClass;
            }
            else
            {
                child2.numClasses = this.numClasses;
                child2.meanClass = this.meanClass;
                child2.stdDevClass = this.stdDevClass;
                child2AttribParents.Add(this);
                child1AttribParents.Add(other);
                child1.numClasses = other.numClasses;
                child1.meanClass = other.meanClass;
                child1.stdDevClass = other.stdDevClass;
            }
            child1AttribIndices.Add(0);
            child2AttribIndices.Add(0);
            //at this point both children know where their classes (and only those) came from

            //missingValueRatio
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

            //create matrices of appropriate sizes
            child1.InitMatrices();
            child2.InitMatrices();

            //create combined vectors of attribute parametres
            Matrix meanNomCombined
                = CombineVectors(this.meanMatrixNominal, other.meanMatrixNominal);
            Matrix meanDisCombined
                = CombineVectors(this.meanMatrixDiscrete, other.meanMatrixDiscrete);
            Matrix meanConCombined
                = CombineVectors(this.meanMatrixContinuous, other.meanMatrixContinuous);
            Matrix sdNomCombined
                = CombineVectors(this.stdDevMatrixNominal, other.stdDevMatrixNominal);
            Matrix sdDisCombined
                = CombineVectors(this.stdDevMatrixDiscrete, other.stdDevMatrixDiscrete);
            Matrix sdConCombined
                = CombineVectors(this.stdDevMatrixContinuous, other.stdDevMatrixContinuous);
            Matrix nomClassCombined
                = CombineVectors(this.nominalClassesMatrix, other.nominalClassesMatrix);
            /*Matrix correlationCombined
                = CombineCorrelMatrices(rnd, this.correlationMatrix, other.correlationMatrix);*/
            
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

                    //store where the attrib came from
                    //attrib came from this parent
                    if (indexCombined < this.NumNominalAttribs)
                    {
                        child1AttribParents.Add(this);
                        child1AttribIndices.Add(1 + indexCombined);
                    }
                    //attrib came from other parent
                    else
                    {
                        child1AttribParents.Add(other);
                        child1AttribIndices.Add(1 + indexCombined - this.NumNominalAttribs);
                    }

                    indexChild++;
                }
            }
            //child2 takes the rest
            indexChild = 0;
            for (indexCombined = 0; indexCombined < meanNomCombined.NoRows; indexCombined++)
                if (!takenAttribsNom.Contains(indexCombined))
                {
                    child2.meanMatrixNominal[indexChild, 0] = meanNomCombined[indexCombined, 0];
                    child2.stdDevMatrixNominal[indexChild, 0] = sdNomCombined[indexCombined, 0];
                    child2.nominalClassesMatrix[indexChild, 0] = nomClassCombined[indexCombined, 0];

                    //store where the attrib came from
                    //attrib came from this parent
                    if (indexCombined < this.NumNominalAttribs)
                    {
                        child2AttribParents.Add(this);
                        child2AttribIndices.Add(1 + indexCombined);
                    }
                    //attrib came from other parent
                    else
                    {
                        child2AttribParents.Add(other);
                        child2AttribIndices.Add(1 + indexCombined - this.NumNominalAttribs);
                    }

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

                    //store where the attrib came from
                    //attrib came from this parent
                    if (indexCombined < this.NumDiscreteAttribs)
                    {
                        child1AttribParents.Add(this);
                        child1AttribIndices.Add(1 + this.NumNominalAttribs + indexCombined);
                    }
                    //attrib came from other parent
                    else
                    {
                        child1AttribParents.Add(other);
                        child1AttribIndices.Add(1 + other.NumNominalAttribs + indexCombined
                            - this.NumDiscreteAttribs);
                    }

                    indexChild++;
                }
            }
            //child2 takes the rest
            indexChild = 0;
            for (indexCombined = 0; indexCombined < meanDisCombined.NoRows; indexCombined++)
                if (!takenAttribsDis.Contains(indexCombined))
                {
                    child2.meanMatrixDiscrete[indexChild, 0] = meanDisCombined[indexCombined, 0];
                    child2.stdDevMatrixDiscrete[indexChild, 0] = sdDisCombined[indexCombined, 0];

                    //store where the attrib came from
                    //attrib came from this parent
                    if (indexCombined < this.NumDiscreteAttribs)
                    {
                        child2AttribParents.Add(this);
                        child2AttribIndices.Add(1 + this.NumNominalAttribs + indexCombined);
                    }
                    //attrib came from other parent
                    else
                    {
                        child2AttribParents.Add(other);
                        child2AttribIndices.Add(1 + other.NumNominalAttribs + indexCombined
                            - this.NumDiscreteAttribs);
                    }

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

                    //store where the attrib came from
                    //attrib came from this parent
                    if (indexCombined < this.NumContinuousAttribs)
                    {
                        child1AttribParents.Add(this);
                        child1AttribIndices.Add(1 + this.NumNominalAttribs + this.NumDiscreteAttribs
                            + indexCombined);
                    }
                    //attrib came from other parent
                    else
                    {
                        child1AttribParents.Add(other);
                        child1AttribIndices.Add(1 + other.NumNominalAttribs + other.NumDiscreteAttribs
                            + indexCombined - this.NumContinuousAttribs);
                    }

                    indexChild++;
                }
            }
            //child2 takes the rest
            indexChild = 0;
            for (indexCombined = 0; indexCombined < meanConCombined.NoRows; indexCombined++)
                if (!takenAttribsCon.Contains(indexCombined))
                {
                    child2.meanMatrixContinuous[indexChild, 0] = meanConCombined[indexCombined, 0];
                    child2.stdDevMatrixContinuous[indexChild, 0] = sdConCombined[indexCombined, 0];

                    //store where the attrib came from
                    //attrib came from this parent
                    if (indexCombined < this.NumContinuousAttribs)
                    {
                        child2AttribParents.Add(this);
                        child2AttribIndices.Add(1 + this.NumNominalAttribs + this.NumDiscreteAttribs
                            + indexCombined);
                    }
                    //attrib came from other parent
                    else
                    {
                        child2AttribParents.Add(other);
                        child2AttribIndices.Add(1 + other.NumNominalAttribs + other.NumDiscreteAttribs
                            + indexCombined - this.NumContinuousAttribs);
                    }

                    indexChild++;
                }

            //Correlation matrix for child1
            FillChildCorrelationMatrix(child1, child1AttribParents, child1AttribIndices, rnd);
            //Correlation matrix for child2
            FillChildCorrelationMatrix(child2, child2AttribParents, child2AttribIndices, rnd);

            #endregion //pass attributes on to children randomly

            return ret;
        } //public List<GeneSet> Breed(GeneSet other)

        #endregion Inheritance operators

        #region Correlation matrix manipulations

        /// <summary>
        /// Mutates a correlation matrix
        /// </summary>
        /// <param name="correlationMatrix">The correlation matrix</param>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="chance">The mutation chance</param>
        /// <returns>The mutated matrix</returns>
        private Matrix MutateCorrelMatrix(Matrix correlationMatrix, Random rnd, double chance)
        {
            Matrix ret = ManetToMatrixLib(CholeskyOfCorrelations(correlationMatrix));

            for (int i = 0; i < correlationMatrix.NoRows; i++)
                for (int j = 0; j < i + 1; j++)
                    if (GetsMutated(rnd, chance))
                        ret[i, j] = RandomDouble(rnd, -1, 1);

            return DeCholesky(ret);
        }

        /// <summary>
        /// Converts a MaNet Matrix to a MatrixLibrary Matrix
        /// </summary>
        /// <param name="matrix">MaNet matrix</param>
        /// <returns>MatrixLibrary Matrix</returns>
        private Matrix ManetToMatrixLib(MaNet.Matrix matrix)
        {
            int size = matrix.RowDimension;
            Matrix ret = new Matrix(size, size);

            for (int row = 0; row < size; row++)
                for (int column = 0; column < size; column++)
                    ret[row, column] = matrix.Get(row, column);

            return ret;
        } 

        /// <summary>
        /// Creates a random correlation matrix based on
        /// C = M * M^T
        /// </summary>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <param name="size">The size of the random correlation matrix</param>
        /// <returns>
        /// The random correlation matrix
        /// </returns>
        private Matrix RandomCorrelMatrix(Random rnd, int size)
        {
            Matrix m = new Matrix(size, size);
            for (int row = 0; row < size; row++)
                for (int column = 0; column < size; column++)
                    //initialise one half
                    if (column > row)
                        //correlationMatrix elements between -1 and 1
                        m[row, column] = RandomDouble(rnd, -1, 1);
                    //centerline is filled with 1's
                    else if (column == row)
                        m[row, column] = 1;
                    //other half copied
                    else
                        m[row, column] = m[column, row];

            return DeCholesky(m);
        } //private Matrix RandomCorrelMatrix(Random rnd)

        private Matrix DeCholesky(Matrix m)
        {
            NormaliseRows(m);
            RoundMatrixValues(m, 15);
            Matrix ret = m * Matrix.Transpose(m);
            return RoundMatrixValues(ret, 14);
        }

        /// <summary>
        /// Fills the correlation matrix of a child during breeding.
        /// </summary>
        /// <param name="child">The target child</param>
        /// <param name="attribParents">The parents for each attribute</param>
        /// <param name="attribIndices">The indices for each attribute (in the parent)</param>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        private void FillChildCorrelationMatrix(
            GeneSet child,
            List<GeneSet> attribParents,
            List<int> attribIndices,
            Random rnd)
        {
            //Step 1: fill in values from parents, into 2 compressed matrices
            List<int> motherIndices = new List<int>();
            List<int> fatherIndices = new List<int>();
            List<int> motherAttribsInFinal = new List<int>();
            List<int> fatherAttribsInFinal = new List<int>();
            GeneSet mother = null;
            GeneSet father = null;
            for (int i = 0; i < attribParents.Count; i++)
                if (attribParents[i] == attribParents[0])
                {
                    motherIndices.Add(attribIndices[i]);
                    mother = attribParents[i];
                    motherAttribsInFinal.Add(i);
                }
                else
                {
                    fatherIndices.Add(attribIndices[i]);
                    father = attribParents[i];
                    fatherAttribsInFinal.Add(i);
                }

            Matrix motherCorrel = null;
            Matrix fatherCorrel = null;
            if (mother != null)
            {
                motherCorrel = PartialCorrelationMatrix(mother.correlationMatrix, motherIndices);

                if (father != null)
                {
                    fatherCorrel = PartialCorrelationMatrix(father.correlationMatrix, fatherIndices);

                    //Step 3: Combine mother and father
                    child.correlationMatrix = CombineCorrelMatrices(motherCorrel, fatherCorrel, rnd);

                    //Step 4: Rearrange to original order of attribs
                    for (int a = 0; a < fatherAttribsInFinal.Count; a++)
                        motherAttribsInFinal.Add(fatherAttribsInFinal[a]);

                    child.correlationMatrix
                        = RearrangeCorrelMatrix(child.correlationMatrix,
                            motherAttribsInFinal);
                } //if (father != null)
                else
                    child.correlationMatrix = motherCorrel;
            } //if (mother != null)
            else
                child.correlationMatrix = new Matrix(Matrix.Identity(attribIndices.Count));
        }

        /// <summary>
        /// Combines two correlation matrices.
        /// </summary>
        /// <param name="motherCorrel">The mother correlation matrix</param>
        /// <param name="fatherCorrel">The father correlation matrix</param>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <returns>Combined matrix, mother first</returns>
        private Matrix CombineCorrelMatrices(Matrix motherCorrel, Matrix fatherCorrel, Random rnd)
        {
            int n = motherCorrel.NoRows + fatherCorrel.NoRows;
            Matrix ret = new Matrix(n, n);

            MaNet.Matrix motherR = CholeskyOfCorrelations(motherCorrel);
            MaNet.Matrix fatherR = CholeskyOfCorrelations(fatherCorrel);

            Matrix xVector = new Matrix(motherCorrel.NoRows + 1, 1);
            for (int v = 0; v < motherCorrel.NoRows + 1; v++)
                xVector[v, 0] = RandomDouble(rnd, -1, 1);
            NormaliseColumns(xVector);

            for (int row = 0; row < n; row++)
                for (int col = 0; col < n; col++)
                    //top/right half
                    if (col > row)
                        ret[row, col] = 0;
                    //bottom/left half + diagonal
                    else
                        //we have a value
                        if (row < motherCorrel.NoRows)
                            ret[row, col] = motherR.Get(row, col);
                        //use X vector
                        else if (motherCorrel.NoRows == row)
                            ret[row, col] = xVector[col, 0];
                        //new area or lower right corner
                        else
                            //new part
                            if (col < motherCorrel.NoRows + 1)
                                ret[row, col] =
                                    fatherR.Get(row - motherCorrel.NoRows - 1, 0)
                                        * xVector[col, 0];
                            //we have a value
                            else
                                ret[row, col] = fatherR.Get(row - motherCorrel.NoRows - 1,
                                    col - motherCorrel.NoRows - 1);

            return DeCholesky(ret);
        }

        /// <summary>
        /// Rearranges a correlation matrix to the original order of random variables
        /// </summary>
        /// <param name="parentCorrel">The starting correlation matrix</param>
        /// <param name="indices">The target indices of the first some attributes
        /// (does not have to cover all of them)</param>
        /// <returns>The rearranged correlation matrix</returns>
        private Matrix RearrangeCorrelMatrix(Matrix parentCorrel, List<int> indices)
        {
            Matrix ret = new Matrix(Matrix.Identity(parentCorrel.NoRows));

            if(parentCorrel.NoRows > indices.Count)
                //assign every unassigned place to the new variables
                for (int i = 0; i < parentCorrel.NoRows; i++)
                    if (!indices.Contains(i))
                        indices.Add(i);

            //now indices contains a valid index for all variables
            for (int row = 0; row < indices.Count; row++)
                //main diagonal already done and the matrix is symmetrical
                for (int col = row + 1; col < indices.Count; col++)
                {
                    ret[indices[row], indices[col]] = parentCorrel[row, col];
                    ret[indices[col], indices[row]] = parentCorrel[row, col];
                }

            return ret;
        }

        /// <summary>
        /// Extracts a partial correlation matrix from a bigger one
        /// </summary>
        /// <param name="parentCorrel">The parent correlation matrix</param>
        /// <param name="indices">The indices of the attributes to include</param>
        /// <returns>the partial correlation matrix</returns>
        private Matrix PartialCorrelationMatrix(Matrix parentCorrel, List<int> indices)
        {
            Matrix ret = new Matrix(indices.Count, indices.Count);
            //indices changes (gets appended) here below
            Matrix orderedCorrel = RearrangeCorrelMatrix(parentCorrel, indices);
            MaNet.Matrix r = CholeskyOfCorrelations(orderedCorrel); 

            for (int row = 0; row < ret.NoRows; row++)
                //the matrix is lower triangular
                for (int col = 0; col < ret.NoRows; col++)
                {
                    if (col < row + 1)
                        ret[row, col] = r.Get(indices[row], indices[col]);
                    else
                        ret[row, col] = 0;
                }

            return DeCholesky(ret);
        }
        
        /// <summary>
        /// Expands a correlation matrix.
        /// Simulates the adding of new, randomly correlated attribs
        /// </summary>
        /// <param name="c">The input correlation matrix.</param>
        /// <param name="newNumAttribs">The new number of attributes</param>
        /// <param name="rnd">The <see cref="Random"/> object to use</param>
        /// <returns>
        /// The expanded correlation matrix
        /// </returns>
        private Matrix ExpandCorrelMatrix(Matrix c, int newNumAttribs, Random rnd)
        {
            Matrix l2 = new Matrix(c.NoRows + newNumAttribs, c.NoRows + newNumAttribs);

            //Cholesky gives C -> L
            MaNet.Matrix l1 = CholeskyOfCorrelations(c);

            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            for (int row1 = 0; row1 < c.NoRows; row1++)
                for (int col1 = 0; col1 < c.NoRows; col1++)
                {
                    if (l1.Get(row1, col1) < min)
                        min =  l1.Get(row1, col1);
                    if (l1.Get(row1, col1) > max)
                        max = l1.Get(row1, col1);
                }

            //Add numbers
            for (int row = 0; row < l2.NoRows; row++)
                for (int col = 0; col < l2.NoRows; col++)
                    //existing values copied
                    if (row < c.NoRows && col < c.NoRows)
                        l2[row, col] = l1.Get(row, col);
                    //diagonal random > 0
                    else if (row == col)
                        l2[row, col] = RandomDouble(rnd, 0, max);
                    //bottom side random values
                    else if (row > col)
                        l2[row, col] = RandomDouble(rnd, min, max);
                    //right side: zeroes
                    else
                        l2[row, col] = 0;

            return DeCholesky(l2);
        }

        /// <summary>
        /// Returns the Cholesky decomposition of a correlation matrix
        /// </summary>
        /// <param name="correlMatrix">The correlation matrix</param>
        /// <returns>Cholesky decomposition of correlation matrix
        /// (lower triangular)</returns>
        private MaNet.Matrix CholeskyOfCorrelations(Matrix correlMatrix)
        {
            double[][] jaggedCorrel = new double[correlMatrix.NoRows][];
            for (int i = 0; i < correlMatrix.NoRows; i++)
            {
                jaggedCorrel[i] = new double[correlMatrix.NoCols];
                for (int j = 0; j < correlMatrix.NoCols; j++)
                    jaggedCorrel[i][j] = correlMatrix[i, j];
            }

            MaNet.CholeskyDecomposition cholesky
                = new MaNet.CholeskyDecomposition(
                    new MaNet.Matrix(jaggedCorrel,
                                    correlMatrix.NoRows,
                                    correlMatrix.NoCols));

            MaNet.Matrix rMatrix = null;
            if (cholesky.IsSPD())
                rMatrix = cholesky.getL();
            else
                throw new Exception(
                    "The correlation matrix needs to be symmetric and positive-definite.");

            return rMatrix;
        }

        /// <summary>
        /// Validates the correlation matrix
        /// </summary>
        /// <returns>true if the correlation matrix is valid, exception otherwise</returns>
        public bool ValidateCorrelMatrix()
        {
            return ValidateCorrelMatrix(correlationMatrix);
        }

        /// <summary>
        /// Validates a correlation matrix for
        /// - being SPD 
        /// - having elements in [-1, 1]
        /// - diagonal values = 1
        /// </summary>
        /// <param name="correls">The correlation matrix to validate</param>
        /// <returns>true if the correlation matrix is valid, exception otherwise</returns>
        private bool ValidateCorrelMatrix(Matrix correls)
        {
            bool ret = true;
            //SPD
            try
            {
                CholeskyOfCorrelations(correls);
            }
            catch
            {
                ret = false;
            }
            
            //values in [-1, 1], Main diagonal all 1's
            if (ret)
                for (int row = 0; row < correls.NoRows; row++)
                    if (ret)
                        for (int col = 0; col < row + 1; col++)
                            if ((col == row && correls[row, col] != 1) ||
                                (col != row &&
                                (correls[row, col] < -1 || correls[row, col] > 1)))
                            {
                                ret = false;
                                break;
                            }

            if (!ret)
                throw new Exception("The correlation matrix is invalid.");

            return ret;
        }

        #endregion //Correlation Matrix manipulations

        #region General-purpose matrix and vector operations

        /// <summary>
        /// Mutates the values in the target matrix.
        /// Do not use for correlation matrices!
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
        /// Do not use on correlation matrices!
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
                    //there is potentially a value to be copied
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
            return RoundMatrixValues(target, 0);
        }

        /// <summary>
        /// Rounds the values of the target matrix
        /// </summary>
        /// <param name="target">The target matrix</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>
        /// The rounded matrix
        /// </returns>
        private Matrix RoundMatrixValues(Matrix target, int decimalPlaces)
        {
            for (int row = 0; row < target.NoRows; row++)
                for (int col = 0; col < target.NoCols; col++)
                    target[row, col] = Math.Round(target[row, col], decimalPlaces);

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

        /// <summary>
        /// Normalises the columns of a matrix, meaning:
        /// divide all elements by the 2-norm of its column
        /// </summary>
        /// <param name="mtrx">The matrix to normalise</param>
        private void NormaliseColumns(Matrix mtrx)
        {
            double norm;

            for (int col = 0; col < mtrx.NoCols; col++)
            {
                norm = 0;
                for (int row1 = 0; row1 < mtrx.NoRows; row1++)
                    norm += mtrx[row1, col] * mtrx[row1, col];
                norm = Math.Sqrt(norm);

                for (int row2 = 0; row2 < mtrx.NoRows; row2++)
                    mtrx[row2, col] = mtrx[row2, col] / norm;
            }
        }

        /// <summary>
        /// Normalises the rows of a matrix, meaning:
        /// divide all elements by the 2-norm of its row
        /// </summary>
        /// <param name="mtrx">The matrix to normalise</param>
        private void NormaliseRows(Matrix mtrx)
        {
            double norm;

            for (int row = 0; row < mtrx.NoCols; row++)
            {
                norm = 0;
                for (int col1 = 0; col1 < mtrx.NoRows; col1++)
                    norm += mtrx[row, col1] * mtrx[row, col1];
                norm = Math.Sqrt(norm);

                for (int col2 = 0; col2 < mtrx.NoRows; col2++)
                    mtrx[row, col2] = mtrx[row, col2] / norm;
            }
        }

        /// <summary>
        /// Normalises the rows of a matrix, meaning:
        /// divide all elements by the 2-norm of its row
        /// </summary>
        /// <param name="mtrx">The matrix to normalise</param>
        private void NormaliseRows(float[,] mtrx)
        {
            float norm;

            for (int row = 0; row < mtrx.GetLength(0); row++)
            {
                norm = 0;
                for (int col1 = 0; col1 < mtrx.GetLength(0); col1++)
                    norm += mtrx[row, col1] * mtrx[row, col1];
                norm = (float)Math.Sqrt(norm);

                for (int col2 = 0; col2 < mtrx.GetLength(0); col2++)
                    mtrx[row, col2] = mtrx[row, col2] / norm;
            }
        }

        #endregion //general-purpose matrix and vector operations

        #region ToString() override and co

        /// <summary>
        /// Indents the multiline string.
        /// </summary>
        /// <param name="str">The tring to indent</param>
        /// <param name="indentString">The string to put in front of every row of str.</param>
        /// <returns>The indented string</returns>
        private string IndentMultilineString(string str, string indentString)
        {
            string[] strs = str.Split(Environment.NewLine.ToCharArray());
            for (int i = 0; i < strs.GetLength(0); i++)
                strs[i] = indentString + strs[i];
            return string.Join(Environment.NewLine, strs);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string leadingSpaces = new String(' ', GEMLogLayout.padding);

            return "guid: " + guid.ToString() + Environment.NewLine
                + leadingSpaces + "dataSetSize: " + dataSetSize.ToString() + Environment.NewLine
                + leadingSpaces + "numAttribs: " + numAttribs.ToString() + Environment.NewLine
                + leadingSpaces + "numClasses: " + numClasses.ToString() + Environment.NewLine
                + leadingSpaces + "nominalAttribRatio: " + nominalAttribRatio.ToString() + Environment.NewLine
                + leadingSpaces + "discreteAttribRatio: " + discreteAttribRatio.ToString() + Environment.NewLine
                + leadingSpaces + "missingValueRatio: " + missingValueRatio.ToString() + Environment.NewLine
                + leadingSpaces + "meanClass: " + meanClass.ToString() + Environment.NewLine
                + leadingSpaces + "stdDevClass: " + stdDevClass.ToString() + Environment.NewLine
                + leadingSpaces + "meanMatrix: " + Environment.NewLine
                + IndentMultilineString(meanMatrix.ToString(), leadingSpaces) + Environment.NewLine
                + leadingSpaces + "stdDevMatrix: " + Environment.NewLine
                + IndentMultilineString(stdDevMatrix.ToString(), leadingSpaces) + Environment.NewLine
                + leadingSpaces + "nominalClassesMatrix: " + Environment.NewLine
                + IndentMultilineString(nominalClassesMatrix.ToString(), leadingSpaces) + Environment.NewLine
                + leadingSpaces + "correlationMatrix: " + Environment.NewLine
                + IndentMultilineString(correlationMatrix.ToString(), leadingSpaces) + Environment.NewLine
                ;
        }

        #endregion //ToString() override and co

        #endregion //methods
    }
}
