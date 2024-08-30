// https://github.com/grensen/language_fingerprint
// https://x.com/matthen2/status/1775531115874246837

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Input;
using System.Net;

public class FingerPrint : Window
{
    // layout
    Canvas canGlobal = new();
    // support 
    readonly CultureInfo ci = CultureInfo.GetCultureInfo("en-us");
    readonly Typeface tf = new("TimesNewRoman"); // "Arial"
    int count = -1;

    [STAThread]
    public static void Main() { new Application().Run(new FingerPrint()); }
    // CONSTRUCTOR - LOADED - ONINIT
    private FingerPrint() // constructor
    {
        // set window   
        Title = "Language Fingerprint"; //        
        Content = canGlobal;
        Background = Brushes.Black;
        Width = 1580;
        Height = 780; // HeightG;

        MouseDown += Mouse_Down;
        MouseWheel += Mouse_Wheel;

        count++;
        Run(!false, count);
    } // constructor end

    void Mouse_Wheel(object sender, MouseWheelEventArgs e)
    {
        // int delta = e.Delta; bool wheel = delta > 0;
        count++;
        Run(!false, count);
    }

    void Mouse_Down(object sender, MouseButtonEventArgs e)
    {
        canGlobal.Children.Clear();
        DrawingVisualElement drawingVisual = new();
        canGlobal.Children.Add(drawingVisual);
        DrawingContext dc = drawingVisual.drawingVisual.RenderOpen();
        dc.Close();

        Run(e.ChangedButton == MouseButton.Left);
    }

    void Run(bool animation, int wheel = 0)
    {

        string url = "https://jamesmccaffrey.wordpress.com/";
        string articleTitle = "Artificial_intelligence";

        if (wheel % 3 == 0)
            url = $"https://en.wikipedia.org/wiki/Artificial_intelligence";
        if (wheel % 3 == 1)
            url = "https://www.computerbase.de/";
        if (wheel % 3 == 2)
            url = "https://jamesmccaffrey.wordpress.com/";

        int numberOfCharacters = 100000; // Number of characters to retrieve
        
        using var client = new WebClient();           
        // Download the HTML content
        string html = client.DownloadString(url);

        // Get the first n characters of the HTML content
        //   string firstNCharacters = GetFirstNCharacters(html, numberOfCharacters);
        string firstNCharacters = html.Substring(0, Math.Min(numberOfCharacters, html.Length));
        string extract = firstNCharacters;
        Console.WriteLine($"First {numberOfCharacters} characters of the webpage:\n{firstNCharacters}");
            
        // Extract letters from the text, keeping only lowercase letters and spaces
        List<char> allLetters = new List<char>();
        foreach (char c in extract) // Check if the character is a letter (lowercase) or a space
            if (c == ' ' || char.IsLetter(c)) // Convert to lowercase and add to the list
                allLetters.Add(char.ToLower(c));

        if (animation)
        {
            var cleanedLetters = allLetters.ToArray();
            Fingerprint(cleanedLetters, url);
        }
        else
        {
            var cleanedLetters = allLetters.ToArray();
            for (int i = 0; i < cleanedLetters.Length - 10; i++)
            {
                Fingerprint(cleanedLetters.AsSpan().Slice(0, i + 10), url);
            }
        }
    }

