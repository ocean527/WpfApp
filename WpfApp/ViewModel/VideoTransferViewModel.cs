using OpenCvSharp.Extensions;
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
using System.Windows.Media;
using ag.WPF.ColorPicker;
using Color = System.Windows.Media.Color;
using OpenCvSharp.WpfExtensions;

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

        private string backgroudImagePath = string.Empty;
        public string BackgroudImagePath
        { 
            get => backgroudImagePath;
            set { backgroudImagePath = value; this.RaisePropertyChanged("BackgroudImagePath"); }
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
            get { return runStatus; }
            set { runStatus = value; this.RaisePropertyChanged("RunStatus"); }
        }

        private ImageSource previewImage;
        public ImageSource PreviewImage
        {
            get { return previewImage; }
            set { previewImage = value; this.RaisePropertyChanged("PreviewImage"); }
        }

        private bool isValid = true;
        public bool IsValid
        { 
            get => isValid; 
            set { isValid = value; this.RaisePropertyChanged("IsValid"); }
        }

        private Color videoBackgroundColor;
        public Color VideoBackgroundColor
        {
            get { return videoBackgroundColor; }
            set { videoBackgroundColor = value; this.RaisePropertyChanged(nameof(VideoBackgroundColor)); }
        }

        private Color adBackgroundColor;
        public Color AdBackgroundColor
        {
            get { return adBackgroundColor; }
            set { adBackgroundColor = value; this.RaisePropertyChanged(nameof(adBackgroundColor)); }
        }

        private bool isRomoveVideoBgChecked;
        public bool IsRomoveVideoBgChecked
        { 
            get { return isRomoveVideoBgChecked; }
            set {  isRomoveVideoBgChecked = value; this.RaisePropertyChanged(nameof(IsRomoveVideoBgChecked)); }
        }

        private bool isRomoveAdBgChecked;
        public bool IsRomoveAdBgChecked
        { 
            get { return isRomoveAdBgChecked; }
            set { isRomoveAdBgChecked = value; this.RaisePropertyChanged(nameof(IsRomoveAdBgChecked)); }
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
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                //ShowRunStatus("正在抽取视频帧...");
                                //string extractDir = _ExtractVideo(this.VideoPath);

                                //ShowRunStatus("正在替换广告图片...");
                                //string swapImageDir = _SwapVideoAdImage(extractDir, this.SelectLocation);

                                //ShowRunStatus("正在合成视频...");
                                //_ComposeVideo(swapImageDir);

                            
                                ShowRunStatus("正在转换中...", false);
                                var videoInfo = new FileInfo(VideoPath);
                                string videoName = videoInfo.Name.Replace(videoInfo.Extension, "");
                                string newVideoPath = AppDomain.CurrentDomain.BaseDirectory + "\\swap\\" + "\\" + videoName + ".mp4";
                                SwapGreenBackground(VideoPath, AdImagePath, BackgroudImagePath, newVideoPath);
                                ShowRunStatus("转换完毕", true);
                            
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.StackTrace);
                                ShowRunStatus("转换出错：" + ex.Message, true);
                            
                            }
                            finally
                            { 
                                this.IsValid = true;
                            }
                        });
                    });
                }
                return transferCommand;
            }
        }

        private void ShowRunStatus(string status, bool IsEnable)
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
 
            dispatcher.Invoke(() => {
                this.RunStatus = status;
                this.IsValid = IsEnable;
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
                            Filter = "视频文件(*.mp4,*.avi)|*.mp4;.avi"
                        };
                        var res = openFileDialog.ShowDialog();
                        if (res == true)
                        {
                            this.VideoPath = openFileDialog.FileName;
                            Mat frame = GetVideoFirstFrame(this.VideoPath);
                            this.PreviewImage = MatToBitmapImage(frame);
                        }
                    });
                }
                return chooseVideoCommand;
            }
        }

        private RelayCommand<object> chooseBackgroudImageCommand = null;
        public RelayCommand<object> ChooseBackgroudImageCommand
        {
            get
            {
                if (chooseBackgroudImageCommand == null)
                {
                    chooseBackgroudImageCommand = new RelayCommand<object>((o) =>
                    {
                        var openFileDialog = new OpenFileDialog()
                        {
                            Filter = "图片文件(*.png,*.jpg)|*.png;*.jpg"
                        };
                        var res = openFileDialog.ShowDialog();
                        if (res == true)
                        {
                            this.BackgroudImagePath = openFileDialog.FileName;
                        }
                    });
                }
                return chooseBackgroudImageCommand;
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
                            Filter = "图片文件(*.png,*.jpg)|*.png;*.jpg"
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

        private RelayCommand<object> chooseVideoBackColorCommand = null;
        public RelayCommand<object> ChooseVideoBackColorCommand
        {
            get
            {
                if (chooseVideoBackColorCommand == null)
                {
                    chooseVideoBackColorCommand = new RelayCommand<object>((o) =>
                    {
                        var picker = o as ColorPicker;
                        this.VideoBackgroundColor = picker.SelectedColor;
                    });
                }
                return chooseVideoBackColorCommand;
            }
        }

        private RelayCommand<object> chooseAdBackColorCommand = null;
        public RelayCommand<object> ChooseAdBackColorCommand
        {
            get
            {
                if (chooseAdBackColorCommand == null)
                {
                    chooseAdBackColorCommand = new RelayCommand<object>((o) =>
                    {
                        var picker = o as ColorPicker;
                        this.AdBackgroundColor = picker.SelectedColor;
                    });
                }
                return chooseAdBackColorCommand;
            }
        }

        private RelayCommand<object> removeVideoBgCheckedCommand = null;
        public RelayCommand<object> RemoveVideoBgCheckedCommand
        {
            get
            {
                if (removeVideoBgCheckedCommand == null)
                {
                    removeVideoBgCheckedCommand = new RelayCommand<object>((o) =>
                    {
                        if (VideoPath == string.Empty)
                        {
                            MessageBox.Show("请先选择视频文件");
                            return;  
                        }
                        Mat preImage = ImageSourceToMat(PreviewImage); 
                        RemoveImageScreen(preImage,
                            p =>
                            {
                                double diff = Math.Pow(p.Item0 - VideoBackgroundColor.B, 2) + Math.Pow(p.Item1 - VideoBackgroundColor.G, 2) + Math.Pow(p.Item2 - VideoBackgroundColor.R, 2);
                                int threshold = 200;
                                if (Math.Abs(p.Item0 - VideoBackgroundColor.B) < threshold && Math.Abs(p.Item1 - VideoBackgroundColor.G) < threshold && Math.Abs(p.Item2 - VideoBackgroundColor.R) < threshold)
                                    return true;
                                return false;
                            });
                        this.PreviewImage = MatToBitmapImage(preImage);
                    });
                }
                return removeVideoBgCheckedCommand;
            }
        }


        public void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
        {
            var Background = new SolidColorBrush(e.NewValue);
        }

        private string _ExtractVideo(string videoPath)
        {
            var videoInfo = new FileInfo(videoPath);
            string videoName = videoInfo.Name.Replace(videoInfo.Extension, "");
            string extractDir = AppDomain.CurrentDomain.BaseDirectory + "\\extract\\" + videoName;
            if (Directory.Exists(extractDir))
            { 
                Directory.Delete(extractDir, true);
            }
            Directory.CreateDirectory(extractDir);
            string extractPath = extractDir + "\\frame_%05d.jpg";
            RunFFmpegCommand($"-i {videoPath} -r 24 -f image2 {extractPath}");
            return extractDir;
        }

        private string _SwapVideoAdImage(string imageDir, HSVColorModel color)
        {
            DirectoryInfo imgDir = new DirectoryInfo(imageDir);
            FileInfo[] images = imgDir.GetFiles("*.jpg");
            string newImgDir = AppDomain.CurrentDomain.BaseDirectory + "\\swap\\" + imgDir.Name;
            if (Directory.Exists(newImgDir))
            {
                Directory.Delete(newImgDir, true);
            }
            Directory.CreateDirectory(newImgDir);
            foreach (FileInfo img in images)
            {
                string imgPath = img.FullName;
                Bitmap newImg = _ShowHsvProcess(imgPath, color.H_min, color.H_max, color.S_min, color.S_max, color.V_min, color.V_max, this.AdImagePath);
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
            RunFFmpegCommand($"-y -f image2 -framerate 24 -i {imagesFilePath} {outputVideoPath}");
        }

        private Bitmap _ShowHsvProcess(string path, int hMin, int hMax, int sMin, int sMax, int vMin, int vMax, string adImagePath)
        {
            Mat src = new Mat(path, ImreadModes.AnyColor);
            Mat hsv = new Mat();
            Mat adImage = new Mat(adImagePath, ImreadModes.AnyColor);
            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);       //转化为HSV

            Mat dst = new Mat();
            Scalar scL = new Scalar(hMin, sMin, vMin);
            Scalar scH = new Scalar(hMax, sMax, vMax);
            Cv2.InRange(hsv, scL, scH, dst);                            //获取HSV处理图片
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(20, 20), new OpenCvSharp.Point(-1, -1));
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
                    Cv2.Resize(adImage, adImage, new OpenCvSharp.Size(rect.Width, rect.Height));
                    ImageOverlapping(imgTar, adImage, rect.X, rect.Y);
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

        public BitmapImage MatToBitmapImage(Mat image)
        {
            Bitmap bitmap = BitmapConverter.ToBitmap(image);
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public Mat ImageSourceToMat(ImageSource imageSource)
        {
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            // 将Bitmap转换为Mat对象
            Mat mat = BitmapSourceConverter.ToMat(bitmapSource);
            return mat;
        }

            private OpenCvSharp.Rect _ShowHsvProcess2(Mat src)
        {
            Mat hsv = new Mat();
            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);       //转化为HSV

            Mat dst = new Mat();
            Scalar scL = new Scalar(100, 43, 46);
            Scalar scH = new Scalar(124, 255, 255);
            Cv2.InRange(hsv, scL, scH, dst);                            //获取HSV处理图片
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(20, 20), new OpenCvSharp.Point(-1, -1));
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
                    return rect;
                }
            }
                
            throw new Exception("找不到广告位");
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

        public Mat ImageUpscale(Mat src, OpenCvSharp.Size size, MatType type, OpenCvSharp.Rect pos)
        {
            Mat blackMat = new Mat(size, type, Scalar.Black);
            int width = pos.X + src.Width > size.Width ? pos.X + src.Width - size.Width : src.Width;
            int height = pos.Y + src.Height > size.Height ? pos.Y + src.Height - size.Height : src.Height;
            OpenCvSharp.Rect roi = new OpenCvSharp.Rect(pos.X, pos.Y, width, height);
            Mat matRectInBackground = new Mat(blackMat, roi);
            //Cv2.ImShow("000003", src);
            src.CopyTo(matRectInBackground);
            //Cv2.ImShow("000004", blackMat);
            return blackMat;
        }

        private Mat GetVideoFirstFrame(string videoPath)
        {
            Mat frameMat = new Mat();
            using (VideoCapture vc = new VideoCapture(videoPath))
            {
                vc.Read(frameMat);
            }
            return frameMat;
        }

        private void SwapGreenBackground(string greenBackgroundVideoPath, string adImagePath, string swapbackgroundImagePath, string outputPath)
        {
            using (VideoCapture videoCapture = new VideoCapture(greenBackgroundVideoPath))
            using (Mat frameMat = new Mat())
            using (Mat mat_bg = Cv2.ImRead(swapbackgroundImagePath))
            using (Mat adimg_bg = Cv2.ImRead(adImagePath))
            using (VideoWriter videoWriter = new VideoWriter(outputPath, FourCC.MPG4, videoCapture.Fps, new OpenCvSharp.Size(videoCapture.FrameWidth, videoCapture.FrameHeight), true))
            {
                videoCapture.PosFrames = 0;
                while (true)
                {
                    if (!videoCapture.Read(frameMat))
                    {
                        break;
                    }

                    //去除广告图片白色背景
                    RemoveImageScreen(adimg_bg,
                        p =>
                        {
                            //int max = Math.Max(p.Item0, Math.Max(p.Item1, p.Item2));
                            if (p.Item0 > 250 && p.Item1 > 250 && p.Item2 > 250)  //白幕
                                return true;
                            return false;
                        });

                    OpenCvSharp.Rect adPositionRect = _ShowHsvProcess2(frameMat);  //找出蓝色广告位的位置
                    //扣除广告位蓝幕
                    RemoveImageScreen(frameMat,
                        p =>
                        {
                            int max = Math.Max(p.Item0, Math.Max(p.Item1, p.Item2));
                            if (max == p.Item0 && p.Item0 > 40)  //蓝幕
                                return true;
                            return false;
                        });

                    //广告图缩放
                    //Cv2.Resize(adimg_bg, adimg_bg, new OpenCvSharp.Size(adPositionRect.Width, adPositionRect.Height));
                    var adImageUpscale = ImageUpscale(adimg_bg, frameMat.Size(), frameMat.Type(), adPositionRect);

                    
                    //将广告图叠在背景图上
                    //Mat res = ImageOverlapping(frameMat, adimg_bg, adPositionRect.X, adPositionRect.Y);

                    var adImageUpscale_clone = adImageUpscale.Clone();
                    //替换广告位图片
                    MergeImage(frameMat, adImageUpscale_clone,
                        p =>
                        {
                            if (p == new Vec3b(0, 0, 0))
                            {
                                return false;
                            }
                            return true;
                        });

                    //扣除绿幕
                    RemoveImageScreen(frameMat,
                        p =>
                        {
                            int max = Math.Max(p.Item0, Math.Max(p.Item1, p.Item2));
                            if (max == p.Item1 && p.Item1 > 100)  //绿幕
                                return true;
                            return false;
                        });
                    var bg_clone = mat_bg.Clone();

                    //Cv2.ImShow("press any key to quit", adImageUpscale_clone);
                    //替换背景
                    MergeImage(bg_clone, frameMat,
                        p =>
                        {
                            if (p == new Vec3b(0, 0, 0))
                            {
                                return false;
                            }
                            return true;
                        });
                    //Cv2.ImShow("press any key to quit", adImageUpscale);
                    //if (Cv2.WaitKey(1) > 0)
                    //{
                    //    break;
                    //}

                    this.PreviewImage = MatToBitmapImage(bg_clone);
                    videoWriter.Write(bg_clone);
                }
            }
        }

        private static unsafe void RemoveImageScreen(Mat src, Func<Vec3b, bool> func)
        {
            Vec3b* start = (Vec3b*)src.DataStart;
            Vec3b* end = (Vec3b*)src.DataEnd;
            for (Vec3b* p = start; p <= end; p++)
            {
                if (func(*p))
                {
                    *p = new Vec3b(0, 0, 0);
                }
            }
        }

        private static unsafe void MergeImage(Mat bg, Mat src, Func<Vec3b, bool> func)
        {
            Cv2.Resize(bg, bg, src.Size());
            Vec3b* bg_pointer = (Vec3b*)bg.DataStart;
            Vec3b* start = (Vec3b*)src.DataStart;
            Vec3b* end = (Vec3b*)src.DataEnd;
            for (Vec3b* p = start; p <= end; p++, bg_pointer++)
            {
                *bg_pointer = func(*p) ? *p : *bg_pointer;
            }
        }

        private static unsafe void MergeAdImage(Mat bg, Mat src, OpenCvSharp.Rect adPosition, Func<Vec3b, bool> func)
        {
            Vec3b* src_pointer = (Vec3b*)src.DataStart;
            Vec3b* start = (Vec3b*)src.DataStart;
            Vec3b* end = (Vec3b*)src.DataEnd;
            for (Vec3b* p = start; p <= end; p++, src_pointer++)
            {
                *src_pointer = func(*p) ? *p : *src_pointer;
            }
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
