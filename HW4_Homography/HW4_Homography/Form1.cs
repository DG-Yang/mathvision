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
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

namespace HW4_Homography
{
    public partial class FormMain : Form
    {
        List<PointF> pList = new List<PointF>();
        int clickNum = 0;
        Image<Bgr, byte> img;
        int radius = 4;
        int thickness = 2;
        static bool isStop;
        Bgr[] colors;

        PointF[] ptSrcArr = new PointF[]
             {new PointF(100, 100), new PointF(200, 100),
                new PointF(200, 200), new PointF(100, 200)};

        public FormMain()
        {
            InitializeComponent();
            txtHistory.Text = "Program Started";
            img = new Image<Bgr, byte>(picMain.Size);
            img.SetValue(new Bgr(Color.White));

            colors = new Bgr[4];
            colors[0] = new Bgr(Color.Black);
            colors[1] = new Bgr(Color.Red);
            colors[2] = new Bgr(Color.Green);
            colors[3] = new Bgr(Color.Blue);
        }

        public void AddHistory(string str)
        {
            str = "\r\n" + DateTime.Now.ToString("HH:MM") + "  " + str;
            txtHistory.Text += str;
            txtHistory.SelectionStart = txtHistory.Text.Length + 1;
            txtHistory.ScrollToCaret();
        }

        public void ClearHistory()
        {
            txtHistory.Text = "";
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //p[clickNum] = new PointF(e.X, e.Y);

            if (clickNum > 3)
            {
                pList.RemoveAt(0);
            }

            pList.Add(new PointF(e.X, e.Y));
            clickNum++;
            AddHistory(String.Format("New Point : {0:0}, {1:0}", pList.Last().X, pList.Last().Y));


            // for graphic
            img.SetValue(new Bgr(Color.White));
            for (int i = 0; i < pList.Count; i++)
                img.Draw(new CircleF(pList[i], radius), colors[i], thickness);
            if (pList.Count > 0)
                for (int i = 0; i < pList.Count-1; i++)
                    img.Draw(new LineSegment2DF(pList[i],
                        pList[i+1]), colors[i], thickness);

            Image<Bgr, byte> imgTemp = img;

            if (pList.Count > 3)
                imgTemp.Draw(new LineSegment2DF(pList.Last(), pList[0]), colors[3], thickness);
            if (pList.Count > 3 && btnExe.Enabled == false)
                btnExe.Enabled = true;

            picMain.Image = imgTemp.ToBitmap();
        }

        private void btnExe_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(picMain.Image as Bitmap);
            DrawRectangleOri(ref image);
            picMain.Image = image.Bitmap;

            Mat Homography = new Mat(new Size(3, 3), Emgu.CV.CvEnum.DepthType.Cv32F, 1);

            PointF[] ptDstArr = pList.ToArray();
            Homography = CvInvoke.FindHomography(ptSrcArr, ptDstArr, Emgu.CV.CvEnum.RobustEstimationAlgorithm.AllPoints, 3, null);

            string result = classifyHomography(SignOfVectors(Homography, ptSrcArr, null));
            AddHistory(result);
        }

        private void DrawRectangleOri(ref Image<Bgr, byte> image)
        {
            for (int i = 0; i < 4; i++)
            {
                int j = (i == 3) ? 0 : i + 1;
                image.Draw(new LineSegment2DF(ptSrcArr[i], ptSrcArr[j]), colors[i], thickness);
            }
        }
        
