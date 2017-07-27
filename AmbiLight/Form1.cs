using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmbiLight
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
        }

        //The event that is animating the Frames
        private void timer_Tick(object sender, EventArgs e)
        {
            picture.Image = Program.Capture();
        }
    }
}
