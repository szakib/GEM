using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;

namespace GEM
{
    /// <summary>
    /// The main genetic algorithm engine
    /// </summary>
    public class GeneticAlgo
    {
        #region fields & properties

        private int                     populationSize;
        //resume or start from scratch
        private bool                    resume;
        private Logger                  logger              = new Logger();
        //flag signalling that Stop has been pressed on the UI
        private bool                    stop                = false;
        private MainForm                mainForm;

        //fields for properties below
        private Learner                 targetLearner;
        private List<Learner>           controlGroup;
        private List<Individual>        goodPopulation;
        private List<Individual>        badPopulation;
        
        /// <summary>
        /// Gets or sets the target learner
        /// </summary>
        /// <value>
        /// The target learner
        /// </value>
        public Learner TargetLearner
        {
            get
            {
                return targetLearner;
            }
            set
            {
                targetLearner = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the control group of learners.
        /// The target learner is compared against these.
        /// </summary>
        /// <value>
        /// The control group
        /// </value>
        public List<Learner> ControlGroup
        {
            get
            {
                return controlGroup;
            }
            set
            {
                controlGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the good population.
        /// This population contains the individuals which are better
        /// for the target learner than for the control group.
        /// </summary>
        /// <value>
        /// The good population
        /// </value>
        public List<Individual> GoodPopulation
        {
            get
            {
                return goodPopulation;
            }
            set
            {
                goodPopulation = value;
            }
        }

        /// <summary>
        /// Gets or sets the bad population.
        /// This population contains the individuals which are worse
        /// for the target learner than for the control group.
        /// </summary>
        /// <value>
        /// The bad population
        /// </value>
        public List<Individual> BadPopulation
        {
            get
            {
                return badPopulation;
            }
            set
            {
                badPopulation = value;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgo"/> class
        /// Do not use the default constructor!
        /// </summary>
        public GeneticAlgo()
        {
            throw new Exception("Please use the GeneticAlgo(MainForm main) constructor instead.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgo"/> class
        /// </summary>
        /// <param name="main">The main form of the UI</param>
        public GeneticAlgo(MainForm main)
        {
            mainForm = main;
            ReadConfig();
            //TODO init:
            //targetLearner
            //controlGroup
            //populations
        }

        /// <summary>
        /// Reads the settings from the config file
        /// </summary>
        private void ReadConfig()
        {
            populationSize = ConfigSettings.ReadInt("PopulationSize");
            resume = ConfigSettings.ReadBool("Resume");
            //TODO invent and read more params
        }

        /// <summary>
        /// Starts processing the GA
        /// </summary>
        public void Start()
        {
            stop = false;
            mainForm.StateLabel.Text = "Running";
            mainForm.StateLabel.ForeColor = Color.Green;

            //if this is the first run, the populations need init
            if (!resume)
                InitPopulations();

            NextGeneration();
        }

        /// <summary>
        /// Stops this instance (after waiting for the async calls to finish)
        /// by setting the stop flag
        /// </summary>
        public void Stop()
        {
            stop = true;
            mainForm.StateLabel.Text = "Stopping, please wait.";
            mainForm.StateLabel.ForeColor = Color.Red;
        }

        /// <summary>
        /// Starts the processing of the next generation asynchronously
        /// </summary>
        private void NextGeneration()
        {
            //start new generation by calling ProcessGeneration asynchronously
            //some combo of ProcessGeneration() and NextGenCallBack() here

            //TODO remove :)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes one generation.
        /// This method should only be called asynchronously!
        /// </summary>
        private void ProcessGeneration()
        {
            //TODO remove :)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Callback function for ProcessGeneration.
        /// If stop was not pressed, starts a new generation.
        /// </summary>
        private void NextGenCallBack()
        {
            if (stop)
            {
                mainForm.StateLabel.Text = "Stopped, safe to exit.";
                mainForm.StateLabel.ForeColor = Color.Green;
            }
            else
                NextGeneration();
        }

        private void InitPopulations()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
