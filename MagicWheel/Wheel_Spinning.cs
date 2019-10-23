using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace MagicWheel
{
    public partial class Wheel
    {
        public void Spin(SpinDirection spinDirection = SpinDirection.Clockwise, SpinPowerType spinPowerType = SpinPowerType.Random, int spinStrength = 5)
        {
            if (IsSpinning) { return; }
            if (wheelImage == null) { return; }

            if (spinPowerType != SpinPowerType.Manual) { spinStrength = UpdateSpinStrength(spinPowerType); }
            else { spinStrength = spinStrength > 11 ? 11 : spinStrength < 1 ? 1 : spinStrength; }
            
            if (currentArrowDirection != ArrowLocation)
            {
                if (_ControlWheel.InvokeRequired)
                {
                    ContentWindow.Invoke((MethodInvoker)delegate
                    {
                        ReDraw();
                    });
                }
                else
                {
                    ReDraw();
                }
            }

            Entry currentEntry = EntryList[0];
            Color c = Color.White;
            IsSpinning = true;

            Bitmap newImage = new Bitmap((int)(_WheelSize.Diameter + _WheelSize.Left + 2), (int)(_WheelSize.Diameter + _WheelSize.Top + 2));
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                Matrix rotationMatrix = new Matrix();
                float rotationAngle = 0;
                Random r = new Random();

                float random = 360;//Max rotation angles
                float spinAddedSpeed = 0;
                if (spinStrength < 3)
                { random = r.Next(360, 360 * (spinStrength * 2) + 1); }
                else if (spinStrength < 5)
                { random = r.Next(360 * 3, 360 * (spinStrength * 2) + 1); spinAddedSpeed += 5; }
                else if (spinStrength < 7)
                { random = r.Next(360 * 5, 360 * (spinStrength * 2) + 1); spinAddedSpeed += 7; }
                else if (spinStrength < 10)
                { random = r.Next(360 * 6, 360 * (spinStrength * 2) + 1); spinAddedSpeed += 8; }
                else if (spinStrength == 10)
                { random = r.Next(360 * 8, 360 * (spinStrength * 2) + 1); spinAddedSpeed += 10; }
                else if (spinStrength > 10)
                { random = r.Next(360 * 12, 360 * (spinStrength * 3) + 1); spinAddedSpeed += 30; }
                //if (ContentWindow.InvokeRequired)
                //{
                //    ContentWindow.Invoke((MethodInvoker)delegate
                //    {
                //        ContentWindow.SuspendLayout();
                //    });
                //}
                //else
                //{
                //    ContentWindow.SuspendLayout();
                //}
                while (rotationAngle <= random)
                {
                    float originalrotationangle = rotationAngle;
                    rotationAngle = CalculateNextAngle(rotationAngle, random, spinAddedSpeed, spinDirection);
                    rotationMatrix = new Matrix();
                    rotationMatrix.RotateAt(rotationAngle, _WheelSize.Center);
                    if (spinDirection == SpinDirection.CounterClockwise) { rotationAngle *= -1; }
                    graphics.Transform = rotationMatrix;
                    graphics.Clear(_ControlWheel.BackColor);
                    graphics.DrawImage(wheelImage, 0, 0);
                    try { _ControlWheel.Image = newImage; }
                    catch (InvalidOperationException) {
                        try { _ControlWheel.Image = newImage; }
                        catch (InvalidOperationException) { }
                    }
                    try { _ControlArrow.Image = arrowImage; }
                    catch (InvalidOperationException) {
                        try { _ControlArrow.Image = arrowImage; }
                        catch (InvalidOperationException) { }
                    }

                    currentEntry = checkAngleEntry(rotationAngle, spinDirection);

                    WheelSpinCall?.Invoke(currentEntry, new string[] { spinPowerType.ToString() + "|" + spinStrength.ToString(), rotationAngle.ToString(), (rotationAngle - originalrotationangle).ToString(), random.ToString()});
                    try
                    {
                        if (ContentWindow.InvokeRequired)
                        {
                            ContentWindow.Invoke((MethodInvoker)delegate
                            {
                                _ControlWheel.Update();
                                _ControlArrow.Update();
                            });
                        }
                        else
                        {
                            _ControlWheel.Update();
                            _ControlArrow.Update();
                        }
                    }
                    catch { break; }

                    rotationAngle += 1;

                    if (spinStop) { break; }
                }
                //try
                //{
                //    if (ContentWindow.InvokeRequired)
                //    {
                //        ContentWindow.Invoke((MethodInvoker)delegate
                //        {
                //            ContentWindow.ResumeLayout();
                //        });
                //    }
                //    else
                //    {
                //        ContentWindow.ResumeLayout();
                //    }
                //}
                //catch { }
            }
            spinStop = false;
            IsSpinning = false;
            WheelStopCall?.Invoke(currentEntry);
        }
        public void Stop()
        {
            spinStop = IsSpinning ? true : false;
        }

        private int UpdateSpinStrength(SpinPowerType spinPowerType)
        {
            int spinStrength = 5;
            Random r = new Random();
            if (spinPowerType == SpinPowerType.Weak) { spinStrength = r.Next(1, 3 + 1); }
            else if (spinPowerType == SpinPowerType.Average) { spinStrength = r.Next(4, 8 + 1); }
            else if (spinPowerType == SpinPowerType.Strong) { spinStrength = r.Next(9, 10 + 1); }
            else if (spinPowerType == SpinPowerType.Super) { spinStrength = 11; }
            else if (spinPowerType == SpinPowerType.Random) { spinStrength = RandomSpinStrength(); }
            return spinStrength;
        }
        private int RandomSpinStrength()
        {
            Random r = new Random();
            int ss = r.Next(1, 10 + 1);
            if (ss < 4) { ss = r.Next(1, 10 + 1); }
            if (ss == 1) { ss = r.Next(1, 10 + 1); }
            else if (ss == 10) { ss = r.Next(1, 10 + 1); }
            return ss;
        }

        private Entry checkAngleEntry(float angle, SpinDirection direction)
        {

            List<Entry> revList = EntryList.ToList();
            revList.Reverse();

            if (direction == SpinDirection.Clockwise)
            {
                float fixAngle = EntryList[0].WheelLocation - EntryList[1].WheelLocation;// _locationData[0] - _locationData[1];//ArrowLocation.Right
                if (ArrowLocation == ArrowLocation.Bottom) { fixAngle += 90; }
                else if (ArrowLocation == ArrowLocation.Left) { fixAngle += 180; }
                else if (ArrowLocation == ArrowLocation.Top) { fixAngle += 270; }
                angle -= fixAngle;
            }
            else
            {
                float fixAngle = revList[0].WheelLocation - revList[1].WheelLocation;//_locationData[0] + _locationData[1];//ArrowLocation.Right
                if (ArrowLocation == ArrowLocation.Bottom) { fixAngle -= 90; }
                else if (ArrowLocation == ArrowLocation.Left) { fixAngle -= 180; }
                else if (ArrowLocation == ArrowLocation.Top) { fixAngle -= 270; }
                angle -= fixAngle;
                angle *= -1;
            }

            while (angle < 0) { angle += 360; }
            while (angle > 360) { angle -= 360; }

            int entry = 0;
            for (int i = EntryList.Count - 1; EntryList.Count > i && i >= 0; i--)
            {
                if (angle > EntryList[i].WheelLocation) { entry = i; break; }
            }

            return revList[entry];
        }
        private float CalculateNextAngle(float currentAngle, float finalAngle, float spinAddedSpeed, SpinDirection direction)
        {
            if (direction == SpinDirection.CounterClockwise)
            {
                if (finalAngle < 0) { finalAngle *= -1; }
                if (currentAngle < 0) { currentAngle *= -1; }
            }

            float remaining = finalAngle - currentAngle;
            if (remaining < 360 && remaining >= 300) { currentAngle -= 0.05f; currentAngle += spinAddedSpeed / 30f; }
            else if (remaining < 300 && remaining >= 240) { currentAngle -= 0.1f; currentAngle += spinAddedSpeed / 40f; }
            else if (remaining < 240 && remaining >= 180) { currentAngle -= 0.2f; currentAngle += spinAddedSpeed / 50f; }
            else if (remaining < 180 && remaining >= 120) { currentAngle -= 0.3f; }
            else if (remaining < 120 && remaining >= 60) { currentAngle -= 0.4f; }
            else if (remaining < 60 && remaining >= 40) { currentAngle -= 0.5f; }
            else if (remaining < 40 && remaining >= 20) { currentAngle -= 0.6f; }
            else if (remaining < 20 && remaining >= 10) { currentAngle -= 0.7f; }
            else if (remaining < 10 && remaining >= 5) { currentAngle -= 0.8f; }
            else if (remaining < 5 && remaining >= 0) { currentAngle -= 0.9f; }

            else if (currentAngle < (finalAngle / 100f)) { currentAngle += 2f; }//slow

            else if (currentAngle < (finalAngle / 15f)) { currentAngle += 5f; currentAngle += spinAddedSpeed; }//fast
            else if (currentAngle < (finalAngle / 14f)) { currentAngle += 4.7f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 13f)) { currentAngle += 4.3f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 12f)) { currentAngle += 4f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 11f)) { currentAngle += 3.7f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 10f)) { currentAngle += 3.3f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 9f)) { currentAngle += 3f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 8f)) { currentAngle += 2.7f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 7f)) { currentAngle += 2.3f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 6.5f)) { currentAngle += 2f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 6f)) { currentAngle += 1.7f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 5.5f)) { currentAngle += 1.3f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 5f)) { currentAngle += 1.1f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 4.5f)) { currentAngle += 0.7f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 4f)) { currentAngle += 0.3f; currentAngle += spinAddedSpeed; }
            else if (currentAngle < (finalAngle / 3.5f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 1.5f; }
            else if (currentAngle < (finalAngle / 3f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 1f; }
            else if (currentAngle < (finalAngle / 2.7f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 1.5f; }
            else if (currentAngle < (finalAngle / 2.5f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 2f; }
            else if (currentAngle < (finalAngle / 2.3f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 2.5f; }
            else if (currentAngle < (finalAngle / 2f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 3f; }
            else if (currentAngle < (finalAngle / 1.7f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 4f; }
            else if (currentAngle < (finalAngle / 1.5f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 5f; }
            else if (currentAngle < (finalAngle / 1.3f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 10f; }
            else if (currentAngle < (finalAngle / 1f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 20f; }
            else if (currentAngle < (finalAngle / 0.7f)) { currentAngle += 0f; currentAngle += spinAddedSpeed / 30f; }

            if (direction == SpinDirection.CounterClockwise)
            { currentAngle *= -1; }

            return currentAngle;
        }
    }
}
