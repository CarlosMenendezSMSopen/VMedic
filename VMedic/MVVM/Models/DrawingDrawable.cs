using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.MVVM.Models
{
    public class DrawingDrawable : IDrawable
    {
        public List<List<PointF>> Lines { get; set; } = new();
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;

            foreach (var line in Lines)
            {
                if (line.Count < 2) continue;
                for (int i = 0; i < line.Count - 1; i++)
                {
                    canvas.DrawLine(line[i], line[i + 1]);
                }
            }
        }
    }
}
