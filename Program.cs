using System.Drawing.Imaging;

namespace cloudbells.TerrainGeneration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HeightmapGenerator gen = new HeightmapGenerator();
            // Measure how much of the height map is mountainous vs flat.
            double average = 0;
            double lastAverage = 0;
            // Uncomment to evaluate.
            //for (int i = 0; i < 1001; i++)
            //{
            //    if (i > 0 && i % 100 == 0)
            //    {
            //        Console.WriteLine(i + " done, average score is currently " + average / i);
            //        if (Math.Abs(average / i / lastAverage - 1) <= 0.02) // If we haven't changed by more than 2%, we most likely have converged.
            //        {
            //            lastAverage = average / i;
            //            break;
            //        }
            //        lastAverage = average / i;
            //    }
            //    Bitmap map = gen.GenerateHeightmap();
            //    int count = 0;
            //    for (int col = 0; col < map.Width; col++)
            //    {
            //        for (int row = 0; row < map.Height; row++)
            //        {
            //            if (map.GetPixel(col, row).R >= 127) // If a pixel is part of a mountain or the start of a mountain (with exceptions).
            //            {
            //                count++;
            //            }
            //        }
            //    }
            //    average += (double)count / (256 * 256); // The lower the count, the flatter the heightmap and vice versa.
            //}
            //Console.WriteLine("Average score: " + lastAverage);

            // Save one map (probably in debug folder).
            gen.GenerateHeightmap().Save("heightmap.png", ImageFormat.Png);
        }
    }
}