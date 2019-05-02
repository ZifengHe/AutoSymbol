﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace FrameByFrame
{

    public class ProjData
    {
        public static string TempFile;

        [XmlIgnore]
        public string CSVContent
        {
            get
            {
                return Helper.Base64Decode(CSVContent64);
            }
            set
            {
                CSVContent64 = Helper.Base64Encode(value);
            }
        }

        public string CSVContent64;

        [XmlIgnore]
        public string [] Header;

        public List<OneRow> Rows = new List<OneRow>();
        
        public List<RichTextConfig> RichTexts = new List<RichTextConfig>();
        

        public void ProcessCSVFile()
        {
            int currentRowNum = 0;
            File.WriteAllText(TempFile, CSVContent);
            using (TextFieldParser parser = new TextFieldParser(TempFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                bool firstLine = true;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (firstLine)
                    {
                        Header = fields;
                        firstLine = false;
                        continue;
                    }

                    if (fields.Length == Header.Length && fields[0].Length > 3 && fields[1].Length > 5)
                    {
                        OneRow one = new OneRow();
                        one.SeriesName = fields[0];
                        one.SeriesCode = fields[1];
                        one.CountryName = fields[2];
                        one.CountryCode = fields[3];
                        one.RowNumber = currentRowNum;
                        currentRowNum++;

                        for (int i = 4; i < Header.Length; i++)
                        {
                            one.Data[Header[i]] = fields[i];
                        }

                        this.Rows.Add(one);
                    }
                }
            }
        }

        public OneRow GetRow(string id)
        {
            foreach (var one in Rows)
                if (one.Id() == id)
                    return one;
            return null;
        }

        public void DeleteRTC(string title)
        {
            RichTexts.Remove(GetRTC(title));

        }

        public RichTextConfig GetRTC(string title)
        {
            foreach (var one in RichTexts)
                if (one.Title == title)
                    return one;
            return null;
        }
    }

    public class RichTextConfig
    {
        public string Title;

                
        public string xamlStr
        {
            get
            {
                return Helper.Base64Decode(xamlStr64);
            }
            set
            {
                xamlStr64 = Helper.Base64Encode(value);
            }
        }
        public string xamlStr64;

        public string GetKey()
        {
            return Title;
        }            
    }
    public class CountryCodeMapping
    {
        public string Name;
        public string ShortCode;
        public string LongCode;
    }
    public enum RendorMode
    {
        Invalid,
        Single,
        Horizontal,
        Vertical
    }
    public class Country
    {
        public static Dictionary<string, string> CodeMapping = new Dictionary<string, string>();

        static Country()
        {
        }
    }

 
    public class OneRow
    {
        public OneRow()
        {
            TextColor = Color.FromRgb(0, 0, 0);
            LineColor = Color.FromRgb(0, 0, 0);
        }

        [XmlIgnore]
        public string SeriesName
        {
            get
            {
                return Helper.Base64Decode(SeriesName64);
            }
            set
            {
                SeriesName64 = Helper.Base64Encode(value);
            }
        }

        public string SeriesName64;
        public string SeriesCode;

        public string CountryName;
        public string CountryCode;

        public Color TextColor;
        public Color LineColor;

        public int LineThickness = 10;
        public int RowNumber = -1;
        

        [XmlIgnore]
        public Dictionary<string, string> Data = new Dictionary<string, string>();

       

        public string Id()
        {
            return CountryName + " " + SeriesName;
        }
    }

    public class Pair
    {
        public string First;
        public string Second;
    }
}