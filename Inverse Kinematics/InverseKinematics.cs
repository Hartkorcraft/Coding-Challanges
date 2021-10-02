using Godot;
using System;
//using System.Colections.Generic;

public class InverseKinematics : Node2D
{
    public static Color White { get; } = new Color(1, 1, 1, 1);
    public Vector2 MousePos { get => GetLocalMousePosition(); }
    Segment segment;

    public override void _Ready()
    {
        segment = new Segment(Vector2.Zero, 100, 0);
    }

    public override void _Process(float delta)
    {
        Update();

    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("DebugUpdate"))
        {
            Update();
        }
    }
    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 5, White);

        segment.Follow(MousePos);
        segment.UpdateSegment();
        DrawArm();
    }

    public void DrawArm()
    {
        DrawLine(
            segment.PointA,
            segment.PointB,
            White, 5);
    }

    public class Segment
    {
        public Vector2 PointA { get; set; }
        public Vector2 PointB { get; private set; }
        public float Lenght { get; set; }
        public float Angle { get; set; }

        public void UpdateSegment()
        {
            CalculateB();
        }

        public void CalculateB()
        {
            float dx = Lenght * Mathf.Cos(Angle);
            float dy = Lenght * Mathf.Sin(Angle);
            PointB = new Vector2(PointA.x + dx, PointA.y + dy);
        }

        public void Follow(Vector2 target)
        {
            Vector2 dir = target - PointA;
            Angle = dir.Angle();
            //GD.Print(Mathf.Rad2Deg(Angle));

            dir = dir.Normalized() * Lenght;
            dir *= -1;

            PointA = target + dir;
        }

        public Segment(Vector2 _pointA, float _lenght, float _angle)
        {
            PointA = _pointA;
            Lenght = _lenght;
            Angle = _angle;
            CalculateB();
        }

        // public Segment(Segment _parent, float _lenght, float _angle)
        // {
        //     PointA = _pointA;
        //     Lenght = _lenght;
        //     Angle = _angle;
        //     CalculateB();
        // }
    }
}


