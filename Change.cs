using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CircleDraw
{
    public abstract class Change : EventArgs
    {
        public int id;
        public Form1 form;
        public Change(int id, Form1 form)
        {
            this.id = id;
            this.form = form;
        }
        public abstract void Undo();
        public abstract void Redo();
    }
}
