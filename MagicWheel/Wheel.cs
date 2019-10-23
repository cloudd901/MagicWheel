using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MagicWheel
{
    internal class WheelSize
    {
        public int Diameter { get; set; }
        public float Radius { get; set; }
        public float Top { get; set; }
        public float Left { get; set; }
        public PointF Center { get; set; }
    }
    public class WheelProperties
    {
        public ArrowPosition ArrowPosition { get; set; } = ArrowPosition.Top;
        public Image ArrowImage { get => arrowImage; set { arrowImage = value; isNewArrowImage = true; } }
        internal Image arrowImage = null;
        internal bool isNewArrowImage = false;
        public WheelText TextToShow { get; set; } = WheelText.NameAndID;
        public bool ForceUniqueEntryColors { get; set; } = false;
        public Color LineColor { get; set; } = Color.Black;
        public float LineWidth { get; set; } = 1f;
        public string TextFontFamily { get; set; } = "Arial";
        public FontStyle TextFontStyle { get; set; } = FontStyle.Regular;
        public Color TextColor { get; set; } = Color.Black;
        public bool TextColorAuto { get; set; } = true;
    }
    public class Entry
    {
        public int UniqueID { get; set; } = -1;
        public string Name { get; set; }
        public Color Aura { get; set; } = Color.White;
        internal float WheelLocation { get; set; } = 0;
    }
    public enum WheelText
    {
        NameAndID = 0,
        Name = 1,
        ID = 2,
        None = 3
    }
    public enum ArrowPosition
    {
        Top=0,
        Right=1,
        Bottom=2,
        Left=3
    }
    public enum SpinPowerType
    {
        Manual = 0,
        Weak = 1,
        Average = 2,
        Strong = 3,
        Super = 4,
        Random = 5
    }
    public enum SpinDirection
    {
        Clockwise = 0,
        CounterClockwise = 1
    }
    public partial class Wheel
    {
        public delegate void WheelSpinInfoEventHandler(Entry entry, string[] spinInfo);
        public event WheelSpinInfoEventHandler WheelSpinCall;

        public delegate void WheelStopInfoEventHandler(Entry entry);
        public event WheelStopInfoEventHandler WheelStopCall;

        private readonly PictureBox _ControlWheel = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), Size = new Size(0, 0), BorderStyle = BorderStyle.None, Visible = false };

        private readonly PictureBox _ControlArrow = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), ClientSize = new Size(20, 20), BorderStyle = BorderStyle.None, Visible = false };

        private Form ContentWindow;

        private readonly WheelSize _WheelSize = new WheelSize();
        public readonly WheelProperties _WheelProperties = new WheelProperties();

        public List<Entry> EntryList { get; } = new List<Entry>();

        public bool AllowExceptions { get; set; } = true;
        public bool IsSpinning { get; private set; } = false;
        private bool spinStop = false;

        private Image wheelImage = null;

        private ArrowPosition currentArrowDirection = ArrowPosition.Top;

        public Wheel(Form contentForm)
        {
            ContentWindow = contentForm ?? throw new NullReferenceException("A valid control must be used.");
            ContentWindow.Controls.Add(_ControlWheel);
            _ControlWheel.Controls.Add(_ControlArrow);
        }

        private void SetPictureBoxes()
        {
            _ControlWheel.Top = (int)_WheelSize.Top;
            _ControlWheel.Left = (int)_WheelSize.Left;
            _ControlWheel.ClientSize = new Size(_WheelSize.Diameter, _WheelSize.Diameter);
            _ControlWheel.Visible = true;
            if (_WheelProperties.ArrowPosition == ArrowPosition.Top)
            {
                _ControlArrow.Top = 0;
                _ControlArrow.Left = (int)_WheelSize.Center.X - 10;
            }
            else if (_WheelProperties.ArrowPosition == ArrowPosition.Right)
            {
                _ControlArrow.Top = (int)_WheelSize.Center.X - 10;
                _ControlArrow.Left = (int)_WheelSize.Diameter - 20;
            }
            else if (_WheelProperties.ArrowPosition == ArrowPosition.Bottom)
            {
                _ControlArrow.Top = (int)_WheelSize.Diameter - 20;
                _ControlArrow.Left = (int)_WheelSize.Center.X - 10;
            }
            else if (_WheelProperties.ArrowPosition == ArrowPosition.Left)
            {
                _ControlArrow.Top = (int)_WheelSize.Center.X - 10;
                _ControlArrow.Left = 0;
            }
            _ControlArrow.Visible = true;
        }
    }
}
