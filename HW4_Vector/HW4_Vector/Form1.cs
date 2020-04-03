using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace HW4_Vector
{
    public partial class FormMain : Form
    {
        List<PointF> pList = new List<PointF>();
        int clickNum = 0;
        Image<Bgr, byte> img;
        int radius = 2;
        int thickness = 1;

        public FormMain()
        {
            InitializeComponent();
            txtHistory.Text = "Program Started";
            img = new Image<Bgr, byte>(picMain.Size);
            img.SetValue(new Bgr(Color.White));
        }

        private void picMain_MouseClick(object sender, MouseEventArgs e)
        {
            //p[clickNum] = new PointF(e.X, e.Y);
            pList.Add(new PointF(e.X, e.Y));
            clickNum++;
            if (clickNum == 4)
            {
                btnExe.Enabled = true;
            }
            AddHistory(String.Format("Point {0:0} : {1:0}, {2:0}", 
                pList.Count - 1, pList.Last().X, pList.Last().Y));


            // for graphic
            img.Draw(new CircleF(pList.Last(), radius), new Bgr(Color.DarkGreen), thickness);
            if (pList.Count > 1)
                img.Draw(new LineSegment2DF(pList.Last(), 
                    pList[pList.Count - 2]), new Bgr(Color.DarkGreen), thickness);
            picMain.Image = img.ToBitmap();
        }

        private void btnExe_Click(object sender, EventArgs e)
        {
            float S = 0;
            PointF[] p = pList.ToArray();
            int pNum = p.Count();
            PointF[] v = new PointF[pNum - 1];

            string strTemp = "";

            for (int i = 1; i < pNum; i++)
            {
                v[i - 1] = new PointF(p[i].X - p[0].X, p[i].Y - p[0].Y);
                strTemp += "\r\n";
                strTemp += String.Format("vec {0} : {1:0}, {2:0}", i - 1, v[i - 1].X, v[i - 1].Y);
            }

            AddHistory(strTemp);

            for (int j = 1; j < pNum - 1; j++)
                S += ( (v[j - 1].X * v[j].Y) - (v[j - 1].Y * v[j].X) );

            S = Math.Abs(S) / 2;
            AddHistory(String.Format("Area of polygon : {0:f}", S));

            Image<Bgr, byte> imgTemp = new Image<Bgr, byte>((Bitmap)picMain.Image);
            imgTemp.Draw(new LineSegment2DF(p[0], p[p.Count() - 1]), new Bgr(Color.Magenta), thickness);
            picMain.Image = imgTemp.Bitmap;
        }


        public void AddHistory(string str)
        {
            str = "\r\n" + DateTime.Now.ToString("HH:MM") + "  " + str;
            txtHistory.Text += str;
            txtHistory.SelectionStart = txtHistory.Text.Length + 1;
            txtHistory.ScrollToCaret();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            pList.Clear();
            img.SetValue(new Bgr(Color.White));
            picMain.Image = img.Bitmap;
            AddHistory("Clear the data");
            clickNum = 0;
        }
    }
}
