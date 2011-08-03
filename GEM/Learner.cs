using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using weka.core;
using weka.classifiers;
using weka.classifiers.bayes;
using weka.classifiers.trees;
using weka.classifiers.functions;
using System.Diagnostics;

namespace GEM
{
    /// <summary>
    /// Types of learners currently acceptable
    /// </summary>
    public enum LearnerType
    {
        NaiveBayes,
        J48,
        SimpleLinearRegression,
        LinearRegression,
        MultilayerPerceptron,
        Logistic,
        SimpleLogistic,
        SMO
    };

    /// <summary>
    /// A learning algorithm
    /// </summary>
    public class Learner
    {
        #region fields & properties

        //public Stopwatch stopWatch = new Stopwatch();
        
        /// <summary>
        /// Type of learner to use
        /// </summary>
        public LearnerType toolType;

        /// <summary>
        /// The learning algorithm
        /// </summary>
        private Classifier tool;

        /// <summary>
        /// Options to send to the classifier
        /// </summary>
        public string[] options;
        
        /*/// <summary>
        /// Ratio of test set / all data
        /// </summary>
        private double testSetRatio = 0.3;*/

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Learner"/> class.
        /// </summary>
        public Learner()
        {
            throw new Exception(
                "Please use the Learner(toolType, options) constructor instead.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Learner"/> class.
        /// </summary>
        /// <param name="toolType">Type of learner to use</param>
        /// <param name="options">Options to send to the classifier</param>
        public Learner(LearnerType toolType, string[] options)
        {
            this.toolType = toolType;
            this.options = options;
        }

        /// <summary>
        /// Learns the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Fitness of learning</returns>
        public double Learn(Instances data, int numCrossValids)
        {
            //stopWatch.Start();
            /*//split data into training / test set
            Instances train = new Instances(data, 0);
            Instances test = new Instances(data, 0);
            Random rnd = new Random();
            for (int i = 0; i < data.numInstances(); i++)
                if (rnd.NextDouble() > testSetRatio)
                    train.add(data.instance(i));
                else
                    test.add(data.instance(i));*/

            //Weka
            switch (toolType)
            {
                case LearnerType.NaiveBayes:
                    tool = new NaiveBayes();
                    break;
                case LearnerType.J48:
                    tool = new J48();
                    break;
                case LearnerType.SimpleLinearRegression:
                    tool = new SimpleLinearRegression();
                    break;
                case LearnerType.LinearRegression:
                    tool = new LinearRegression();
                    break;
                case LearnerType.MultilayerPerceptron:
                    tool = new MultilayerPerceptron();
                    break;
                case LearnerType.Logistic:
                    tool = new Logistic();
                    break;
                case LearnerType.SimpleLogistic:
                    tool = new SimpleLogistic();
                    break;
                case LearnerType.SMO:
                    tool = new SMO();
                    break;
                default:
                    throw new Exception("LearnerType invalid");
            }

            if (options != null)
                tool.setOptions(options);

            Evaluation eval = new Evaluation(data);
            try
            {
                eval.crossValidateModel(tool, data, numCrossValids, new java.util.Random());
            }
            catch
            {
                //stopWatch.Stop();
                return 0;
            }

            double ret = eval.pctCorrect();

            //stopWatch.Stop();
            
            if (double.IsNaN(ret))
                return 0;
            else
                return ret;
        }

        #endregion
    }
}