    void Fingerprint(Span<char> cleanedLetters, string url)
    {

        // menu sizes
        const int outputTile = 25;

        int yStart = 50;
        int confusionX = 130;
        int lineWidth = 700;

        canGlobal.Children.Clear();
        DrawingVisualElement drawingVisual = new();
        canGlobal.Children.Add(drawingVisual);
        DrawingContext dc = drawingVisual.drawingVisual.RenderOpen();

        dc.DrawText(new(url, ci, FlowDirection.LeftToRight, tf, 14, Brushes.CornflowerBlue, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(20, 22 - 15));

        dc.DrawText(new("cleanedLetters " + cleanedLetters.Length.ToString(), ci, FlowDirection.LeftToRight, tf, 10, Brushes.CornflowerBlue, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(20, 25));
        char[] alphabetArray = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        // Initialize an array to store letter counts, assuming only lowercase letters 'a' to 'z' are present
        int[] weights = new int[alphabetArray.Length * alphabetArray.Length]; // abc... = 26 + start = " " and end = " " = 28 x 28
        int[] counts = new int[alphabetArray.Length];

        // letter to letter
        int letterToLetter = 1;
        for (int i = 0; i < cleanedLetters.Length - letterToLetter; i++)
        {
            char c = cleanedLetters[i];
            char cNext = cleanedLetters[i + letterToLetter];

            bool isNoWord = false;
            if (c != ' ')
                for (int j = 1; j < letterToLetter; j++)
                {
                    char cc = cleanedLetters[i + j];
                    if (cc == ' ')
                    {
                        isNoWord = true;
                        break;
                    }
                }
            if (isNoWord) continue;

            if (cNext != ' ' && c != ' ' && c >= 'a' && c <= 'z' && cNext >= 'a' && cNext <= 'z') // abc... letters
            {
                int idInputLetter = c - 'a';
                int idLabel = cNext - 'a';
                weights[idInputLetter + alphabetArray.Length * idLabel]++;
                counts[idInputLetter]++;
            }
        }
        int maxCount = 0;
        for (int i = 0; i < weights.Length; i++)
            if (weights[i] > maxCount) maxCount = weights[i];

        for (int i = 0; i < alphabetArray.Length; i++)
            dc.DrawText(new($"{(char)('a' + i)} = {counts[i]}", ci, FlowDirection.LeftToRight, tf, 10, Brushes.CornflowerBlue, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(20, 35 + i * 10));

        // draw target and prediction info tiles            
        for (int k = 0; k < alphabetArray.Length; k++)
        {
            double ystep = yStart + k * outputTile, xstep = confusionX - outputTile;
            dc.DrawText(new(alphabetArray[k].ToString(), ci, FlowDirection.LeftToRight, tf, 12, Brushes.Gold, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(xstep + 9, ystep + 2));
            double ystepRow = yStart - outputTile, xstepRow = confusionX + k * outputTile;
            dc.DrawText(new(alphabetArray[k].ToString(), ci, FlowDirection.LeftToRight, tf, 12, Brushes.Gold, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(xstepRow + 9, ystepRow + 7));
        }


        for (int i = 0; i < alphabetArray.Length; i++) // label
        {
            for (int k = 0; k < alphabetArray.Length; k++) // input
            {
                double prob = weights[i + k * alphabetArray.Length] / (double)maxCount;
                double ystep = yStart + i * outputTile, xstep = k * outputTile + confusionX;
                if (prob == 0)
                {
                    dc.DrawRectangle(BrF(((int)(prob * 15) + 50), ((int)(prob * 15) + 50), ((int)(prob * 175) + 50))
                    , null, new Rect(xstep, ystep, outputTile - 1, outputTile - 1));
                    continue;
                }

                dc.DrawRectangle(i == k ? BrF(((int)((prob) * 201) + 21), ((int)((prob) * 163) + 16), 0)
                    : BrF(((int)(prob * 15) + 20), ((int)(prob * 15) + 20), ((int)(prob * 185) + 70))
                    , null, new Rect(xstep, ystep, outputTile - 1, outputTile - 1));

                Text(ref dc, weights[i + k * alphabetArray.Length].ToString("F0"), 10, Brushes.Black, (int)(xstep + 1), (int)(ystep + 0.0));
            }
        }

        yStart += outputTile / 2;
        int xStart = confusionX + outputTile * alphabetArray.Length + 35;

        // draw target and prediction info tiles            
        for (int k = 0; k < alphabetArray.Length; k++)
        {
            {
                double ystep = yStart + k * outputTile - outputTile / 2, xstep = xStart - outputTile;
                dc.DrawText(new(alphabetArray[k].ToString(), ci, FlowDirection.LeftToRight, tf, 12, Brushes.Gold, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(xstep + 13, ystep + 4));
            }
            {
                double ystep = yStart + k * outputTile - outputTile / 2, xstep = lineWidth + xStart;
                dc.DrawText(new(alphabetArray[k].ToString(), ci, FlowDirection.LeftToRight, tf, 12, Brushes.Gold, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(xstep + 5, ystep + 4));
            }
        }

        for (int col = 0; col < alphabetArray.Length; col++) // input letter
        {
            for (int row = 0; row < alphabetArray.Length; row++) // label latter 
            {
              //  if (col == row) continue;

                double prob = weights[col + row * alphabetArray.Length] / (double)maxCount;
                double propt = (prob) * 5;
                StreamGeometry linesGeometry = new();
                using (StreamGeometryContext linesContext = linesGeometry.Open())
                {
                    int start = (int)(yStart + col * outputTile + propt);
                    linesContext.BeginFigure(new System.Windows.Point(xStart, start), false, false);

                    int end = (int)(yStart + row * outputTile + propt);
                    linesContext.BezierTo(new(xStart + lineWidth / 2, start), // Control point 1
                                            new(xStart + lineWidth / 2, end), // Control point 2
                                            new(xStart + lineWidth, end), true, true); // End point
                }
                linesGeometry.Freeze();


                var clr = col == row ? BrF(((int)(prob * 201) + 21), ((int)(prob * 163) + 16), 0) : BrF(((int)(prob * 15) + 20), ((int)(prob * 15) + 20), ((int)(prob * 185) + 70));
                Pen pen = new Pen(clr, 8 * prob + 0.0);
                dc.DrawGeometry(null, pen, linesGeometry);
            }
        }

        void Text(ref DrawingContext dc, string str, int size, Brush clr, double x, double y) =>
            dc.DrawText(new FormattedText(str, ci, FlowDirection.LeftToRight, tf, size, clr, VisualTreeHelper.GetDpi(this).PixelsPerDip), new System.Windows.Point(x, y));

        dc.Close();
        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));

    }

    static Brush BrF(float red, float green, float blue)
    {
        Brush frozenBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)red, (byte)green, (byte)blue));
        frozenBrush.Freeze();
        return frozenBrush;
    }
} // TheWindow end

public class DrawingVisualElement : FrameworkElement
{
    private readonly VisualCollection _children;
    public DrawingVisual drawingVisual;
    public DrawingVisualElement()
    {
        _children = new VisualCollection(this);
        drawingVisual = new DrawingVisual();
        _children.Add(drawingVisual);
    }
    void ClearVisualElement()
    {
        (this)._children.Clear();
    }
    protected override int VisualChildrenCount
    {
        get { return _children.Count; }
    }
    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= _children.Count)
            throw new();
        return _children[index];
    }
} // DrawingVisualElement



