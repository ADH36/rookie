using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public enum ProgressStyle
    {
        Continuous,
        Marquee
    }

    /// <summary>
    /// Modern progress indicator with smooth animations and customizable appearance
    /// </summary>
    public class ModernProgressIndicator : UserControl
    {
        private Timer _animationTimer;
        private ProgressStyle _progressStyle = ProgressStyle.Continuous;
        private int _value = 0;
        private int _maximum = 100;
        private int _minimum = 0;
        private Color _progressColor = Color.FromArgb(0, 120, 215);
        private Color _backgroundProgressColor = Color.FromArgb(45, 45, 48);
        private bool _showPercentage = false;
        private int _cornerRadius = 0;
        private bool _glowEffect = false;
        private int _animationSpeed = 200;
        private float _currentAnimatedValue = 0;
        private DateTime _lastMarqueeUpdate = DateTime.Now;
        private float _marqueePosition = 0;
        private int _marqueeWidth = 50;

        public ModernProgressIndicator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint | 
                    ControlStyles.DoubleBuffer | 
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor, true);
            
            BackColor = Color.Transparent;
            Size = new Size(200, 20);
            
            InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_progressStyle == ProgressStyle.Marquee)
            {
                var elapsed = (DateTime.Now - _lastMarqueeUpdate).TotalMilliseconds;
                _marqueePosition += (float)(elapsed * 0.1); // Adjust speed as needed
                if (_marqueePosition > Width + _marqueeWidth)
                    _marqueePosition = -_marqueeWidth;
                _lastMarqueeUpdate = DateTime.Now;
            }
            else
            {
                // Smooth animation for continuous progress
                var diff = _value - _currentAnimatedValue;
                if (Math.Abs(diff) > 0.1f)
                {
                    _currentAnimatedValue += diff * 0.1f;
                }
                else
                {
                    _currentAnimatedValue = _value;
                }
            }
            
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var rect = new Rectangle(0, 0, Width, Height);
            
            if (_progressStyle == ProgressStyle.Marquee)
            {
                DrawMarqueeProgress(g, rect);
            }
            else
            {
                DrawContinuousProgress(g, rect);
            }
        }

        private void DrawContinuousProgress(Graphics g, Rectangle rect)
        {
            // Draw background
            using (var brush = new SolidBrush(_backgroundProgressColor))
            {
                if (_cornerRadius > 0)
                {
                    using (var path = CreateRoundedRectangle(rect, _cornerRadius))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Calculate progress width
            var progressWidth = (int)(((_currentAnimatedValue - _minimum) / (float)(_maximum - _minimum)) * rect.Width);
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, progressWidth, rect.Height);
                
                // Draw progress with gradient if glow effect is enabled
                if (_glowEffect)
                {
                    using (var brush = new LinearGradientBrush(progressRect, 
                        Color.FromArgb(180, _progressColor), _progressColor, LinearGradientMode.Horizontal))
                    {
                        if (_cornerRadius > 0)
                        {
                            using (var path = CreateRoundedRectangle(progressRect, _cornerRadius))
                            {
                                g.FillPath(brush, path);
                            }
                        }
                        else
                        {
                            g.FillRectangle(brush, progressRect);
                        }
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(_progressColor))
                    {
                        if (_cornerRadius > 0)
                        {
                            using (var path = CreateRoundedRectangle(progressRect, _cornerRadius))
                            {
                                g.FillPath(brush, path);
                            }
                        }
                        else
                        {
                            g.FillRectangle(brush, progressRect);
                        }
                    }
                }
            }

            // Draw percentage text if enabled
            if (_showPercentage)
            {
                var percentage = (int)(((_currentAnimatedValue - _minimum) / (float)(_maximum - _minimum)) * 100);
                var text = $"{percentage}%";
                var textSize = g.MeasureString(text, Font);
                var textRect = new PointF(
                    rect.X + (rect.Width - textSize.Width) / 2,
                    rect.Y + (rect.Height - textSize.Height) / 2);
                
                using (var brush = new SolidBrush(ForeColor))
                {
                    g.DrawString(text, Font, brush, textRect);
                }
            }
        }

        private void DrawMarqueeProgress(Graphics g, Rectangle rect)
        {
            // Draw background
            using (var brush = new SolidBrush(_backgroundProgressColor))
            {
                if (_cornerRadius > 0)
                {
                    using (var path = CreateRoundedRectangle(rect, _cornerRadius))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Draw moving marquee bar
            var marqueeRect = new Rectangle((int)_marqueePosition, rect.Y, _marqueeWidth, rect.Height);
            
            // Clip to the progress bar bounds
            var clippedRect = Rectangle.Intersect(marqueeRect, rect);
            if (!clippedRect.IsEmpty)
            {
                using (var brush = new LinearGradientBrush(
                    new Rectangle((int)_marqueePosition, rect.Y, _marqueeWidth, rect.Height),
                    Color.FromArgb(0, _progressColor),
                    _progressColor,
                    LinearGradientMode.Horizontal))
                {
                    brush.WrapMode = WrapMode.TileFlipX;
                    
                    if (_cornerRadius > 0)
                    {
                        using (var path = CreateRoundedRectangle(clippedRect, _cornerRadius))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else
                    {
                        g.FillRectangle(brush, clippedRect);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
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

        public ProgressStyle Style
        {
            get => _progressStyle;
            set
            {
                _progressStyle = value;
                if (value == ProgressStyle.Marquee)
                {
                    _marqueePosition = -_marqueeWidth;
                    _lastMarqueeUpdate = DateTime.Now;
                }
                Invalidate();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(_minimum, Math.Min(_maximum, value));
                Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(_minimum, value);
                if (_value > _maximum)
                    _value = _maximum;
                Invalidate();
            }
        }

        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = Math.Min(_maximum, value);
                if (_value < _minimum)
                    _value = _minimum;
                Invalidate();
            }
        }

        public Color ProgressColor
        {
            get => _progressColor;
            set
            {
                _progressColor = value;
                Invalidate();
            }
        }

        public Color BackgroundColor
        {
            get => _backgroundProgressColor;
            set
            {
                _backgroundProgressColor = value;
                Invalidate();
            }
        }

        public bool ShowPercentage
        {
            get => _showPercentage;
            set
            {
                _showPercentage = value;
                Invalidate();
            }
        }

        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        public bool GlowEffect
        {
            get => _glowEffect;
            set
            {
                _glowEffect = value;
                Invalidate();
            }
        }

        #endregion
    }
}