﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using weka.core;
using weka.core.converters;
using System.IO;
using System.Runtime.Serialization;

namespace GEM
{
    [Serializable()]
    /// <summary>
    /// A dataset
    /// </summary>
    public class DataSet //: ISerializable
    {
        #region fields & properties

        /// <summary>
        /// The gene set describing this data set
        /// </summary>
        private GeneSet geneSet;

        /// <summary>
        /// Data structure for Weka
        /// </summary>
        //[NonSerialized()]
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
                if (double.IsNaN(value))
                    throw new Exception("Trying to set a fitness value of NaN.");
                else if (double.IsInfinity(value))
                    throw new Exception("Trying to set a fitness value of infinity.");
                else
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

            InitData();
        }

        /// <summary>
        /// Initialises the data instances
        /// </summary>
        private void InitData()
        {
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
            //I did consider refactoring everything to work with
            //MaNet matrices instead of MatrixLibrary ones.
            //Would be better not to convert,
            //but the MaNet.Matrix is generally not so good as MatrixLibrary.Matrix
            //(It's not a good idea to use a jagged array to represent a matrix
            //which has to have rows of the same size.)

            if (geneSet.correlationMatrix.NoRows != geneSet.correlationMatrix.NoCols)
                throw new Exception("The correlation matrix needs to be a square matrix.");

            MatrixLibrary.Matrix ret
                = new MatrixLibrary.Matrix(geneSet.dataSetSize, geneSet.numAttribs);

            //Step 1: C -> R matrix by Cholesky decomposition
            //R is a lower triangular matrix
            MaNet.Matrix rMatrix = geneSet.CholeskyMatrix;

            //"Step 2: Create BVn variables" implicitly done by using RandomNormal()
            Random rnd = new Random();

            //Step 3: Create data columns
            MatrixLibrary.Matrix meanMatrix = geneSet.meanMatrix;
            MatrixLibrary.Matrix stdDevMatrix = geneSet.stdDevMatrix;
            double factor;
            bool done;
            //class, nominal, discrete attribs need to be rounded
            int roundThisManyAttribs
                = geneSet.NumNominalAttribs + geneSet.NumDiscreteAttribs + 1;
            double minOfCurrentAttrib;
            double maxOfCurrentAttrib;
            double meanOfCurrentAttrib;
            double stdDevOfCurrentAttrib;

            //each column is an attribute
            for (int colRet = 0; colRet < ret.NoCols; colRet++)
            {
                minOfCurrentAttrib = geneSet.MinOfAttrib(colRet);
                maxOfCurrentAttrib = geneSet.MaxOfAttrib(colRet);
                meanOfCurrentAttrib = meanMatrix[colRet, 0];
                if (meanOfCurrentAttrib < minOfCurrentAttrib
                    || meanOfCurrentAttrib > maxOfCurrentAttrib)
                    throw new Exception(
                        "The mean of attributes needs to be between the min and max.");

                stdDevOfCurrentAttrib = stdDevMatrix[colRet, 0];
                //each row is a sample
                for (int rowRet = 0; rowRet < ret.NoRows; rowRet++)
                {
                    done = false;

                    while (!done)
                    {
                        factor = 0;

                        //for (int rowR = 0; rowR < rMatrix.RowDimension; rowR++)
                        for (int colR = 0; colR <= colRet; colR++)
                        {
                            factor += RandomNormal(rnd) * rMatrix.Get(colRet, colR);
                        }

                        ret[rowRet, colRet]
                            = meanOfCurrentAttrib + stdDevOfCurrentAttrib * factor;

                        //round if necessary
                        if (colRet < roundThisManyAttribs)
                            ret[rowRet, colRet] = Math.Round(ret[rowRet, colRet]);

                        //only accept the new value if it's within range.
                        if (ret[rowRet, colRet] <= maxOfCurrentAttrib
                            && ret[rowRet, colRet] >= minOfCurrentAttrib)
                            done = true;
                    } //while
                } //for (int rowRet = 0; rowRet < ret.NoRows; rowRet++)
            } //for (int colRet = 0; colRet < ret.NoCols; colRet++)

            return ret;
        }

        /// <summary>
        /// Returns a random value from a normal distribution
        /// (mean=0, stddev = 1)
        /// </summary>
        /// <param name="rnd">The Random object to use</param>
        /// <returns>The normally distributed random value</returns>
        private double RandomNormal(Random rnd)
        {
            //code based on http://stackoverflow.com/questions/218060/random-gaussian-variables
            double u1 = rnd.NextDouble();
            double u2 = rnd.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
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

        /// <summary>
        /// Saves the data in an ARFF file.
        /// </summary>
        /// <param name="path">The path and name of the output file</param>
        public void SaveArff(string path)
        {
            if (!File.Exists(path))
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                ArffSaver saver = new ArffSaver();
                saver.setInstances(data);
                saver.setFile(new java.io.File(path));
                saver.writeBatch();
            }
        }

        #endregion //methods
    }
}
