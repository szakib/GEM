using System;

namespace GEM
{
    /// <summary>
    /// Types of learners currently acceptable
    /// </summary>
    public enum LearnerType
    {
        NaiveBayes,
        J48,
        //Only classifiers work as of now.
        //SimpleLinearRegression,
        //LinearRegression,
        //MultilayerPerceptron,
        Logistic,
        SimpleLogistic,
        SMO
    };
}