        private bool[] SignOfVectors(Mat Homography, PointF[] pointsOri, PointF[] pointsTrans)
        {
            PointF[] ptsTrans = new PointF[4];

            if (pointsTrans == null)
                ptsTrans = CvInvoke.PerspectiveTransform(pointsOri, Homography);
            else
                ptsTrans = pointsTrans;

            string str = "Homography Matrix";

            Matrix<double> Res = new Matrix<double>(Homography.Size);
            Homography.CopyTo(Res);

            for (int i = 0; i < 3; i++)
            {
                str += "\r\n";
                for (int j = 0; j < 3; j++)
                    str += String.Format("{0:N2}\t", Res[i, j]);
            }
            AddHistory(str);

            string str2 = "\r\nx,y\tx',y'";
            for (int i = 0; i < 4; i++)
            {
                str2 += string.Format("\r\n{0:N0},{1:N0}\t{2:N0},{3:N0}", pointsOri[i].X, pointsOri[i].Y, ptsTrans[i].X, ptsTrans[i].Y);
            }
            AddHistory(str2);


            str2 = "Vectors to each points";
            PointF[] VectorsOri = new PointF[4];
            PointF[] VectorsTrans = new PointF[4];
            for (int i = 0; i < 4; i++)
            {
                int j;
                j = (i == 3) ? 0 : (i + 1);
                VectorsOri[i].X = (pointsOri[j].X - pointsOri[i].X);
                VectorsOri[i].Y = (pointsOri[j].Y - pointsOri[i].Y);

                VectorsTrans[i].X = (ptsTrans[j].X - ptsTrans[i].X);
                VectorsTrans[i].Y = (ptsTrans[j].Y - ptsTrans[i].Y);
                str2 += string.Format("\r\nVector {0}: {1:N0}\t{2:N0}", i.ToString(), VectorsTrans[i].X, VectorsTrans[i].Y);
            }
            AddHistory(str2);


            str2 = "Sign of outer product\r\n" + "original\r\n";
            bool[] signOri = new bool[4];
            bool[] signRes = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                int j;
                j = (i == 3) ? 0 : (i + 1);
                signOri[i] = (((VectorsOri[i].X * VectorsOri[j].Y) - (VectorsOri[i].Y) * VectorsOri[j].X) >= 0);
                str2 += String.Format("{0}   ", ((signOri[i] == true) ? "T" : "F"));
            }
            str2 += "\r\n" + "translated\r\n";
            for (int i = 0; i < 4; i++)
            {
                int j;
                j = (i == 3) ? 0 : (i + 1);
                signRes[i] = (((VectorsTrans[i].X * VectorsTrans[j].Y) - (VectorsTrans[i].Y) * VectorsTrans[j].X) >= 0);
                str2 += String.Format("{0}   ", ((signRes[i] == true) ? "T" : "F"));
            }
            AddHistory(str2);

