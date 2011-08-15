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
    /// A learning algorithm
    /// </summary>
    public class Learner
    {
        #region fields & properties

        /// <summary>
        /// Timer for timing how much time is spent with this learner,
        /// switch on for debug if needed
        /// </summary>
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

            //Weka
            switch (toolType)
            {
                case LearnerType.NaiveBayes:
                    tool = new NaiveBayes();
                    break;
                case LearnerType.J48:
                    tool = new J48();
                    break;
                //later, when the task can be regression as well
                /*case LearnerType.SimpleLinearRegression:
                    tool = new SimpleLinearRegression();
                    break;
                case LearnerType.LinearRegression:
                    tool = new LinearRegression();
                    break;
                case LearnerType.MultilayerPerceptron:
                    tool = new MultilayerPerceptron();
                    break;*/
                case LearnerType.Logistic:
                    tool = new Logistic();
                    break;
                case LearnerType.SimpleLogistic:
                    tool = new SimpleLogistic();
                    break;
                case LearnerType.SMO:
                    tool = new SMO();
                    break;
                case LearnerType.NBTree:
                    tool = new NBTree();
                    break;
                case LearnerType.REPTree:
                    tool = new REPTree();
                    break;
                case LearnerType.Id3:
                    tool = new Id3();
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

            double ret = eval.fMeasure(0);

            //stopWatch.Stop();
            
            if (double.IsNaN(ret))
                return 0;
            else
                return ret;
        }

        #endregion
    }
}
