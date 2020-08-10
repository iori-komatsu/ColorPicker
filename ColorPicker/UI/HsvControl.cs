using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorPicker.UI {
    public delegate void HsvChangedEventHandler(HsvControl sender, Hsv newHsv);

    public class HsvControl : Control {
        private enum State {
            Initial,
            GrabbingHue,
            GrabbingSV,
        }

        private const float HueCircleWidth = 20.0f;
        private const float SVPlaneMargin = 6.0f;
        private const float PointerCircleOuterRadius = 6.5f;
        private const float PointerCircleInnerRadius = 3.5f;
        private const float BorderWidth = 1.0f;
        private readonly Color BorderColor = Color.FromRgb(200, 200, 200);

        private readonly WriteableBitmap bitmap = new WriteableBitmap(256, 256, 96, 96, PixelFormats.Bgra32, null);
        private State currentState = State.Initial;

        private float currentHue = 0.0f;
        private float currentSaturation = 0.0f;
        private float currentValue = 0.0f;

        public event HsvChangedEventHandler SelectedHsvChanged;

        static HsvControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HsvControl), new FrameworkPropertyMetadata(typeof(HsvControl)));
        }

        public HsvControl() {
            Width = bitmap.PixelWidth;
            Height = bitmap.PixelHeight;
            IsTabStop = false;
        }

        public Hsv SelectedHsv {
            get => (Hsv)GetValue(SelectedHsvProperty);
            set => SetValue(SelectedHsvProperty, value);
        }

        public static readonly DependencyProperty SelectedHsvProperty =
            DependencyProperty.Register(
                nameof(SelectedHsv),
                typeof(Hsv),
                typeof(HsvControl),
                new PropertyMetadata(
                    new Hsv(0, 0, 0),
                    (sender, e) => {
                        var newValue = (Hsv)e.NewValue;
                        var oldValue = (Hsv)e.OldValue;
                        if (newValue != oldValue) {
                            var hsvControl = (HsvControl)sender;
                            hsvControl.currentHue = newValue.H;
                            hsvControl.currentSaturation = newValue.S;
                            hsvControl.currentValue = newValue.V;
                            hsvControl.InvalidateVisual();
                        }
                    })
                );

        private void NotifySelectionChanged(float h, float s, float v) {
            // 選択されている色が変化したことを通知する。
            // このメソッドでは currentXxx は書き換えない。
            // これは、通知を受け取った側が SelectedHsv を set するフローであるため。
            SelectedHsvChanged?.Invoke(this, new Hsv(h, s, v));
        }

        private static void WritePixels(WriteableBitmap bitmap, Func<int, int, Color> plotter) {
            try {
                bitmap.Lock();

                unsafe {
                    IntPtr pBackBuffer = bitmap.BackBuffer;
                    for (int y = 0; y < bitmap.PixelHeight; ++y) {
                        IntPtr pRow = pBackBuffer + y * bitmap.BackBufferStride;
                        for (int x = 0; x < bitmap.PixelWidth; ++x) {
                            IntPtr pPixel = pRow + x * 4;
                            Color c = plotter(x, y);
                            *(int*)pPixel = c.A << 24 | c.R << 16 | c.G << 8 | c.B;
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            } finally {
                bitmap.Unlock();
            }
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);

            float w = bitmap.PixelWidth;
            float h = bitmap.PixelHeight;
            float centerX = w / 2;
            float centerY = h / 2;

            float hueCircleOuterRadius = (h - 2 * BorderWidth) / 2;
            float hueCircleInnerRadius = hueCircleOuterRadius - HueCircleWidth;

            float svPlaneWidth = hueCircleInnerRadius * (float)Math.Sqrt(2) - SVPlaneMargin;
            Rect svPlaneRect = new Rect(
                centerX - svPlaneWidth / 2, centerY - svPlaneWidth / 2,
                svPlaneWidth, svPlaneWidth);
            Rect svBorderRect = new Rect(
                svPlaneRect.Left - BorderWidth, svPlaneRect.Top - BorderWidth,
                svPlaneWidth + 2 * BorderWidth, svPlaneWidth + 2 * BorderWidth);

            float currentSVPointX = MathF.Lerp((float)svPlaneRect.Left, (float)svPlaneRect.Right, currentSaturation);
            float currentSVPointY = MathF.Lerp((float)svPlaneRect.Bottom, (float)svPlaneRect.Top, currentValue);

            float hueCircleAverageRadius = (hueCircleInnerRadius + hueCircleOuterRadius) / 2;
            float currentHuePointX = centerX - hueCircleAverageRadius * (float)Math.Cos(2.0 * Math.PI * currentHue);
            float currentHuePointY = centerY - hueCircleAverageRadius * (float)Math.Sin(2.0 * Math.PI * currentHue);

            WritePixels(bitmap, (x_, y_) => {
                float x = x_ + 0.5f;
                float y = y_ + 0.5f;
                Color color;

                float r1 = MathF.Sq(x - centerX) + MathF.Sq(y - centerY);
                if (MathF.Between(r1, MathF.Sq(hueCircleInnerRadius), MathF.Sq(hueCircleOuterRadius))) {
                    // Hue
                    double hue = Math.Atan2(centerY - y, centerX - x) / (2.0 * Math.PI);
                    if (hue < 0) hue += 1.0;
                    color = ColorF.FromHsv((float)hue, 1, 1).ToColor();
                } else if (MathF.Between(r1, MathF.Sq(hueCircleOuterRadius), MathF.Sq(hueCircleOuterRadius + BorderWidth)) ||
                           MathF.Between(r1, MathF.Sq(hueCircleInnerRadius - BorderWidth), MathF.Sq(hueCircleInnerRadius))) {
                    // Hue border
                    color = BorderColor;
                } else if (svPlaneRect.Contains(x, y)) {
                    // SV
                    float s = (x - (float)svPlaneRect.Left) / svPlaneWidth;
                    float v = 1.0f - (y - (float)svPlaneRect.Top) / svPlaneWidth;
                    color = ColorF.FromHsv(currentHue, s, v).ToColor();
                } else if (svBorderRect.Contains(x, y)) {
                    // SV border
                    color = BorderColor;
                } else {
                    // Background
                    color = Color.FromArgb(255, 255, 255, 255);
                }

                float r2 = MathF.Sq(x - currentSVPointX) + MathF.Sq(y - currentSVPointY);
                if (MathF.Between(r2, MathF.Sq(PointerCircleInnerRadius), MathF.Sq(PointerCircleOuterRadius))) {
                    color = color.Invert();
                }

                float r3 = MathF.Sq(x - currentHuePointX) + MathF.Sq(y - currentHuePointY);
                if (MathF.Between(r3, MathF.Sq(PointerCircleInnerRadius), MathF.Sq(PointerCircleOuterRadius))) {
                    color = color.Invert();
                }

                return color;
            });

            drawingContext.DrawImage(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);

            float w = bitmap.PixelWidth;
            float h = bitmap.PixelHeight;

            float centerX = w / 2;
            float centerY = h / 2;

            float hueCircleOuterRadius = (h - 2 * BorderWidth) / 2;
            float hueCircleInnerRadius = hueCircleOuterRadius - HueCircleWidth;

            float svPlaneWidth = hueCircleInnerRadius * (float)Math.Sqrt(2) - SVPlaneMargin;
            Rect svPlaneRect = new Rect(
                centerX - svPlaneWidth / 2, centerY - svPlaneWidth / 2,
                svPlaneWidth, svPlaneWidth);

            float x = (float)e.GetPosition(this).X + 0.5f;
            float y = (float)e.GetPosition(this).Y + 0.5f;

            if (svPlaneRect.Contains(x, y)) {
                float s = MathF.Saturate((x - (float)svPlaneRect.Left) / svPlaneWidth);
                float v = MathF.Saturate(1.0f - (y - (float)svPlaneRect.Top) / svPlaneWidth);
                if (currentSaturation != s || currentValue != v) {
                    NotifySelectionChanged(currentHue, s, v);
                }
                currentState = State.GrabbingSV;
                Mouse.Capture(this);
                return;
            }

            float r2 = MathF.Sq(x - centerX) + MathF.Sq(y - centerY);
            if (MathF.Between(r2, MathF.Sq(hueCircleInnerRadius), MathF.Sq(hueCircleOuterRadius))) {
                double hue = Math.Atan2(centerY - y, centerX - x) / (2.0 * Math.PI);
                if (hue < 0) hue += 1.0;
                if (currentHue != hue) {
                    NotifySelectionChanged((float)hue, currentSaturation, currentValue);
                }
                currentState = State.GrabbingHue;
                Mouse.Capture(this);
                return;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            float w = bitmap.PixelWidth;
            float h = bitmap.PixelHeight;

            float centerX = w / 2;
            float centerY = h / 2;

            float hueCircleOuterRadius = (h - 2 * BorderWidth) / 2;
            float hueCircleInnerRadius = hueCircleOuterRadius - HueCircleWidth;

            float svPlaneWidth = hueCircleInnerRadius * (float)Math.Sqrt(2) - SVPlaneMargin;
            Rect svPlaneRect = new Rect(
                centerX - svPlaneWidth / 2, centerY - svPlaneWidth / 2,
                svPlaneWidth, svPlaneWidth);

            float x = (float)e.GetPosition(this).X + 0.5f;
            float y = (float)e.GetPosition(this).Y + 0.5f;

            if (currentState == State.GrabbingSV) {
                float s = MathF.Saturate((x - (float)svPlaneRect.Left) / svPlaneWidth);
                float v = MathF.Saturate(1.0f - (y - (float)svPlaneRect.Top) / svPlaneWidth);
                if (currentSaturation != s || currentValue != v) {
                    NotifySelectionChanged(currentHue, s, v);
                }
                return;
            }

            if (currentState == State.GrabbingHue) {
                double hue = Math.Atan2(centerY - y, centerX - x) / (2.0 * Math.PI);
                if (hue < 0) hue += 1.0;
                if (currentHue != hue) {
                    NotifySelectionChanged((float)hue, currentSaturation, currentValue);
                }
                currentState = State.GrabbingHue;
                return;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            currentState = State.Initial;
            Mouse.Capture(null);
        }
    }
}
