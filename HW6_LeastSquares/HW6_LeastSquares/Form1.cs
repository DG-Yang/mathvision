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
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;

namespace HW6_LeastSquares
{
    public partial class Form1 : Form
    {
        List<PointF> pList;

        Image<Bgr, byte> img;

        int radius = 4;
        int thickness = 2;

        public Form1()
        {
            InitializeComponent();
            pList = new List<PointF>();
            img = new Image<Bgr, byte>(picMain.Width, picMain.Height, new Bgr(Color.White));
            picMain.Image = img.ToBitmap();

            int a = 2;
            int b = 4;
            int c = 3;
            Matrix<double> test1 = new Matrix<double>(a, b);
            Matrix<double> test2 = new Matrix<double>(c, b);

            test1.SetValue(1); test2.SetValue(2);
            Matrix<double> test3 = test1.Mul(test2.Transpose());
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            string[] Info = new string[3];
            Info[0] = "Maker : D.G.Yang";
            Info[1] = ".Net Framework :  4.6.1";
            Info[2] = "EmguCV : 4.2.0, 64 bit";
            string str = Info[0];
            for (int i = 1; i < Info.Length; i++)
                str += "\r\n" + Info[i];
            MessageBox.Show(str, "Information");
        }

        public void AddHistory(string str)
        {
            str = "\r\n" + DateTime.Now.ToString("HH:MM") + "  " + str;
            txtHistory.Text += str;
            txtHistory.SelectionStart = txtHistory.Text.Length + 1;
            txtHistory.ScrollToCaret();
        }

        private void picMain_MouseClick(object sender, MouseEventArgs e)
        {
            PointF pt = new PointF(e.X, e.Y);
            pList.Add(pt);
            img.Draw(new CircleF(pt, 2f), new Bgr(0, 0, 0), -1);
            AddHistory(String.Format("Point {0} : {1}, {2}", pList.Count, e.X, e.Y));
            picMain.Image = img.ToBitmap();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            pList.Clear();
            img.SetValue(new Bgr(Color.White));
            picMain.Image = img.ToBitmap();
        }

        private void btnExeCircle_Click(object sender, EventArgs e)
        {
            if (pList.Count < 3) return;
            Matrix<double> ptsMatA = new Matrix<double>(pList.Count, 3);
            Matrix<double> ptsMatAPseudo = new Matrix<double>(3, pList.Count);
            Matrix<double> ptsMatB = new Matrix<double>(pList.Count, 1);
            Matrix<double> ResMat = new Matrix<double>(3, 1);

            for (int i = 0; i < pList.Count; i++)
            {
                double xn = pList[i].X;
                double yn = pList[i].Y;
                ptsMatA[i, 0] = xn;
                ptsMatA[i, 1] = yn;
                ptsMatA[i, 2] = 1;
                ptsMatB[i, 0] = -(xn * xn + yn * yn);
            }
            CvInvoke.Invert(ptsMatA, ptsMatAPseudo, DecompMethod.Svd);
            ResMat = ptsMatAPseudo.Mul(ptsMatB);

            double a, b, c;
            a = ResMat[0, 0];
            b = ResMat[1, 0];
            c = ResMat[2, 0];
            string str1 = String.Format("Equation \r\nx^2+y^2+{0:2f}x+{1:2f}y+{2:2f}=0", a, b, c);
            string str2 = String.Format("a = {0:2f}, b = {1:2f}, c = {2:2f}", a, b, c);
            AddHistory(str1);
            AddHistory(str2);

            float x1, y1, r;
            x1 = Convert.ToSingle(- a / 2);
            y1 = Convert.ToSingle(- b / 2);
            r = Convert.ToSingle(Math.Sqrt(x1 * x1 + y1 * y1 - c));
            Image<Bgr, byte> imgTemp = img.Clone();
            imgTemp.Draw(new CircleF(new PointF(x1, y1), r), new Bgr(Color.Blue), 1);
            picMain.Image = imgTemp.ToBitmap();
        }

