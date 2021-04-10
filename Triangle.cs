using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CircleMove
{
    [Serializable]
    class Triangle : Figure
    {

        public Triangle(int x, int y)
            : base(x, y)
        {
            InitTriangle();
        }

        private void InitTriangle()
        {
            int gen_x = x - Figure.radius / 2;
            int gen_y = y - Figure.radius / 2;
            var generatingPoints = new List<Point>();
            generatingPoints.Add(new Point((int)(x + Figure.radius * Math.Cos(Math.PI / 3) + Figure.radius / 2), (int)(y + Figure.radius * Math.Sin(Math.PI / 3))));
            generatingPoints.Add(new Point(x + Figure.radius / 2, y));
            generatingPoints.Add(new Point((int)(x + Figure.radius * Math.Cos(2 * Math.PI / 3) + Figure.radius / 2), (int)(y + Figure.radius * Math.Sin(2 * Math.PI / 3))));
            
            this.polygonPoints = generatingPoints;
        }

        public override void Draw(PaintEventArgs e)
        {
            InitTriangle();
            e.Graphics.FillPolygon(new SolidBrush(Figure.insideColor), polygonPoints.ToArray());
            e.Graphics.DrawPolygon(new Pen(Figure.outsideColor), polygonPoints.ToArray());
        }

        public override Point GetPoint()
        {
            return new Point(polygonPoints[2].X + Figure.radius / 2, polygonPoints[1].Y + Figure.radius / 2);
        }


        private bool HasCollided(Point point)
        {
            //righter line
            double k1 = ((double)polygonPoints[1].Y - (double)polygonPoints[2].Y) / ((double)polygonPoints[1].X - (double)polygonPoints[2].X);
            double b1 = (double)polygonPoints[1].Y - k1 * (double)polygonPoints[1].X;
            //lefter line
            double k2 = ((double)polygonPoints[0].Y - (double)polygonPoints[1].Y) / ((double)polygonPoints[0].X - (double)polygonPoints[1].X);
            double b2 = (double)polygonPoints[0].Y - k2 * (double)polygonPoints[0].X;
            //downer line
            double k3 = ((double)polygonPoints[0].Y - (double)polygonPoints[2].Y) / ((double)polygonPoints[0].X - (double)polygonPoints[2].X);
            double b3 = (double)polygonPoints[0].Y - k3 * (double)polygonPoints[0].X;
            //check
            if ((point.Y >= k1 * point.X + b1) && (point.Y >= k2 * point.X + b2) && (point.Y <= k3 * point.X + b3))
                return true;
            return false;
        }

        public override bool IsInside(Point point)
        {
            if (HasCollided(point))
            {
                dx = x - point.X;
                dy = y - point.Y;
                isMoving = true;
            }
            return isMoving;
        }

        public override void Move(Form context, MouseEventArgs e)
        {
            if(isMoving)
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
