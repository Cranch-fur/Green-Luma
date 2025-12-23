using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;





public class ColoredProgressBar : ProgressBar
{
    [Category("Appearance")]
    [Description("The color of the progress bar.")]
    public Color Color { get; set; } = Color.Blue;


    [Browsable(false)] // Hides the ForeColor property from the Properties window
    [EditorBrowsable(EditorBrowsableState.Never)] // Hides it from IntelliSense
    public override Color ForeColor { get; set; } = SystemColors.Highlight;


    [Category("Appearance")]
    [Description("Whether or not to show the border.")]
    public bool ShowBorder { get; set; } = false;


    [Category("Appearance")]
    [Description("The thickness of the border.")]
    public int BorderThickness { get; set; } = 1;


    [Category("Appearance")]
    [Description("The color of the border.")]
    public Color BorderColor { get; set; } = Color.Black;


    public ColoredProgressBar()
    {
        // Prevent default style from being rendered
        SetStyle(ControlStyles.UserPaint, true);
    }


    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        this.DoubleBuffered = true;
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        using (SolidBrush backgroundBrush = new SolidBrush(SystemColors.Control))
        {
            using (SolidBrush progressBrush = new SolidBrush(Color))
            {
                // Draw background
                e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);

                // Calculate and draw progress rectangle
                Rectangle progressRect = new Rectangle(0, 0, (int)(this.Width * ((double)Value / Maximum)), this.Height);
                e.Graphics.FillRectangle(progressBrush, progressRect);
            }
        }


        if (ShowBorder)
        {
            using (Pen borderPen = new Pen(BorderColor, BorderThickness))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - BorderThickness, this.Height - BorderThickness);
            }
        }
    }
}


public class GradientProgressBar : ProgressBar
{
    [Category("Appearance")]
    [Description("The starting color of the gradient.")]
    public Color StartColor { get; set; } = Color.LimeGreen;


    [Category("Appearance")]
    [Description("The ending color of the gradient.")]
    public Color EndColor { get; set; } = Color.IndianRed;


    [Category("Appearance")]
    [Description("Whether or not to show the border.")]
    public bool ShowBorder { get; set; } = false;


    [Category("Appearance")]
    [Description("The thickness of the border.")]
    public int BorderThickness { get; set; } = 1;


    [Category("Appearance")]
    [Description("The color of the border.")]
    public Color BorderColor { get; set; } = Color.Black;


    [Browsable(false)] // Hides the ForeColor property from the Properties window
    [EditorBrowsable(EditorBrowsableState.Never)] // Hides it from IntelliSense
    public override Color ForeColor { get; set; } = SystemColors.Highlight;


    public GradientProgressBar()
    {
        // Prevent default style from being rendered
        SetStyle(ControlStyles.UserPaint, true);
    }


    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        this.DoubleBuffered = true;
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        // Fill the background
        using (SolidBrush backgroundBrush = new SolidBrush(SystemColors.Control))
        {
            e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);
        }


        // Draw the gradient progress
        if (Value > 0)
        {
            double progressFraction = (double)Value / Maximum;
            Rectangle progressRect = new Rectangle(0, 0, (int)(this.Width * progressFraction), this.Height);

            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                progressRect,
                StartColor,
                EndColor,
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(gradientBrush, progressRect);
            }
        }


        // Optionally draw border
        if (ShowBorder)
        {
            using (Pen borderPen = new Pen(BorderColor, BorderThickness))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

    }
}
