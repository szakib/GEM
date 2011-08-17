using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/*using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;*/
using System.Diagnostics;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GEM
{
    /// <summary>
    /// The main genetic algorithm engine
    /// </summary>
    public class GeneticAlgo
    {
        #region fields & properties

        /// <summary>
        /// Size of each of the populations
        /// </summary>
        private int                     populationSize;
        
        /// <summary>
        /// Number of Cross-validations to do per dataset
        /// </summary>
        private int                     numCrossValids;

        /// <summary>
        /// How big part of the population gets kept during the elitist selection
        /// </summary>
        private double                  eliteRatio;

        /// <summary>
        /// resume or start from scratch
        /// </summary>
        private bool                    resume;

        /// <summary>
        /// Logger
        /// </summary>
        private static Logger           log                 = LogManager.GetLogger("GeneticAlgo");


        /// <summary>
        /// flag signalling that Stop has been pressed on the UI
        /// </summary>
        private bool                    stop                = false;

        /// <summary>
        /// Flag signalling that the stop criterion has been met
        /// </summary>
        private bool                    done                = false;

        /// <summary>
        /// Pointer to UI
        /// </summary>
        private MainForm                mainForm;

        /// <summary>
        /// Path for save files and log
        /// </summary>
        private string                  savePath;

        /// <summary>
        /// Experiment ID
        /// </summary>
        private int                     experimentID;

        /// <summary>
        /// Number of current generation
        /// </summary>
        private int                     currentGeneration;

        /// <summary>
        /// Sum of the fitness values of the best of the two populations
        /// </summary>
        private double                  overallFitness      = 0;

        /// <summary>
        /// between 0 and 1, ratio of mutations
        /// </summary>
        private double                  mutationSeverity;

        /// <summary>
        /// List of fitness values of past generations, used in StopCrit()
        /// </summary>
        private List<double>            pastFitness         = new List<double>();

        /// <summary>
        /// Number of past fitness values to use in StopCrit()
        /// </summary>
        private double                  pastFitnessCount;
        
        /// <summary>
        /// Fitness derivative threshold for StopCrit()
        /// </summary>
        private double                  minFitnessDerivative;

        /// <summary>
        /// Save frequency, minutes
        /// </summary>
        private int                     saveFrequency;

        /// <summary>
        /// For measuring time between two saves.
        /// </summary>
        private Stopwatch               t                   = new Stopwatch();

        /// <summary>
        /// Type of the target learner
        /// </summary>
        private LearnerType             targetLearnerType;

        /// <summary>
        /// Types of control group learners
        /// </summary>
        private List<LearnerType>       controlLearnerTypes;

        /// <summary>
        /// Target learner
        /// </summary>
        private Learner                 targetLearner;

        /// <summary>
        /// Control group of other learners
        /// </summary>
        private List<Learner>           controlGroup        = new List<Learner>();

        /// <summary>
        /// The population aiming to maximise bias for the target algorithm
        /// </summary>
        private List<Individual>        goodPopulation      = new List<Individual>();

        /// <summary>
        /// The population aiming to maximise bias against the target algorithm
        /// </summary>
        private List<Individual> badPopulation = new List<Individual>();

        /// <summary>
        /// Best individual in the good population
        /// </summary>
        private Individual              bestGood            = null;

        /// <summary>
        /// Best individual in the bad population
        /// </summary>
        private Individual              bestBad             = null;

        private bool singleThreadHack = false;

        //these might be useful at a certain point
        /*/// <summary>
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
        }*/

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgo"/> class
        /// Do not use the default constructor!
        /// </summary>
        public GeneticAlgo()
        {
            throw new Exception(
                "Please use the GeneticAlgo(MainForm main) constructor instead.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgo"/> class
        /// </summary>
        /// <param name="main">The main form of the UI</param>
        public GeneticAlgo(MainForm main)
        {
            mainForm = main;
            ReadConfig();
            targetLearner = new Learner(targetLearnerType, null);
            foreach (LearnerType lt in controlLearnerTypes)
                controlGroup.Add(new Learner(lt, null));

            t.Start();

            InitLog();
        }

        /// <summary>
        /// Initialises the logger.
        /// </summary>
        private void InitLog()
        {
            // Step 1. Create configuration object 
            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = Path.Combine(savePath, "GEM_log.txt");
            fileTarget.Layout = "${date:format=dd/MM/yyyy HH.mm.ss} ${message}";

            // Step 4. Define rules
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Info, fileTarget);
            config.LoggingRules.Add(rule1);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            log.Info("*****************************");
            log.Info("GEM started, init successful.");
        }

        /// <summary>
        /// Reads the settings from the config file
        /// </summary>
        private void ReadConfig()
        {
            singleThreadHack = ConfigSettings.ReadBool("SingleThreadHack");
            populationSize = ConfigSettings.ReadInt("PopulationSize");
            resume = ConfigSettings.ReadBool("Resume");
            savePath = ConfigSettings.ReadString("SavePath");
            experimentID = ConfigSettings.ReadInt("ExperimentID");
            currentGeneration = ConfigSettings.ReadInt("CurrentGeneration");
            mutationSeverity = ConfigSettings.ReadDouble("MutationSeverity");
            if (0 > mutationSeverity || 1 < mutationSeverity)
                throw new Exception("MutationSeverity has to be between 0 and 1.");
            numCrossValids = ConfigSettings.ReadInt("NumCrossValids");
            eliteRatio = ConfigSettings.ReadDouble("EliteRatio");
            pastFitnessCount = ConfigSettings.ReadInt("PastFitnessCount");
            minFitnessDerivative = ConfigSettings.ReadDouble("MinFitnessDerivative");
            saveFrequency = ConfigSettings.ReadInt("SaveFrequency");
            targetLearnerType
                = (LearnerType)ConfigSettings.ReadEnum(
                    "TargetLearner", typeof(LearnerType));
            controlLearnerTypes = new List<LearnerType>();
            string controlTypes = ConfigSettings.ReadString("ControlGroupMembers");
            string[] typesArray
                = controlTypes.Split(" ,;.'/?:|".ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in typesArray)
            {
                try
                {
                    LearnerType ltype = (LearnerType)Enum.Parse(typeof(LearnerType), s);
                    controlLearnerTypes.Add(ltype);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        "Error while reading ControlGroupMembers from config file. Exception text:"
                        + e.Message);
                }
            }
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
            ConfigSettings.WriteSetting("NumCrossValids", numCrossValids.ToString());
            ConfigSettings.WriteSetting("EliteRatio", eliteRatio.ToString());
            ConfigSettings.WriteSetting("PastFitnessCount", pastFitnessCount.ToString());
            ConfigSettings.WriteSetting("MinFitnessDerivative", minFitnessDerivative.ToString());
            ConfigSettings.WriteSetting("SaveFrequency", saveFrequency.ToString());
            //since the learners cannot be modified while the program is running,
            //they are not saved here
        }

        /// <summary>
        /// Starts processing the GA
        /// </summary>
        public void Start()
        {
            stop = false;
            done = false;
            mainForm.StateLabel.Text = "Starting";
            mainForm.StateLabel.ForeColor = Color.Green;

            //if this is the first run, the populations need init
            //otherwise they get loaded
            if (resume)
            {
                mainForm.StateLabel.Text = "Loading data, please wait.";
                LoadPopulations(experimentID, currentGeneration);
            }
            else
            {
                InitPopulations();
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

            //the 2 initial populations will not be the same, this is by design
            goodPopulation = RandomPopulation(populationSize);
            badPopulation = RandomPopulation(populationSize);

            experimentID++;
            currentGeneration = 0;

            SetLabel2("");
        }

        /// <summary>
        /// Constructs a random population
        /// </summary>
        /// <param name="size">Number of individuals</param>
        /// <returns>The random population</returns>
        private List<Individual> RandomPopulation(int size)
        {
            List<Individual> ret = new List<Individual>();

            for (int i = 0; i < size; i++)
            {
                ret.Add(new Individual());
            }

            return ret;
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

            string path1 = Path.Combine(savePath, filename + "_good");
            string path2 = Path.Combine(savePath, filename + "_bad");
            if (File.Exists(path1) && File.Exists(path2))
            {
                //Load good population
                Stream stream = File.Open(path1, FileMode.Open);
                goodPopulation = (List<Individual>)bFormatter.Deserialize(stream);
                stream.Close();
                //Load bad population
                Stream stream2 = File.Open(path2, FileMode.Open);
                badPopulation = (List<Individual>)bFormatter.Deserialize(stream2);
                stream2.Close();
                stream2.Dispose();
            }
            else
                InitPopulations();

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
            
            //check if save dir exists, create it if needed
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            //Save good population
            string path = Path.Combine(savePath, filename + "_good");
            Stream stream1 = File.Open(path, FileMode.Create);
            bFormatter.Serialize(stream1, goodPopulation);
            stream1.Close();
            stream1.Dispose();

            //Save bad population
            path = Path.Combine(savePath, filename + "_bad");
            Stream stream2 = File.Open(path, FileMode.Create);
            bFormatter.Serialize(stream2, badPopulation);
            stream2.Close();
            stream2.Dispose();
            resume = true;

            SetLabel2("");
        }

        /// <summary>
        /// Terminates the current experiment
        /// </summary>
        public void Reset()
        {
            mainForm.StateLabel.Text = "Resetting, please wait.";
            mainForm.StateLabel.ForeColor = Color.Red;
            log.Info("Reset button pressed.");
            pastFitness = new List<double>();

            resume = false;
            SaveConfig();
            
            mainForm.StateLabel.Text = "Reset done.";
            mainForm.StateLabel.ForeColor = Color.Green;
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
            log.Info("Stop button pressed.");
        }

        /// <summary>
        /// Starts the processing of the next generation asynchronously
        /// </summary>
        private void NextGeneration()
        {
            currentGeneration++;

            log.Info("Generation " + currentGeneration + " started.");
            mainForm.StateLabel.Text = "Running";
            mainForm.StateLabel.ForeColor = Color.Green;
            SetExperimentDetailLabels();

            if (singleThreadHack)
            {
                ProcessGeneration();
                NextGenCallBack(null);
            }
            else
            {
                //start new generation by calling ProcessGeneration asynchronously
                AsynchProcessGen caller = new AsynchProcessGen(this.ProcessGeneration);
                caller.BeginInvoke(new AsyncCallback(NextGenCallBack), ""); 
            }
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

            //save in the beginning, at stopping for resume and every few minutes
            if (currentGeneration == 1 || (stop && !done)
                || t.Elapsed.TotalMinutes >= saveFrequency)
            {
                SavePopulations();
                t.Restart();
            }

            //Sort the populations by fitness (descending)
            goodPopulation.Sort(new IndividualComparer());
            badPopulation.Sort(new IndividualComparer());

            //Breeding chance will depend on accumulated fitness
            //see http://en.wikipedia.org/wiki/Selection_%28genetic_algorithm%29
            double totalFitGood = AccumulateFitness(goodPopulation, out bestGood);
            double totalFitBad = AccumulateFitness(badPopulation, out bestBad);
            List<Individual> newGoodPop = new List<Individual>();
            List<Individual> newBadPop = new List<Individual>();

            log.Info("Best individual in good population:");
            logIndividual(bestGood);
            if (null != bestGood && bestGood.Fitness > 1)
            {
                bestGood.SaveArff(savePath);
                bestGood.ImmuniseAgainstMutations();
                newGoodPop = new List<Individual>();
                //Elite survives...
                for (int i = 0; i < populationSize * eliteRatio; i++)
                    //... but only if it's not useless
                    if (goodPopulation[i].Fitness > 1)
                        newGoodPop.Add(goodPopulation[i]);
            }
            else
            {
                log.Info("Re-initialising good population.");
                newGoodPop = RandomPopulation(populationSize);
            }
            log.Info("Best individual in bad population:");
            logIndividual(bestBad);
            if (null != bestBad && bestBad.Fitness > 1)
            {
                bestBad.SaveArff(savePath);
                bestBad.ImmuniseAgainstMutations();
                newBadPop = new List<Individual>();
                //Elite survives...
                for (int i = 0; i < populationSize * eliteRatio; i++)
                    //... but only if it's not useless
                    if (badPopulation[i].Fitness > 1)
                        newBadPop.Add(badPopulation[i]);
            }
            else
            {
                log.Info("Re-initialising bad population.");
                newBadPop = RandomPopulation(populationSize);
            }

            //Create the next generation, which has two groups in it:
            //1.: Elite from previous
            //2.: Random children of current generation

            //Breeding to fill the rest of the places
            Random rnd = new Random();
            FillRestWithChildren(newBadPop, badPopulation, totalFitBad, rnd);
            FillRestWithChildren(newGoodPop, goodPopulation, totalFitGood, rnd);

            //Swap in the new populations
            goodPopulation = newGoodPop;
            badPopulation = newBadPop;

            //Mutate (or not) each individual
            foreach (Individual i in goodPopulation)
                i.Mutate(mutationSeverity);
            foreach (Individual j in badPopulation)
                j.Mutate(mutationSeverity);
        } //non-surviving individuals of old populations get garbage collected here

        /// <summary>
        /// Logs an individual's parametres
        /// </summary>
        /// <param name="i">The individual to write into the log</param>
        private void logIndividual(Individual i)
        {
            if (null == i)
                log.Info("Individual is null.");
            else
            {
                log.Info("Fitness = " + i.Fitness);
                log.Info(i.Genes.ToString());
            }
        }

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
                SetProgressBarValue(
                    (int)Math.Round((double)(toFill.Count / populationSize) * 100));

                Individual parent1 = null;
                Individual parent2 = null;

                while (null == parent1)
                    parent1 = SelectParent(fillFrom, totalAccFit, rnd);
                //2nd condition makes sure the parents are different
                while (null == parent2)
                {
                    parent2 = SelectParent(fillFrom, totalAccFit, rnd);
                    if (parent1 == parent2)
                    {
                        int p1idx = fillFrom.IndexOf(parent1);
                        if (p1idx < fillFrom.Count - 1)
                            parent2 = fillFrom[p1idx + 1];
                        else if (p1idx > 0)
                            parent2 = fillFrom[p1idx - 1];
                        else
                            throw new Exception(
                                "reproduction is not possible without at least 2 potential parents.");
                    }
                }

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
        private Individual SelectParent(
            List<Individual> fillFrom,
            double totalAccFit,
            Random rnd)
        {
            //selection of parents is based on this value
            double currTotFit = rnd.NextDouble() * totalAccFit;

            return fillFrom.Find(
                    delegate(Individual i)
                    {
                        return i.AccumFitness >= currTotFit;
                    }
                    );
        }

        /// <summary>
        /// Calculates (non-normalised) accumulated fitness values
        /// of all individuals of the given population
        /// Side effect: it selects the best individual and stores it in an output variable
        /// </summary>
        /// <param name="population">The population to work with</param>
        /// <param name="best">The individual with the highest fitness value</param>
        /// <returns>
        /// The total of the fitness values
        /// </returns>
        private double AccumulateFitness(List<Individual> population, out Individual best)
        {
            if (null == population)
                throw new Exception("Population parameter of AccumulateFitness() is null.");
            double total = 0;
            double bestFit = 0;
            best = null;

            foreach (Individual i in population)
            {
                if (null == i)
                    throw new Exception("Member " + i + " of one of the populations is null.");
                else
                {
                    total += i.Fitness;
                    i.AccumFitness = total;
                    if (i.Fitness > bestFit)
                    {
                        best = i;
                        bestFit = i.Fitness;
                    }
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

            pastFitness.Add(newOverallFitness);

            //this here is the stop criterion for the entire genetic algorithm!
            if (StopCrit())
            {
                done = true;
                stop = true;
                string happyMessage = "Stop criterion reached.";
                resume = false;
                SetLabel2(happyMessage);
                log.Info(happyMessage);
            }
            else
                SetLabel2("");

            overallFitness = newOverallFitness;
            log.Info("Overall fitness: " + overallFitness.ToString());
        }

        /// <summary>
        /// The stop criterion of the genetic algorithm
        /// </summary>
        /// <returns>true, if the criterion is met</returns>
        private bool StopCrit()
        {
            //not enough data yet
            if (pastFitness.Count < pastFitnessCount)
                return false;
            else
            {
                //sum of individual deltas
                double sum = 0;
                
                //keep only the specified number of old values
                while (pastFitness.Count > pastFitnessCount)
                    pastFitness.RemoveAt(0);
                for (int i = 0; i < pastFitness.Count - 1; i++)
                    sum += pastFitness[i + 1] - pastFitness[i];

                if (sum / (pastFitness.Count - 1) > minFitnessDerivative)
                    return false;
                //stop crit reached
                else
                    return true;
            }
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
                {
                    controlScore += l.Learn(i.DataSet.data, numCrossValids);
                }

                controlScore = controlScore / controlGroup.Count;

                //if the target score is 0, this is perfectly justified.
                //if the control score is 0, only rejected for practical reasons.
                if (0 == controlScore || 0 == targetScore)
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
                if (!done)
                    SavePopulations();
                SetStateLabel("Stopped, safe to exit or continue by pressing Start.");
                //mainForm.StateLabel.ForeColor = Color.Green;
                EnableResetButton();
                log.Info("Processing stopped.");
            }
            else
            {
                NextGeneration();
            }
        }

        #region UI manipulation

        /// <summary>
        /// Delegate for EnableResetButton
        /// </summary>
        delegate void EnableResetButtonCallBack();

        /// <summary>
        /// Enables the reset button.
        /// </summary>
        private void EnableResetButton()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.ResetButton.InvokeRequired)
            {
                EnableResetButtonCallBack d
                    = new EnableResetButtonCallBack(EnableResetButton);
                mainForm.ResetButton.Invoke(d);
            }
            else
            {
                mainForm.ResetButton.Enabled = true;
            }
        }

        /// <summary>
        /// Delegate for SetLabel2
        /// </summary>
        /// <param name="txt">The text to set</param>
        delegate void SetStateLabelCallback(string txt);
        
        /// <summary>
        /// Sets stateLabel2 on the main form in a thread-safe way
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetStateLabel(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.StateLabel2.InvokeRequired)
            {
                SetStateLabelCallback d = new SetStateLabelCallback(SetStateLabel);
                mainForm.StateLabel.Invoke(d, new object[] { text });
            }
            else
            {
                mainForm.StateLabel.Text = text;
            }
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
            SetGenerationLabel(currentGeneration.ToString());
            SetFitnessLabel(overallFitness.ToString());
            if (null != bestGood && null != bestGood.DataSet)
                SetGoodFitnessLabel(bestGood.Fitness.ToString());
            if (null != bestBad && null != bestBad.DataSet)
                SetBadFitnessLabel(bestBad.Fitness.ToString());
        }

        /// <summary>
        /// Delegate for SetGenerationLabel
        /// </summary>
        /// <param name="txt">The text to set</param>
        delegate void SetGenerationLabelCallback(string txt);

        /// <summary>
        /// Sets GenerationLabel on the main form in a thread-safe way
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetGenerationLabel(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.GenerationLabel.InvokeRequired)
            {
                SetGenerationLabelCallback d
                    = new SetGenerationLabelCallback(SetGenerationLabel);
                mainForm.GenerationLabel.Invoke(d, new object[] { text });
            }
            else
            {
                mainForm.GenerationLabel.Text = text;
            }
        }

        /// <summary>
        /// Delegate for SetFitnessLabel
        /// </summary>
        /// <param name="txt">The text to set</param>
        delegate void SetFitnessLabelCallback(string txt);

        /// <summary>
        /// Sets FitnessLabel on the main form in a thread-safe way
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetFitnessLabel(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.FitnessLabel.InvokeRequired)
            {
                SetFitnessLabelCallback d = new SetFitnessLabelCallback(SetFitnessLabel);
                mainForm.FitnessLabel.Invoke(d, new object[] { text });
            }
            else
            {
                mainForm.FitnessLabel.Text = text;
            }
        }

        /// <summary>
        /// Delegate for SetGoodFitnessLabel
        /// </summary>
        /// <param name="txt">The text to set</param>
        delegate void SetGoodFitnessLabelCallback(string txt);

        /// <summary>
        /// Sets GoodFitnessLabel on the main form in a thread-safe way
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetGoodFitnessLabel(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.GoodFitnessLabel.InvokeRequired)
            {
                SetGoodFitnessLabelCallback d
                    = new SetGoodFitnessLabelCallback(SetGoodFitnessLabel);
                mainForm.GoodFitnessLabel.Invoke(d, new object[] { text });
            }
            else
            {
                mainForm.GoodFitnessLabel.Text = text;
            }
        }

        /// <summary>
        /// Delegate for SetBadFitnessLabel
        /// </summary>
        /// <param name="txt">The text to set</param>
        delegate void SetBadFitnessLabelCallback(string txt);

        /// <summary>
        /// Sets BadFitnessLabel on the main form in a thread-safe way
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetBadFitnessLabel(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (mainForm.BadFitnessLabel.InvokeRequired)
            {
                SetBadFitnessLabelCallback d
                    = new SetBadFitnessLabelCallback(SetBadFitnessLabel);
                mainForm.BadFitnessLabel.Invoke(d, new object[] { text });
            }
            else
            {
                mainForm.BadFitnessLabel.Text = text;
            }
        }

        #endregion //UI manipulation

        #endregion
    }
}
