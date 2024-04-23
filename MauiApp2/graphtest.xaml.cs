using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Console = System.Console;

namespace MauiApp2;

public partial class graphtest : ContentPage
{
    private graphtestVM graphiel;
    private FileStream inStream;
    private FileStream inAccStream;
    private StreamReader reader;
    private StreamReader accReader;
    private FileStream inGyroStream;
    private StreamReader gyroReader;
    private List<float> csvData;
    private List<float> xcsv;
    private List<float> ycsv;//could be condensed into 1 list if you want to be fancy about it
    private List<float> zcsv;//and optimize i guess
    private List<string> axis;

    public graphtest()
    {
        InitializeComponent();
        graphiel = new graphtestVM();
        csvData = new List<float>();
        xcsv = new List<float>();
        ycsv = new List<float>();
        zcsv = new List<float>();
        axis = new List<string>();
    }
    
    /*private void OnNewData(object sender, EventArgs e)
    {
        Console.WriteLine("Data Changed");
        Grapho.Series = graphiel.PopulateSeries(csvData); //graphiel.OnNewDataDX(sender, e);
    }*/

    private void readFile(object sender, EventArgs e)
    {
        inStream = File.OpenRead(System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "lightData.csv")/*"/Users/michaelhensel/Desktop/graphs/lightData20240415194928009.csv"*/);
        reader = new StreamReader(inStream);
        string line;
        csvData.Clear();//empties out the csvdata list once data has been uploaded to graph
        axis.Clear();

        Console.WriteLine("Line: " + reader.ReadLine());
        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            Console.WriteLine("Line: " + line);
            if (line != null)
            {
                axis.Add(line.Split(',')[0]);
                csvData.Add(float.Parse(line.Split(',')[1]));
            } else Console.WriteLine("uh");
            
        }
        reader.Close();
        Console.WriteLine("Data Changed");
        Grapho.Series = graphiel.PopulateSeries(csvData); //graphiel.OnNewDataDX(sender, e);
        Grapho.XAxes = graphiel.PopulateAxis(axis);
        Grapho.YAxes = graphiel.ChangeYLabel(2);
    }
    
    private void accFile(object sender, EventArgs e)
    {
        inAccStream = File.OpenRead(System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "accData.csv")/*"/Users/michaelhensel/Desktop/graphs/lightData20240415194928009.csv"*/);
        accReader = new StreamReader(inAccStream);
        string line;
        xcsv.Clear();//empties out the csvdata list once data has been uploaded to graph
        ycsv.Clear();
        zcsv.Clear();
        axis.Clear();
        Console.WriteLine("Line: " + accReader.ReadLine());
        while (!accReader.EndOfStream)
        {
            line = accReader.ReadLine();
            Console.WriteLine("Line: " + line);
            if (line != null)
            {
                axis.Add(line.Split(',')[0]);
                xcsv.Add(float.Parse(line.Split(',')[1]));
                ycsv.Add(float.Parse(line.Split(',')[2]));
                zcsv.Add(float.Parse(line.Split(',')[3]));
            } else Console.WriteLine("uh");
            
        }
        accReader.Close();
        Console.WriteLine("Data Changed");
        Grapho.Series = graphiel.PopulateAccSeries(xcsv, ycsv, zcsv); //graphiel.OnNewDataDX(sender, e);
        Grapho.XAxes = graphiel.PopulateAxis(axis);
        Grapho.YAxes = graphiel.ChangeYLabel(1);
    }
    
    private void gyroFile(object sender, EventArgs e)
    {
        inGyroStream = File.OpenRead(System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "gyroData.csv")/*"/Users/michaelhensel/Desktop/graphs/lightData20240415194928009.csv"*/);
        gyroReader = new StreamReader(inGyroStream);
        string line;
        xcsv.Clear();//empties out the csvdata list once data has been uploaded to graph
        ycsv.Clear();
        zcsv.Clear();
        axis.Clear();
        Console.WriteLine("Line: " + gyroReader.ReadLine());
        while (!gyroReader.EndOfStream)
        {
            line = gyroReader.ReadLine();
            Console.WriteLine("Line: " + line);
            if (line != null)
            {
                axis.Add(line.Split(',')[0]);
                xcsv.Add(float.Parse(line.Split(',')[1]));
                ycsv.Add(float.Parse(line.Split(',')[2]));
                zcsv.Add(float.Parse(line.Split(',')[3]));
            } else Console.WriteLine("uh");
            
        }
        gyroReader.Close();
        Console.WriteLine("Data Changed");
        Grapho.Series = graphiel.PopulateAccSeries(xcsv, ycsv, zcsv); //graphiel.OnNewDataDX(sender, e);
        Grapho.XAxes = graphiel.PopulateAxis(axis);
        Grapho.YAxes = graphiel.ChangeYLabel(3);
    }
    
    async void OnBack(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}

