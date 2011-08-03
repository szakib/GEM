using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEM
{
    /// <summary>
    /// The main form of the application
    /// </summary>
    public partial class MainForm : Form
    {
        #region fields & properties

        private GeneticAlgo ga;

        public Label StateLabel
        {
            get
            {
                return stateLabel;
            }
        }

        public Label StateLabel2
        {
            get
            {
                return stateLabel2;
            }
        }

        public Label ExperimentIDLabel
        {
            get
            {
                return experimentIDLabel;
            }
        }

        public Label PopSizeLabel
        {
            get
            {
                return popSizeLabel;
            }
        }

        public Label GenerationLabel
        {
            get
            {
                return generationLabel;
            }
        }

        public Label FitnessLabel
        {
            get
            {
                return fitnessLabel;
            }
        }

        public Label GoodFitnessLabel
        {
            get
            {
                return goodPopFitnessLabel;
            }
        }

        public Label BadFitnessLabel
        {
            get
            {
                return badPopFitnessLabel;
            }
        }

        public ProgressBar ProgBar
        {
            get
            {
                return progressBar;
            }
        }

        public Button ResetButton
        {
            get
            {
                return resetButton;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            //Generated, do not remove, has to be 1st
            InitializeComponent();

            ga = new GeneticAlgo(this);
        }

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the progressBar1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void progressBar1_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the stateLabel2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void stateLabel2_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the stateLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void stateLabel_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the stop button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;
            ga.Stop();
        }

        /// <summary>
        /// Handles the Click event of the start button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;
            resetButton.Enabled = false;
            ga.Start();
        }

        #endregion //methods

        private void resetButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;
            ga.Reset();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
