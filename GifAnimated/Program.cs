using System.Drawing;
using System.Drawing.Imaging;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Digite o nome do arquivo GIF (sem extensão) que deseja executar: ");
        string gifFileName = Console.ReadLine();

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string gifFolderPath = Path.Combine(desktopPath, "Gifs");
        string gifFilePath = Path.Combine(gifFolderPath, gifFileName + ".gif");

        if(!File.Exists(gifFilePath))
        {
            Console.WriteLine("Esse GIF não existe!!");
            return;
        }

        try
        {
            //Loads the GIF file into an Image object.
            using (var gif = Image.FromFile(gifFilePath))
            {
                FrameDimension dimension = new FrameDimension(gif.FrameDimensionsList[0]); //Retrieves the dimensions of the frames in the GIF.
                int frameCount = gif.GetFrameCount(dimension);

                for (int i = 0; i < frameCount; i++)
                {
                    gif.SelectActiveFrame(dimension, i); //Selects the current frame to be processed

                    //A Bitmap is a representation of a digital image. It stores the image data as a grid of pixels, where each pixel has a specific color value (usually represented by red, green, and blue components) and transparency.
                    using (Bitmap bmp = new Bitmap(gif.Width, gif.Height, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.DrawImage(gif, Point.Empty);
                        }

                        string asciiArt = ConvertToAscii(bmp);
                        Console.Clear(); // Clears the console to prepare for displaying the next frame.
                        Console.WriteLine(asciiArt);

                        Thread.Sleep(100);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string ConvertToAscii(Bitmap image)
    {
        // Resize the image to reduce ASCII output size
        int width = 80; 
        int height = (int)(image.Height * (width / (double)image.Width * 0.55));

        Bitmap resized = new Bitmap(image, new Size(width, height));

        string asciiArt = ""; //empty string to hold the ASCII art

        for (int y = 0; y < resized.Height; y++)
        {
            for (int x = 0; x < resized.Width; x++)
            {
                Color pixelColor = resized.GetPixel(x, y);
                char asciiChar = AsciiGradient(pixelColor.GetBrightness());
                string coloredChar = GetColoredString(asciiChar, pixelColor); //Converts the ASCII character to a colored string using ANSI escape codes.
                asciiArt += coloredChar;
            }
            asciiArt += "\n";
        }

        return asciiArt;
    }

    static char AsciiGradient(float brightness)
    {
        // ASCII characters gradient based on brightness
        char[] gradient = { ' ', '.', ',', ':', ';', 'o', 'x', '%', '#', '@' };

        int index = (int)(brightness * (gradient.Length - 1));

        return gradient[index];
    }

    //  Generates a colored string using ANSI escape codes (tell the terminal or console how to format the following text).So, when you see \x1B[31m, the terminal knows to make the next part of the text red. 
    static string GetColoredString(char c, Color color)
    {
        return $"\x1b[38;2;{color.R};{color.G};{color.B}m{c}\x1b[0m";
    }
}
