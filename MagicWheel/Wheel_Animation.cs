using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RandomTool
{
    public partial class Wheel
    {
        bool wheelReversePaused = false;
        public void Start(int animDirection = (int)Direction.Clockwise, int randPowerType = (int)PowerType.Random, int randStrength = 5)
        {
            if (IsBusy) { if (AllowExceptions) { throw new InvalidOperationException("Wheel is currently busy."); } else { return; } }
            if (ToolProperties.objectImage == null) { if (AllowExceptions) { throw new InvalidOperationException("Please initialize wheel using Draw()."); } else { return; } }
            if (EntryList.Count <= 0) { if (AllowExceptions) { throw new IndexOutOfRangeException("Must have more than zero Entries to Spin."); } else { return; } }

            wheelReversePaused = true;
            int manualStrength = 0;
            if (randPowerType == (int)PowerType.Infinite) { randStrength = -1; }
            else if (randPowerType != (int)PowerType.Manual) { randStrength = (int)UpdateRandStrength(randPowerType); }
            else { manualStrength = randStrength; }// randStrength = randStrength > 11 ? 11 : randStrength < 1 ? 1 : randStrength; }

            if (ToolProperties.currentArrowDirection != ToolProperties.ArrowPosition)
            {
                if (_ControlWheel.InvokeRequired)
                {
                    _ControlWheel.Invoke((MethodInvoker)delegate
                    {
                        Refresh();
                    });
                }
                else
                {
                    Refresh();
                }
            }

            Entry currentEntry = EntryList[0];
            Color c = Color.White;
            IsBusy = true;

            Bitmap newImage = new Bitmap((int)(ToolSize.Diameter + ToolSize.Left + 2), (int)(ToolSize.Diameter + ToolSize.Top + 2));
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                Matrix animationMatrix = new Matrix();
                float animationAngle = 0;
                float spinAddedSpeed = 0;
                Random r = new Random();

                float random = 360;//Max rotation angles
                if (manualStrength > 0)
                {
                    random = (manualStrength * 360) + r.Next(-360, 361);
                    if (random > 1000) { spinAddedSpeed = 3; }
                    else if (random > 2000) { spinAddedSpeed = 5; }
                    else if (random > 5000) { spinAddedSpeed = 10; }
                    else if (random > 10000) { spinAddedSpeed = 15; }
                    else if (random > 15000) { spinAddedSpeed = 20; }
                    else if (random > 20000) { spinAddedSpeed = 25; }
                    else if (random > 30000) { spinAddedSpeed = 30; }
                }
                else
                {
                    if (randStrength == -1)
                    { random = -1; spinAddedSpeed = 15; }
                    else if (randStrength < 3)
                    { random = r.Next(360, 360 * (randStrength * 2) + 1); }
                    else if (randStrength < 5)
                    { random = r.Next(360 * 3, 360 * (randStrength * 2) + 1); spinAddedSpeed += 5; }
                    else if (randStrength < 7)
                    { random = r.Next(360 * 5, 360 * (randStrength * 2) + 1); spinAddedSpeed += 7; }
                    else if (randStrength < 10)
                    { random = r.Next(360 * 6, 360 * (randStrength * 2) + 1); spinAddedSpeed += 8; }
                    else if (randStrength == 10)
                    { random = r.Next(360 * 15, 360 * (randStrength * 2) + 1); spinAddedSpeed += 10; }
                    else if (randStrength > 10)
                    { random = r.Next(360 * 50, 360 * (randStrength * 10) + 1); spinAddedSpeed += 30; }
                }

                while ((random <= -1 || animationAngle <= random))// && IsBusy)
                {
                    if (random > -1 && !IsBusy) { break; }
                    else if (random <= -1 && !IsBusy) { random = r.Next(360*2, 360*4) + animationAngle; IsBusy = true; }

                    float originalAnimationAngle = animationAngle;

                    if (random > -1) { animationAngle = CalculateNextAngle(animationAngle, random, spinAddedSpeed, animDirection); }
                    else { animationAngle += 40f * ToolProperties.AnimationSpeed; }

                    animationMatrix = new Matrix();
                    animationMatrix.RotateAt((animDirection == (int)Direction.CounterClockwise) ? animationAngle * -1 : animationAngle, ToolSize.Center);
                    graphics.Transform = animationMatrix;
                    graphics.Clear(_ControlWheel.BackColor);
                    graphics.DrawImage(ToolProperties.objectImage, 0, 0);

                    try { _ControlWheel.Image = newImage; }
                    catch (InvalidOperationException) {
                        try { _ControlWheel.Image = newImage; }
                        catch (InvalidOperationException) { }
                    }
                    try { _ControlArrow.Image = ToolProperties.arrowImage; }
                    catch (InvalidOperationException) {
                        try { _ControlArrow.Image = ToolProperties.arrowImage; }
                        catch (InvalidOperationException) { }
                    }

                    currentEntry = checkAngleEntry(animationAngle, animDirection);

                    ToolActionCall?.Invoke(currentEntry, new string[] { randPowerType.ToString() + "|" + randStrength.ToString(), animationAngle.ToString(), (animationAngle - originalAnimationAngle).ToString(), random.ToString()});
                    try
                    {
                        if (_ControlWheel3D.Parent.InvokeRequired)
                        {
                            _ControlWheel3D.Parent.Invoke((MethodInvoker)delegate
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
                    
                    animationAngle += 1;
                }
            }
            IsBusy = false;
            ToolStopCall?.Invoke(currentEntry);
        }
        public void Stop()
        {
            IsBusy = false;
        }

        private int UpdateRandStrength(int randPowerType)
        {
            int randStrength = 5;
            Random r = new Random();
            if (randPowerType == (int)PowerType.Weak) { randStrength = r.Next(1, 3 + 1); }
            else if (randPowerType == (int)PowerType.Average) { randStrength = r.Next(4, 8 + 1); }
            else if (randPowerType == (int)PowerType.Strong) { randStrength = r.Next(9, 10 + 1); }
            else if (randPowerType == (int)PowerType.Super) { randStrength = 11; }
            else if (randPowerType == (int)PowerType.Random) { randStrength = RandomRandStrength(); }
            return randStrength;
        }
        private int RandomRandStrength()
        {
            Random r = new Random();
            int ss = r.Next(1, 10 + 1);
            if (ss < 4) { ss = r.Next(1, 10 + 1); }
            if (ss == 1) { ss = r.Next(1, 10 + 1); }
            else if (ss == 10) { ss = r.Next(1, 10 + 1); }
            return ss;
        }

        private Entry checkAngleEntry(float angle, int direction)
        {
            List<Entry> revList = EntryList.ToList();
            revList.Reverse();

            if (EntryList.Count > 1)
            {
                if (direction == (int)Direction.Clockwise)
                {
                    float fixAngle = EntryList[0].EntryLocation - EntryList[1].EntryLocation;// _locationData[0] - _locationData[1];//ArrowLocation.Right
                    if (ToolProperties.ArrowPosition == ArrowLocation.Bottom) { fixAngle += 90; }
                    else if (ToolProperties.ArrowPosition == ArrowLocation.Left) { fixAngle += 180; }
                    else if (ToolProperties.ArrowPosition == ArrowLocation.Top) { fixAngle += 270; }
                    angle -= fixAngle;
                }
                else
                {
                    float fixAngle = revList[0].EntryLocation - revList[1].EntryLocation;//_locationData[0] + _locationData[1];//ArrowLocation.Right
                    if (ToolProperties.ArrowPosition == ArrowLocation.Bottom) { fixAngle -= 90; }
                    else if (ToolProperties.ArrowPosition == ArrowLocation.Left) { fixAngle -= 180; }
                    else if (ToolProperties.ArrowPosition == ArrowLocation.Top) { fixAngle -= 270; }
                    angle -= fixAngle;
                    angle *= -1;
                }
            }

            while (angle < 0) { angle += 360; }
            while (angle > 360) { angle -= 360; }

            int entry = 0;
            if (EntryList.Count < 4)
            {
                //List correction needed for less than 4 entries
                for (int i = EntryList.Count - 1; EntryList.Count > i && i >= 0; i--)
                {
                    int choice = i - 1;
                    if (choice < 0) { choice = EntryList.Count - 1; }
                    if (angle > EntryList[i].EntryLocation) { entry = choice; break; }
                }
            }
            else
            {
                for (int i = EntryList.Count - 1; EntryList.Count > i && i >= 0; i--)
                {
                    if (angle > EntryList[i].EntryLocation) { entry = i; break; }
                }
            }
            return revList[entry];
        }
        private float CalculateNextAngle(float currentAngle, float finalAngle, float spinAddedSpeed, int direction)
        {
            float pullback = 100f;
            if (finalAngle > 20000f) { pullback = 340f; }
            else if (finalAngle > 10000f) { pullback = 270f; }
            else if (finalAngle > 5000f) { pullback = 190f; }
            else if (finalAngle > 1000f) { pullback = 130f; }

            // pullback reset for calculations
            if (currentAngle < pullback) { currentAngle *= -1f; }

            if (direction == (int)Direction.CounterClockwise)
            {
                if (finalAngle < 0) { finalAngle *= -1; }
                if (currentAngle < 0) { currentAngle *= -1; }
            }
            
            float remaining = finalAngle - currentAngle;
            float perc = (remaining * 100f) / finalAngle;
            // perc is percent remaining from 100 to 0

            bool pullbackCheck = currentAngle < pullback;
            float spinSpeed = ToolProperties.animationSpeed;
            float pullSpeed = 0.03f;

            // Code to adjust backup spin animation if speed is set too low.
            if (pullbackCheck)
            {
                if (spinSpeed <= 0.04f) { pullSpeed = spinSpeed - 0.02f; }
                if (pullSpeed <= 0f) { spinSpeed = 0.03f; pullSpeed = 0.01f; }
            }
            // -------

            currentAngle += perc * spinSpeed;
            pullbackCheck = currentAngle < pullback;

            float addSpeed;
            if (ToolProperties.AnimationSpeedBoost) { addSpeed = spinAddedSpeed / ((100f - perc) + 0.01f); }
            else { addSpeed = 0f; }

            if (pullbackCheck) { currentAngle -= perc * pullSpeed; currentAngle *= -1f; }
            //Steady slowdown by 0.02 angles stacking past 40 percent remaining.
            else if (perc <= 40f) { currentAngle -= perc - (40f - ((40f - perc) * 1.02f)) - 0.02f; }
            else if (wheelReversePaused) { wheelReversePaused = false; Thread.Sleep(500); }

            if (!pullbackCheck && addSpeed > 0f) { currentAngle += addSpeed; }

            return currentAngle;
        }
    }
}
