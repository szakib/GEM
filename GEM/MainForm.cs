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

        #endregion

        #region methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            //Generated, do not remove, has to be 1st
            InitializeComponent();

            //event handlers
            this.startButton.Click += new System.EventHandler(this.Start_Click);
            this.stopButton.Click += new System.EventHandler(this.Stop_Click);

            ga = new GeneticAlgo(this);
        }

        /// <summary>
        /// Handles the Click event of the start button
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
        public void Start_Click(object sender, EventArgs e)
        {
            ga.Start();
        }

        /// <summary>
        /// Handles the Click event of the stop button
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
        public void Stop_Click(object sender, EventArgs e)
        {
            ga.Stop();
        }

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
