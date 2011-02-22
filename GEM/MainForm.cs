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
        #region methods & properties

        private GeneticAlgo ga = new GeneticAlgo();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            //Generated, do not remove
            InitializeComponent();

            this.startButton.Click += new System.EventHandler(this.ga.Start_Click);
            this.stopButton.Click += new System.EventHandler(this.ga.Stop_Click);
        }
    }
}
