using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GEM
{
    /// <summary>
    /// The main genetic algorithm engine
    /// </summary>
    public class GeneticAlgo
    {
        #region fields & properties

        private int                     populationSize;

        /// <summary>
        /// resume or start from scratch
        /// </summary>
        private bool                    resume;

        private Logger                  logger              = new Logger();

        /// <summary>
        /// flag signalling that Stop has been pressed on the UI
        /// </summary>
        private bool                    stop                = false;

        private MainForm                mainForm;
        private string                  savePath;
        private int                     experimentID;
        private int                     currentGeneration;

        /// <summary>
        /// between 0 and 100; 100 means very many mutations
        /// </summary>
        private int                     mutationSeverity;

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
        }

        /// <summary>
        /// Reads the settings from the config file
        /// </summary>
        private void ReadConfig()
        {
            populationSize = ConfigSettings.ReadInt("PopulationSize");
            resume = ConfigSettings.ReadBool("Resume");
            savePath = ConfigSettings.ReadString("SavePath");
            experimentID = ConfigSettings.ReadInt("ExperimentID");
            currentGeneration = ConfigSettings.ReadInt("CurrentGeneration");
            mutationSeverity = ConfigSettings.ReadInt("MutationSeverity");
            //TODO target learner, control group
        }

        /// <summary>
        /// Saves the settings to the config file
        /// </summary>
        private void SaveConfig()
        {
            ConfigSettings.WriteSetting("PopulationSize", populationSize.ToString());
            ConfigSettings.WriteSetting("Resume", resume.ToString());
            ConfigSettings.WriteSetting("SavePath", savePath);
            ConfigSettings.WriteSetting("ExperimentID", experimentID.ToString());
            ConfigSettings.WriteSetting("CurrentGeneration", currentGeneration.ToString());
            ConfigSettings.WriteSetting("MutationSeverity", mutationSeverity.ToString());
            //TODO target learner, control group
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
            //otherwise they get loaded
            if (resume)
                LoadPopulations(experimentID, currentGeneration);
            else
            {
                InitPopulations();
                experimentID++;
                currentGeneration = 0;
            }

            NextGeneration();
        }

        /// <summary>
        /// Initialises the populations
        /// </summary>
        private void InitPopulations()
        {
            goodPopulation = new List<Individual>();
            badPopulation = new List<Individual>();

            for (int i = 0; i < populationSize; i++)
            {
                //the 2 initial populations will not be the same, this is by design
                goodPopulation.Add(new Individual());
                badPopulation.Add(new Individual());
            }
        }

        /// <summary>
        /// Loads current populations from save file
        /// </summary>
        private void LoadPopulations()
        {
            LoadPopulations(experimentID, currentGeneration);
        }

        /// <summary>
        /// Loads a certain set of populations from save file
        /// </summary>
        /// <param name="expID">The experiment ID</param>
        /// <param name="generation">The generation</param>
        private void LoadPopulations(int expID, int generation)
        {
            string filename = "GEM_"
                                + expID.ToString()
                                + "_"
                                + generation.ToString()
                                + ".save";
            BinaryFormatter bFormatter = new BinaryFormatter();

            //Load good population
            string path = Path.Combine(savePath, filename + "_good");
            Stream stream = File.Open(path, FileMode.Open);
            goodPopulation = (List<Individual>)bFormatter.Deserialize(stream);
            stream.Close();

            //Load bad population
            path = Path.Combine(savePath, filename + "_bad");
            stream = File.Open(path, FileMode.Open);
            badPopulation = (List<Individual>)bFormatter.Deserialize(stream);
            stream.Close();
        }

        /// <summary>
        /// Saves current populations to save file
        /// </summary>
        private void SavePopulations()
        {
            string filename =   "GEM_"
                                + experimentID.ToString()
                                + "_"
                                + currentGeneration.ToString()
                                + ".save";
            BinaryFormatter bFormatter = new BinaryFormatter();
            
            //Save good population
            string path = Path.Combine(savePath, filename + "_good");
            Stream stream = File.Open(path, FileMode.Create);
            bFormatter.Serialize(stream, goodPopulation);
            stream.Close();

            //Save bad population
            path = Path.Combine(savePath, filename + "_bad");
            stream = File.Open(path, FileMode.Create);
            bFormatter.Serialize(stream, badPopulation);
            stream.Close();
            resume = true;
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
            AsynchProcessGen caller = new AsynchProcessGen(this.ProcessGeneration);

            caller.BeginInvoke(new AsyncCallback(NextGenCallBack), ""); 

            //some combo of ProcessGeneration() and NextGenCallBack() here

            //TODO remove :)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delegate for calling ProcessGeneration asynchronously
        /// </summary>
        private delegate void AsynchProcessGen();

        /// <summary>
        /// Processes one generation.
        /// This method should only be called asynchronously!
        /// </summary>
        private void ProcessGeneration()
        {
            //Calculate breeding chances according to fitness
            //Do interbreeding to get new populations

            //Mutate (or not) each individual
            double mutationCoefficient = mutationSeverity / 100;
            foreach (Individual i in goodPopulation)
                i.Genes.Mutate(mutationCoefficient);
            foreach (Individual j in badPopulation)
                j.Genes.Mutate(mutationCoefficient);
        }

        /// <summary>
        /// Callback function for ProcessGeneration.
        /// If stop was not pressed, starts a new generation.
        /// </summary>
        /// <param name="ar">The IAsyncResult response, unused</param>
        private void NextGenCallBack(IAsyncResult ar)
        {
            if (stop)
            {
                SavePopulations();
                SaveConfig();
                mainForm.StateLabel.Text = "Stopped, safe to exit.";
                mainForm.StateLabel.ForeColor = Color.Green;
            }
            else
                NextGeneration();
        }

        #endregion
    }
}