public partial class graphtestVM
{
    public ISeries[] Series { get; set; }
            = new ISeries[]
            {
                new LineSeries<float>
                {
                    Values = new float[] {},
                    Name = "X",
                    Fill = null
                }
            };

    public Axis[] XAxis { get; set; }
        = 
        {
            new Axis
            {
                Name = "Milliseconds",
                Labels = new string[] {},
                LabelsPaint = new SolidColorPaint(SKColors.DarkRed),
                NamePaint = new SolidColorPaint(SKColors.DarkRed)
            }
        };
    public Axis[] YAxis { get; set; }
        = 
        {
            new Axis
            {
                Name = "Unit",
                LabelsPaint = new SolidColorPaint(SKColors.DarkRed),
                NamePaint = new SolidColorPaint(SKColors.DarkRed)
            }
        };

    public Axis[] ChangeYLabel(int unit)
    {
        return unit switch
        {
            1 => YAxis = new Axis[] { new Axis() { Name = "G", LabelsPaint = new SolidColorPaint(SKColors.DarkRed), NamePaint = new SolidColorPaint(SKColors.DarkRed) } } ,
            2 => YAxis = new Axis[] { new Axis() { Name = "Lux", LabelsPaint = new SolidColorPaint(SKColors.DarkRed), NamePaint = new SolidColorPaint(SKColors.DarkRed) } },
            _ => YAxis = new Axis[] { new Axis() { Name = "dps", LabelsPaint = new SolidColorPaint(SKColors.DarkRed), NamePaint = new SolidColorPaint(SKColors.DarkRed) } }
        };
    }

    /*public ISeries[] OnNewDataDX(object sender, EventArgs e)
    {
        Console.WriteLine("Data Changed");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = new float[] { 5, 6, 7, 10, 9, 8, 4, 5, 5, -2 },
                Name = "X",
                Fill = null
            }
        };
    }*/

    public ISeries[] PopulateSeries(List<float> list)
    {
        Console.WriteLine("Data Loading");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = list,
                Name = "Lux",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Red),
                GeometrySize = 0,
                GeometryFill = new SolidColorPaint(SKColors.Red),
                GeometryStroke = new SolidColorPaint(SKColors.Red)
            }
        };
    }

    public Axis[] PopulateAxis(List<string> axis)
    {
       Console.WriteLine("Loading Data into Axis");
       return XAxis = new Axis[]
       {
           new Axis
           {
               Name = "Milliseconds",
               Labels = axis,
               LabelsPaint = new SolidColorPaint(SKColors.DarkRed),
               NamePaint = new SolidColorPaint(SKColors.DarkRed)
           }
       };
    }
    
    public ISeries[] PopulateAccSeries(List<float> xlist, List<float> ylist, List<float> zlist)
    {
        Console.WriteLine("Data Loading");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = xlist,
                Name = "X",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Red),
                GeometrySize = 0,
                GeometryFill = new SolidColorPaint(SKColors.Red),
                GeometryStroke = new SolidColorPaint(SKColors.Red)
            },
            new LineSeries<float>
            {
            Values = ylist,
            Name = "Y",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.MediumBlue),
            GeometrySize = 0,
            GeometryFill = new SolidColorPaint(SKColors.MediumBlue),
            GeometryStroke = new SolidColorPaint(SKColors.MediumBlue)
            },
            new LineSeries<float>
            {
            Values = zlist,
            Name = "Z",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Green),
            GeometrySize = 0,
            GeometryFill = new SolidColorPaint(SKColors.Green),
            GeometryStroke = new SolidColorPaint(SKColors.Green)
            }
            
        };
    }
}


