using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MagicWheel
{
    public partial class Wheel
    {
        public void Draw(PointF center, float radius)
        {
            _WheelSize.Left = center.X - radius;
            _WheelSize.Top = center.Y - radius;
            _WheelSize.Radius = radius;
            _WheelSize.Diameter = (int)(radius * 2f);
            _WheelSize.Center = center;
            ReDraw();
        }
        public void Draw(int left, int top, float radius)
        {
            _WheelSize.Left = left;
            _WheelSize.Top = top;
            _WheelSize.Radius = radius;
            _WheelSize.Diameter = (int)(radius * 2f);
            _WheelSize.Center = new PointF(radius, radius);
            ReDraw();
        }
        private void DrawArrow()
        {
            if (arrowImage == null)
            {
                arrowImage = new Bitmap(20, 20);
                using (Graphics graphics = Graphics.FromImage(arrowImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    graphics.FillPolygon(Brushes.Black, new Point[] { new Point(0, 0), new Point(20, 0), new Point(10, 12), });
                    graphics.FillPolygon(Brushes.White, new Point[] { new Point(5, 2), new Point(15, 2), new Point(10, 5), });
                    graphics.DrawImage(arrowImage, new Point(0, 0));
                }
                _ControlArrow.Image = arrowImage;
            }
            else { _ControlArrow.Image = arrowImage; }

            if (ArrowLocation != currentArrowDirection || isNewArrowImage)
            {
                _ControlArrow.Visible = false;
                Bitmap newImage = new Bitmap(20, 20);
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.Clear(Color.Transparent);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    
                    Matrix rotationMatrix = new Matrix();
                    rotationMatrix.RotateAt(0, new PointF(10, 10));
                    graphics.Transform = rotationMatrix;

                    if (currentArrowDirection == ArrowLocation.Top || isNewArrowImage)
                    {
                        rotationMatrix.RotateAt(90 * (long)ArrowLocation, new PointF(10, 10));
                    }
                    else if (currentArrowDirection == ArrowLocation.Right)
                    {
                        if (ArrowLocation == ArrowLocation.Bottom)
                        { rotationMatrix.RotateAt(90, new PointF(10, 10)); }
                        if (ArrowLocation == ArrowLocation.Left)
                        { rotationMatrix.RotateAt(180, new PointF(10, 10)); }
                        if (ArrowLocation == ArrowLocation.Top)
                        { rotationMatrix.RotateAt(270, new PointF(10, 10)); }
                    }
                    else if (currentArrowDirection == ArrowLocation.Bottom)
                    {
                        if (ArrowLocation == ArrowLocation.Left)
                        { rotationMatrix.RotateAt(90, new PointF(10, 10)); }
                        if (ArrowLocation == ArrowLocation.Top)
                        { rotationMatrix.RotateAt(180, new PointF(10, 10)); }
                        if (ArrowLocation == ArrowLocation.Right)
                        { rotationMatrix.RotateAt(270, new PointF(10, 10)); }
                    }
                    else if (currentArrowDirection == ArrowLocation.Left)
                    {
                        if (ArrowLocation == ArrowLocation.Top)
                        { rotationMatrix.RotateAt(90, new PointF(10, 10)); }
                        if (ArrowLocation == ArrowLocation.Right)
                        { rotationMatrix.RotateAt(180, new PointF(10, 10)); }
                        if (ArrowLocation == ArrowLocation.Bottom)
                        { rotationMatrix.RotateAt(270, new PointF(10, 10)); }
                    }
                    currentArrowDirection = ArrowLocation;

                    graphics.Clear(_ControlArrow.BackColor);
                    graphics.Transform = rotationMatrix;
                    graphics.DrawImage(arrowImage, new Point(0, 0));
                }
                arrowImage = newImage;
                _ControlArrow.Image = arrowImage;
                _ControlArrow.Visible = true;
                isNewArrowImage = false;
            }
            _ControlArrow.Refresh();
        }
        private void DrawWheel()
        {
            int entries = EntryList.Count;
            SetPictureBoxes();
            wheelImage = new Bitmap(_WheelSize.Diameter, _WheelSize.Diameter);
            Image namesImage = new Bitmap(_WheelSize.Diameter, _WheelSize.Diameter);
            Random r = new Random();
            using (Graphics graphics = Graphics.FromImage(wheelImage))
            {
                Brush brush;
                Pen pen = new Pen(_WheelProperties.LineColor, _WheelProperties.LineWidth);
                Font drawFont;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                graphics.FillRectangle(new SolidBrush(ContentWindow.BackColor), new RectangleF(0, 0, _ControlWheel.Width, _ControlWheel.Height));

                float angle = 360f / (float)entries;
                float currentangle = angle;

                List<PointF> pointList = new List<PointF>();
                //_locationData.Clear();
                //_locationData.Add(0);
                //EntryList[0].WheelLocation = 0;
                for (int i = 0; i <= entries-1; i++)
                {
                    //if (currentangle != 360) { _locationData.Add(currentangle); }
                    EntryList[i].WheelLocation = currentangle - angle;
                    Color randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                    if (i == 0) { randomColor = Color.White; }
                    brush = new SolidBrush(EntryList[i].Aura);
                    PointF newPoint = new PointF((_WheelSize.Radius - 10) * (float)Math.Cos(currentangle * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 10) * (float)Math.Sin(currentangle * Math.PI / 180F) + _WheelSize.Center.Y);
                    pointList.Add(newPoint);
                    graphics.FillPie(brush, 1, 1, _WheelSize.Diameter-2, _WheelSize.Diameter-2, currentangle, angle);
                    graphics.DrawLine(pen, _WheelSize.Center, new PointF((_WheelSize.Radius - 1) * (float)Math.Cos(currentangle * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 1) * (float)Math.Sin(currentangle * Math.PI / 180F) + _WheelSize.Center.Y));

                    currentangle += angle;
                }

                graphics.DrawEllipse(pen, 1, 1, _WheelSize.Diameter-2, _WheelSize.Diameter-2);

                if (_WheelProperties.ShowWheelText != WheelText.None)
                {
                    using (Graphics graphicsNames = Graphics.FromImage(wheelImage))
                    {
                        graphicsNames.SmoothingMode = SmoothingMode.AntiAlias;
                        graphicsNames.CompositingQuality = CompositingQuality.HighQuality;
                        graphicsNames.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        for (int i = 0; i <= entries - 1; i++)
                        {
                            if (_WheelProperties.TextColorAuto)
                            {
                                if (!IsReadable(EntryList[i].Aura, Color.Black)) { brush = Brushes.White; }
                                else { brush = Brushes.Black; }
                            }
                            else
                            {
                                brush = new SolidBrush(_WheelProperties.TextColor);
                            }

                            PointF midPoint1 = new Point(0, 0);
                            PointF midPoint2 = new Point(0, 0);
                            PointF midPoint = new Point(0, 0);

                            PointF usePoint1 = pointList[i];
                            PointF usePoint2 = new Point(0, 0);
                            try { usePoint2 = pointList[i + 1]; } catch { usePoint2 = pointList[0]; }

                            if (entries < 3)
                            {
                                if (i == 0)
                                {
                                    midPoint1 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(45 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(45 * Math.PI / 180F) + _WheelSize.Center.Y);
                                    midPoint2 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(135 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(135 * Math.PI / 180F) + _WheelSize.Center.Y);
                                }
                                else if (i == 1)
                                {
                                    midPoint1 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(225 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(225 * Math.PI / 180F) + _WheelSize.Center.Y);
                                    midPoint2 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(315 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(315 * Math.PI / 180F) + _WheelSize.Center.Y);
                                }
                            }
                            else if (entries == 3)
                            {
                                if (i == 0)
                                {
                                    midPoint1 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(30 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(30 * Math.PI / 180F) + _WheelSize.Center.Y);
                                    midPoint2 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(90 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(90 * Math.PI / 180F) + _WheelSize.Center.Y);
                                }
                                else if (i == 1)
                                {
                                    midPoint1 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(150 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(150 * Math.PI / 180F) + _WheelSize.Center.Y);
                                    midPoint2 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(210 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(210 * Math.PI / 180F) + _WheelSize.Center.Y);
                                }
                                else if (i == 2)
                                {
                                    midPoint1 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(270 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(270 * Math.PI / 180F) + _WheelSize.Center.Y);
                                    midPoint2 = new PointF((_WheelSize.Radius - 20) * (float)Math.Cos(330 * Math.PI / 180F) + _WheelSize.Center.X, (_WheelSize.Radius - 20) * (float)Math.Sin(330 * Math.PI / 180F) + _WheelSize.Center.Y);
                                }
                            }
                            else
                            {
                                midPoint1 = MidPoint(usePoint1, usePoint2);
                                midPoint2 = MidPoint(midPoint1, usePoint2);
                            }
                            if (entries >= 35)
                            {
                                if (entries > 70) { midPoint1 = usePoint2; midPoint2 = usePoint2; }
                                else
                                {
                                    int repeat = (entries / 10) - 1;
                                    if (entries > 45) { repeat += 1; }
                                    if (entries > 55) { repeat += 2; }
                                    if (entries > 60) { repeat += 3; }
                                    for (int i2 = 1; i2 <= repeat; i2++)
                                    { midPoint2 = MidPoint(midPoint2, usePoint2); }
                                }
                            }
                            midPoint = MidPoint(midPoint2, midPoint1);

                            //Adjust Text Angle
                            double deltaX = Math.Pow((midPoint.X - _WheelSize.Center.X), 2);
                            double deltaY = Math.Pow((midPoint.Y - _WheelSize.Center.Y), 2);
                            double radians = Math.Atan2((_WheelSize.Center.Y - midPoint.Y), (_WheelSize.Center.X - midPoint.X));
                            double textAngle = radians * ((180) / Math.PI);
                            Matrix rotationMatrix = new Matrix();
                            rotationMatrix.RotateAt((float)textAngle - 2, midPoint);
                            graphicsNames.Transform = rotationMatrix;

                            drawFont = new Font(_WheelProperties.TextFontFamily, GetCorrectSize(EntryList[i].Name), _WheelProperties.TextFontStyle);
                            string drawText = "";
                            if (_WheelProperties.ShowWheelText == WheelText.NameAndID) { drawText = $"{EntryList[i].Name}_{EntryList[i].UniqueID}"; }
                            else if (_WheelProperties.ShowWheelText == WheelText.Name) { drawText = $"{EntryList[i].Name}"; }
                            else if (_WheelProperties.ShowWheelText == WheelText.ID) { drawText = $"{EntryList[i].UniqueID}"; }
                            graphicsNames.DrawString(drawText, drawFont, brush, midPoint, StringFormat.GenericTypographic);
                        }
                        graphicsNames.DrawImage(namesImage, new Point(0, 0));
                    }
                }
                graphics.DrawImage(wheelImage, new Point(0,0));
                _ControlWheel.Image = wheelImage;
                _ControlWheel.Refresh();
            }
        }

        private void ReDraw()
        {
            if (IsSpinning) { return; }
            DrawWheel();
            DrawArrow();
        }

        private PointF MidPoint(PointF p1, PointF p2)
        {
            p1.X -= _WheelSize.Left; p2.X += _WheelSize.Left;
            PointF p = new PointF(0f, 0f);
            p = new PointF ((float)((p1.X + p2.X) / 2f) , (float)((p1.Y + p2.Y) / 2f));
            return p;
        }
        private int GetCorrectSize(string word)
        {
            int size = 9;
            if (_WheelSize.Radius <= 70)
            { size = 5; }

            else if(_WheelSize.Radius < 150 && word.Length <= 5)
            { size = 7; }
            else if (_WheelSize.Radius < 150 && word.Length > 5 && word.Length <= 15)
            { size = 6; }
            else if (_WheelSize.Radius < 150 && word.Length >= 15)
            { size = 5; }

            else if (_WheelSize.Radius >= 150 && _WheelSize.Radius < 250 && word.Length <= 5)
            { size = 10; }
            else if (_WheelSize.Radius >= 150 && _WheelSize.Radius < 250 && word.Length > 5 && word.Length <= 15)
            { size = 9; }
            else if (_WheelSize.Radius >= 150 && _WheelSize.Radius < 250 && word.Length > 15)
            { size = 8; }

            else if (_WheelSize.Radius >= 250 && _WheelSize.Radius < 350 && word.Length <= 5)
            { size = 13; }
            else if (_WheelSize.Radius >= 250 && _WheelSize.Radius < 350 && word.Length > 5 && word.Length <= 15)
            { size = 12; }
            else if (_WheelSize.Radius >= 250 && _WheelSize.Radius < 350 && word.Length > 15)
            { size = 11; }

            else if (_WheelSize.Radius >= 350 && _WheelSize.Radius < 500 && word.Length <= 5)
            { size = 16; }
            else if (_WheelSize.Radius >= 350 && _WheelSize.Radius < 500 && word.Length > 5 && word.Length <= 15)
            { size = 15; }
            else if (_WheelSize.Radius >= 350 && _WheelSize.Radius < 500 && word.Length > 15)
            { size = 14; }

            else if (_WheelSize.Radius >= 500)
            { size = 16; }
            return size;
        }
        public bool IsReadable(Color color1, Color color2)
        {
            return Math.Abs(color1.GetBrightness() - color2.GetBrightness()) >= 0.5f;
        }
    }
}
