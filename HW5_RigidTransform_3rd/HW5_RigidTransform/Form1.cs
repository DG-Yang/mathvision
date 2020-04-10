using Emgu.CV;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace HW5_RigidTransform
{
    public partial class Form1 : Form
    {
        Matrix<double>[] pts = new Matrix<double>[3];
        Matrix<double>[] ptsTrans = new Matrix<double>[3];

        Matrix<double>[] ptsVerify = {new Matrix<double>(3, 1)};
        Mat ptsVerified;

        Matrix<double>[] ptsSrc = { new Matrix<double>(3, 1) };
        Mat ptsDst;
        struct Rot_TwoVec
        {
            public Mat RotAxis;
            public double costheta;
            public Rot_TwoVec(Mat rotAxis, double cosTheta)
            {
                RotAxis = rotAxis;
                costheta = cosTheta;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            string[] Info = new string[3];
            Info[0] = "Maker : D.G.Yang";
            Info[1] = ".Net Framework :  4.6.1";
            Info[2] = "EmguCV : 4.1.1, 64 bit";
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
        
        // func for check
        public void AddHistory(Mat mat)
        {
            string str = string.Format("{0}\t{1}\t{2}", mat.GetValue(0,0), mat.GetValue(1, 0), mat.GetValue(2, 0));
            txtHistory.Text += str;
            txtHistory.SelectionStart = txtHistory.Text.Length + 1;
            txtHistory.ScrollToCaret();
        }

        private void btnExe_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                pts[i] = new Matrix<double>(3, 1);
                ptsTrans[i] = new Matrix<double>(3, 1);
            }

            pts[0].Data = new double[3, 1] { { -0.5 }, { 0 }, { 2.121320 } };
            pts[1].Data = new double[3, 1] { { 0.5 }, { 0 }, { 2.121320 } };
            pts[2].Data = new double[3, 1] { { 0.5 }, { -0.707107 }, { 2.828427 } };

            ptsTrans[0].Data = new double[3, 1] { { 1.363005 }, { -0.427130 }, { 2.339082 } };
            ptsTrans[1].Data = new double[3, 1] { { 1.748084 }, { 0.437983 }, { 2.017688 } };
            ptsTrans[2].Data = new double[3, 1] { { 2.636461 }, { 0.184843 }, { 2.400710 } };

            ////for test : { 1,1,1} should be { -2,1,1}
            //pts[0].Data = new double[3, 1] { { 1 }, { 1 }, { 0 } };
            //pts[1].Data = new double[3, 1] { { 1 }, { 2 }, { 0 } };
            //pts[2].Data = new double[3, 1] { { 2 }, { 2 }, { 0 } };

            //ptsTrans[0].Data = new double[3, 1] { { -2 }, { 0 }, { 1 } };
            //ptsTrans[1].Data = new double[3, 1] { { -1 }, { 0 }, { 1 } };
            //ptsTrans[2].Data = new double[3, 1] { { -1 }, { 0 }, { 2 } };

            // pts should be array... if not, getvalue doesn't works well...
            ptsVerify[0].Data = new double[3, 1] { { 0.5 }, { 0.707107 }, { 2.828427 } };
            ptsSrc[0].Data = new double[3, 1] { { 1 }, { 1 }, { 1 } };

            // 1st. Translation = P1'-P1
            // vt : vector, translation
            Mat vt = pts[0].Mat - ptsTrans[0].Mat;

            // 2nd, 3rd : h, h' are normal vectors
            // vn : vector, normal
            Mat nv_src = FindNormalVector_3Points(new Mat[3] { pts[0].Mat, pts[1].Mat, pts[2].Mat });
            Mat nv_dst = FindNormalVector_3Points(new Mat[3] { ptsTrans[0].Mat, ptsTrans[1].Mat, ptsTrans[2].Mat });

            // 4th : R1*h = h'
            Rot_TwoVec R1temp;
            FindRotationVector(nv_src, nv_dst, out R1temp.RotAxis, out R1temp.costheta);
            Mat R1 = RotationWithUnitVector(R1temp.RotAxis, R1temp.costheta);

            // 5th : R2R1(P1P2) = P1'P2'
            Rot_TwoVec R2temp;
            Mat v1 = pts[1].Mat - pts[0].Mat;
            Mat R1v1 = MatCross(R1, v1);
            Mat v1Trans = ptsTrans[1].Mat - ptsTrans[0].Mat;
            FindRotationVector(R1v1, v1Trans, out R2temp.RotAxis, out R2temp.costheta);
            Mat R2 = RotationWithUnitVector(R2temp.RotAxis, R2temp.costheta);

            //ptsVerified = ptsVerify.Mat.Cross(R1).Cross(R2) + vt;
            //ptsVerified = R2.Cross(R1).Cross(ptsVerify.Mat) + vt;
            ptsVerified = MatCross(R2, MatCross(R1, ptsVerify[0].Mat - pts[0].Mat)) + ptsTrans[0].Mat;
            ptsDst = MatCross(R2, MatCross(R1, ptsSrc[0].Mat - pts[0].Mat)) + ptsTrans[0].Mat;

            AddHistory(MatToString(R1, "R1"));
            AddHistory(MatToString(R2, "R2"));
            AddHistory(MatToString(vt, "T"));
            AddHistory(MatToString(R1, R2, vt));

            string str1 = "Point 4, before\r\n";
            string str2 = "Point 4, after\r\n";
            string str3 = "Point 5, before\r\n";
            string str4 = "Point 5, after\r\n";
            for (int i = 0; i < 3; i++)
            {
                str1 += String.Format("{0:f3}\t", ptsVerify[0].Mat.GetValue(i, 0));
                str2 += String.Format("{0:f3}\t", ptsVerified.GetValue(i, 0));
                str3 += String.Format("{0:f3}\t", ptsSrc[0].Mat.GetValue(i, 0)); 
                str4 += String.Format("{0:f3}\t", ptsDst.GetValue(i, 0));
            }
            AddHistory(str1);
            AddHistory(str2);
            AddHistory(str3);
            AddHistory(str4);
            AddHistory(String.Format("Det(R1) : {0:F3}", CvInvoke.Determinant(R1)));
            AddHistory(String.Format("Det(R2) : {0:F3}", CvInvoke.Determinant(R2)));
        }
        
        private Mat FindNormalVector_3Points(Mat[] pts)
        {
            Mat temp = (pts[1] - pts[0]).Cross(pts[2] - pts[0]);
            Mat res = temp / Math.Sqrt(temp.Dot(temp));
            return res.Clone();
        }

        private Mat FindNormalVector_2Vectors(Mat vec1, Mat vec2)
        {
            Mat temp = vec1.Cross(vec2);
            Mat res = temp / Math.Sqrt(Math.Abs(temp.Dot(temp)));
            return res.Clone();
        }

        private void FindRotationVector(Mat src, Mat dst, out Mat RotAxis, out double cost)
        {
            Mat temp = src.Cross(dst);

            RotAxis = temp / Math.Sqrt(temp.Dot(temp));
            //cost = src.Dot(dst) / Math.Sqrt(src.Dot(src) + dst.Dot(dst));
            cost = src.Dot(dst) / (Math.Sqrt(src.Dot(src)) * Math.Sqrt(dst.Dot(dst)));

        }

        private unsafe Mat RotationWithUnitVector(Mat vn, double cost)
        {
            double x, y, z;
            x = vn.GetValue(0, 0);
            y = vn.GetValue(1, 0);
            z = vn.GetValue(2, 0);
            Matrix<double> Rot = new Matrix<double>(3, 3);
            double sint = Math.Sqrt(1 - cost * cost);

            Rot.Data[0,0] = cost + (x * x) * (1 - cost);
            Rot.Data[0,1] = x * y * (1 - cost) - z * sint;
            Rot.Data[0,2] = x * z * (1 - cost) + y * sint;

            Rot.Data[1,0] = y * x * (1 - cost) + z * sint;
            Rot.Data[1,1] = cost + y * y * (1 - cost);
            Rot.Data[1,2] = y * z * (1 - cost) - x * sint;

            Rot.Data[2,0] = z * x * (1 - cost) - y * sint;
            Rot.Data[2,1] = z * y * (1 - cost) + x * sint;
            Rot.Data[2,2] = cost + z * z * (1 - cost);

            return Rot.Mat.Clone();
        }

        private Mat MatCross(Mat Rmat, Mat Pmat)
        {
            Mat res = Pmat.Clone();
            for (int i = 0; i < 3; i++)
            {
                double temp = 0;
                for (int j = 0; j < 3; j++)
                {
                    temp += Rmat.GetValue(i, j) * Pmat.GetValue(j, 0);
                }
                res.SetValue(i, 0, temp);
            }
            return res.Clone();
        }

        private String MatToString(Mat R1, Mat R2, Mat T)
        {
            string str = "R|T Matrix\r\n";
            double[,] Res = new double[3, 4];
            
            Matrix<double> mat1 = new Matrix<double>(3, 3);
            Matrix<double> mat2 = new Matrix<double>(3, 3);
            R1.CopyTo(mat1);
            R2.CopyTo(mat2);
            Matrix<double> matRes = mat1.Mul(mat2);

            Mat RMat = matRes.Mat.Clone();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    Res[i, j] = RMat.GetValue(i, j);
                Res[i, 3] = T.GetValue(i, 0);
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                    str += String.Format("{0:f3}\t", Res[i, j]);
                str += "\r\n";
            }
            return str;
        }

        private String MatToString(Mat mat, string MatName)
        {
            string str = MatName + "\r\n";
            
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                    str += String.Format("{0:f3}\t", mat.GetValue(i, j));
                str += "\r\n";
            }
            return str;
        }
    }

    public static class MatExtension
    {
        public static double GetValue(this Mat mat, int row, int col)
        {
            double[] value = new double[1];
            //Marshal.Copy(value, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }
        public static void SetValue(this Mat mat, int row, int col, double value)
        {
            var target = new[] { value };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        }
    }
}
