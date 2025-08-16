using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace AndroidSideloader
{
    public static class AnimationHelper
    {
        /// <summary>
        /// Smoothly fades a control in or out
        /// </summary>
        /// <param name="control">The control to animate</param>
        /// <param name="fadeIn">True to fade in, false to fade out</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static async Task FadeAsync(Control control, bool fadeIn, int duration = 300)
        {
            if (control == null || control.IsDisposed) return;

            const int steps = 30;
            const int stepDelay = duration / steps;
            
            double startOpacity = fadeIn ? 0.0 : 1.0;
            double endOpacity = fadeIn ? 1.0 : 0.0;
            double opacityStep = (endOpacity - startOpacity) / steps;

            if (fadeIn && !control.Visible)
            {
                control.Opacity = 0.0;
                control.Visible = true;
            }

            for (int i = 0; i <= steps; i++)
            {
                if (control.IsDisposed) return;
                
                double opacity = startOpacity + (opacityStep * i);
                control.Opacity = Math.Max(0.0, Math.Min(1.0, opacity));
                
                await Task.Delay(stepDelay);
            }

            if (!fadeIn)
            {
                control.Visible = false;
            }
        }

        /// <summary>
        /// Smoothly slides a control to a new location
        /// </summary>
        /// <param name="control">The control to animate</param>
        /// <param name="targetLocation">The target location</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static async Task SlideToAsync(Control control, Point targetLocation, int duration = 400)
        {
            if (control == null || control.IsDisposed) return;

            const int steps = 40;
            const int stepDelay = duration / steps;
            
            Point startLocation = control.Location;
            int deltaX = targetLocation.X - startLocation.X;
            int deltaY = targetLocation.Y - startLocation.Y;

            for (int i = 0; i <= steps; i++)
            {
                if (control.IsDisposed) return;
                
                // Use easing function for smooth animation
                double progress = EaseInOutCubic((double)i / steps);
                
                int newX = startLocation.X + (int)(deltaX * progress);
                int newY = startLocation.Y + (int)(deltaY * progress);
                
                control.Location = new Point(newX, newY);
                
                await Task.Delay(stepDelay);
            }
        }

        /// <summary>
        /// Smoothly resizes a control to new dimensions
        /// </summary>
        /// <param name="control">The control to animate</param>
        /// <param name="targetSize">The target size</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static async Task ResizeToAsync(Control control, Size targetSize, int duration = 400)
        {
            if (control == null || control.IsDisposed) return;

            const int steps = 40;
            const int stepDelay = duration / steps;
            
            Size startSize = control.Size;
            int deltaWidth = targetSize.Width - startSize.Width;
            int deltaHeight = targetSize.Height - startSize.Height;

            for (int i = 0; i <= steps; i++)
            {
                if (control.IsDisposed) return;
                
                double progress = EaseInOutCubic((double)i / steps);
                
                int newWidth = startSize.Width + (int)(deltaWidth * progress);
                int newHeight = startSize.Height + (int)(deltaHeight * progress);
                
                control.Size = new Size(newWidth, newHeight);
                
                await Task.Delay(stepDelay);
            }
        }

        /// <summary>
        /// Animates a button press effect
        /// </summary>
        /// <param name="button">The button to animate</param>
        public static async Task ButtonPressAsync(Control button)
        {
            if (button == null || button.IsDisposed) return;

            Size originalSize = button.Size;
            Point originalLocation = button.Location;
            
            // Slightly shrink the button
            Size pressedSize = new Size(originalSize.Width - 2, originalSize.Height - 2);
            Point pressedLocation = new Point(originalLocation.X + 1, originalLocation.Y + 1);

            // Press down animation
            await Task.WhenAll(
                ResizeToAsync(button, pressedSize, 100),
                SlideToAsync(button, pressedLocation, 100)
            );

            // Release animation
            await Task.WhenAll(
                ResizeToAsync(button, originalSize, 100),
                SlideToAsync(button, originalLocation, 100)
            );
        }

        /// <summary>
        /// Creates a smooth color transition effect
        /// </summary>
        /// <param name="control">The control to animate</param>
        /// <param name="targetColor">The target background color</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static async Task ColorTransitionAsync(Control control, Color targetColor, int duration = 300)
        {
            if (control == null || control.IsDisposed) return;

            const int steps = 30;
            const int stepDelay = duration / steps;
            
            Color startColor = control.BackColor;
            
            int deltaR = targetColor.R - startColor.R;
            int deltaG = targetColor.G - startColor.G;
            int deltaB = targetColor.B - startColor.B;
            int deltaA = targetColor.A - startColor.A;

            for (int i = 0; i <= steps; i++)
            {
                if (control.IsDisposed) return;
                
                double progress = EaseInOutCubic((double)i / steps);
                
                int newR = startColor.R + (int)(deltaR * progress);
                int newG = startColor.G + (int)(deltaG * progress);
                int newB = startColor.B + (int)(deltaB * progress);
                int newA = startColor.A + (int)(deltaA * progress);
                
                control.BackColor = Color.FromArgb(
                    Math.Max(0, Math.Min(255, newA)),
                    Math.Max(0, Math.Min(255, newR)),
                    Math.Max(0, Math.Min(255, newG)),
                    Math.Max(0, Math.Min(255, newB))
                );
                
                await Task.Delay(stepDelay);
            }
        }

        /// <summary>
        /// Easing function for smooth animations
        /// </summary>
        /// <param name="t">Progress value between 0 and 1</param>
        /// <returns>Eased progress value</returns>
        private static double EaseInOutCubic(double t)
        {
            return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// Creates a pulsing effect for notifications or highlights
        /// </summary>
        /// <param name="control">The control to animate</param>
        /// <param name="pulseColor">The color to pulse to</param>
        /// <param name="cycles">Number of pulse cycles</param>
        public static async Task PulseAsync(Control control, Color pulseColor, int cycles = 2)
        {
            if (control == null || control.IsDisposed) return;

            Color originalColor = control.BackColor;
            
            for (int i = 0; i < cycles; i++)
            {
                await ColorTransitionAsync(control, pulseColor, 200);
                await ColorTransitionAsync(control, originalColor, 200);
            }
        }

        /// <summary>
        /// Smoothly shows/hides a panel with slide animation
        /// </summary>
        /// <param name="panel">The panel to animate</param>
        /// <param name="show">True to show, false to hide</param>
        /// <param name="direction">Direction of the slide (Left, Right, Up, Down)</param>
        public static async Task SlidePanel(Panel panel, bool show, SlideDirection direction = SlideDirection.Down)
        {
            if (panel == null || panel.IsDisposed) return;

            Size targetSize = show ? panel.MaximumSize : Size.Empty;
            
            if (show && !panel.Visible)
            {
                panel.Size = Size.Empty;
                panel.Visible = true;
            }

            await ResizeToAsync(panel, targetSize, 300);

            if (!show)
            {
                panel.Visible = false;
            }
        }
    }

    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }
}