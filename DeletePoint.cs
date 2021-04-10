using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using PolygonLibrary;

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
            form.figures[id] = null;
            form.Refresh();
        }

        public override void Undo()
        {
            form.figures[id] = shape;
            form.Refresh();
        }
    }
}