        //private void btnExeEllipse_Click(object sender, EventArgs e)
        //{
        //    if (pList.Count < 5) return;
        //    Matrix<double> ptsMatA = new Matrix<double>(pList.Count, 6);
        //    Matrix<double> ptsMatAPseudo = new Matrix<double>(6, pList.Count);
        //    Matrix<double> ptsMatB = new Matrix<double>(pList.Count, 1);
        //    //Matrix<double> U_SVD = new Matrix<double>(pList.Count, pList.Count);
        //    Matrix<double> W_SVD = new Matrix<double>(pList.Count, 6);
        //    Matrix<double> U_SVD = new Matrix<double>(pList.Count, pList.Count);
        //    Matrix<double> V_SVD = new Matrix<double>(6, 6);

        //    for (int i = 0; i < pList.Count; i++)
        //    {
        //        double xn = pList[i].X;
        //        double yn = pList[i].Y;
        //        ptsMatA[i, 0] = xn * xn;
        //        ptsMatA[i, 1] = xn * yn;
        //        ptsMatA[i, 2] = yn * yn;
        //        ptsMatA[i, 3] = xn;
        //        ptsMatA[i, 4] = yn;
        //        ptsMatA[i, 5] = 1;
        //        ptsMatB[i, 0] = 0;
        //    }
        //    CvInvoke.SVDecomp(ptsMatA, W_SVD, U_SVD, V_SVD, SvdFlag.FullUV);

        //    //Matrix<double> W_SVD_Inv = W_SVD;
        //    //for (int i = 0; i < 6; i++)
        //    //    W_SVD_Inv[i, i] = 1 / W_SVD[i, i];

        //    Matrix<double> W_SVD_Inv = new Matrix<double>(W_SVD.Transpose().Size);
        //    W_SVD_Inv.SetValue(0);
        //    for (int i = 0; i < 6; i++)
        //        W_SVD_Inv[i, i] = 1 / W_SVD.Mat.GetValue(i, 0);


        //    //Matrix<double> temp = new Matrix<double>(6, pList.Count);
        //    //Matrix<double> ptsMatA_Pinv = new Matrix<double>(6, pList.Count);
        //    Matrix<double> ptsMatA_Pinv;

        //    //CvInvoke.Gemm(W_SVD_Inv, V_SVD, 1, null, 0, temp);
        //    //CvInvoke.Gemm(U_SVD, temp, 1, null, 0, ptsMatA_Pinv);
        //    //ptsMatA_Pinv = (V_SVD.Mul(W_SVD_Inv.Transpose()));
        //    //ptsMatA_Pinv = (V_SVD.Mul(W_SVD_Inv.Transpose())).Mul(U_SVD.Transpose());
        //    ptsMatA_Pinv = V_SVD.Mul(W_SVD_Inv).Mul(U_SVD.Transpose());


        //    //Matrix<double> resMat = ptsMatA_Pinv.Mul(ptsMatB);
        //    //Matrix<double> test = ptsMatA.Mul(ptsMatB);

        //    double[] res = new double[6];
        //    string str = "Result\r\n";
        //    for (int i = 0; i < 6; i++)
        //    {
        //        res[i] = V_SVD[i, 5];
        //        str += String.Format("{0:f4}\t", res[i]);
        //    }

        //    Matrix<double> Verify = ptsMatA.Mul(new Matrix<double>(res));

        //    AddHistory(str);

        //    double A, B, C, D, E, F;
        //    A = res[0]; B = res[1]; C = res[2]; D = res[3]; E = res[4]; F = res[5];
        //    double a, b, x0, y0, theta;
        //    a = -Math.Sqrt(2 * (A * E * E + C * D * D - B * D * E + (B * B - 4 * A * C) * F) * 
        //        ((A + C) + Math.Sqrt((A - C) * (A - C) + B * B))) / (B * B - 4 * A * C);
        //    b = -Math.Sqrt(2 * (A * E * E + C * D * D - B * D * E + (B * B - 4 * A * C) * F) *
        //        ((A + C) - Math.Sqrt((A - C) * (A - C) + B * B))) / (B * B - 4 * A * C);
        //    x0 = (2 * C * D - B * E) / (B * B - 4 * A * C);
        //    y0 = (2 * A * E - B * D) / (B * B - 4 * A * C);
        //    if (B != 0)
        //        theta = Math.Atan((C - A - Math.Sqrt((A - C) * (A - C) + B * B)) / B);
        //    else if (A < C)
        //        theta = 0;
        //    else
        //        theta = Math.PI / 2;

