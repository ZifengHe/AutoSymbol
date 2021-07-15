using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;
using Path = System.IO.Path;
using System.Xml.Serialization;
using System.Windows.Markup;
using System.Windows.Forms;
//using AForge.Video.FFMPEG;
using Accord;
using Accord.Video;
using System.Drawing;
using System.Diagnostics;
using Accord.Video.FFMPEG;
using System.Runtime;
using System.Reflection;
using System.Windows.Threading;
using M = System.Windows.MessageBox;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace FrameByFrame
{
    public class Setting
    {        
        public static Dictionary<string, string> All = new Dictionary<string, string>();
        public static string CSVFile;
        public static string VideoFile;
        public static string ImgFolder;
        public static double RowStrokeThickness;
        public static double RowMaxLength;
        public static double RowHeight;
        public static double RowTopY;
        public static double RowLeftX;
    }

    public class DataSeries
    {
        public static List<OneFrameData> AllFrameData = new List<OneFrameData>();
        public static Dictionary<string, BitmapImage> AllImages = new Dictionary<string, BitmapImage>();
        public static Dictionary<string, string> AllDisplayName = new Dictionary<string, string>();
        public static void LoadData()
        {
            /// Step 1. Read raw csv
            /// Step 2. Smooth data to KeyFrame
            /// Step 3. Sort KeyFrame Order 
            /// Step 4. Smooth individual frame height number

            int totalKeyFrame = int.Parse(Setting.All["TotalKeyFrame"]);
            int lockFramesPerKey = int.Parse(Setting.All["LockFramesPerKey"]);
            int mobileFramesPerKey = int.Parse(Setting.All["MobileFramesPerKey"]);
            int totalFramesPerKey = lockFramesPerKey + mobileFramesPerKey;
            int totalFrames = totalFramesPerKey * totalKeyFrame;


            List<string[]> rawData = ReadRawData();
            List<OneFrameData> keyFrameData = ExtractKeyFrameData(rawData, totalKeyFrame);
            SetAllFrameData(keyFrameData, totalFramesPerKey, lockFramesPerKey);
            LoadImages();
        }

        private static void LoadImages()
        {
            string [] files = Directory.GetFiles(Setting.ImgFolder);
            foreach(var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                AllImages[name.ToUpper()] = new BitmapImage(new Uri(file));
            }
        }
        private static void SetAllFrameData(List<OneFrameData> list, int totalFramesPerKey, int lockFramesPerKey)
        {
            AllFrameData.Clear();
            for(int i=0; i < list.Count-2; i++)
            {
                AllFrameData.Add(list[i]);
                long tsTick =(list[i + 1].TimeStamp - list[i].TimeStamp).Ticks/(totalFramesPerKey+1);
                for(int j=0; j< totalFramesPerKey; j++)
                {
                    OneFrameData one = new OneFrameData();
                    one.TimeStamp = list[i].TimeStamp.Add(new TimeSpan(tsTick * (j + 1)));

                    foreach(var pair in list[i].LengthByKey)
                    {
                        double current = list[i].LengthByKey[pair.Key];
                        double next = list[i + 1].LengthByKey[pair.Key];
                        one.LengthByKey[pair.Key] = current + (next - current) * (j + 1) / (totalFramesPerKey + 1);

                        if (j < lockFramesPerKey)
                            one.HeightOrder[pair.Key] = list[i].HeightOrder[pair.Key];
                        else
                        {
                            double c = list[i].HeightOrder[pair.Key];
                            double n = list[i+1].HeightOrder[pair.Key];
                            one.HeightOrder[pair.Key] = c + (n - c) * (j - lockFramesPerKey + 1) / (totalFramesPerKey - lockFramesPerKey + 1);
                        }                      
                    }
                    one.TopLength = one.LengthByKey.OrderByDescending(x => x.Value).First().Value;

                    AllFrameData.Add(one);
                }
            }
            AllFrameData.Add(list[list.Count - 1]);
        }

        private static List<OneFrameData> ExtractKeyFrameData(List<string[]> rawData, int totalKeyFrame)
        {
            List<OneFrameData> list = new List<OneFrameData>();
            int framesPerKey = (rawData.Count - 2) / totalKeyFrame;
            for (int i = 2; i < rawData.Count; i++)
            {
                if((i-2)%framesPerKey==0)  //ignore 1st 2 rows
                {
                    OneFrameData one = new OneFrameData();
                    list.Add(one);
                    one.TimeStamp = DateTime.Parse(rawData[i][0]);

                    for(int j=1; j< rawData[i].Length; j++)
                      one.LengthByKey[rawData[0][j]] = double.Parse(rawData[i][j]);

                    int heightOrder = 1;
                    foreach(var item in one.LengthByKey.OrderByDescending(x=>x.Value))
                    {
                        one.HeightOrder[item.Key] = heightOrder;
                        heightOrder++;
                    }

                    one.TopLength = one.LengthByKey.OrderByDescending(x => x.Value).First().Value;
                }
            }
            return list;
        }

        private static List<string[]> ReadRawData()
        {
            List<string[]> rawData = new List<string[]>();
            
            foreach(var line in File.ReadAllLines(Setting.CSVFile))
            {
                rawData.Add(line.ToUpper().Split(','));
            }

            for (int i = 1; i < rawData[0].Length; i++)
                AllDisplayName[rawData[0][i]] = rawData[1][i];
           
            return rawData;
        }

    }

    public class OneFrameData
    {
        public DateTime TimeStamp;
        public Dictionary<string, double> LengthByKey = new Dictionary<string, double>();
        public Dictionary<string, double> HeightOrder = new Dictionary<string, double>();
        public double TopLength;
    }

    public class OneMomentData
    {
        public Dictionary<string, double> LengthData;
        public Dictionary<string, double> HeightOrder;
    }

    public static class Mapper
    {
        public static Dictionary<string, Brush> Brush = new Dictionary<string, Brush>();

        static Mapper()
        {
            Brush["Coral"] = Brushes.Coral;
            Brush["DeepPink"] = Brushes.DeepPink;
            Brush["Crimson"] = Brushes.Crimson;

            Brush["Aqua"] = Brushes.Aqua;
            Brush["Azure"] = Brushes.Azure;
            Brush["Red"] = Brushes.Red;          
            Brush["Black"] = Brushes.Black;
            Brush["Blue"] = Brushes.Blue;
            
            
            Brush["Cyan"] = Brushes.Cyan;
            Brush["DarkBlue"] = Brushes.DarkBlue;
            Brush["Firebrick"]=Brushes.Firebrick;
            Brush["ForestGreen"]=Brushes.ForestGreen;
            Brush["Fuchsia"]=Brushes.Fuchsia;
            Brush["GreenYellow"]=Brushes.GreenYellow;
            Brush["Green"] = Brushes.Green;
            Brush["Gold"] = Brushes.Gold;
            Brush["Red"] = Brushes.Red;
            Brush["Pink"] = Brushes.Pink;
            Brush["Yellow"] = Brushes.Yellow;
            Brush["White"] = Brushes.White;
            Brush["MistyRose"]= Brushes.MistyRose;
            Brush["Violet"] = Brushes.Violet;
            Brush["Navy"] = Brushes.Navy;

        }
    }

}