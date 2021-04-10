
using CircleMove;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CircleDraw
{
    class MPolygon : Figure
    {
        public List<List<Point>> lines;
        public static bool isJarvis = true;

        public MPolygon(List<Point> points)
            : base(0, 0)
        {
            lines = new List<List<Point>>();
            if (isJarvis)
            {
                polygonPoints = CreatePolygonJarvis(points); //Jarvis
            }
            else
            {
                CreatePolygonTraditionally(points); //Traditional method (lines)
            }
        }

        //Jarvis
        public List<Point> CreatePolygonJarvis(List<Point> startingPoints)
        {
            var y_coord_list = new List<int>();
            var y_max_values_ids = new List<int>();
            var x_coord_list = new List<int>();
            int min_x = int.MaxValue;
            int max_y = int.MinValue;
            Point currentPoint;
            List<Point> polygon = new List<Point>();
            foreach(Point point in startingPoints)
            {
                y_coord_list.Add(point.Y);
                x_coord_list.Add(point.X);
            }
            foreach(int value in y_coord_list)
            {
                if(value > max_y)
                {
                    max_y = value;
                }
            }
            for(int i = 0; i < y_coord_list.Count; i++)
            {
                if(y_coord_list[i] == max_y)
                {
                    y_max_values_ids.Add(i);
                }
            }
            foreach(int id in y_max_values_ids)
            {
                if(x_coord_list[id] < min_x)
                {
                    min_x = x_coord_list[id];
                }
            }
            currentPoint = new Point(min_x, max_y);
            Point endPoint;
            do
            {
                polygon.Add(currentPoint);
                endPoint = startingPoints[0];
                for(int i = 1; i < startingPoints.Count; i++)
                {
                    if ((currentPoint == endPoint) || 
                        ((endPoint.X - currentPoint.X) * (startingPoints[i].Y - currentPoint.Y) - (startingPoints[i].X - currentPoint.X) * (endPoint.Y - currentPoint.Y)) > 0)
                    {
                        endPoint = startingPoints[i];
                    }
                }
                currentPoint = endPoint;
            }
            while (endPoint != polygon[0]);
            return polygon;
        }

        public void CreatePolygonTraditionally(List<Point> startingPoints)
        {
            var polygon = new List<Point>();
            foreach(Point firstPoint in startingPoints)
            {
                foreach (Point secondPoint in startingPoints)
                {
                    if (firstPoint == secondPoint) continue;
                    if (firstPoint.X == secondPoint.X)
                    {
                        int righter_points = 0, left_points = 0;
                        foreach(Point checkingPoint in startingPoints)
                        {
                            if(checkingPoint.X > firstPoint.X)
                            {
                                righter_points++;
                            }
                            if(checkingPoint.X < firstPoint.X)
                            {
                                left_points++;
                            }
                            if (righter_points > 0 && left_points > 0) break;
                        }
                        if ((righter_points == 0 && left_points > 0) || (righter_points > 0 && left_points == 0))
                        {
                            polygon.Add(firstPoint);
                            polygon.Add(secondPoint);
                            var line_list = new List<Point>();
                            line_list.Add(firstPoint);
                            line_list.Add(secondPoint);
                            lines.Add(line_list);
                        }
                    }
                    else
                    {
                        double k = ((double)firstPoint.Y - (double)secondPoint.Y) / ((double)firstPoint.X - (double)secondPoint.X);
                        double b = (double)firstPoint.Y - k * (double)firstPoint.X;
                        int upper_points = 0, down_points = 0;
                        foreach (Point checkingPoint in startingPoints)
                        {
                            if (checkingPoint == firstPoint || checkingPoint == secondPoint) continue;
                            if (checkingPoint.Y > k * checkingPoint.X + b)
                            {
                                upper_points++;
                            }
                            if (checkingPoint.Y < k * checkingPoint.X + b)
                            {
                                down_points++;
                            }
                            if (upper_points > 0 && down_points > 0) break;
                        }
                        if ((upper_points == 0 && down_points > 0) || (upper_points > 0 && down_points == 0))
                        {
                            polygon.Add(firstPoint);
                            polygon.Add(secondPoint);
                            var line_list = new List<Point>();
                            line_list.Add(firstPoint);
                            line_list.Add(secondPoint);
                            lines.Add(line_list);
                        }
                    }
                }
            }
            polygonPoints = polygon;
        }

        public override void Draw(PaintEventArgs e)
        {
            if(isJarvis)
                e.Graphics.DrawPolygon(new Pen(new SolidBrush(outsideColor)), polygonPoints.ToArray());
            else
            {
                foreach(var line in lines)
                {
                    e.Graphics.DrawLine(new Pen(new SolidBrush(outsideColor)), line[0], line[1]);
                }
            }
        }

        public override bool IsInside(Point point)
        {
            if (isJarvis)
            {
                Point previousPoint = polygonPoints[polygonPoints.Count - 1];
                for (int i = 0; i < polygonPoints.Count; i++)
                {
                    Point currentPoint = polygonPoints[i];
                    if (currentPoint.X > previousPoint.X)
                    {
                        if (currentPoint.X < point.X && point.X <= previousPoint.X &&
                            (point.Y - previousPoint.Y) * (currentPoint.X - previousPoint.X) <
                            (currentPoint.Y - previousPoint.Y) * (point.X - previousPoint.X))
                        {
                            isMoving = !isMoving;
                        }
                        if (currentPoint.X > point.X && point.X >= previousPoint.X &&
                            (point.Y - previousPoint.Y) * (currentPoint.X - previousPoint.X) <
                            (currentPoint.Y - previousPoint.Y) * (point.X - previousPoint.X))
                        {
                            isMoving = !isMoving;
                        }
                    }
                    else
                    {
                        if (currentPoint.X < point.X && point.X <= previousPoint.X &&
                            (point.Y - currentPoint.Y) * (previousPoint.X - currentPoint.X) <
                            (previousPoint.Y - currentPoint.Y) * (point.X - currentPoint.X))
                        {
                            isMoving = !isMoving;
                        }
                        if (currentPoint.X > point.X && point.X >= previousPoint.X &&
                        (point.Y - currentPoint.Y) * (previousPoint.X - currentPoint.X) <
                        (previousPoint.Y - currentPoint.Y) * (point.X - currentPoint.X))
                        {
                            isMoving = !isMoving;
                        }
                    }
                    previousPoint = currentPoint;
                }
            }
            else
            {
                Point constPoint = new Point(0, point.Y);
                int counter = 0;
                foreach(var line in lines)
                {
                    double k = ((double)line[0].Y - (double)line[1].Y) / ((double)line[0].X - (double)line[1].X);
                    double b = (double)line[0].Y - k * (double)line[0].X;
                    if (k == 0)
                        continue;
                    double _x = (((double)constPoint.Y - b) / k);
                    if(_x <= point.X)
                    {
                        if ((_x >= line[0].X && _x <= line[1].X))
                        {
                            counter++;
                        }
                    }
                }
                if(counter % 2 != 0)
                {
                    isMoving = true;
                }
            }
            if (isMoving)
            {
                dx = polygonPoints[0].X - point.X;
                dy = polygonPoints[0].Y - point.Y;
            }
            return isMoving;
        }

        public override void Move(Form context, MouseEventArgs e)
        {
            /*
            for(int i = 0; i < polygonPoints.Count; i++)
            {
                polygonPoints[i] = Point.Subtract(e.Location, (Size)objectPoint[i]);
            }
            context.Refresh();
            */
        }

        public override Point GetPoint()
        {
            return polygonPoints[0];
        }

        public override void OnPolygonMove(Point point)
        {
            dx = polygonPoints[0].X - point.X;
            dy = polygonPoints[0].Y - point.Y;
            isMoving = true;
        }
    }
}
