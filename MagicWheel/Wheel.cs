using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RandomTool
{
    public delegate void ToolActionEventHandler(object entry, string[] actionInfo);
    public delegate void ToolStopEventHandler(object entry);

    public partial class Wheel : IRandomTool, IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public event ToolActionEventHandler ToolActionCall;
        public event ToolStopEventHandler ToolStopCall;

        private readonly PictureBox _ControlWheel = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), Size = new Size(0, 0), BorderStyle = BorderStyle.None, Visible = false };
        private readonly PictureBox _ControlWheel3D = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), Size = new Size(0, 0), BorderStyle = BorderStyle.None, Visible = false };
        private readonly PictureBox _ControlArrow = new PictureBox()
        { BackColor = Color.Transparent, Location = new Point(0, 0), ClientSize = new Size(20, 20), BorderStyle = BorderStyle.None, Visible = false };

        private ObjectSize ToolSize { get; } = new ObjectSize();
        public ToolProperties ToolProperties { get; set; } = new ToolProperties();
        public List<Entry> EntryList { get; } = new List<Entry>();
        public bool AllowExceptions { get; set; } = true;
        public bool IsBusy { get; private set; } = false;

        public Wheel(Form contentForm)
        {
            Form ContentWindow = contentForm ?? throw new NullReferenceException("A valid control must be used.");
            _ControlWheel3D.Parent = ContentWindow;
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
            if (ToolProperties.ShadowVisible)
            {
                shadowPoint = new Point(ToolProperties.ShadowLength, ToolProperties.ShadowLength);
                if (ToolProperties.ShadowPosition == ShadowPosition.BottomLeft)
                {
                    shadowPoint = new Point(0, ToolProperties.ShadowLength);
                    wheelPoint = new Point(ToolProperties.ShadowLength, 0);
                }
                else if (ToolProperties.ShadowPosition == ShadowPosition.TopLeft)
                {
                    shadowPoint = new Point(0, 0);
                    wheelPoint = new Point(ToolProperties.ShadowLength, ToolProperties.ShadowLength);
                }
                else if (ToolProperties.ShadowPosition == ShadowPosition.TopRight)
                {
                    shadowPoint = new Point(ToolProperties.ShadowLength, 0);
                    wheelPoint = new Point(0, ToolProperties.ShadowLength);
                }
            }
            return new Point[] { shadowPoint, wheelPoint };
        }
        private void SetPictureBoxes()
        {
            Point[] controlPoints = GetPictureBoxPoints();

            _ControlWheel3D.Top = (int)ToolSize.Top;
            _ControlWheel3D.Left = (int)ToolSize.Left;
            _ControlWheel3D.ClientSize = new Size(ToolSize.Diameter + ToolProperties.ShadowLength, ToolSize.Diameter + ToolProperties.ShadowLength);
            _ControlWheel3D.Visible = true;

            _ControlWheel.Top = controlPoints[1].Y;
            _ControlWheel.Left = controlPoints[1].X;
            _ControlWheel.ClientSize = new Size(ToolSize.Diameter + ToolProperties.ShadowLength, ToolSize.Diameter + ToolProperties.ShadowLength);
            _ControlWheel.Visible = true;
            if (ToolProperties.ArrowPosition == ArrowLocation.Top)
            {
                _ControlArrow.Top = 0;
                _ControlArrow.Left = (int)ToolSize.Center.X - 10;
            }
            else if (ToolProperties.ArrowPosition == ArrowLocation.Right)
            {
                _ControlArrow.Top = (int)ToolSize.Center.X - 10;
                _ControlArrow.Left = (int)ToolSize.Diameter - 20;
            }
            else if (ToolProperties.ArrowPosition == ArrowLocation.Bottom)
            {
                _ControlArrow.Top = (int)ToolSize.Diameter - 20;
                _ControlArrow.Left = (int)ToolSize.Center.X - 10;
            }
            else if (ToolProperties.ArrowPosition == ArrowLocation.Left)
            {
                _ControlArrow.Top = (int)ToolSize.Center.X - 10;
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
            { return; }

            if (disposing)
            {
                EntriesClear();

                ToolProperties.objectImage3D.Dispose();
                ToolProperties.objectImage.Dispose();
                ToolProperties.arrowImage.Dispose();

                _ControlArrow.Dispose();
                _ControlWheel.Dispose();
                _ControlWheel3D.Dispose();

                ToolSize.initialized = false;
            }

            IsDisposed = true;
        }
    }
}
