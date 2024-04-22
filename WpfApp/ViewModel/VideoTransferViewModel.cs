﻿using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Common;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing.Imaging;
using System.Windows.Interop;
using OpenCvSharp.Text;
using System.IO;
using System.Diagnostics;
using WpfApp.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using System.Printing;
using System.Windows.Threading;

namespace WpfApp.ViewModel
{
    class VideoTransferViewModel : ObservableObject
    {
        private BitmapSource image = null;
        public BitmapSource Image
        {
            get { return image; }
            set { image = value; this.RaisePropertyChanged("Image"); }
        }

        private string videoPath = string.Empty;
        public string VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; this.RaisePropertyChanged("VideoPath"); }
        }

        private string adImagePath = string .Empty;
        public string AdImagePath
        {
            get { return adImagePath; }
            set { adImagePath = value; this.RaisePropertyChanged("AdImagePath"); }
        }

        private HSVColorModel selectLocation;
        public HSVColorModel SelectLocation
        {
            get
            {
                return this.selectLocation;
            }
            set
            {
                this.selectLocation = value;
                this.RaisePropertyChanged("SelectLocation");
            }
        }

        private string runStatus = string.Empty;
        public string RunStatus
        { 
            get{ return runStatus; }
            set {  runStatus = value; this.RaisePropertyChanged("RunStatus"); }
        }

        private bool isValid = true;
        public bool IsValid
        { 
            get => isValid; set { isValid = value; this.RaisePropertyChanged("IsValid"); }
        }

        private ObservableCollection<HSVColorModel> adColorSource = null;
        public ObservableCollection<HSVColorModel> AdColorSource
        {
            get
            {
                if (this.adColorSource == null)
                {
                    this.adColorSource = new ObservableCollection<HSVColorModel>() {
                        new HSVColorModel() { ColorName = "黑", H_min = 0, H_max = 180, S_min = 0, S_max = 255, V_min = 0, V_max = 46 },
                        new HSVColorModel() { ColorName = "灰", H_min = 0, H_max = 180, S_min = 0, S_max = 43, V_min = 46, V_max = 220 },
                        new HSVColorModel() { ColorName = "白", H_min = 0, H_max = 180, S_min = 0, S_max = 30, V_min = 221, V_max = 255 },
                        new HSVColorModel() { ColorName = "红", H_min = 0, H_max = 10, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                        new HSVColorModel() { ColorName = "橙", H_min = 11, H_max = 25, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                        new HSVColorModel() { ColorName = "黄", H_min = 26, H_max = 34, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                        new HSVColorModel() { ColorName = "绿", H_min = 35, H_max = 77, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                        new HSVColorModel() { ColorName = "青", H_min = 78, H_max = 99, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                        new HSVColorModel() { ColorName = "蓝", H_min = 100, H_max = 124, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                        new HSVColorModel() { ColorName = "紫", H_min = 125, H_max = 155, S_min = 43, S_max = 255, V_min = 46, V_max = 255 },
                    };
                }
                return this.adColorSource;
            }
            set { this.adColorSource = value; this.RaisePropertyChanged("AdColorSource"); }
        }

        public VideoTransferViewModel() 
        {
            this.SelectLocation = this.AdColorSource[8];
        }

        private RelayCommand<object> transferCommand = null;
        public RelayCommand<object> TransferCommand
        {
            get
            {
                if (transferCommand == null)
                {
                    transferCommand = new RelayCommand<object>((o) =>
                    {
                        try
                        {
                            this.IsValid = false;
                            ShowRunStatus("正在抽取视频帧...");
                            string extractDir = _ExtractVideo(this.VideoPath);

                            ShowRunStatus("正在替换广告图片...");
                            string swapImageDir = _SwapVideoAdImage(extractDir, this.SelectLocation);

                            ShowRunStatus("正在合成视频");
                            _ComposeVideo(swapImageDir);

                            ShowRunStatus("转换完毕");
                        }
                        catch (Exception ex)
                        {
                            ShowRunStatus("转换出错：" + ex.Message);
                        }
                        finally
                        { 
                            this.IsValid = true;
                        }
                    });
                }
                return transferCommand;
            }
        }

        private void ShowRunStatus(string status)
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            Task.Factory.StartNew(() => {
                dispatcher.Invoke(() => {
                    this.RunStatus = status;
                });
            });
        }

        private RelayCommand<object> chooseVideoCommand = null;
        public RelayCommand<object> ChooseVideoCommand
        {
            get
            {
                if (chooseVideoCommand == null)
                {
                    chooseVideoCommand = new RelayCommand<object>((o) =>
                    {
                        var openFileDialog = new OpenFileDialog()
                        {
                            Filter = "视频文件|*.mp4|*.avi|"
                        };
                        var res = openFileDialog.ShowDialog();
                        if (res == true)
                        {
                            this.VideoPath = openFileDialog.FileName;
                        }
                    });
                }
                return chooseVideoCommand;
            }
        }

        private RelayCommand<object> chooseAdImageCommand = null;
        public RelayCommand<object> ChooseAdImageCommand
        {
            get
            {
                if (chooseAdImageCommand == null)
                {
                    chooseAdImageCommand = new RelayCommand<object>((o) =>
                    {
                        var openFileDialog = new OpenFileDialog()
                        {
                            Filter = "图片文件|*.png|*.jpg|"
                        };
                        var res = openFileDialog.ShowDialog();
                        if (res == true)
                        {
                            this.AdImagePath = openFileDialog.FileName;
                        }
                    });
                }
                return chooseAdImageCommand;
            }
        }

        private string _ExtractVideo(string videoPath)
        {
            var videoInfo = new FileInfo(videoPath);
            string videoName = videoInfo.Name.Replace(videoInfo.Extension, "");
            string extractDir = AppDomain.CurrentDomain.BaseDirectory + "\\extract\\" + videoName;
            if (!Directory.Exists(extractDir))
            { 
                Directory.CreateDirectory(extractDir);
            }
            string extractPath = extractDir + "\\frame_%05d.jpg";
            RunFFmpegCommand($"-i {videoPath} -r 24 -f image2 {extractPath}");
            return extractDir;
        }

        private string _SwapVideoAdImage(string imageDir, HSVColorModel color)
        {
            DirectoryInfo imgDir = new DirectoryInfo(imageDir);
            FileInfo[] images = imgDir.GetFiles("*.jpg");
            string newImgDir = AppDomain.CurrentDomain.BaseDirectory + "\\swap\\" + imgDir.Name;
            if (!Directory.Exists(newImgDir))
            {
                Directory.CreateDirectory(newImgDir);
            }
            foreach (FileInfo img in images)
            {
                string imgPath = img.FullName;
                Bitmap newImg = _ShowHsvProcess(imgPath, color.H_min, color.H_max, color.S_min, color.S_max, color.V_min, color.V_max);
                string newImgPath = newImgDir + "\\" + img.Name;
                newImg.Save(newImgPath, ImageFormat.Jpeg);
            }
            return newImgDir;
        }

        private void _ComposeVideo(string imageDir)
        { 
            var imageDirInfo = new DirectoryInfo(imageDir);
            string videoName = imageDirInfo.Name;
            string imagesFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\swap\\" + videoName + "\\frame_%05d.jpg";
            string outputVideoPath = AppDomain.CurrentDomain.BaseDirectory + "\\output\\" + videoName + ".mp4";
            RunFFmpegCommand($"-f image2 -framerate 24 -i -y {imagesFilePath} {outputVideoPath}");
        }

        private Bitmap _ShowHsvProcess(string path, int hMin, int hMax, int sMin, int sMax, int vMin, int vMax)
        {
            Mat src = new Mat(path, ImreadModes.AnyColor);
            Mat hsv = new Mat();
            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);       //转化为HSV

            Mat dst = new Mat();
            Scalar scL = new Scalar(hMin, sMin, vMin);
            Scalar scH = new Scalar(hMax, sMax, vMax);
            Cv2.InRange(hsv, scL, scH, dst);                            //获取HSV处理图片
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(20, 20),
               new OpenCvSharp.Point(-1, -1));
            Cv2.Threshold(dst, dst, 0, 255, ThresholdTypes.Binary);         //二值化
            Cv2.Dilate(dst, dst, kernel);                                   //膨胀
            Cv2.Erode(dst, dst, kernel);                                    //腐蚀
            Cv2.FindContours(dst, out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple, null);
            if (contours.Length > 0)
            {
                var boxes = contours.Select(Cv2.BoundingRect).Where(w => w.Height >= 10 && w.Width > 10);
                var imgTar = src.Clone();
                foreach (var rect in boxes)
                {
                    //Cv2.Rectangle(imgTar, new OpenCvSharp.Point(rect.X, rect.Y), new OpenCvSharp.Point(rect.X + rect.Width, rect.Y + rect.Height), new OpenCvSharp.Scalar(0, 0, 255), 1);

                    string adImgPath = AppDomain.CurrentDomain.BaseDirectory + "\\tt.png";
                    
                    ImageOverlapping(imgTar, new Mat(adImgPath, ImreadModes.AnyColor), rect.X, rect.Y);
                }
                Bitmap bitmap = BitmapConverter.ToBitmap(imgTar);
                return bitmap;
            }
            else
            {
                Bitmap bitmap = BitmapConverter.ToBitmap(src);
                return bitmap;
            }
        }

        public Bitmap ImageOverlapping(Bitmap background, Bitmap fontground, int x, int y)
        {
            using (Graphics g = Graphics.FromImage(background))
            {
                g.DrawImage(fontground, new System.Drawing.Point(x, y));
            }

            return background;
        }

        public Mat ImageOverlapping(string backgroudImgPath, string frontImgPath)
        {
            Mat matBackground = new Mat(backgroudImgPath, ImreadModes.AnyColor);
            Mat matFront = new Mat(frontImgPath);
            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(10, 20, matFront.Width, matFront.Height);
            Mat matRectInBackground = new Mat(matBackground, rect);
            matFront.CopyTo(matRectInBackground);
            return matBackground;
        }

        public Mat ImageOverlapping(Mat matBackground, Mat matFront, int x, int y)
        {
            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(x, y, matFront.Width, matFront.Height);
            Mat matRectInBackground = new Mat(matBackground, rect);
            matFront.CopyTo(matRectInBackground);
            return matBackground;
        }

        public static void RunFFmpegCommand(string commandStr)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\ffmpeg.exe";
                process.StartInfo.Arguments = " " + commandStr;//启动该进程时传递的命令行参数
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
                process.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
                process.StartInfo.RedirectStandardError = false;//重定向标准错误输出
                process.StartInfo.CreateNoWindow = false;//不显示程序窗口
                process.Start();//启动程序
                process.WaitForExit();//等待程序执行完退出进程(避免进程占用文件或者是合成文件还未生成)*
            }
        }
    }
}