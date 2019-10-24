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
        internal bool initialized { get; set; } = false;
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
        public bool ShadowVisible { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.DarkGray;
        public ShadowPosition ShadowPosition { get; set; } = ShadowPosition.BottomRight;
        public int ShadowLength { get; set; } = 6;
        public bool CenterDotVisible { get; set; } = true;
        public Color CenterDotColor { get; set; } = Color.DarkGray;
        public int CenterDotSize { get; set; } = 2;
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
    public enum ShadowPosition
    {
        BottomRight = 0,
        BottomLeft = 1,
        TopLeft = 2,
        TopRight = 3
    }
    public partial class Wheel : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public delegate void WheelSpinInfoEventHandler(Entry entry, string[] spinInfo);
        public event WheelSpinInfoEventHandler WheelSpinCall;

        public delegate void WheelStopInfoEventHandler(Entry entry);
        public event WheelStopInfoEventHandler WheelStopCall;

        private Form ContentWindow;

        private readonly PictureBox _ControlWheel = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), Size = new Size(0, 0), BorderStyle = BorderStyle.None, Visible = false };
        private readonly PictureBox _ControlWheel3D = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), Size = new Size(0, 0), BorderStyle = BorderStyle.None, Visible = false };

        private Image wheelImage = null;
        private Image wheelImage3D = null;

        private readonly PictureBox _ControlArrow = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), ClientSize = new Size(20, 20), BorderStyle = BorderStyle.None, Visible = false };

        private readonly WheelSize _WheelSize = new WheelSize();
        public readonly WheelProperties _WheelProperties = new WheelProperties();

        public List<Entry> EntryList { get; } = new List<Entry>();

        public bool AllowExceptions { get; set; } = true;
        public bool IsSpinning { get; private set; } = false;
        private bool spinStop = false;

        private ArrowPosition currentArrowDirection = ArrowPosition.Top;

        public Wheel(Form contentForm)
        {
            ContentWindow = contentForm ?? throw new NullReferenceException("A valid control must be used.");
            ContentWindow.Controls.Add(_ControlWheel3D);
            _ControlWheel3D.Controls.Add(_ControlWheel);
            _ControlWheel.Controls.Add(_ControlArrow);
        }

        public void BringToFront()
        {
            _ControlWheel3D.BringToFront();
            _ControlWheel3D.Update();
            _ControlWheel.Update();
            _ControlArrow.Update();
        }
        public void SendToBack()
        {
            _ControlWheel3D.SendToBack();
            _ControlWheel3D.Update();
            _ControlWheel.Update();
            _ControlArrow.Update();
        }
        private Point[] GetPictureBoxPoints()
        {
            Point wheelPoint = new Point(0, 0);
            Point shadowPoint = new Point(0, 0);
            if (_WheelProperties.ShadowVisible)
            {
                shadowPoint = new Point(_WheelProperties.ShadowLength, _WheelProperties.ShadowLength);
                if (_WheelProperties.ShadowPosition == ShadowPosition.BottomLeft)
                {
                    shadowPoint = new Point(0, _WheelProperties.ShadowLength);
                    wheelPoint = new Point(_WheelProperties.ShadowLength, 0);
                }
                else if (_WheelProperties.ShadowPosition == ShadowPosition.TopLeft)
                {
                    shadowPoint = new Point(0, 0);
                    wheelPoint = new Point(_WheelProperties.ShadowLength, _WheelProperties.ShadowLength);
                }
                else if (_WheelProperties.ShadowPosition == ShadowPosition.TopRight)
                {
                    shadowPoint = new Point(_WheelProperties.ShadowLength, 0);
                    wheelPoint = new Point(0, _WheelProperties.ShadowLength);
                }
            }
            return new Point[] { shadowPoint, wheelPoint };
        }
        private void SetPictureBoxes()
        {
            Point[] controlPoints = GetPictureBoxPoints();

            _ControlWheel3D.Top = (int)_WheelSize.Top;
            _ControlWheel3D.Left = (int)_WheelSize.Left;
            _ControlWheel3D.ClientSize = new Size(_WheelSize.Diameter + _WheelProperties.ShadowLength, _WheelSize.Diameter + _WheelProperties.ShadowLength);
            _ControlWheel3D.Visible = true;

            _ControlWheel.Top = controlPoints[1].Y;
            _ControlWheel.Left = controlPoints[1].X;
            _ControlWheel.ClientSize = new Size(_WheelSize.Diameter + _WheelProperties.ShadowLength, _WheelSize.Diameter + _WheelProperties.ShadowLength);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                EntriesClear();

                wheelImage3D.Dispose();
                wheelImage.Dispose();
                _WheelProperties.arrowImage.Dispose();

                _ControlArrow.Dispose();
                _ControlWheel.Dispose();
                _ControlWheel3D.Dispose();

                _WheelSize.initialized = false;
            }

            IsDisposed = true;
        }
    }
}
