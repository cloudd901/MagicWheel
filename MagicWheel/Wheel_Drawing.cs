using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RandomTool
{
    public partial class Wheel
    {
        public void Draw(PointF center, float radius)
        {
            if (radius < 1) { if (AllowExceptions) { throw new ArgumentOutOfRangeException ("Radius must be greater than 1"); } else { return; } }
            ToolSize.Left = center.X - radius;
            ToolSize.Top = center.Y - radius;
            ToolSize.Radius = radius;
            ToolSize.Diameter = (int)(radius * 2f);
            ToolSize.Center = center;
            ToolSize.initialized = true;
            Refresh();
        }
        public void Draw(int left, int top, float radius)
        {
            if (radius < 1) { if (AllowExceptions) { throw new ArgumentOutOfRangeException("Radius must be greater than 1"); } else { return; } }
            ToolSize.Left = left;
            ToolSize.Top = top;
            ToolSize.Radius = radius;
            ToolSize.Diameter = (int)(radius * 2f);
            ToolSize.Center = new PointF(radius, radius);
            ToolSize.initialized = true;
            Refresh();
        }
        private void DrawArrow()
        {
            if (ToolProperties.arrowImage == null)
            {
                ToolProperties.arrowImage = new Bitmap(20, 20);
                using (Graphics graphics = Graphics.FromImage(ToolProperties.arrowImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    graphics.FillPolygon(Brushes.Black, new Point[] { new Point(0, 0), new Point(20, 0), new Point(10, 12), });
                    graphics.FillPolygon(Brushes.White, new Point[] { new Point(5, 2), new Point(15, 2), new Point(10, 5), });
                    graphics.DrawImage(ToolProperties.arrowImage, new Point(0, 0));
                }
            }
            _ControlArrow.Image = ToolProperties.arrowImage;

            if (ToolProperties.ArrowPosition != ToolProperties.currentArrowDirection || ToolProperties.isNewArrowImage)
            {
                _ControlArrow.Visible = false;
                Bitmap newImage = new Bitmap(20, 20);
                PointF arrowCenter = new PointF((float)newImage.Width/2, (float)newImage.Height/2);
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.Clear(Color.Transparent);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    
                    Matrix rotationMatrix = new Matrix();
                    rotationMatrix.RotateAt(0, arrowCenter);
                    graphics.Transform = rotationMatrix;

                    if (ToolProperties.currentArrowDirection == ArrowLocation.Top || ToolProperties.isNewArrowImage)
                    {
                        rotationMatrix.RotateAt(90 * (long)ToolProperties.ArrowPosition, arrowCenter);
                    }
                    else if (ToolProperties.currentArrowDirection == ArrowLocation.Right)
                    {
                        if (ToolProperties.ArrowPosition == ArrowLocation.Bottom)
                        { rotationMatrix.RotateAt(90, arrowCenter); }
                        else if (ToolProperties.ArrowPosition == ArrowLocation.Left)
                        { rotationMatrix.RotateAt(180, arrowCenter); }
                        else if (ToolProperties.ArrowPosition == ArrowLocation.Top)
                        { rotationMatrix.RotateAt(270, arrowCenter); }
                    }
                    else if (ToolProperties.currentArrowDirection == ArrowLocation.Bottom)
                    {
                        if (ToolProperties.ArrowPosition == ArrowLocation.Left)
                        { rotationMatrix.RotateAt(90, arrowCenter); }
                        else if (ToolProperties.ArrowPosition == ArrowLocation.Top)
                        { rotationMatrix.RotateAt(180, arrowCenter); }
                        else if (ToolProperties.ArrowPosition == ArrowLocation.Right)
                        { rotationMatrix.RotateAt(270, arrowCenter); }
                    }
                    else if (ToolProperties.currentArrowDirection == ArrowLocation.Left)
                    {
                        if (ToolProperties.ArrowPosition == ArrowLocation.Top)
                        { rotationMatrix.RotateAt(90, arrowCenter); }
                        else if (ToolProperties.ArrowPosition == ArrowLocation.Right)
                        { rotationMatrix.RotateAt(180, arrowCenter); }
                        else if (ToolProperties.ArrowPosition == ArrowLocation.Bottom)
                        { rotationMatrix.RotateAt(270, arrowCenter); }
                    }
                    ToolProperties.currentArrowDirection = ToolProperties.ArrowPosition;

                    graphics.Clear(_ControlArrow.BackColor);
                    graphics.Transform = rotationMatrix;
                    graphics.DrawImage(ToolProperties.arrowImage, new Point(0, 0));
                }
                ToolProperties.arrowImage = newImage;
                _ControlArrow.Image = ToolProperties.arrowImage;
                _ControlArrow.Visible = true;
                ToolProperties.isNewArrowImage = false;
            }
            _ControlArrow.Refresh();
        }
        private void DrawWheel()
        {
            if (!ToolSize.initialized) { if (AllowExceptions) { throw new InvalidOperationException("Please initialize wheel using Draw()."); } else { return; } }
            int entries = EntryList.Count;
            //if (entries < 1) { if (AllowExceptions) { throw new IndexOutOfRangeException("Must have more than one Entry"); } else { return; } }
            SetPictureBoxes();

            int shadowLength = ToolProperties.ShadowVisible ? ToolProperties.ShadowLength : 0;
            ToolProperties.objectImage = new Bitmap(ToolSize.Diameter + shadowLength, ToolSize.Diameter + shadowLength);

            using (Graphics graphics = Graphics.FromImage(ToolProperties.objectImage))
            {
                Point[] controlPoints = GetPictureBoxPoints();
                Pen pen = new Pen(ToolProperties.LineColor, ToolProperties.LineWidth);
                
                    
                ToolProperties.objectImage3D = new Bitmap(ToolSize.Diameter + shadowLength, ToolSize.Diameter + shadowLength);
                using (Graphics graphics3D = Graphics.FromImage(ToolProperties.objectImage3D))
                {
                    if (ToolProperties.ShadowVisible)
                    {
                        graphics3D.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics3D.CompositingQuality = CompositingQuality.HighQuality;
                        graphics3D.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        graphics3D.FillEllipse(new SolidBrush(ToolProperties.ShadowColor), controlPoints[0].X, controlPoints[0].Y, ToolSize.Diameter - 2, ToolSize.Diameter - 2);
                        graphics3D.DrawEllipse(pen, controlPoints[0].X, controlPoints[0].Y, ToolSize.Diameter - 2, ToolSize.Diameter - 2);

                        graphics3D.DrawImage(ToolProperties.objectImage3D, new Point(0, 0));
                    }
                    _ControlWheel3D.Image = ToolProperties.objectImage3D;
                    _ControlWheel3D.Refresh();
                }
                
                Image namesImage = new Bitmap(ToolSize.Diameter, ToolSize.Diameter);
                Random r = new Random();
                Brush brush;
                Font drawFont;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                float angle = 360f / (float)entries;
                float currentangle = angle;

                List<PointF> pointList = new List<PointF>();
                PointF linePoint;
                for (int i = 0; i <= entries - 1; i++)
                {
                    EntryList[i].EntryLocation = currentangle - angle;
                    pointList.Add(new PointF((ToolSize.Radius - 10) * (float)Math.Cos(currentangle * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 10) * (float)Math.Sin(currentangle * Math.PI / 180F) + ToolSize.Center.Y));

                    int colorCorrection = i;
                    //Color correction needed for less than 4 entries
                    if (entries <= 2) { colorCorrection -= 1; if (colorCorrection < 0) { colorCorrection = entries - 1; } }
                    else if (entries == 3) { colorCorrection -= 2; if (colorCorrection == -1) { colorCorrection = entries - 1; } else if (colorCorrection == -2) { colorCorrection = entries - 2; } }

                    Color randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                    if (i == 0) { randomColor = Color.White; }
                    brush = new SolidBrush(EntryList[colorCorrection].Aura);

                    graphics.FillPie(brush, 1, 1, ToolSize.Diameter - 2, ToolSize.Diameter - 2, currentangle, angle);

                    linePoint = new PointF((ToolSize.Radius - 1) * (float)Math.Cos(currentangle * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 1) * (float)Math.Sin(currentangle * Math.PI / 180F) + ToolSize.Center.Y);

                    PointF newMidPoint;
                    if (ToolProperties.CenterVisible)
                    { newMidPoint = MidPoint(ToolSize.Center, linePoint, 4); }
                    else
                    { newMidPoint = ToolSize.Center; }
                    graphics.DrawLine(pen, newMidPoint, linePoint);
                    currentangle += angle;
                }
                linePoint = new PointF((ToolSize.Radius - 1) * (float)Math.Cos(currentangle * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 1) * (float)Math.Sin(currentangle * Math.PI / 180F) + ToolSize.Center.Y);
                PointF newPoint = MidPoint(ToolSize.Center, linePoint, 4);
                graphics.DrawLine(pen, newPoint, linePoint);

                if (entries <= 0) { graphics.FillEllipse(new SolidBrush(Color.LightGray), 1, 1, ToolSize.Diameter - 2, ToolSize.Diameter - 2); }
                graphics.DrawEllipse(pen, 1, 1, ToolSize.Diameter - 2, ToolSize.Diameter - 2);
                if (ToolProperties.CenterVisible)
                {
                    float centRadius = (float)(Math.Sqrt(Math.Pow((newPoint.X - ToolSize.Center.X), 2) + Math.Pow((newPoint.Y - ToolSize.Center.Y), 2))) * ToolProperties.CenterSize;
                    graphics.FillEllipse(new SolidBrush(ToolProperties.CenterColor), ToolSize.Center.X - (centRadius / 2), ToolSize.Center.Y - (centRadius / 2), centRadius, centRadius);
                    graphics.DrawEllipse(pen, ToolSize.Center.X - (centRadius / 2), ToolSize.Center.Y - (centRadius / 2), centRadius, centRadius);
                }

                if (ToolProperties.TextToShow != TextType.None)
                {
                    using (Graphics graphicsNames = Graphics.FromImage(ToolProperties.objectImage))
                    {
                        graphicsNames.SmoothingMode = SmoothingMode.AntiAlias;
                        graphicsNames.CompositingQuality = CompositingQuality.HighQuality;
                        graphicsNames.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        for (int i = 0; i <= entries - 1; i++)
                        {
                            if (ToolProperties.TextColorAuto)
                            {
                                if (!IsReadable(EntryList[i].Aura, Color.Black)) { brush = Brushes.White; }
                                else { brush = Brushes.Black; }
                            }
                            else
                            {
                                brush = new SolidBrush(ToolProperties.TextColor);
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
                                    midPoint1 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(45 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(45 * Math.PI / 180F) + ToolSize.Center.Y);
                                    midPoint2 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(135 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(135 * Math.PI / 180F) + ToolSize.Center.Y);
                                }
                                else if (i == 1)
                                {
                                    midPoint1 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(225 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(225 * Math.PI / 180F) + ToolSize.Center.Y);
                                    midPoint2 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(315 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(315 * Math.PI / 180F) + ToolSize.Center.Y);
                                }
                            }
                            else if (entries == 3)
                            {
                                if (i == 0)
                                {
                                    midPoint1 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(30 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(30 * Math.PI / 180F) + ToolSize.Center.Y);
                                    midPoint2 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(90 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(90 * Math.PI / 180F) + ToolSize.Center.Y);
                                }
                                else if (i == 1)
                                {
                                    midPoint1 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(150 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(150 * Math.PI / 180F) + ToolSize.Center.Y);
                                    midPoint2 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(210 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(210 * Math.PI / 180F) + ToolSize.Center.Y);
                                }
                                else if (i == 2)
                                {
                                    midPoint1 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(270 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(270 * Math.PI / 180F) + ToolSize.Center.Y);
                                    midPoint2 = new PointF((ToolSize.Radius - 20) * (float)Math.Cos(330 * Math.PI / 180F) + ToolSize.Center.X, (ToolSize.Radius - 20) * (float)Math.Sin(330 * Math.PI / 180F) + ToolSize.Center.Y);
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
                                    else if (entries > 50) { repeat += 2; }
                                    else if (entries > 55) { repeat += 3; }
                                    else if (entries > 60) { repeat += 5; }
                                    midPoint2 = MidPoint(usePoint2, usePoint1, repeat);
                                }
                            }
                            midPoint = MidPoint(midPoint2, midPoint1);

                            //Adjust Text Angle
                            double deltaX = Math.Pow((midPoint.X - ToolSize.Center.X), 2);
                            double deltaY = Math.Pow((midPoint.Y - ToolSize.Center.Y), 2);
                            double radians = Math.Atan2((ToolSize.Center.Y - midPoint.Y), (ToolSize.Center.X - midPoint.X));
                            double textAngle = radians * ((180) / Math.PI);
                            Matrix rotationMatrix = new Matrix();
                            rotationMatrix.RotateAt((float)textAngle - 2, midPoint);
                            graphicsNames.Transform = rotationMatrix;

                            drawFont = new Font(ToolProperties.TextFontFamily, GetCorrectSize(EntryList[i].Name), ToolProperties.TextFontStyle);
                            string drawText = "";
                            if (ToolProperties.TextToShow == TextType.NameAndID) { drawText = $"{EntryList[i].Name}_{EntryList[i].UniqueID}"; }
                            else if (ToolProperties.TextToShow == TextType.Name) { drawText = $"{EntryList[i].Name}"; }
                            else if (ToolProperties.TextToShow == TextType.ID) { drawText = $"{EntryList[i].UniqueID}"; }
                            graphicsNames.DrawString(drawText, drawFont, brush, midPoint, StringFormat.GenericTypographic);
                        }
                        graphicsNames.DrawImage(namesImage, new Point(0, 0));
                    }
                }
                graphics.DrawImage(ToolProperties.objectImage, new Point(0, 0));
                _ControlWheel.Image = ToolProperties.objectImage;
                _ControlWheel.Refresh();
            }
            DrawArrow();
        }

        public void Refresh()
        {
            if (IsBusy) { if (AllowExceptions) { throw new InvalidOperationException("Wheel is currently busy."); } else { return; } }
            DrawWheel();
        }

        private PointF CalculateMidPoint(PointF p1, PointF p2)
        {
            p1.X -= ToolSize.Left; p2.X += ToolSize.Left;
            return new PointF((float)((p1.X + p2.X) / 2f), (float)((p1.Y + p2.Y) / 2f));
        }
        private PointF MidPoint(PointF p1, PointF p2, int cycles = 0)
        {
            PointF p = p2;
            for (int i = 0; i <= cycles; i++)
            { p = CalculateMidPoint(p1, p); }
            return p;
        }
        private int GetCorrectSize(string word)
        {
            int size = 9;
            if (ToolSize.Radius <= 70)
            { size = 5; }

            else if(ToolSize.Radius < 150 && word.Length <= 5)
            { size = 7; }
            else if (ToolSize.Radius < 150 && word.Length > 5 && word.Length <= 15)
            { size = 6; }
            else if (ToolSize.Radius < 150 && word.Length >= 15)
            { size = 5; }

            else if (ToolSize.Radius >= 150 && ToolSize.Radius < 250 && word.Length <= 5)
            { size = 10; }
            else if (ToolSize.Radius >= 150 && ToolSize.Radius < 250 && word.Length > 5 && word.Length <= 15)
            { size = 9; }
            else if (ToolSize.Radius >= 150 && ToolSize.Radius < 250 && word.Length > 15)
            { size = 8; }

            else if (ToolSize.Radius >= 250 && ToolSize.Radius < 350 && word.Length <= 5)
            { size = 13; }
            else if (ToolSize.Radius >= 250 && ToolSize.Radius < 350 && word.Length > 5 && word.Length <= 15)
            { size = 12; }
            else if (ToolSize.Radius >= 250 && ToolSize.Radius < 350 && word.Length > 15)
            { size = 11; }

            else if (ToolSize.Radius >= 350 && ToolSize.Radius < 500 && word.Length <= 5)
            { size = 16; }
            else if (ToolSize.Radius >= 350 && ToolSize.Radius < 500 && word.Length > 5 && word.Length <= 15)
            { size = 15; }
            else if (ToolSize.Radius >= 350 && ToolSize.Radius < 500 && word.Length > 15)
            { size = 14; }

            else if (ToolSize.Radius >= 500)
            { size = 16; }
            return size;
        }
        public bool IsReadable(Color color1, Color color2)
        {
            return Math.Abs(color1.GetBrightness() - color2.GetBrightness()) >= 0.5f;
        }
    }
}
