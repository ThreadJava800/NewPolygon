using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CircleDraw
{
    class ChangeOutsideColor : Change
    {
        Color oldCol, newCol;
        public ChangeOutsideColor(Form1 form, Color oldCol, Color newCol)
            :base(-1, form)
        {
            this.oldCol = oldCol;
            this.newCol = newCol;
        }

        public override void Redo()
        {
            Figure.outsideColor = newCol;
            form.Refresh();
        }

        public override void Undo()
        {
            Figure.outsideColor = oldCol;
            form.Refresh();
        }
    }
}
