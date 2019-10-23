using System;
using System.Drawing;
using System.Linq;

namespace MagicWheel
{
    public partial class Wheel
    {
        public int EntryAdd(Entry entry)
        {
            if (IsSpinning) { return -1; }
            if (entry.UniqueID == -1)
            { if (EntryList.Count() == 0) { entry.UniqueID = 0; } else { entry.UniqueID = EntryList.Last().UniqueID + 1; } }

            int check1 = EntryList.FindIndex(x => x.UniqueID == entry.UniqueID);
            if (check1 != -1)
            { return -1; }

            if (_WheelProperties.ForceUniqueEntryColors)
            {
                int check2 = EntryList.FindIndex(x => x.Name == entry.Name);
                if (check2 != -1)
                { entry.Aura = EntryList[check2].Aura; }//use same color as others with same name
                else
                {
                    int check3 = EntryList.FindIndex(x => x.Aura == entry.Aura);
                    if (check3 != -1)
                    {
                        entry.Aura = ColorIncrementRnd(EntryList[check3].Aura, 25);
                        int check4 = EntryList.FindIndex(x => x.Aura == entry.Aura);
                        if (check4 != -1)
                        {
                            entry.Aura = ColorIncrementRnd(EntryList[check3].Aura, 50);
                        }//use new color since another name is using this color
                    }//use new color since another name is using this color
                    
                }
            }

            EntryList.Add(entry);

            return entry.UniqueID;
        }
        public bool EntryRemove(int UniqueID)
        {
            if (IsSpinning) { return false; }
            try { EntryList.RemoveAt(EntryList.FindIndex(x => x.UniqueID == UniqueID)); return true; }
            catch { return false; }
        }
        public void EntriesClear()
        {
            if (IsSpinning) { return; }
            EntryList.Clear();
            arrowImage = null;
            wheelImage = null;
            _ControlWheel.Image = null;
            _ControlArrow.Image = null;
            DrawWheel();
            DrawArrow();
        }

        private Color ColorIncrementRnd(Color original, int change = 25)
        {
            Color c = original;
            int red = 0; int blue = 0; int green = 0;
            change = new Random().Next(change - 2, change + 11);

            if (change > 0)
            {
                if (c.R <= 255 - change) { red = c.R + change; } else { red = c.R - change; }
                if (c.G <= 255 - change) { green = c.G + change; } else { green = c.G - change; }
                if (c.B <= 255 - change) { blue = c.B + change; } else { blue = c.B - change; }
            }
            else
            {
                if (c.R >= change) { red = c.R - change; } else { red = c.R + change; }
                if (c.G >= change) { green = c.G - change; } else { green = c.G + change; }
                if (c.B >= change) { blue = c.B - change; } else { blue = c.B + change; }
            }

            return Color.FromArgb(red, green, blue);
        }
    }
}
