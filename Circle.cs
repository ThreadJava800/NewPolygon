using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CircleMove
{
    [Serializable]
    class Circle : Figure
    {
        public Circle(int x, int y)
            :base(x, y)
        {

        }

        public override void Draw(PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(Figure.insideColor), new Rectangle(x, y, Figure.radius, Figure.radius));
            e.Graphics.DrawEllipse(new Pen(Figure.outsideColor), new Rectangle(x, y, Figure.radius, Figure.radius));
        }

        public override Point GetPoint()
        {
            return new Point(x + Figure.radius / 2, y + Figure.radius / 2);
        }

        public override bool IsInside(Point point)
        {
            if ((point.X - (x + Figure.radius / 2)) * (point.X - (x + Figure.radius / 2)) + (point.Y - (y + Figure.radius / 2)) * (point.Y - (y + Figure.radius / 2)) <= Figure.radius * Figure.radius / 4)
            {
                dx = x - point.X;
                dy = y - point.Y;
                isMoving = true;
            }
            return isMoving;
        }

        public override void Move(Form context, MouseEventArgs e)
        {
            if (isMoving)
            {
                x = e.X + dx;
                y = e.Y + dy;
            }
        }

        public override void OnPolygonMove(Point point)
        {
            dx = x - point.X;
            dy = y - point.Y;
            isMoving = true;
        }
    }
}
