using System;
using System.Drawing;

public abstract class Shape
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public bool IsSelected { get; set; } = false;
    public abstract void Draw(Graphics g);
    public abstract bool Contains(Point point);
}

public class Line : Shape
{
    public Line(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public override void Draw(Graphics g)
    {
        using (Pen pen = new Pen(IsSelected ? Color.Red : Color.White, 3))
        {
            g.DrawLine(pen, Start, End);
        }
    }

    public override bool Contains(Point point)
    {
       
        const int tolerance = 3;
        using (Pen pen = new Pen(Color.White, tolerance))
        {
            return IsPointOnLine(point, Start, End, pen.Width);
        }
    }

    private bool IsPointOnLine(Point point, Point start, Point end, float width)
    {
        float distance = Math.Abs((end.Y - start.Y) * point.X - (end.X - start.X) * point.Y + end.X * start.Y - end.Y * start.X) /
                         (float)Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2));
        return distance <= width / 2;
    }
}

public class RectangleShape : Shape
{
    public RectangleShape(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public override void Draw(Graphics g)
    {
        using (Pen pen = new Pen(IsSelected ? Color.Red : Color.White, 3)) 
        {
            g.DrawRectangle(pen, new Rectangle(Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y));
        }
    }

    public override bool Contains(Point point)
    {
        return new Rectangle(Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y).Contains(point);
    }
}

public class Ellipse : Shape
{
    public Ellipse(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public override void Draw(Graphics g)
    {
        using (Pen pen = new Pen(IsSelected ? Color.Red : Color.White, 3))
        {
            g.DrawEllipse(pen, new Rectangle(Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y));
        }
    }

    public override bool Contains(Point point)
    {
        float a = Math.Abs(End.X - Start.X) / 2f;
        float b = Math.Abs(End.Y - Start.Y) / 2f;
        float centerX = (Start.X + End.X) / 2f;
        float centerY = (Start.Y + End.Y) / 2f;

        if (a <= 0.0 || b <= 0.0)
            return false;

        return Math.Pow(point.X - centerX, 2) / Math.Pow(a, 2) + Math.Pow(point.Y - centerY, 2) / Math.Pow(b, 2) <= 1.0;
    }
}
