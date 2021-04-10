using CircleMove;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PolygonLibrary;

namespace CircleDraw
{
    public partial class RadiusChangerForm : Form
    {
        public delegate void RadiusChangedHandler(object sender, RadiusEventArgs e);
        public event RadiusChangedHandler radiusHandler;
        private int oldRadius;
        private Form1 form;
        private long clickCounter;

        public RadiusChangerForm(Form1 form, int currentRadius)
        {
            InitializeComponent();
            trackBar1.Value = currentRadius;
            this.form = form;
            clickCounter = 0;
        }

        public void setValue(int radius)
        {
            trackBar1.Value = radius;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            radiusHandler(this, new RadiusEventArgs(trackBar1.Value, Figure.radius));
        }

        private void RadiusChangerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.current_radius_form = null;
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            oldRadius = trackBar1.Value;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            form.OnChange(new List<Change> { new ChangeRadius(form, this, oldRadius, trackBar1.Value) });
        }

        private void trackBar1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
            {
                form.OnChange(new List<Change> { new ChangeRadius(form, this, oldRadius, trackBar1.Value) });
            }
            clickCounter = 0;
        }

        private void trackBar1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Left) && clickCounter == 0)
            {
                oldRadius = trackBar1.Value;
            }
            clickCounter++;
        }
    }
}
