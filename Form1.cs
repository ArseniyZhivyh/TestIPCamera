using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using System.Xml;
using System.IO;
using System.Net;

namespace TestIPCamera
{
    public partial class Form1 : Form
    {
        MJPEGStream stream;
        string[] Inners = new string[5];
        Dictionary<string, string> IDs = new Dictionary<string, string>(5);
        int i = 0;
        int j = 0;
        public XmlDocument MakeRequest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://demo.macroscop.com:8080/configex?login=root");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();
                //doc.Load(new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)));
                doc.Load(new StreamReader(response.GetResponseStream()));
                XmlElement xRoot = doc.DocumentElement;
                foreach (XmlNode xnode in xRoot)
                {
                    try
                    {
                        Inners[i] = xnode["Configuration"]["Channels"]["ChannelInfo"].InnerText;
                        i++;
                    }
                    catch(NullReferenceException e)
                    {

                    }
                }
                foreach (string inner in Inners)
                {
                    string[] words = inner.Split(new char[] { '"' });
                    IDs.Add(words[1], words[3]);
                }
                comboBox1.DataSource = new BindingSource(IDs, null);
                comboBox1.DisplayMember = "Key";
                comboBox1.ValueMember = "Value";
                return doc;
            }
        }

        public Form1()
        {
            InitializeComponent();
            MakeRequest();
        }
        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bmp;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            stream = new MJPEGStream("http://demo.macroscop.com:8080/mobile?login=root&channelid=773bad89-c18a-4e7e-a70d-c2a37897a92d");
            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stream.Stop();
        }
    }
}
