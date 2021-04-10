using CircleMove;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolygonLibrary;

namespace CircleDraw
{
    class ChangeRadius : Change
    {
        int oldRad, newRad;
        RadiusChangerForm radiusForm;

        public ChangeRadius(Form1 form, RadiusChangerForm radiusForm, int oldRad, int newRad)
            :base(-1, form)
        {
            this.oldRad = oldRad;
            this.newRad = newRad;
            this.radiusForm = radiusForm;
        }

        public override void Redo()
        {
            Figure.radius = newRad;
            radiusForm.setValue(newRad);
            for (int i = 0; i < form.figures.Count; i++)
            {
                if (form.figures[i] != null)
                {
                    form.figures[i].x = form.figures[i].x - (newRad / 2 - oldRad / 2);
                    form.figures[i].y = form.figures[i].y - (newRad / 2 - oldRad / 2);
                }
            }
            form.Refresh();
        }

        public override void Undo()
        {
            Figure.radius = oldRad;
            radiusForm.setValue(oldRad);
            for (int i = 0; i < form.figures.Count; i++)
            {
                if (form.figures[i] != null)
                {
                    form.figures[i].x = form.figures[i].x - (oldRad / 2 - newRad / 2);
                    form.figures[i].y = form.figures[i].y - (oldRad / 2 - newRad / 2);
                }
            }
            form.Refresh();
        }
    }
}
