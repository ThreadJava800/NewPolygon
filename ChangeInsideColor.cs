using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CircleDraw
{
    public class ChangeInsideColor : Change
    {
        Color oldCol, newCol;
        public ChangeInsideColor(Form1 form, Color oldCol, Color newCol):
            base(-1, form)
        {
            this.oldCol = oldCol;
            this.newCol = newCol;
        }

        public override void Redo()
        {
            Figure.insideColor = newCol;
            form.Refresh();
        }

        public override void Undo()
        {
            Figure.insideColor = oldCol;
            form.Refresh();
        }
    }
}
