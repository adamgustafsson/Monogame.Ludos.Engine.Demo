using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;

namespace Model.World
{
    public class MovingPlatform
    {
        public System.Drawing.RectangleF DetectionBounds { get { return _detectionBounds; } }
        public System.Drawing.RectangleF Bounds { get { return _platform; } }
        public System.Drawing.PointF Change {get { return _change; } }

        private Polyline _path;
        private int _currentLine;
        private int _direction;
        private Vector2 _position;
        private System.Drawing.PointF _change;
        private System.Drawing.RectangleF _platform;
        private System.Drawing.RectangleF _detectionBounds;

        public MovingPlatform(Polyline polylinePath, Point size)
        {
            _path = polylinePath;
            _direction = 1;
            _currentLine = 0;
            _change = new System.Drawing.PointF(0, 0);
            _position = new Vector2(polylinePath.Bounds.X, polylinePath.Bounds.Y);
            _platform = new System.Drawing.RectangleF(_position.X, _position.Y, size.X, size.Y);
        }

        public void Update(float a_elapsedTime)
        {

            var currentLineLength = _path.Lines[_currentLine].Length - _platform.Width;


            if (Vector2.Distance(_path.Lines[_currentLine].Start, _position) > _path.Lines[_currentLine].Length)
            {
                if (_currentLine + 1 < _path.Lines.Length)
                {
                    _currentLine += 1;
                    _position = _path.Lines[_currentLine].Start;
                }
                else
                {
                    _direction = -1;
                    _position = _path.Lines[_currentLine].End;
                }
            }
            else if (Vector2.Distance(_position, _path.Lines[_currentLine].End) > _path.Lines[_currentLine].Length)
            {
                if (_currentLine - 1 >= 0)
                {
                    _currentLine -= 1;
                    _position = _path.Lines[_currentLine].End;
                }
                else
                {
                    _direction = 1;
                    _position = _path.Lines[_currentLine].Start;
                }
            }

            float targetPct = (_path.Lines[_currentLine].Length - Vector2.Distance(_position, _path.Lines[_currentLine].End)
                                + (a_elapsedTime * 75) * _direction) / _path.Lines[_currentLine].Length;

            _position = Vector2.Lerp(_path.Lines[_currentLine].Start, _path.Lines[_currentLine].End, targetPct);
            _change.X = _position.X - _platform.X;
            _change.Y = _position.Y - _platform.Y;


            _platform.X = _position.X;
            _platform.Y = _position.Y;

            _detectionBounds = new System.Drawing.RectangleF(_platform.X, _platform.Y, _platform.Width, _platform.Height * 0.13f);

        }
    }
}
