using System.Collections.Generic;
using System.Drawing;

namespace RandomTool
{
    public interface IRandomTool
    {
        bool IsBusy { get; }
        bool AllowExceptions { get; set; }
        List<Entry> EntryList { get; }
        ToolProperties ToolProperties { get; set; }
        bool IsDisposed { get; }
        void Dispose();

        void Draw(PointF center, float radius);
        void Draw(int left, int top, float radius);

        void Refresh();
        void BringToFront();
        void SendToBack();
        bool IsReadable(Color color1, Color color2);

        void Start(int animDirection = (int)Direction.Clockwise, int randPowerType = (int)PowerType.Random, int randStrength = 5);
        void Stop();

        int EntryAdd(Entry entry);
        bool EntryRemove(int UniqueID);
        void EntriesClear();
        void ShuffleEntries();

        event ToolActionEventHandler ToolActionCall;
        event ToolStopEventHandler ToolStopCall;
    }
    

    public class Entry
    {
        public int UniqueID { get; set; } = -1;
        public string Name { get; set; }
        public Color Aura { get; set; } = Color.White;
        internal float EntryLocation { get; set; } = 0;
    }
    public class ToolProperties
    {
        public ArrowLocation ArrowPosition { get; set; } = ArrowLocation.Top;
        internal ArrowLocation currentArrowDirection = ArrowLocation.Top;
        public Image ArrowImage { get => arrowImage; set { arrowImage = value; isNewArrowImage = true; } }
        internal Image arrowImage = null;
        internal bool isNewArrowImage = false;
        internal Image objectImage = null;
        internal Image objectImage3D = null;
        public TextType TextToShow { get; set; } = TextType.NameAndID;
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
        public bool CenterVisible { get; set; } = true;
        public Color CenterColor { get; set; } = Color.DarkGray;
        public int CenterSize { get; set; } = 2;
    }
    internal class ObjectSize
    {
        public int Diameter { get; set; }
        public float Radius { get; set; }
        public float Top { get; set; }
        public float Left { get; set; }
        public PointF Center { get; set; }
        internal bool initialized { get; set; } = false;
    }


    public enum TextType
    {
        NameAndID = 0,
        Name = 1,
        ID = 2,
        None = 3
    }
    public enum ArrowLocation
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }
    public enum PowerType
    {
        Manual = 0,
        Weak = 1,
        Average = 2,
        Strong = 3,
        Super = 4,
        Random = 5,
        Infinite = 6
    }
    public enum Direction
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
}
