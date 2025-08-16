using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AndroidSideloader
{
    /// <summary>
    /// Modern loading spinner with smooth animations and customizable appearance
    /// </summary>
    public class ModernLoadingSpinner : UserControl
    {
        private Timer _animationTimer;
        private float _angle = 0;
        private Color _spinnerColor = Color.FromArgb(0, 120, 215);
        private Color _backgroundColor = Color.Transparent;
        private int _thickness = 4;
        private int _speed = 100; // milliseconds per rotation
        private bool _isSpinning = true;
        private SpinnerStyle _style = SpinnerStyle.Arc;
        private int _dotCount = 8;
        private float _dotSize = 6;

        public enum SpinnerStyle
        {
            Arc,
            Dots,
            Pulse,
            Ring
        }

        public ModernLoadingSpinner()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Size = new Size(32, 32);

            InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
            
            if (_isSpinning)
            {
                _animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _angle += 360f / (_speed / 16f); // Smooth rotation based on speed
            if (_angle >= 360)
                _angle -= 360;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var rect = new Rectangle(0, 0, Width, Height);
            var centerX = rect.Width / 2f;
            var centerY = rect.Height / 2f;
            var radius = Math.Min(centerX, centerY) - _thickness;

            switch (_style)
            {
                case SpinnerStyle.Arc:
                    DrawArcSpinner(g, centerX, centerY, radius);
                    break;
                case SpinnerStyle.Dots:
                    DrawDotsSpinner(g, centerX, centerY, radius);
                    break;
                case SpinnerStyle.Pulse:
                    DrawPulseSpinner(g, centerX, centerY, radius);
                    break;
                case SpinnerStyle.Ring:
                    DrawRingSpinner(g, centerX, centerY, radius);
                    break;
            }
        }

        private void DrawArcSpinner(Graphics g, float centerX, float centerY, float radius)
        {
            var rect = new RectangleF(centerX - radius, centerY - radius, radius * 2, radius * 2);
            
            // Draw background arc
            using (var pen = new Pen(Color.FromArgb(30, _spinnerColor), _thickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawEllipse(pen, rect);
            }
            
            // Draw animated arc
            using (var pen = new Pen(_spinnerColor, _thickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawArc(pen, rect, _angle, 90);
            }
        }

        private void DrawDotsSpinner(Graphics g, float centerX, float centerY, float radius)
        {
            for (int i = 0; i < _dotCount; i++)
            {
                var angle = (360f / _dotCount * i + _angle) * Math.PI / 180;
                var x = centerX + (float)(Math.Cos(angle) * radius);
                var y = centerY + (float)(Math.Sin(angle) * radius);
                
                // Calculate opacity based on position
                var opacity = (float)(0.3 + 0.7 * (i / (double)_dotCount));
                var color = Color.FromArgb((int)(255 * opacity), _spinnerColor);
                
                using (var brush = new SolidBrush(color))
                {
                    var dotRect = new RectangleF(x - _dotSize / 2, y - _dotSize / 2, _dotSize, _dotSize);
                    g.FillEllipse(brush, dotRect);
                }
            }
        }

        private void DrawPulseSpinner(Graphics g, float centerX, float centerY, float radius)
        {
            // Calculate pulse scale based on angle
            var pulseScale = (float)(0.5 + 0.5 * Math.Sin(_angle * Math.PI / 180));
            var currentRadius = radius * pulseScale;
            
            var rect = new RectangleF(centerX - currentRadius, centerY - currentRadius, currentRadius * 2, currentRadius * 2);
            
            // Create gradient brush for pulse effect
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb((int)(100 * pulseScale), _spinnerColor);
                    brush.SurroundColors = new[] { Color.FromArgb(0, _spinnerColor) };
                    
                    g.FillPath(brush, path);
                }
            }
            
            // Draw border
            using (var pen = new Pen(Color.FromArgb((int)(255 * pulseScale), _spinnerColor), 2))
            {
                g.DrawEllipse(pen, rect);
            }
        }

        private void DrawRingSpinner(Graphics g, float centerX, float centerY, float radius)
        {
            var rect = new RectangleF(centerX - radius, centerY - radius, radius * 2, radius * 2);
            
            // Create gradient for the ring
            using (var brush = new LinearGradientBrush(
                new PointF(centerX - radius, centerY),
                new PointF(centerX + radius, centerY),
                Color.FromArgb(0, _spinnerColor),
                _spinnerColor))
            {
                // Rotate the gradient
                var matrix = new Matrix();
                matrix.RotateAt(_angle, new PointF(centerX, centerY));
                brush.Transform = matrix;
                
                using (var pen = new Pen(brush, _thickness))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawEllipse(pen, rect);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Properties

        public bool IsSpinning
        {
            get => _isSpinning;
            set
            {
                _isSpinning = value;
                if (_isSpinning)
                {
                    _animationTimer?.Start();
                }
                else
                {
                    _animationTimer?.Stop();
                }
            }
        }

        public Color SpinnerColor
        {
            get => _spinnerColor;
            set
            {
                _spinnerColor = value;
                Invalidate();
            }
        }

        public int Thickness
        {
            get => _thickness;
            set
            {
                _thickness = Math.Max(1, value);
                Invalidate();
            }
        }

        public int Speed
        {
            get => _speed;
            set
            {
                _speed = Math.Max(10, value);
            }
        }

        public SpinnerStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                Invalidate();
            }
        }

        public int DotCount
        {
            get => _dotCount;
            set
            {
                _dotCount = Math.Max(3, Math.Min(20, value));
                Invalidate();
            }
        }

        public float DotSize
        {
            get => _dotSize;
            set
            {
                _dotSize = Math.Max(2, value);
                Invalidate();
            }
        }

        #endregion

        /// <summary>
        /// Start the spinning animation
        /// </summary>
        public void Start()
        {
            IsSpinning = true;
        }

        /// <summary>
        /// Stop the spinning animation
        /// </summary>
        public void Stop()
        {
            IsSpinning = false;
        }

        /// <summary>
        /// Reset the spinner to initial position
        /// </summary>
        public void Reset()
        {
            _angle = 0;
            Invalidate();
        }
    }
}