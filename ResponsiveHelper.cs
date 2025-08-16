using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    /// <summary>
    /// Helper class for implementing responsive design and window resizing behavior
    /// </summary>
    public static class ResponsiveHelper
    {
        private static readonly Dictionary<Control, ResponsiveConfig> _responsiveControls = new Dictionary<Control, ResponsiveConfig>();
        private static Size _lastFormSize = Size.Empty;
        private static bool _isResizing = false;

        /// <summary>
        /// Configuration for responsive control behavior
        /// </summary>
        public class ResponsiveConfig
        {
            public AnchorStyles OriginalAnchor { get; set; }
            public Size OriginalSize { get; set; }
            public Point OriginalLocation { get; set; }
            public float ScaleFactor { get; set; } = 1.0f;
            public bool MaintainAspectRatio { get; set; } = false;
            public Size MinSize { get; set; } = Size.Empty;
            public Size MaxSize { get; set; } = Size.Empty;
        }

        /// <summary>
        /// Initialize responsive behavior for a form
        /// </summary>
        public static void InitializeResponsive(Form form)
        {
            if (form == null) return;

            _lastFormSize = form.Size;
            form.ResizeBegin += Form_ResizeBegin;
            form.ResizeEnd += Form_ResizeEnd;
            form.Resize += Form_Resize;

            // Store original configurations for all controls
            StoreOriginalConfigurations(form);
        }

        /// <summary>
        /// Register a control for responsive behavior
        /// </summary>
        public static void RegisterControl(Control control, bool maintainAspectRatio = false, Size minSize = default, Size maxSize = default)
        {
            if (control == null) return;

            var config = new ResponsiveConfig
            {
                OriginalAnchor = control.Anchor,
                OriginalSize = control.Size,
                OriginalLocation = control.Location,
                MaintainAspectRatio = maintainAspectRatio,
                MinSize = minSize == default ? Size.Empty : minSize,
                MaxSize = maxSize == default ? Size.Empty : maxSize
            };

            _responsiveControls[control] = config;
        }

        /// <summary>
        /// Store original configurations for all controls in a form
        /// </summary>
        private static void StoreOriginalConfigurations(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                RegisterControl(control);
                
                // Recursively store configurations for child controls
                if (control.HasChildren)
                {
                    StoreOriginalConfigurations(control);
                }
            }
        }

        /// <summary>
        /// Handle form resize begin event
        /// </summary>
        private static void Form_ResizeBegin(object sender, EventArgs e)
        {
            _isResizing = true;
            var form = sender as Form;
            if (form != null)
            {
                form.SuspendLayout();
            }
        }

        /// <summary>
        /// Handle form resize end event
        /// </summary>
        private static void Form_ResizeEnd(object sender, EventArgs e)
        {
            _isResizing = false;
            var form = sender as Form;
            if (form != null)
            {
                form.ResumeLayout(true);
                ApplyResponsiveLayout(form);
            }
        }

        /// <summary>
        /// Handle form resize event
        /// </summary>
        private static void Form_Resize(object sender, EventArgs e)
        {
            if (_isResizing) return;

            var form = sender as Form;
            if (form != null)
            {
                ApplyResponsiveLayout(form);
            }
        }

        /// <summary>
        /// Apply responsive layout to all registered controls
        /// </summary>
        private static void ApplyResponsiveLayout(Form form)
        {
            if (form == null || _lastFormSize.IsEmpty) return;

            float scaleX = (float)form.Width / _lastFormSize.Width;
            float scaleY = (float)form.Height / _lastFormSize.Height;

            foreach (var kvp in _responsiveControls)
            {
                var control = kvp.Key;
                var config = kvp.Value;

                if (control == null || control.IsDisposed) continue;

                // Calculate new size
                var newSize = CalculateNewSize(config, scaleX, scaleY);
                
                // Calculate new location
                var newLocation = CalculateNewLocation(config, scaleX, scaleY);

                // Apply changes
                control.Size = newSize;
                control.Location = newLocation;
            }

            _lastFormSize = form.Size;
        }

        /// <summary>
        /// Calculate new size for a control based on scale factors
        /// </summary>
        private static Size CalculateNewSize(ResponsiveConfig config, float scaleX, float scaleY)
        {
            int newWidth, newHeight;

            if (config.MaintainAspectRatio)
            {
                float scale = Math.Min(scaleX, scaleY);
                newWidth = (int)(config.OriginalSize.Width * scale);
                newHeight = (int)(config.OriginalSize.Height * scale);
            }
            else
            {
                newWidth = (int)(config.OriginalSize.Width * scaleX);
                newHeight = (int)(config.OriginalSize.Height * scaleY);
            }

            // Apply size constraints
            if (!config.MinSize.IsEmpty)
            {
                newWidth = Math.Max(newWidth, config.MinSize.Width);
                newHeight = Math.Max(newHeight, config.MinSize.Height);
            }

            if (!config.MaxSize.IsEmpty)
            {
                newWidth = Math.Min(newWidth, config.MaxSize.Width);
                newHeight = Math.Min(newHeight, config.MaxSize.Height);
            }

            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// Calculate new location for a control based on scale factors
        /// </summary>
        private static Point CalculateNewLocation(ResponsiveConfig config, float scaleX, float scaleY)
        {
            int newX = (int)(config.OriginalLocation.X * scaleX);
            int newY = (int)(config.OriginalLocation.Y * scaleY);

            return new Point(newX, newY);
        }

        /// <summary>
        /// Set minimum and maximum form size based on content
        /// </summary>
        public static void SetFormSizeConstraints(Form form, Size? minSize = null, Size? maxSize = null)
        {
            if (form == null) return;

            if (minSize.HasValue)
            {
                form.MinimumSize = minSize.Value;
            }
            else
            {
                // Calculate minimum size based on essential controls
                var calculatedMinSize = CalculateMinimumFormSize(form);
                form.MinimumSize = calculatedMinSize;
            }

            if (maxSize.HasValue)
            {
                form.MaximumSize = maxSize.Value;
            }
        }

        /// <summary>
        /// Calculate minimum form size based on essential controls
        /// </summary>
        private static Size CalculateMinimumFormSize(Form form)
        {
            int minWidth = 800; // Base minimum width
            int minHeight = 600; // Base minimum height

            // Find essential controls and calculate required space
            foreach (Control control in form.Controls)
            {
                if (control.Visible && control.Enabled)
                {
                    int requiredWidth = control.Location.X + control.Width + 20; // Add padding
                    int requiredHeight = control.Location.Y + control.Height + 20; // Add padding

                    minWidth = Math.Max(minWidth, requiredWidth);
                    minHeight = Math.Max(minHeight, requiredHeight);
                }
            }

            return new Size(minWidth, minHeight);
        }

        /// <summary>
        /// Apply smooth resize animation
        /// </summary>
        public static async Task AnimateResize(Form form, Size targetSize, int duration = 300)
        {
            if (form == null) return;

            var startSize = form.Size;
            var startTime = DateTime.Now;
            var endTime = startTime.AddMilliseconds(duration);

            while (DateTime.Now < endTime)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                var progress = Math.Min(elapsed / duration, 1.0);
                
                // Apply easing function
                var easedProgress = EaseInOutCubic(progress);
                
                var currentWidth = (int)(startSize.Width + (targetSize.Width - startSize.Width) * easedProgress);
                var currentHeight = (int)(startSize.Height + (targetSize.Height - startSize.Height) * easedProgress);
                
                form.Size = new Size(currentWidth, currentHeight);
                
                await Task.Delay(16); // ~60 FPS
            }

            form.Size = targetSize;
        }

        /// <summary>
        /// Easing function for smooth animations
        /// </summary>
        private static double EaseInOutCubic(double t)
        {
            return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// Clean up responsive helper resources
        /// </summary>
        public static void Cleanup()
        {
            _responsiveControls.Clear();
            _lastFormSize = Size.Empty;
            _isResizing = false;
        }
    }
}