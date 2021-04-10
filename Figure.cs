using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CircleMove
{
    [Serializable]
    public abstract class Figure
    {
        public static Color insideColor = Color.Aquamarine;
        public static Color outsideColor = Color.Red;
        public static int radius = 45;
        public int dx, dy;
        public int x, y;
        public List<Point> polygonPoints;
        public bool isMoving;

        public Figure(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.dx = 0;
            this.dy = 0;
            this.isMoving = false;
        }

        public void Stop()
        {
            isMoving = false;
        }

        public abstract bool IsInside(Point point);
        public abstract void OnPolygonMove(Point point);
        public abstract void Draw(PaintEventArgs e);
        public abstract void Move(Form context, MouseEventArgs e);
        public abstract Point GetPoint();
    }
}
