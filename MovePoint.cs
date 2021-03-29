using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CircleDraw
{
    public class MovePoint : Change
    {
        int dx, dy;
        public MovePoint(int id, Form1 form, int dx, int dy) :
            base(id, form)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public override void Redo()
        {
            form.figures[id].x -= dx;
            form.figures[id].y -= dy;
            form.Refresh();
        }

        public override void Undo()
        {
            form.figures[id].x += dx;
            form.figures[id].y += dy;
            form.Refresh();
        }
    }
}
