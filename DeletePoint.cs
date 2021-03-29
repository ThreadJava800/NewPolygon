using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CircleDraw
{
    public class DeletePoint : Change
    {
        Figure shape;
        public DeletePoint(int id, Form1 form, Figure shape):
            base(id, form)
        {
            this.shape = shape;
        }

        public override void Redo()
        {
            form.figures.Remove(shape);
            form.Refresh();
        }

        public override void Undo()
        {
            form.figures.Insert(form.figures.Count - id, shape);
            form.Refresh();
        }
    }
}