        //    Image<Bgr, byte> imgTemp = img;
        //    imgTemp.Draw(new Ellipse(new PointF(Convert.ToSingle(x0), Convert.ToSingle(y0)), 
        //        new SizeF(Convert.ToSingle(a), Convert.ToSingle(b)), Convert.ToSingle(theta)), 
        //        new Bgr(Color.Blue), 1);
        //    picMain.Image = imgTemp.ToBitmap();
        //}


        private void btnExeEllipse_Click(object sender, EventArgs e)
        {
            //if (pList.Count < 5) return;
            int minNum = Math.Min(pList.Count, 6);
            Matrix<double> ptsMat = new Matrix<double>(pList.Count, 6);
            Mat W_SVD = new Mat();
            Mat U_SVD = new Mat();
            Mat V_SVD = new Mat();

            for (int i = 0; i < pList.Count; i++)
            {
                double xn = pList[i].X;
                double yn = pList[i].Y;
                ptsMat[i, 0] = xn * xn;
                ptsMat[i, 1] = xn * yn;
                ptsMat[i, 2] = yn * yn;
                ptsMat[i, 3] = xn;
                ptsMat[i, 4] = yn;
                ptsMat[i, 5] = 1;
            }
            CvInvoke.SVDecomp(ptsMat.Mat, W_SVD, U_SVD, V_SVD, SvdFlag.FullUV);

            double[] res = new double[6];
            string str = "Result\r\n";
            for (int i = 0; i < 6; i++)
            {
                res[i] = V_SVD.T().GetValue(i, 5);
                str += String.Format("{0:f4}\t", res[i]);
            }

            Matrix<double> Verify = ptsMat.Mul(new Matrix<double>(res));
            str = "Ax Result\r\n";
            for (int i = 0; i < minNum; i++)
                str += String.Format("{0:f6}\r\n", Verify[i, 0]);
            AddHistory(str);
            str = "Singular Value\r\n";
            for(int i = 0; i < W_SVD.Height; i++)
                str += String.Format("{0:f6}\r\n", W_SVD.GetValue(i,i));

            AddHistory(str);

            double A, B, C, D, E, F;
            A = res[0]; B = res[1]; C = res[2]; D = res[3]; E = res[4]; F = res[5];
            double a, b, x0, y0, theta;
            a = -Math.Sqrt(2 * (A * E * E + C * D * D - B * D * E + (B * B - 4 * A * C) * F) *
                ((A + C) + Math.Sqrt((A - C) * (A - C) + B * B))) / (B * B - 4 * A * C);
            b = -Math.Sqrt(2 * (A * E * E + C * D * D - B * D * E + (B * B - 4 * A * C) * F) *
                ((A + C) - Math.Sqrt((A - C) * (A - C) + B * B))) / (B * B - 4 * A * C);
            x0 = (2 * C * D - B * E) / (B * B - 4 * A * C);
            y0 = (2 * A * E - B * D) / (B * B - 4 * A * C);
            if (B != 0)
                theta = Math.Atan((C - A - Math.Sqrt((A - C) * (A - C) + B * B)) / B);
            else if (A < C)
                theta = 0;
            else
                theta = Math.PI / 2;

            try
            {
                Image<Bgr, byte> imgTemp = img;
                imgTemp.Draw(new Ellipse(new PointF(Convert.ToSingle(x0), Convert.ToSingle(y0)),
                    new SizeF(Convert.ToSingle(a * 2), Convert.ToSingle(b * 2)), Convert.ToSingle(theta / Math.PI * 180 + 90)),
                    new Bgr(Color.Blue), 1);
                picMain.Image = imgTemp.ToBitmap();
            }
            catch { }
        }
    }
}