            bool[] result = new bool[4];
            int tempOri;
            int tempRes;
            // Change bool to int to avoid (false && false == false)
            for (int i = 0; i < 4; i++)
            {
                tempOri = (signOri[i] == true ? 1 : 0);
                tempRes = (signRes[i] == true ? 1 : 0);

                result[i] = (tempOri == tempRes);
            }
            return result;
        }

        private string classifyHomography(bool[] signVectors)
        {
            int numTrue = 0;
            foreach (bool istrue in signVectors)
                numTrue += (istrue == true ? 1 : 0);
            switch (numTrue)
            {
                case 0:
                    return "reflection";
                case 1:
                    return "reflective concave";
                case 2:
                    return "twisted";
                case 3:
                    return "concave";
                case 4:
                    return "normal";
                default:
                    return String.Format("error : n = {0}", numTrue);
            }
        }


        private void btnVideo_Click(object sender, EventArgs e)
        {
            isStop = false;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg; *.bmp) | *.jpg; *.bmp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filepath = openFileDialog.FileName;
                    string[] temp;
                    string temp2 = "";
                    string filepath2;

                    temp = filepath.Split('\\');
                    filepath2 = filepath.Split('\\').Last().Split('.')[0] + ".mp4";
                    for (int i = 0; i < temp.Count() - 1; i++)
                        temp2 += temp[i] + '\\';
                    filepath2 = temp2 + filepath2;
                    homographyVideo(filepath, filepath2);
                }
            }
        }

        private async void homographyVideo(string fnamePicture, string fnameVideo)
        {
            double magSrc = 1;
            double magDst = 1;
            Image<Bgr, byte> imageOri = new Image<Bgr, byte>(fnamePicture);
            Image<Bgr, byte> image = imageOri.Resize(magSrc, Emgu.CV.CvEnum.Inter.Area);
            Image<Gray, byte> imageGray = image.Convert<Gray, byte>();

            VideoCapture video = new VideoCapture(fnameVideo, VideoCapture.API.Any);
            double totalFrame = video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
            double currentFrameNo = 0;
            int FPS = Convert.ToInt32(video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
            Mat currentFrame = new Mat();


            //var src = (Image : image, Keypoint : keypoint)

            ORBDetector detector = new ORBDetector();


            VectorOfKeyPoint KeypointImage = new VectorOfKeyPoint();
            VectorOfKeyPoint KeypointVideo = new VectorOfKeyPoint();
            Mat DescriptorImage = new Mat();
            Mat DescriptorVideo = new Mat();

            Mat imgKeypointsImage = new Mat();
            Mat imgKeypointsVideo = new Mat();
            Mat imgMatches = new Mat();
            Mat imgWarped = new Mat();
            //BFMatcher matcher = new BFMatcher(DistanceType.Hamming2, crossCheck: false);

            int k = 2;
            bool crossCheck = false;
            BFMatcher matcher = new BFMatcher(DistanceType.Hamming2, crossCheck);

            //double uniquenessThreshold = 0.8;
            //Matrix<byte> maskMatrix;
            //Mat maskMat;

            //detector.DetectAndCompute(imageGray, null, KeypointImage, DescriptorImage, false);
            detector.DetectAndCompute(image, null, KeypointImage, DescriptorImage, false);
            matcher.Add(DescriptorImage);

            float ms_MAX_DIST = 50f;
            float ms_MIN_RATIO = 0.6f;

            while (currentFrameNo < totalFrame && !isStop)
            {
                video.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, currentFrameNo);
                video.Read(currentFrame);
                currentFrameNo++;

                Image<Bgr, byte> frame = currentFrame.ToImage<Bgr, Byte>().Resize(magDst, Emgu.CV.CvEnum.Inter.Area);
                Image<Gray, byte> frameGray = frame.Convert<Gray, byte>();


                VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();

                //detector.DetectAndCompute(frameGray, null, KeypointVideo, DescriptorVideo, false);
                detector.DetectAndCompute(frame, null, KeypointVideo, DescriptorVideo, false);

                matcher.KnnMatch(DescriptorVideo, matches, k, mask: null);

                try
                {
                    VectorOfVectorOfDMatch filteredMatches = new VectorOfVectorOfDMatch();
                    List<MDMatch[]> filteredMatchesList = new List<MDMatch[]>();

                    // To find homography
                    
                    //matcher.Match(DescriptorVideo, DescriptorImage, matches, mask: null);

                    MDMatch[][] matchesArray = matches.ToArrayOfArray();
                    


                    //Apply ratio test
                    for (int i = 0; i < matchesArray.Length; i++)
                    {
                        //MDMatch first = matchesArray[i][0];
                        float dist1 = matchesArray[i][0].Distance;
                        float dist2 = matchesArray[i][1].Distance;
                        if (dist1 < ms_MIN_RATIO * dist2)
                            filteredMatchesList.Add(matchesArray[i]);
                    }

                    //Filter by threshold
                    MDMatch[][] defCopy = new MDMatch[filteredMatchesList.Count][];
                    filteredMatchesList.CopyTo(defCopy);
                    filteredMatchesList = new List<MDMatch[]>();

                    foreach (var item in defCopy)
                    {
                        if (item[0].Distance < ms_MAX_DIST)
                            filteredMatchesList.Add(item);
                    }
                    
                    filteredMatches = new VectorOfVectorOfDMatch(filteredMatchesList.ToArray());


                    // another filter..
                    //maskMatrix = new Matrix<byte>(DescriptorVideo.Rows, 1);
                    //maskMatrix.SetValue(255);
                    //maskMat = new Mat(maskMatrix.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 1, maskMatrix.Ptr, step: 0);
                    //Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, maskMat);

                    //homography
                    Mat Homography = GetHomography(KeypointImage, KeypointVideo, filteredMatchesList);

                    Matrix<double> Res = new Matrix<double>(Homography.Size);
                    Homography.CopyTo(Res);
                    string str = "Homography Matrix";
                    for (int i = 0; i < 3; i++)
                    {
                        str += "\r\n";
                        for (int j = 0; j < 3; j++)
                            str += String.Format("{0:N5}\t", Res[i, j]);
                    }
                    ClearHistory();
                    AddHistory(str);


                    // // Draw it
                    //int offset = 320;
                    //int width = 320;
                    //int height = 240;
                    //Image<Bgr, byte> imgRes = new Image<Bgr, byte>(picMain.Size);

                    //imgRes.ROI = new Rectangle(0, 0, picMain.Width / 2, picMain.Height);
                    //imageOri.Resize(width, height, Emgu.CV.CvEnum.Inter.Area).CopyTo(imgRes);
                    //imgRes.ROI = new Rectangle(offset, 0, picMain.Width / 2, picMain.Height);
                    //currentFrame.ToImage<Bgr, byte>().Resize(width, height, Emgu.CV.CvEnum.Inter.Area).CopyTo(imgRes);
                    //imgRes.ROI = Rectangle.Empty;

                    //Features2DToolbox.DrawKeypoints(image, KeypointImage, imgKeypointsImage, new Bgr(Color.Red));
                    //Features2DToolbox.DrawKeypoints(frame, KeypointVideo, imgKeypointsVideo, new Bgr(Color.Red));

                    #region draw the projected region on the image
                    if (Homography != null)
                    {  //draw a rectangle along the projected model
                        Rectangle rect = image.ROI;
                        PointF[] pts = new PointF[]
                        {
                         new PointF(rect.Left, rect.Bottom),
                         new PointF(rect.Right, rect.Bottom),
                         new PointF(rect.Right, rect.Top),
                         new PointF(rect.Left, rect.Top)
                        };

                        // To get a result

                        PointF[] ptsTrans = new PointF[4];
                        ptsTrans = CvInvoke.PerspectiveTransform(pts, Homography);

                        Bgr CurrentColor;
                        string result = classifyHomography(SignOfVectors(Homography, pts, ptsTrans));
                        if (result == "normal")
                            CurrentColor = new Bgr(Color.Blue);
                        else
                            CurrentColor = new Bgr(Color.Red);
                        AddHistory(result);

                        image.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, CurrentColor, 5);
                        frame.DrawPolyline(Array.ConvertAll<PointF, Point>(ptsTrans, Point.Round), true, CurrentColor, 5);
                    }
                    #endregion

                    Features2DToolbox.DrawMatches(image, KeypointImage, frame, KeypointVideo, filteredMatches, 
                        imgMatches, new MCvScalar(0, 255, 0), new MCvScalar(0, 0, 255));

                    //picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                    picMain.SizeMode = PictureBoxSizeMode.CenterImage;
                    picMain.Image = imgMatches.Bitmap;

                    //string result = classifyHomography(SignOfVectors(Homography));
                    //AddHistory(result);

                    await Task.Delay(1000 / FPS);
                }
                catch
                {
                    //Features2DToolbox.DrawKeypoints(image, KeypointImage, imgKeypointsImage, new Bgr(Color.Red));
                    //Features2DToolbox.DrawKeypoints(frame, KeypointVideo, imgKeypointsVideo, new Bgr(Color.Red));
                    Features2DToolbox.DrawMatches(image, KeypointImage, frame, KeypointVideo, new VectorOfVectorOfDMatch(),
                        imgMatches, new MCvScalar(0, 0, 255), new MCvScalar(0, 0, 255));

                    picMain.SizeMode = PictureBoxSizeMode.CenterImage;
                    picMain.Image = imgMatches.Bitmap;

                    await Task.Delay(1000 / FPS);
                }
            }
        }

        private static Mat GetHomography(VectorOfKeyPoint keypointSrc, VectorOfKeyPoint keypointDst, List<MDMatch[]> matches)
        {
            MKeyPoint[] kptsSrc = keypointSrc.ToArray();
            MKeyPoint[] kptsDst = keypointDst.ToArray();

            PointF[] srcPoints = new PointF[matches.Count];
            PointF[] dstPoints = new PointF[matches.Count];

            for (int i = 0; i < matches.Count; i++)
            {
                srcPoints[i] = kptsSrc[matches[i][0].TrainIdx].Point;
                dstPoints[i] = kptsDst[matches[i][0].QueryIdx].Point;
            }
            Mat inlier_mask = new Mat(matches.Count, 1, Emgu.CV.CvEnum.DepthType.Cv8U, 3);

            Mat Homography = CvInvoke.FindHomography(srcPoints, dstPoints, Emgu.CV.CvEnum.RobustEstimationAlgorithm.Ransac, 3, inlier_mask);
            return Homography;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            clickNum = 0;
            pList.Clear();
            btnExe.Enabled = false;
            img.SetValue(new Bgr(Color.White));
            picMain.Image = img.ToBitmap();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (isStop == false)
                isStop = true;
        }
    }
}
