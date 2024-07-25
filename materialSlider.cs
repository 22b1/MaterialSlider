/*


Made by 22v1 | GitHub: https://github.com/22b1/MaterialSlider
This is a modified version of ColorSlider: https://github.com/fabricelacharme/ColorSlider
 
Avalible under MIT Licence:
MIT License

Copyright (c) 2024 22b1

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.



 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

[ToolboxBitmap(typeof(TrackBar))]
[DefaultEvent("Scroll")]
[DefaultProperty("BarInnerColor")]
public class MaterialSlider : Control
{
    private Rectangle barRect;
    private Rectangle elapsedRect;
    private int OffsetL;
    private int OffsetR;
    private int _padding;
    private Orientation _barOrientation;
    private bool _drawFocusRectangle;
    private bool _mouseEffects = true;
    private decimal _trackerValue = 30m;
    private decimal _minimum;
    private decimal _maximum = 100m;
    private decimal _smallChange = 1m;
    private decimal _largeChange = 5m;
    private int _mouseWheelBarPartitions = 10;
    private Color _barInnerColor = Color.Black;
    private Color _elapsedPenColorTop = Color.FromArgb(95, 140, 180);
    private Color _elapsedPenColorBottom = Color.FromArgb(99, 130, 208);
    private Color _barPenColorTop = Color.FromArgb(55, 60, 74);
    private Color _barPenColorBottom = Color.FromArgb(87, 94, 110);
    private decimal _scaleDivisions = 10m;
    private decimal _scaleSubDivisions = 5m;
    private bool _showSmallScale;
    private bool _showDivisionsText = true;
    private Size _barSize = new Size(100, 20);

    [Description("Set the size of the bar")]
    [Category("MaterialSlider")]
    [DefaultValue(typeof(Size), "100; 20")]
    public Size BarSize
    {
        get
        {
            return _barSize;
        }
        set
        {
            if (value.Width > 0 && value.Height > 0)
            {
                _barSize = value;
                Invalidate();
            }
            else
            {
                throw new ArgumentOutOfRangeException("BarSize has to be greater than zero");
            }
        }
    }

    private Color[,] aColorSchema = new Color[3, 9]
    {
        {
            Color.White,
            Color.FromArgb(21, 56, 152),
            Color.FromArgb(21, 56, 152),
            Color.Black,
            Color.FromArgb(95, 140, 180),
            Color.FromArgb(99, 130, 208),
            Color.FromArgb(55, 60, 74),
            Color.FromArgb(87, 94, 110),
            Color.FromArgb(21, 56, 152)
        },
        {
            Color.White,
            Color.Red,
            Color.Red,
            Color.Black,
            Color.LightCoral,
            Color.Salmon,
            Color.FromArgb(55, 60, 74),
            Color.FromArgb(87, 94, 110),
            Color.Red
        },
        {
            Color.White,
            Color.Green,
            Color.Green,
            Color.Black,
            Color.SpringGreen,
            Color.LightGreen,
            Color.FromArgb(55, 60, 74),
            Color.FromArgb(87, 94, 110),
            Color.Green
        }
    };

    private bool mouseInRegion;


    private IContainer components;


    [Description("Set Slider padding (inside margins: left & right or bottom & top)")]
    [Category("MaterialSlider")]
    [DefaultValue(0)]
    public new int Padding
    {
        get
        {
            return _padding;
        }
        set
        {
            if (_padding != value)
            {
                _padding = value;
                OffsetL = (OffsetR = _padding);
                Invalidate();
            }
        }
    }

    [Description("Set Slider orientation")]
    [Category("MaterialSlider")]
    [DefaultValue(Orientation.Horizontal)]
    public Orientation Orientation
    {
        get
        {
            return _barOrientation;
        }
        set
        {
            if (_barOrientation != value)
            {
                _barOrientation = value;
                if (base.DesignMode)
                {
                    int width = base.Width;
                    base.Width = base.Height;
                    base.Height = width;
                }

                Invalidate();
            }
        }
    }

    [Description("Set whether to draw focus rectangle")]
    [Category("MaterialSlider")]
    [DefaultValue(false)]
    public bool DrawFocusRectangle
    {
        get
        {
            return _drawFocusRectangle;
        }
        set
        {
            _drawFocusRectangle = value;
            Invalidate();
        }
    }

    [Description("Set whether mouse entry and exit actions have impact on how control look")]
    [Category("MaterialSlider")]
    [DefaultValue(true)]
    public bool MouseEffects
    {
        get
        {
            return _mouseEffects;
        }
        set
        {
            _mouseEffects = value;
            Invalidate();
        }
    }

    [Description("Set Slider value")]
    [Category("MaterialSlider")]
    [DefaultValue(30)]
    public decimal Value
    {
        get
        {
            return _trackerValue;
        }
        set
        {
            if ((value >= _minimum) & (value <= _maximum))
            {
                _trackerValue = value;
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, new EventArgs());
                }

                Invalidate();
                return;
            }

            throw new ArgumentOutOfRangeException("Value is outside appropriate range (min, max)");
        }
    }

    [Description("Set Slider minimal point")]
    [Category("MaterialSlider")]
    [DefaultValue(0)]
    public decimal Minimum
    {
        get
        {
            return _minimum;
        }
        set
        {
            if (value < _maximum)
            {
                _minimum = value;
                if (_trackerValue < _minimum)
                {
                    _trackerValue = _minimum;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged(this, new EventArgs());
                    }
                }

                Invalidate();
                return;
            }

            throw new ArgumentOutOfRangeException("Minimal value is greather than maximal one");
        }
    }

    [Description("Set Slider maximal point")]
    [Category("MaterialSlider")]
    [DefaultValue(100)]
    public decimal Maximum
    {
        get
        {
            return _maximum;
        }
        set
        {
            if (!(value > _minimum))
            {
                return;
            }

            _maximum = value;
            if (_trackerValue > _maximum)
            {
                _trackerValue = _maximum;
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, new EventArgs());
                }
            }

            Invalidate();
        }
    }

    [Description("Set trackbar's small change")]
    [Category("MaterialSlider")]
    [DefaultValue(1)]
    public decimal SmallChange
    {
        get
        {
            return _smallChange;
        }
        set
        {
            _smallChange = value;
        }
    }

    [Description("Set trackbar's large change")]
    [Category("MaterialSlider")]
    [DefaultValue(5)]
    public decimal LargeChange
    {
        get
        {
            return _largeChange;
        }
        set
        {
            _largeChange = value;
        }
    }

    [Description("Set to how many parts is bar divided when using mouse wheel")]
    [Category("MaterialSlider")]
    [DefaultValue(10)]
    public int MouseWheelBarPartitions
    {
        get
        {
            return _mouseWheelBarPartitions;
        }
        set
        {
            if (value > 0)
            {
                _mouseWheelBarPartitions = value;
                return;
            }

            throw new ArgumentOutOfRangeException("MouseWheelBarPartitions has to be greather than zero");
        }
    }

    [Description("Set Slider bar inner color")]
    [Category("MaterialSlider")]
    [DefaultValue(typeof(Color), "Black")]
    public Color BarInnerColor
    {
        get
        {
            return _barInnerColor;
        }
        set
        {
            _barInnerColor = value;
            Invalidate();
        }
    }

    [Description("Gets or sets the top color of the elapsed")]
    [Category("MaterialSlider")]
    public Color ElapsedPenColorTop
    {
        get
        {
            return _elapsedPenColorTop;
        }
        set
        {
            _elapsedPenColorTop = value;
            Invalidate();
        }
    }

    [Description("Gets or sets the bottom color of the elapsed")]
    [Category("MaterialSlider")]
    public Color ElapsedPenColorBottom
    {
        get
        {
            return _elapsedPenColorBottom;
        }
        set
        {
            _elapsedPenColorBottom = value;
            Invalidate();
        }
    }

    [Description("Gets or sets the top color of the bar")]
    [Category("MaterialSlider")]
    public Color BarPenColorTop
    {
        get
        {
            return _barPenColorTop;
        }
        set
        {
            _barPenColorTop = value;
            Invalidate();
        }
    }

    [Description("Gets or sets the bottom color of the bar")]
    [Category("MaterialSlider")]
    public Color BarPenColorBottom
    {
        get
        {
            return _barPenColorBottom;
        }
        set
        {
            _barPenColorBottom = value;
            Invalidate();
        }
    }

    [Description("Set the number of intervals between minimum and maximum")]
    [Category("MaterialSlider")]
    public decimal ScaleDivisions
    {
        get
        {
            return _scaleDivisions;
        }
        set
        {
            if (value > 0m)
            {
                _scaleDivisions = value;
            }

            Invalidate();
        }
    }

    [Description("Set the number of subdivisions between main divisions of graduation.")]
    [Category("MaterialSlider")]
    public decimal ScaleSubDivisions
    {
        get
        {
            return _scaleSubDivisions;
        }
        set
        {
            if (value > 0m && _scaleDivisions > 0m && (_maximum - _minimum) / ((value + 1m) * _scaleDivisions) > 0m)
            {
                _scaleSubDivisions = value;
            }

            Invalidate();
        }
    }

    [Description("Show or hide subdivisions of graduations")]
    [Category("MaterialSlider")]
    public bool ShowSmallScale
    {
        get
        {
            return _showSmallScale;
        }
        set
        {
            if (value)
            {
                if (_scaleDivisions > 0m && _scaleSubDivisions > 0m && (_maximum - _minimum) / ((_scaleSubDivisions + 1m) * _scaleDivisions) > 0m)
                {
                    _showSmallScale = value;
                    Invalidate();
                }
                else
                {
                    _showSmallScale = false;
                }
            }
            else
            {
                _showSmallScale = value;
                Invalidate();
            }
        }
    }

    [Description("Show or hide text value of graduations")]
    [Category("MaterialSlider")]
    public bool ShowDivisionsText
    {
        get
        {
            return _showDivisionsText;
        }
        set
        {
            _showDivisionsText = value;
            Invalidate();
        }
    }

    [Bindable(true)]
    [Browsable(true)]
    [Category("MaterialSlider")]
    [Description("Get or Sets the Font of the Text being displayed.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public override Font Font
    {
        get
        {
            return base.Font;
        }
        set
        {
            base.Font = value;
            Invalidate();
            OnFontChanged(EventArgs.Empty);
        }
    }

    [Bindable(true)]
    [Browsable(true)]
    [Category("MaterialSlider")]
    [Description("Get or Sets the Color of the Text being displayed.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public override Color ForeColor
    {
        get
        {
            return base.ForeColor;
        }
        set
        {
            base.ForeColor = value;
            Invalidate();
            OnForeColorChanged(EventArgs.Empty);
        }
    }

    [Description("Event fires when the Value property changes")]
    [Category("Action")]
    public event EventHandler ValueChanged;

    [Description("Event fires when the Slider position is changed")]
    [Category("Behavior")]
    public event ScrollEventHandler Scroll;

    public MaterialSlider(decimal min, decimal max, decimal value)
    {
        InitializeComponent();
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.UserMouse | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
        BackColor = Color.Transparent;
        ForeColor = Color.White;
        Font = new Font("Microsoft Sans Serif", 6f);
        Minimum = min;
        Maximum = max;
        Value = value;
    }

    public MaterialSlider()
        : this(0m, 100m, 30m)
    {
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        barRect = new Rectangle(OffsetL, (Height - _barSize.Height) / 2, Width - OffsetL - OffsetR, _barSize.Height);
        using (Brush barBrush = new SolidBrush(_barInnerColor))
        {
            using (GraphicsPath barPath = CreateRoundedRectanglePath(barRect, 4))
            {
                g.FillPath(barBrush, barPath);
            }
        }
        int elapsedWidth = (int)((_trackerValue - _minimum) / (_maximum - _minimum) * barRect.Width);
        if (elapsedWidth > 0 && barRect.Height > 0)
        {
            elapsedRect = new Rectangle(OffsetL, (Height - _barSize.Height) / 2, elapsedWidth, _barSize.Height);
            using (Brush elapsedBrush = new LinearGradientBrush(elapsedRect, _elapsedPenColorTop, _elapsedPenColorBottom, LinearGradientMode.Vertical))
            {
                using (GraphicsPath elapsedPath = CreateRoundedRectanglePath(elapsedRect, 4))
                {
                    g.FillPath(elapsedBrush, elapsedPath);
                }
            }
        }
    }

    private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
    {
        GraphicsPath path = new GraphicsPath();
        int arcWidth = cornerRadius * 2;
        path.AddArc(rect.X, rect.Y, arcWidth, arcWidth, 180, 90);
        path.AddArc(rect.Right - arcWidth, rect.Y, arcWidth, arcWidth, 270, 90);
        path.AddArc(rect.Right - arcWidth, rect.Bottom - arcWidth, arcWidth, arcWidth, 0, 90);
        path.AddArc(rect.X, rect.Bottom - arcWidth, arcWidth, arcWidth, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        mouseInRegion = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        mouseInRegion = false;

        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            base.Capture = true;
            if (this.Scroll != null)
            {
                this.Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, (int)_trackerValue));
            }

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }

            OnMouseMove(e);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (base.Capture & (e.Button == MouseButtons.Left))
        {
            ScrollEventType type = ScrollEventType.ThumbPosition;
            Point location = e.Location;
            int num = ((_barOrientation == Orientation.Horizontal) ? location.X : location.Y);
            if (_barOrientation == Orientation.Horizontal)
            {

                _trackerValue = _minimum + (decimal)(num - OffsetL) * (_maximum - _minimum) / (decimal)(base.ClientRectangle.Width - OffsetL - OffsetR);

            }
            else
            {
                _trackerValue = _maximum - (decimal)(num - OffsetR) * (_maximum - _minimum) / (decimal)(base.ClientRectangle.Height - OffsetL - OffsetR);
            }

            int num3 = (int)Math.Round(_trackerValue / _smallChange);
            _trackerValue = (decimal)num3 * _smallChange;
            if (_trackerValue <= _minimum)
            {
                _trackerValue = _minimum;
                type = ScrollEventType.First;
            }
            else if (_trackerValue >= _maximum)
            {
                _trackerValue = _maximum;
                type = ScrollEventType.Last;
            }

            if (this.Scroll != null)
            {
                this.Scroll(this, new ScrollEventArgs(type, (int)_trackerValue));
            }

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
        }

        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        base.Capture = false;
        if (this.Scroll != null)
        {
            this.Scroll(this, new ScrollEventArgs(ScrollEventType.EndScroll, (int)_trackerValue));
        }

        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new EventArgs());
        }

        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (mouseInRegion)
        {
            decimal num = (decimal)(e.Delta / 120) * (_maximum - _minimum) / (decimal)_mouseWheelBarPartitions;
            SetProperValue(Value + num);
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        switch (e.KeyCode)
        {
            case Keys.Left:
            case Keys.Down:
                SetProperValue(Value - (decimal)(int)_smallChange);
                if (this.Scroll != null)
                {
                    this.Scroll(this, new ScrollEventArgs(ScrollEventType.SmallDecrement, (int)Value));
                }

                break;
            case Keys.Up:
            case Keys.Right:
                SetProperValue(Value + (decimal)(int)_smallChange);
                if (this.Scroll != null)
                {
                    this.Scroll(this, new ScrollEventArgs(ScrollEventType.SmallIncrement, (int)Value));
                }

                break;
            case Keys.Home:
                Value = _minimum;
                break;
            case Keys.End:
                Value = _maximum;
                break;
            case Keys.Next:
                SetProperValue(Value - (decimal)(int)_largeChange);
                if (this.Scroll != null)
                {
                    this.Scroll(this, new ScrollEventArgs(ScrollEventType.LargeDecrement, (int)Value));
                }

                break;
            case Keys.Prior:
                SetProperValue(Value + (decimal)(int)_largeChange);
                if (this.Scroll != null)
                {
                    this.Scroll(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, (int)Value));
                }

                break;
        }

        if (this.Scroll != null && Value == _minimum)
        {
            this.Scroll(this, new ScrollEventArgs(ScrollEventType.First, (int)Value));
        }

        if (this.Scroll != null && Value == _maximum)
        {
            this.Scroll(this, new ScrollEventArgs(ScrollEventType.Last, (int)Value));
        }

        Point point = PointToClient(Cursor.Position);
        OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, 0));
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData == Keys.Tab) | (Control.ModifierKeys == Keys.Shift))
        {
            return base.ProcessDialogKey(keyData);
        }

        OnKeyDown(new KeyEventArgs(keyData));
        return true;
    }

    public static GraphicsPath CreateRoundRectPath(Rectangle rect, Size size)
    {
        GraphicsPath graphicsPath = new GraphicsPath();
        graphicsPath.AddLine(rect.Left + size.Width / 2, rect.Top, rect.Right - size.Width / 2, rect.Top);
        graphicsPath.AddArc(rect.Right - size.Width, rect.Top, size.Width, size.Height, 270f, 90f);
        graphicsPath.AddLine(rect.Right, rect.Top + size.Height / 2, rect.Right, rect.Bottom - size.Width / 2);
        graphicsPath.AddArc(rect.Right - size.Width, rect.Bottom - size.Height, size.Width, size.Height, 0f, 90f);
        graphicsPath.AddLine(rect.Right - size.Width / 2, rect.Bottom, rect.Left + size.Width / 2, rect.Bottom);
        graphicsPath.AddArc(rect.Left, rect.Bottom - size.Height, size.Width, size.Height, 90f, 90f);
        graphicsPath.AddLine(rect.Left, rect.Bottom - size.Height / 2, rect.Left, rect.Top + size.Height / 2);
        graphicsPath.AddArc(rect.Left, rect.Top, size.Width, size.Height, 180f, 90f);
        return graphicsPath;
    }

    public static Color[] DesaturateColors(params Color[] colorsToDesaturate)
    {
        Color[] array = new Color[colorsToDesaturate.Length];
        for (int i = 0; i < colorsToDesaturate.Length; i++)
        {
            int num = (int)((double)(int)colorsToDesaturate[i].R * 0.3 + (double)(int)colorsToDesaturate[i].G * 0.6 + (double)(int)colorsToDesaturate[i].B * 0.1);
            array[i] = Color.FromArgb(-65793 * (255 - num) - 1);
        }

        return array;
    }

    public static Color[] LightenColors(params Color[] colorsToLighten)
    {
        Color[] array = new Color[colorsToLighten.Length];
        for (int i = 0; i < colorsToLighten.Length; i++)
        {
            array[i] = ControlPaint.Light(colorsToLighten[i]);
        }

        return array;
    }

    private void SetProperValue(decimal val)
    {
        if (val < _minimum)
        {
            Value = _minimum;
        }
        else if (val > _maximum)
        {
            Value = _maximum;
        }
        else
        {
            Value = val;
        }
    }

    private static bool IsPointInRect(Point pt, Rectangle rect)
    {
        if ((pt.X > rect.Left) & (pt.X < rect.Right) & (pt.Y > rect.Top) & (pt.Y < rect.Bottom))
        {
            return true;
        }

        return false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        base.SuspendLayout();
        base.Size = new System.Drawing.Size(200, 48);
        base.ResumeLayout(false);
    }
}
