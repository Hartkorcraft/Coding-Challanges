using Godot;
using System;
using System.Collections.Generic;

public class FourierSeriesGenerator : Node2D
{
    [Export] Vector2 origin = new Vector2(300, 300);
    [Export] Color seriesColor = new Color(1, 1, 1, 1);
    [Export] Color circleColor = new Color(1, 1, 1, 1);
    [Export] float seriesXOffset = 150;
    [Export] float startingRadiusScale = 80;
    [Export] float seriesWidth = 3;
    [Export] float circleWidth = 1.5f;
    [Export] float circlePointSize = 3.0f;
    [Export] int iterations = 5;
    [Export] float timeMulti = 0.5f;
    [Export] float scale = 0.5f;

    [Export]
    SeriesType CurrentSeriesType
    {
        get => currentSeriesType;
        set
        {
            currentSeriesType = value;
            seriesGenerator = SetSeries(value);
        }
    }

    SeriesType currentSeriesType = SeriesType.Square;
    float time = 0;
    int seriesPointCount = 800;
    List<Vector2> seriesPos = new List<Vector2>();
    Func<int, (float radius, float function, float b)?> seriesGenerator;
    Dictionary<SeriesType, Func<int, (float radius, float function)?>> availableWaves;
    Vector2 circlePointPos = new Vector2(0, 0);

    public override void _Ready()
    {
        CurrentSeriesType = SeriesType.Square;
    }

    public override void _Process(float delta)
    {
        Update();
        time += 0.06f * timeMulti;
    }

    public override void _Draw()
    {
        var x = 0.0f;
        var y = 0.0f;

        circlePointPos = origin;
    
        for (int i = 0; i < iterations; i++)
        {
            var prevX = x;
            var prevY = y;
            var newOrigin = origin + new Vector2(prevX, prevY);

            var n = 2 * i + 1;
            var seriesInfo = seriesGenerator(i);
            if (seriesInfo is null) { break; }

            var radius = seriesInfo.Value.radius;
            var function = seriesInfo.Value.function;
            var b = seriesInfo.Value.b;

            x += (radius * Mathf.Cos(function) + b) * scale;
            y += (radius * Mathf.Sin(function) + b) * scale;

            circlePointPos = origin + new Vector2(x, y);

            DrawArc(newOrigin, radius, Mathf.Deg2Rad(0), Mathf.Deg2Rad(360), 100, circleColor, circleWidth);

            DrawLine(newOrigin, circlePointPos, circleColor);
            DrawCircle(circlePointPos, circlePointSize, circleColor);

        }

        seriesPos.Insert(0, new Vector2(x, y));
        DrawWave();
        if (seriesPos.Count > seriesPointCount) seriesPos.RemoveAt(seriesPos.Count - 1);
    }

    public Func<int, (float radius, float function, float b)?> SetSeries(SeriesType seriesType)
    {
        switch (seriesType)
        {
            case SeriesType.Square: return seriesGenerator = DoSquareWave;
            case SeriesType.Triangle: return seriesGenerator = DoTriangleWave;
            default: return seriesGenerator = DoSquareWave;
        }
    }


    (float radius, float funcion, float b)? DoNothing(int iteration)
    {
        return null;
    }

    (float radius, float funcion, float b)? DoSquareWave(int iteration)
    {
        var n = 2 * iteration + 1;
        var radius = startingRadiusScale * (4 / (n * (float)Math.PI));
        var function = n * time;
        return (radius, function, 0);
    }

    (float radius, float funcion, float b)? DoTriangleWave(int iteration)
    {
        var n = 2 * iteration + 1;
        var radius = startingRadiusScale *
        (8 / Mathf.Pow((float)Math.PI, 2)) * (Mathf.Pow(-1, (n - 1) / 2) / Mathf.Pow(n, 2));

        var function = n * time;
        return (radius, function, 0);
    }

    void DrawWave()
    {
        DrawLine(circlePointPos, origin + new Vector2(seriesXOffset, seriesPos[0].y), circleColor);

        for (int i = 1; i < seriesPos.Count; i++)
        {
            DrawLine(
                origin + new Vector2(seriesXOffset + i, seriesPos[i - 1].y),
                origin + new Vector2(seriesXOffset + i + 1, seriesPos[i].y), seriesColor, seriesWidth);
        }
    }

    public enum SeriesType
    {
        Square,
        Triangle
    }


}

// https://youtu.be/Mm2eYfj0SgA
// https://en.wikipedia.org/wiki/Fourier_series
// https://bilimneguzellan.net/en/purrier-series-meow-and-making-images-speak/
// https://betterexplained.com/articles/an-interactive-guide-to-the-fourier-transform/
// https://www.youtube.com/watch?v=ds0cmAV-Yek&t=0s
// https://www.youtube.com/watch?v=spUNpyF58BY
// https://www.youtube.com/watch?v=N633bLi_YCw
