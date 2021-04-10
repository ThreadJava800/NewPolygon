using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CircleMove
{
    [Serializable]
    class Square : Figure
    {
        public Square(int x, int y)
            : base(x, y)
        {

        }

        public override void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Figure.insideColor), new Rectangle(x, y, Figure.radius, Figure.radius));
            e.Graphics.DrawRectangle(new Pen(Figure.outsideColor), new Rectangle(x, y, Figure.radius, Figure.radius));
        }

        public override Point GetPoint()
        {
            return new Point(x + Figure.radius / 2, y + Figure.radius / 2);
        }

        public override bool IsInside(Point point)
        {
            if (Rectangle.Intersect(new Rectangle(point.X, point.Y, 0, 0), new Rectangle(x, y, Figure.radius, Figure.radius)) != Rectangle.Empty)
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
