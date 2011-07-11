using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using weka.core;
using weka.core.converters;

namespace GEM
{
    [Serializable()]
    /// <summary>
    /// A dataset
    /// </summary>
    public class DataSet
    {
        #region fields & properties

        /// <summary>
        /// The gene set describing this data set
        /// </summary>
        private GeneSet geneSet;

        /// <summary>
        /// Data structure for Weka
        /// </summary>
        public Instances data;

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
            this.geneSet = geneSet;

            // 1. set up attributes
            FastVector attribs = SetupAttribs();

            // 1.5. Generate data into matrix
            MatrixLibrary.Matrix dataMatrix = GenerateData();

            // 2. create Instances object
            data = new Instances("GEM", attribs, 0);
            // 2.5 specify attrib1 to be the class
            data.setClassIndex(0);

            // 3. fill with data
            CreateInstances(dataMatrix);
        } //memory taken by dataMatrix freed here 

        /// <summary>
        /// Generates the dataset from the statistical parametres (genes)
        /// (p45 in Anton's thesis)
        /// </summary>
        /// <returns>The dataset in a matrix</returns>
        private MatrixLibrary.Matrix GenerateData()
        {
            //TODO Do something about minimum / maximum violations.
            //TODO Exactly this: remove BVMatrix and do the generation in-place,
            //in a while loop until value is within range.
            //I did consider refactoring everything to work with
            //MaNet matrices instead of MatrixLibrary ones.
            //Would be better not to convert,
            //but the MaNet.Matrix is generally not so good as MatrixLibrary.Matrix

            if (geneSet.correlationMatrix.NoRows != geneSet.correlationMatrix.NoCols)
                throw new Exception("The correlation matrix needs to be a square matrix.");

            MatrixLibrary.Matrix ret
                = new MatrixLibrary.Matrix(geneSet.dataSetSize, geneSet.numAttribs);

            //Step 1: C -> R matrix by Cholesky decomposition
            //R is a lower triangular matrix

            double[][] jaggedCorrel = new double[geneSet.correlationMatrix.NoRows][];
            for (int i = 0; i < geneSet.correlationMatrix.NoRows; i++)
            {
                jaggedCorrel[i] = new double[geneSet.correlationMatrix.NoCols];
                for (int j = 0; j < geneSet.correlationMatrix.NoCols; j++)
                    jaggedCorrel[i][j] = geneSet.correlationMatrix[i, j];
            }

            MaNet.CholeskyDecomposition cholesky
                = new MaNet.CholeskyDecomposition(
                    new MaNet.Matrix(jaggedCorrel,
                                    geneSet.correlationMatrix.NoRows,
                                    geneSet.correlationMatrix.NoCols));

            MaNet.Matrix rMatrix = cholesky.getL();

            //Step 2: Create BVn variables
            //code based on http://stackoverflow.com/questions/218060/random-gaussian-variables
            Random rnd = new Random();
            //this will contain the BVn random variables. The nth column is the nth variable
            MatrixLibrary.Matrix bvMatrix
                = new MatrixLibrary.Matrix(geneSet.dataSetSize, geneSet.numAttribs);

            double u1;
            double u2;

            //each column is one variable
            for (int col = 0; col < bvMatrix.NoCols; col++)
                //each row is one value
                for (int row = 0; row < bvMatrix.NoRows; row++)
                {
                    u1 = rnd.NextDouble();
                    u2 = rnd.NextDouble();

                    bvMatrix[row, col] = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                 Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                }

            //Step 3: Create data columns
            MatrixLibrary.Matrix meanMatrix = geneSet.meanMatrix;
            MatrixLibrary.Matrix stdDevMatrix = geneSet.stdDevMatrix;
            double factor;

            //each column is an attribute
            for (int colRet = 0; colRet < ret.NoCols; colRet++)
            {
                //each row is a sample
                for (int rowRet = 0; rowRet < ret.NoRows; rowRet++)
                {
                    factor = 0;

                    for (int rowR = 0; rowR < rMatrix.RowDimension; rowR++)
                        for (int colR = 0; colR < rMatrix.ColumnDimension; colR++)
                            factor += bvMatrix[rowRet, colR] * rMatrix.Get(rowR, colR);

                    ret[rowRet, colRet]
                        = meanMatrix[colRet, 0] + stdDevMatrix[colRet, 0] * factor;
                }
            }

            return ret;
        }

        /// <summary>
        /// Fills the attributes with values
        /// </summary>
        /// <param name="dataMatrix">The data matrix made earlier</param>
        private void CreateInstances(MatrixLibrary.Matrix dataMatrix)
        {
            Random rnd = new Random();

            for (int row = 0; row < geneSet.dataSetSize; row++)
            {
                double[] vals = new double[geneSet.numAttribs];

                for (int col = 0; col < geneSet.numAttribs; col++)
                    if (rnd.NextDouble() < geneSet.missingValueRatio)
                        vals[col] = Instance.missingValue();
                    else
                        vals[col] = dataMatrix[row, col];

                data.add(new Instance(1, vals));
            }
        }

        /// <summary>
        /// Sets up the attributes of the data set for use
        /// </summary>
        private FastVector SetupAttribs()
        {
            FastVector attribs = new FastVector();

            FastVector attVals = new FastVector();
            for (int j = 0; j < geneSet.numClasses; j++)
                attVals.addElement(j.ToString());
            attribs.addElement(new weka.core.Attribute("att0", attVals));

            for (int i = 1; i < geneSet.numAttribs; i++)
                //nominal
                if (i < geneSet.NumNominalAttribs + 1)
                {
                    attVals = new FastVector();
                    for (int j = 0; j < geneSet.nominalClassesMatrix[i - 1, 0]; j++)
                        attVals.addElement("v" + j.ToString());
                    attribs.addElement(new weka.core.Attribute("att" + i.ToString(), attVals));
                }
                //discrete and continuous are both numberic
                else
                    attribs.addElement(new weka.core.Attribute("att" + i.ToString()));

            return attribs;
        }

        //constructor to load dataset from file
/*        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class
        /// NOT from an ARFF file, just the class without the data itself
        /// </summary>
        public DataSet()
        {
            throw new System.NotImplementedException();
        }*/

        #endregion
    }
}
