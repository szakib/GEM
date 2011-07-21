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
        /// Number of Cross-validations to do per dataset
        /// </summary>
        private const int               numCrossValids      = 2;

        /// <summary>
        /// How big part of the population gets kept during the elitist selection
        /// </summary>
        private const double            eliteRatio          = 0.05;

        /// <summary>
        /// resume or start from scratch
        /// </summary>
        private bool                    resume;

        //private Logger                  logger              = new Logger();

        /// <summary>
        /// flag signalling that Stop has been pressed on the UI
        /// </summary>
        private bool                    stop                = false;

        private MainForm                mainForm;
        private string                  savePath;
        private int                     experimentID;
        private int                     currentGeneration;
        private double                  overallFitness      = 0;

        /// <summary>
        /// between 0 and 100; 100 means very many mutations
        /// </summary>
        private int                     mutationSeverity;

        //fields for properties below
        private Learner                 targetLearner;
        private List<Learner>           controlGroup = new List<Learner>();
        private List<Individual>        goodPopulation = new List<Individual>();
        private List<Individual>        badPopulation = new List<Individual>();

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
            targetLearner = new Learner(LearnerType.J48, null);
            controlGroup.Add(new Learner(LearnerType.NaiveBayes, null));
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
            if (0 > mutationSeverity || 100 < mutationSeverity)
                throw new Exception("MutationSeverity has to be between 0 and 100.");
            //target learner, control group could perhaps be in config
            //instead of in public GeneticAlgo(MainForm main) above
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
            //TODO target learner, control group, other things
        }

        /// <summary>
        /// Starts processing the GA
        /// </summary>
        public void Start()
        {
            stop = false;
            mainForm.StateLabel.Text = "Starting";
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

            mainForm.ExperimentIDLabel.Text = experimentID.ToString();
            mainForm.PopSizeLabel.Text = populationSize.ToString();
            SetProgressBarValue(0);

            NextGeneration();
        }

        /// <summary>
        /// Initialises the populations
        /// </summary>
        private void InitPopulations()
        {
            SetLabel2("Initialising populations");

            goodPopulation = new List<Individual>();
            badPopulation = new List<Individual>();

            for (int i = 0; i < populationSize; i++)
            {
                //the 2 initial populations will not be the same, this is by design
                goodPopulation.Add(new Individual());
                badPopulation.Add(new Individual());
            }

            SetLabel2("");
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
            SetLabel2("Loading populations");

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

            SetLabel2("");
        }

        /// <summary>
        /// Saves current populations to save file
        /// </summary>
        private void SavePopulations()
        {
            SetLabel2("Saving populations");

            string filename =   "GEM_"
                                + experimentID.ToString()
                                + "_"
                                + currentGeneration.ToString()
                                + ".save";
            BinaryFormatter bFormatter = new BinaryFormatter();
            //might want to use XmlSerializer instead
            
            //check if save dir exists, create it if needed
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

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

            SetLabel2("");
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
            mainForm.StateLabel.Text = "Running";
            mainForm.StateLabel.ForeColor = Color.Green;
            SetExperimentDetailLabels();

            //start new generation by calling ProcessGeneration asynchronously
            AsynchProcessGen caller = new AsynchProcessGen(this.ProcessGeneration);

            caller.BeginInvoke(new AsyncCallback(NextGenCallBack), ""); 
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
            CalculateFitness();

            SavePopulations();

            //Sort the populations by fitness (descending)
            goodPopulation.Sort(new IndividualComparer());
            badPopulation.Sort(new IndividualComparer());

            //Create the next generation, which has two groups in it:
            //1.: Elite from previous
            //2.: Random children of current generation
            List<Individual> newGoodPop = new List<Individual>();
            List<Individual> newBadPop = new List<Individual>();

            //Elite survives
            for (int i = 0; i < populationSize * eliteRatio; i++)
            {
                newBadPop.Add(badPopulation[i]);
                newGoodPop.Add(goodPopulation[i]);
            }

            //Breeding chance will depend on accumulated fitness
            //see http://en.wikipedia.org/wiki/Selection_%28genetic_algorithm%29
            double totalFitGood = AccumulateFitness(goodPopulation);
            double totalFitBad = AccumulateFitness(badPopulation);

            //Breeding to fill the rest of the places
            Random rnd = new Random();
            FillRestWithChildren(newBadPop, badPopulation, totalFitBad, rnd);
            FillRestWithChildren(newGoodPop, goodPopulation, totalFitGood, rnd);

            //Swap in the new populations
            goodPopulation = newGoodPop;
            badPopulation = newBadPop;

            //Mutate (or not) each individual
            double mutationCoefficient = mutationSeverity / 100;
            foreach (Individual i in goodPopulation)
                i.Mutate(mutationCoefficient);
            foreach (Individual j in badPopulation)
                j.Mutate(mutationCoefficient);
        } //non-surviving individuals of old populations get garbage collected here

        /// <summary>
        /// Fills the remaining places of the population with children
        /// </summary>
        /// <param name="toFill">New population to fill</param>
        /// <param name="fillFrom">Old population to choose parents from</param>
        /// <param name="totalAccFit">The total accumulated fitness of the parents</param>
        /// <param name="rnd">The Random object to use for selection</param>
        private void FillRestWithChildren(List<Individual> toFill,
            List<Individual> fillFrom, double totalAccFit, Random rnd)
        {
            SetLabel2("Calculating children");

            while (toFill.Count < populationSize)
            {
                SetProgressBarValue((int)Math.Round((double)(toFill.Count / populationSize) * 100));

                Individual parent1 = null;
                Individual parent2 = null;

                while (null == parent1)
                    parent1 = SelectParent(fillFrom, totalAccFit, rnd);
                //2nd condition makes sure the parents are different
                while (null == parent2 || parent1 == parent2)
                    parent2 = SelectParent(fillFrom, totalAccFit, rnd);

                List<GeneSet> childGenes = parent1.Genes.Breed(parent2.Genes);

                //make sure the population will end up with
                //precisely populationSize individuals
                while(childGenes.Count > 0 && toFill.Count < populationSize)
                {
                    toFill.Add(new Individual(childGenes[0]));
                    childGenes.RemoveAt(0);
                }
            }

            SetLabel2("");
        }

        /// <summary>
        /// Selects a prospective parent for breeding based on the accumulated fitness values
        /// </summary>
        /// <param name="fillFrom">Old population to choose parents from</param>
        /// <param name="totalAccFit">The total accumulated fitness of the parents</param>
        /// <param name="rnd">The Random object to use for selection</param>
        /// <returns>Prospective parent</returns>
        private Individual SelectParent(List<Individual> fillFrom, double totalAccFit, Random rnd)
        {
            //selection of parents is based on this value
            double currTotFit = rnd.NextDouble() * totalAccFit;

            return fillFrom.Find(
                    delegate(Individual i)
                    {
                        return i.AccumFitness > currTotFit;
                    }
                    );
        }

        /// <summary>
        /// Calculates (non-normalised) accumulated fitness values
        /// of all individuals of the given population
        /// </summary>
        /// <param name="population">The population to work with</param>
        /// <returns>The total of the fitness values</returns>
        private double AccumulateFitness(List<Individual> population)
        {
            if (null == population)
                throw new Exception("Population parametre of AccumulateFitness() is null.");
            double total = 0;

            foreach (Individual i in population)
            {
                if (null == i)
                    throw new Exception("Member " + i + " of one of the populations is null.");
                else
                {
                    total += i.Fitness;
                    i.AccumFitness = total;
                }
            }

            return total;
        }

        /// <summary>
        /// Calculates the fitness values of all members of the population
        /// </summary>
        private void CalculateFitness()
        {
            SetLabel2("Calculating fitness values");

            if (null == targetLearner)
                throw new Exception("There is no target learner.");
            if (0 == controlGroup.Count)
                throw new Exception("The control group is empty.");

            SetProgressBarValue(0);

            for (int i = 0; i < goodPopulation.Count; i++)
            {
                FillFitness(goodPopulation[i], false);
                SetProgressBarValue((int)Math.Round((double)(i / goodPopulation.Count) * 50));
            }
            for (int j = 0; j < badPopulation.Count; j++)
            {
                FillFitness(badPopulation[j], true);
                SetProgressBarValue(50 + (int)Math.Round((double)(j / badPopulation.Count) * 50));
            }

            double newOverallFitness
                = goodPopulation.Max(i => i.Fitness) + badPopulation.Max(j => j.Fitness);

            //this here is the stop criterion!
            if (newOverallFitness <= overallFitness)
                stop = true;

            overallFitness = newOverallFitness;

            SetLabel2("");
        }

        /// <summary>
        /// Fills the fitness value of one individual
        /// </summary>
        /// <param name="i">The individual</param>
        /// <param name="invert">If set to <c>true</c>, inverts fitness (for BadPopulation)</param>
        private void FillFitness(Individual i, bool invert)
        {
            //only do this if new or mutated
            if (0 == i.DataSet.Fitness || i.Mutated)
            {
                double targetScore = targetLearner.Learn(i.DataSet.data, numCrossValids);

                double controlScore = 0;
                foreach (Learner l in controlGroup)
                    controlScore += l.Learn(i.DataSet.data, numCrossValids);

                controlScore = controlScore / controlGroup.Count;

                if (0 == controlScore && 0 == targetScore)
                    i.DataSet.Fitness = 0;
                else if (invert)
                    i.DataSet.Fitness = controlScore / targetScore;
                else
                    i.DataSet.Fitness = targetScore / controlScore;
            }
        }

        /// <summary>
        /// Callback function for ProcessGeneration.
        /// If stop was not pressed, starts a new generation.
        /// </summary>
        /// <param name="ar">The IAsyncResult response, unused</param>
        private void NextGenCallBack(IAsyncResult ar)
        {
            SetExperimentDetailLabels();

            if (stop)
            {
                SaveConfig();
                mainForm.StateLabel.Text = "Stopped, safe to exit.";
                mainForm.StateLabel.ForeColor = Color.Green;
            }
            else
                NextGeneration();
        }

        /// <summary>
        /// Delegate for SetLabel2
        /// </summary>
        /// <param name="txt">The text to set</param>
        delegate void SetLabel2Callback(string txt);
        
        /// <summary>
        /// Sets stateLabel2 on the main form in a thread-safe way
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetLabel2(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.StateLabel2.InvokeRequired)
            {
                SetLabel2Callback d = new SetLabel2Callback(SetLabel2);
                mainForm.StateLabel2.Invoke(d, new object[] { text });
            }
            else
            {
                mainForm.StateLabel2.Text = text;
            }
        }

        /// <summary>
        /// Delegate for SetProgressBarValue
        /// </summary>
        /// <param name="value">The value to set, between 0 and 100</param>
        delegate void SetProgressBarValueCallback(int value);
        
        /// <summary>
        /// Sets the value of the progress bar.
        /// </summary>
        /// <param name="value">The value to set, between 0 and 100</param>
        private void SetProgressBarValue(int value)
        {
            if (value < 0 || value > 100)
                throw new Exception("Progress bar value must be between 0 and 100.");

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.ProgBar.InvokeRequired)
            {
                SetProgressBarValueCallback d
                    = new SetProgressBarValueCallback(SetProgressBarValue);
                mainForm.ProgBar.Invoke(d, new object[] { value });
            }
            else
            {
                mainForm.ProgBar.Value = value;
            }
        }

        /// <summary>
        /// Sets the set experiment detail labels
        /// (exp. ID, pop. size, generation, fitness)
        /// </summary>
        private void SetExperimentDetailLabels()
        {
            mainForm.GenerationLabel.Text = currentGeneration.ToString();
            mainForm.FitnessLabel.Text = overallFitness.ToString();
        }

        #endregion
    }
}
