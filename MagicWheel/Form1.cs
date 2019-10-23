using MagicWheel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MagicWheelExample
{
    public partial class Form1 : Form
    {
        private int sorttype = 0;
        private Wheel wheel;

        public Form1()
        {
            InitializeComponent();
            wheel = new Wheel(this);
            wheel._WheelProperties.ForceUniqueEntryColors = true;
            wheel.WheelSpinCall += Wheel_WheelSpinCall;
            wheel.WheelStopCall += Wheel_WheelStopCall;
        }

        #region Events
        private void Wheel_WheelStopCall(Entry entry)
        {
            if (label15.InvokeRequired)
            {
                label15.Invoke((MethodInvoker)delegate
                {
                    if (!wheel.IsReadable(entry.Aura, Color.Black)) { label15.ForeColor = Color.White; }
                    else { label15.ForeColor = Color.Black; }
                    label15.Text = $"{entry.Name}\r\nTicket {entry.UniqueID}";
                    label15.BackColor = entry.Aura;
                    label15.Refresh();
                });
            }
            else
            {
                if (!wheel.IsReadable(entry.Aura, Color.Black)) { label15.ForeColor = Color.White; }
                else { label15.ForeColor = Color.Black; }
                label15.Text = $"{entry.Name}\r\nTicket {entry.UniqueID}";
                label15.BackColor = entry.Aura;
                label15.Refresh();
            }
        }
        private void Wheel_WheelSpinCall(Entry entry, string[] spinInfo)
        {
            if (label15.InvokeRequired)
            {
                label15.Invoke((MethodInvoker)delegate
                {
                    label1.Text = spinInfo[0];
                    label1.Update();
                    label2.Text = spinInfo[1];
                    label2.Update();
                    label3.Text = spinInfo[2];
                    label3.Update();
                    label4.Text = spinInfo[3];
                    label4.Update();
                    label15.Text = $"{entry.Name}\r\nTicket {entry.UniqueID}";
                    label15.BackColor = entry.Aura;
                    label15.Update();
                });
            }
            else
            {
                label1.Text = spinInfo[0];
                label1.Update();
                label2.Text = spinInfo[1];
                label2.Update();
                label3.Text = spinInfo[2];
                label3.Update();
                label4.Text = spinInfo[3];
                label4.Update();
                label15.Text = $"{entry.Name}\r\nTicket {entry.UniqueID}";
                label15.BackColor = entry.Aura;
                label15.Update();
            }
        }
        #endregion

        #region Basics
        private void Spin_Click(object sender, EventArgs e)
        {
            SpinPowerType spt;
            int strength = 5;
            if (checkBox1.Checked) { spt = SpinPowerType.Weak; }
            else if (checkBox2.Checked) { spt = SpinPowerType.Average; }
            else if (checkBox3.Checked) { spt = SpinPowerType.Strong; }
            else if (checkBox4.Checked) { spt = SpinPowerType.Super; }
            else if (checkBox5.Checked) { spt = SpinPowerType.Random; }
            else if (checkBox6.Checked) { spt = SpinPowerType.Manual; strength = int.Parse(textBox2.Text); }
            else { checkBox5.Checked = true; spt = SpinPowerType.Random; }

            SpinDirection sd;
            if (checkBox13.Checked) { sd = SpinDirection.Clockwise; }
            else { sd = SpinDirection.CounterClockwise; }

            new Thread(() => { wheel.Spin(sd, spt, strength); }).Start();
        }
        private void Draw_Click(object sender, EventArgs e)
        {
            if (checkBox12.Checked) { wheel.ArrowImage = pictureBox1.Image; }

            wheel._WheelProperties.TextFontFamily = textBox7.Text;
            wheel._WheelProperties.TextColorAuto = checkBox19.Checked;
            wheel._WheelProperties.TextColor = button13.BackColor;
            wheel._WheelProperties.LineColor = button15.BackColor;
            wheel._WheelProperties.LineWidth = int.Parse(textBox8.Text);

            wheel.Draw(int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox6.Text));
        }
        private void Stop_Click(object sender, EventArgs e)
        {
            wheel.Stop();
        }
        #endregion

        #region SpinType
        private void Weak_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
            }
        }
        private void Average_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
            }
        }
        private void Strong_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
            }
        }
        private void Super_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
            }
        }
        private void Random_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox3.Checked = false;
                checkBox6.Checked = false;
            }
        }
        private void Manual_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox3.Checked = false;
            }
        }

        private void SpinRight_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                checkBox14.Checked = false;
            }
        }
        private void SpinLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                checkBox13.Checked = false;
            }
        }
        #endregion

        #region Entries
        private void AddEntry_Click(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                Random r = new Random();
                Color randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                for (int i = 1; i <= r.Next(1,10+1); i++)
                { wheel.EntryAdd(new Entry() { Name = RandomName(), Aura = randomColor }); }
            }
            else
            {
                for (int i = 1; i <= int.Parse(textBox5.Text); i++)
                { wheel.EntryAdd(new Entry() { Name = textBox1.Text, Aura = button6.BackColor }); }
            }
            Draw_Click(null, null);
        }
        private string RandomName()
        {
            List<string> names = new List<string>();
            names.Add("Mike");
            names.Add("Joe");
            names.Add("Cloud");
            names.Add("Sarah");
            names.Add("Faye");
            names.Add("Mark");
            names.Add("A Long Name");
            names.Add("Gilligan");
            names.Add("FirstName LastName");
            names.Add("Another Very Long Name");

            return names[new Random().Next(0, names.Count())];
        }
        private void Color_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            { button6.BackColor = colorDialog1.Color; }
        }
        private void RandomEntry_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                textBox1.Enabled = false;
                textBox5.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                textBox5.Enabled = true;
                button5.Enabled = true;
            }
        }
       
        private void View_Click(object sender, EventArgs e)
        {
            string text = "";
            foreach (Entry entry in wheel.EntryList)
            {
                text += $"{entry.UniqueID}|{entry.Name}\r\n";
            }
            MessageBox.Show(text);
        }
        private void Reset_Click(object sender, EventArgs e)
        {
            wheel.EntriesClear();
        }
        private void Sort_Click(object sender, EventArgs e)
        {
            if (sorttype == 0)
            { wheel.EntryList.Sort((x, y) => x.UniqueID.CompareTo(y.UniqueID)); sorttype = 1; }
            else if (sorttype == 1)
            { wheel.EntryList.Sort((x, y) => x.Name.CompareTo(y.Name)); sorttype = 0; }

            Draw_Click(null, null);
        }
        private void Random_Click(object sender, EventArgs e)
        {
            int count = wheel.EntryList.Count();
            List<Entry> ListtoSort = new List<Entry>(wheel.EntryList);
            List<int> rngList = new List<int>();
            Random r = new Random();
            for (int i = 0; i < count; i++)
            {
                while (true)
                {
                    int newrng = r.Next(0, count);
                    if (!rngList.Contains(wheel.EntryList[newrng].UniqueID))
                    { rngList.Add(wheel.EntryList[newrng].UniqueID); break; }
                }
            }
            ListtoSort = ListtoSort.OrderBy(x => rngList.IndexOf(x.UniqueID)).ToList();
            wheel.EntryList.Clear();
            for (int i = 0; i < count; i++)
            {
                wheel.EntryList.Add(ListtoSort[i]);
            }

            Draw_Click(null, null);
        }
        #endregion

        #region Arrow
        private void Top_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                checkBox9.Checked = false;
                checkBox10.Checked = false;
                checkBox11.Checked = false;
                wheel.ArrowLocation = ArrowLocation.Top;
            }
        }
        private void Right_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                checkBox8.Checked = false;
                checkBox10.Checked = false;
                checkBox11.Checked = false;
                wheel.ArrowLocation = ArrowLocation.Right;
            }
        }
        private void Left_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                checkBox9.Checked = false;
                checkBox8.Checked = false;
                checkBox11.Checked = false;
                wheel.ArrowLocation = ArrowLocation.Left;
            }
        }
        private void Bottom_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                checkBox9.Checked = false;
                checkBox10.Checked = false;
                checkBox8.Checked = false;
                wheel.ArrowLocation = ArrowLocation.Bottom;
            }
        }
        private void Icon_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.ico, *.bmp, *.png) | *.jpg; *.jpeg; *.ico; *.bmp; *.png";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }
        #endregion

        #region WheelText
        private void NameAndID_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox17.Checked)
            {
                checkBox15.Checked = false;
                checkBox16.Checked = false;
                checkBox18.Checked = false;
                wheel._WheelProperties.ShowWheelText = WheelText.NameAndID;
            }
        }
        private void Name_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox15.Checked)
            {
                checkBox17.Checked = false;
                checkBox16.Checked = false;
                checkBox18.Checked = false;
                wheel._WheelProperties.ShowWheelText = WheelText.Name;
            }
        }
        private void ID_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox16.Checked)
            {
                checkBox15.Checked = false;
                checkBox17.Checked = false;
                checkBox18.Checked = false;
                wheel._WheelProperties.ShowWheelText = WheelText.ID;
            }
        }
        private void None_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox18.Checked)
            {
                checkBox15.Checked = false;
                checkBox16.Checked = false;
                checkBox17.Checked = false;
                wheel._WheelProperties.ShowWheelText = WheelText.None;
            }
        }
        private void FontAutoColor_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox19.Checked)
            {
                button12.Enabled = false;
                button13.Enabled = false;
            }
            else
            {
                button12.Enabled = true;
                button13.Enabled = true;
            }
        }
        private void FontColor_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            { button13.BackColor = colorDialog1.Color; }
        }
        private void LineColor_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            { button15.BackColor = colorDialog1.Color; }
        }
        #endregion
    }
}
