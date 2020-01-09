# MagicWheel
Graphics based random animated wheel

I created this dll as part of another project I am working on.
This library will create a Wheel based upon a list of Entries.
The Wheel is customizable and uses layers of PictureBoxes that get added to your form.

It is using a specific interface in IRandomTool.cs so it can be plug-n-play with my other project.

- Create the wheel object.
- Add Entries to the list.
- Draw the wheel.
- Spin the wheel.

![Icon](https://github.com/cloudd901/MagicWheel/blob/master/WheelScreen.jpg)

The example script provided includes nearly all options available for the wheel.

When initializing the Wheel, any control can be used as the Parent.
- Ex: IRandomTool hat = new Wheel(this);
- Ex: IRandomTool hat = new Wheel(label1);

Draw
- Draw(LocationX, LocationY, Radius);
- or Draw(CenterPoint, Radus);
- Refresh();
  - Calls Draw using the existing location data;

EntryList
- Wheel.EntryList;
  - The list of Entries used for the wheel.
- EntryAdd(Entry);
  - Entry type contains a UniqueID(optional), Name, and Color.
  - Returns a unique ticket ID that can optionally be set in the Entry.
- EntryRemove(UniqueID);
  - Uses an ID provided by EntryAdd.
- EntriesClear();
  - Clears the EntryList.
- ShuffleEntries();
  - Randomizes the EntryList.

Action
- Wheel.IsBusy;
  - Returns true if the wheel is actively spinning.
- Start();
- or Start(animDirection, randPowerType, randStrength);
  - animDirection can be Clockwise or CoutnerClockwise.
  - randPowerType can be Infinite, Weak, Average, Strong, Super, Random, or Manual.
    - If rPT is Manual, the randStrength will set the number of rotations +-3 random.
  - randStrength doesn't do anything if rPT is not Manual.
- Stop();

Other
- IsReadable(Color1, Color2);
  - Public function used to determine text color against the background.
- BringToFront();
  - Brings the PictureBox control to the front of the form.
- SendToBack();
  - Sends the PictureBox control to the back of the form.
- Dispose();
  - Disposes of Image and PictureBox data.
  - Must create a new wheel object after this is called.

Event Handlers
- ToolActionEventHandler(Entry, string[4] actionInfo)
  - This event is called each frame the wheel is moved.
  - Entry provided is the currently selected entry on the wheel.
  - The string array provided is a 4 part array.
    - Total Angles to turn. "1672"
    - Current Angle. "1671.018"
    - Current base speed adjustment. "-0.9000244"
    - Spin Strength. "Random|4"
- ToolStopEventHandler(Entry)
  - This event is called when the wheel comes to a stop.
  - Entry provided is the final selected Entry.

Settings
- Wheel.AllowExceptions = true;
  - If false, actions will return null instead of throwing an Exception.
- Wheel._ToolProperties;
  - Contains all other wheel settings:

_ToolProperties
- ArrowPosition
- ArrowImage
- LineColor
- LineWidth
- ForceUniqueEntryColors
- TextToShow
- TextColor
- TextColorAuto
- TextFontFamily
- TextFontStyle
- ShadowVisible
- ShadowColor
- ShadowPosition
- ShadowLength
- CenterVisible
- CenterColor
- CenterSize
