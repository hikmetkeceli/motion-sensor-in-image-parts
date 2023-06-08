using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;

namespace aforge_görüntü_işleme
{
    public partial class sensor : Form
    {
        public sensor()
        {
            InitializeComponent();
        }
        private FilterInfoCollection fico;
        private VideoCaptureDevice videoSource;
        private PictureBox pictureBoxThis;
        private MotionDetector motionDetector;
        private MotionDetector motionDetector2;
        private float motionValue1;
        private float motionValue2;
        private void Form1_Load(object sender, EventArgs e)
        {
            
            List<string> devices = new List<string>();
            devices = ListAvailableDevices();
            foreach(string device in devices)
            {
                comboBox1.Items.Add(device);
            }
            comboBox1.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ResetConnection(ref pictureBox1);
                timer1.Enabled = true;
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
            StartConnection(comboBox1.SelectedIndex, ref pictureBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PauseImage(ref pictureBox1);
            timer1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ResetConnection(ref pictureBox1);
            timer1.Enabled = false;
            textBox1.Text = "0";
            panel3.BackColor = Color.Gray;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CaptureImage(ref pictureBox1, ref pictureBox3);
            CaptureImage(ref pictureBox2, ref pictureBox4);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = MotionDetection().ToString();
            float motionValue = MotionDetection();
            if (motionValue != 0 || motionValue > 0.001)
            {
                try
                {
                    panel3.BackColor = Color.Green;
                }
                catch (Exception h)
                {
                    MessageBox.Show(h.Message);
                }
            }
            if (motionValue == 0 || motionValue < 0.001)
            {
                try
                {
                    panel3.BackColor = Color.Red;
                }
                catch (Exception h)
                {
                    MessageBox.Show(h.Message);
                }
            }
            textBox2.Text = MotionDetection2().ToString();
            float motionValuee = MotionDetection2();
            if (motionValuee != 0 && motionValuee > 0.001)
            {
                try
                {
                    panel4.BackColor = Color.Green;
                }
                catch (Exception h)
                {
                    MessageBox.Show(h.Message);
                }
            }
            if (motionValuee == 0 && motionValuee < 0.001)
            {
                try
                {
                    panel4.BackColor = Color.Red;
                }
                catch (Exception h)
                {
                    MessageBox.Show(h.Message);
                }
            }
        }
        public List<string> ListAvailableDevices()
        {
            List<string> devicesList = new List<string>();
            try
            {
                fico = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo f in fico)
                {
                    devicesList.Add(f.Name);
                }
                videoSource = new VideoCaptureDevice();
                return devicesList;

            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: \n" + e.Message);
                return null;
            }
        }
        public void StartConnection(int deviceNumber, ref PictureBox pictureBox)
        {
            try
            {
                videoSource = new VideoCaptureDevice(fico[deviceNumber].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(VideoSource_NewFrame);
                videoSource.Start();
                motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
                motionDetector2 = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void ResetConnection(ref PictureBox pictureBox)
        {
            try
            {
                pictureBoxThis = pictureBox;
                videoSource.Stop();
                pictureBoxThis.Image = null;
                pictureBoxThis.Invalidate();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void PauseImage(ref PictureBox pictureBox)
        {
            try
            {
                pictureBoxThis = pictureBox;
                videoSource.Stop();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void CaptureImage(ref PictureBox videoPictureBox, ref PictureBox imagePictureBox)
        {
            try
            {
                imagePictureBox.Image = (Bitmap)videoPictureBox.Image.Clone();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public float MotionDetection()
        {
            return motionValue1;
        }
        public float MotionDetection2()
        {
            return motionValue2;
        }
        public Crop filter = new Crop(new Rectangle(0, 0, 320, 240));
        public Crop filter2 = new Crop(new Rectangle(320, 0, 320, 240));
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap newImage = filter.Apply((Bitmap)eventArgs.Frame);
                motionValue1 = motionDetector.ProcessFrame(newImage);
                Bitmap newImage2 = filter2.Apply((Bitmap)eventArgs.Frame);
                motionValue2 = motionDetector2.ProcessFrame(newImage2);
                pictureBoxThis.Image = (Image)newImage.Clone();
                pictureBox2.Image = (Image)newImage2.Clone();
                newImage.Dispose();
                GC.Collect();
                GC.GetTotalMemory(true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            PauseImage(ref pictureBox1);
            timer1.Enabled = false;
            Application.Exit();
        }
    }
}
