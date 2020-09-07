using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImageMagick;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Koppen
{
    class Program
    {
        const int COLOR_CHANNEL = 0;
        static string _outDir;
        static string _inDir;

        static Dictionary<string, byte[]> Zones = new Dictionary<string, byte[]> {
            //Bytes are RGBA values for the koppen color scheme

            { "Af", new byte[] { 1, 33, 244 , 255 } },
            { "Am", new byte[] { 1, 108, 244 , 255 } },
            { "As", new byte[] { 36, 145, 221 , 255 } },
            { "Aw", new byte[] { 92, 169, 222 , 255 } },

            { "BWh", new byte[] { 255, 1, 26 , 255 } },
            { "BWk", new byte[] { 255, 138, 139 , 255 } },
            { "BSh", new byte[] { 248, 150, 42 , 255 } },
            { "BSk", new byte[] { 228, 189, 91 , 255 } },

            { "Cfa", new byte[] { 189, 252, 87 , 255 } },
            { "Cfb", new byte[] { 81, 251, 70 , 255 } },
            { "Cfc", new byte[] { 38, 189, 41 , 255 } },
            { "Cwa", new byte[] { 133, 252, 148 , 255 } },
            { "Cwb", new byte[] { 80, 190, 97 , 255 } },
            { "Cwc", new byte[] { 43, 138, 53 , 255 } },
            { "Csa", new byte[] { 253, 251, 62 , 255 } },
            { "Csb", new byte[] { 201, 195, 49 , 255 } },
            { "Csc", new byte[] { 133, 131, 34 , 255 } },

            { "Dfa", new byte[] { 1, 252, 251 , 255 } },
            { "Dfb", new byte[] { 38, 192, 245 , 255 } },
            { "Dfc", new byte[] { 1, 115, 114 , 255 } },
            { "Dfd", new byte[] { 1, 62, 84 , 255 } },
            { "Dwa", new byte[] { 153, 168, 250 , 255 } },
            { "Dwb", new byte[] { 59, 112, 217 , 255 } },
            { "Dwc", new byte[] { 61, 72, 166 , 255 } },
            { "Dwd", new byte[] { 41, 16, 123 , 255 } },
            { "Dsa", new byte[] { 255, 30, 244 , 255 } },
            { "Dsb", new byte[] { 196, 22, 184 , 255 } },
            { "Dsc", new byte[] { 143, 48, 136 , 255 } },
            { "Dsd", new byte[] { 132, 84, 133 , 255 } },

            { "ET", new byte[] { 148, 148, 148 , 255 } },
            { "EF", new byte[] { 84, 84, 84 , 255 } },
        };

        static void Main(string[] args) {

            //TODO: Make args
            // var summerTemperatureOffset = 0;
            // var winterTemperatureOffset = 0;
            const short hottestTemperatureOffset = 2;
            const short coldestTemperatureOffset = -2;
            //

            Directory.SetCurrentDirectory(args[0]);

            _inDir = Path.Join(Directory.GetCurrentDirectory(), "input");
            _outDir = Path.Join(Directory.GetCurrentDirectory(), "output");

            Console.WriteLine($"Koppen Classifier reading from: {_inDir}");

            var imageInfo = new MagickImageInfo(Path.Join(_inDir, "alpha.png"));
            var imageWidth = imageInfo.Width;
            var imageHeight = imageInfo.Height;
            var halfHeight = imageHeight / 2;
            var sampleCount = imageWidth * imageHeight;

            Console.WriteLine($"Map Resolution: {imageWidth}x{imageHeight} ({sampleCount} Samples)");

            var climateMaps = InitializeClimateMaps(
                imageWidth, imageHeight);

            Console.Write("Loading Maps... ");

            var landMask = ReadImage("alpha.png");

            var summerRain = ReadImage("summerRain.png");
            var winterRain = ReadImage("winterRain.png");
            var summerTemperature = ReadImage("summerTemperature.png");
            var winterTemperature = ReadImage("winterTemperature.png");
            var hottestTemperature = ReadImage("hottestTemperature.png");
            var coldestTemperature = ReadImage("coldestTemperature.png");

            //TODO: Check that all image dimensions match

            Console.WriteLine("Done!");
            Console.WriteLine("Classification Progress...");

            void ClassifySample(int x, int y) {
                if (landMask[x, y][COLOR_CHANNEL] == 0) return;

                var sampleHemisphere = y <= halfHeight
                    ? Hemisphere.North
                    : Hemisphere.South;

                //TODO: Check bounds given offsets
                //TODO: use summer and winter offsets
                var sample = new WeatherSample(
                    sampleHemisphere,
                    ToRainfall(ReadPixel(summerRain, x, y)),
                    ToRainfall(ReadPixel(winterRain, x, y)),
                    ToTemperature(ReadPixel(summerTemperature, x, y)),
                    ToTemperature(ReadPixel(winterTemperature, x, y)),
                    (short) (ToTemperature(ReadPixel(hottestTemperature, x, y)) +
                             hottestTemperatureOffset),
                    (short) (ToTemperature(ReadPixel(coldestTemperature, x, y)) +
                             coldestTemperatureOffset)
                );

                var zone = KoppenClassifier.Classify(sample);

                climateMaps[zone][x, y] = true;
            }

            Console.WriteLine("0%");
            var completed = 0;
            var cursorPos = Console.CursorTop - 1;

            void UpdateProgress(object _) {
                Console.SetCursorPosition(0, cursorPos);
                Console.WriteLine($"{Math.Round((completed / (float) sampleCount) * 100, 1)}%      ");
            }

            var updateProgressPeriod = TimeSpan.FromSeconds(3);

            var watch = Stopwatch.StartNew();

            using (new Timer(UpdateProgress, null, updateProgressPeriod, updateProgressPeriod)) {
                // for (var x = 0; x < imageWidth; x++) {
                //     for (var y = 0; y < imageHeight; y++) {
                Parallel.For(0, imageWidth, x => {
                    Parallel.For(0, imageHeight, y => {
                        ClassifySample(x, y);
                        Interlocked.Increment(ref completed); //threadsafe completed++
                    });
                });
            }

            watch.Stop();

            Console.SetCursorPosition(0, cursorPos);
            Console.WriteLine("100.0%    ");
            Console.WriteLine($"Classification Completed in {watch.ElapsedMilliseconds/1000f}s");

            Console.WriteLine("Writing Climate Maps...");

            SaveClimateZoneMaps(climateMaps);

            Console.WriteLine("Climate Maps Complete!");
        }

        static IPixelCollection<byte> ReadImage(string fileName)
        {
            return new MagickImage(Path.Join(_inDir, fileName))
                .GetPixels();
        }

        static byte ReadPixel(IPixelCollection<byte> pixels, int x, int y) {
            return pixels[x, y][COLOR_CHANNEL];
        }

        static void SaveClimateZoneMaps(Dictionary<string, bool[,]> climateMaps) {
            if (!Directory.Exists(_outDir))
                Directory.CreateDirectory(_outDir);

            // foreach (var zone in climateMaps.Keys) {
            Parallel.ForEach(climateMaps.Keys, zone => {
                SaveClimateZoneMap(climateMaps[zone], zone);
                Console.WriteLine($"{zone} ✓");
            });
        }

        static void SaveClimateZoneMap(bool[,] climateMap, string zoneName) {
            var w = climateMap.GetLength(0);
            var h = climateMap.GetLength(1);

            using (var img = new MagickImage(MagickColors.Transparent, w,h)) {
                using (var pixels = img.GetPixels())
                {
                    for (var x = 0; x < w; x++) {
                        for (var y = 0; y < h; y++) {
                            if (climateMap[x, y]) {
                                pixels.SetPixel(x,y, Zones[zoneName]);
                            }
                        }
                    }
                }
                img.Write(Path.Join(_outDir, $"{zoneName}.png"));
            }
        }

        //pixel value from 0 - 255 => temperature value in C from -45 to 55
        const float TempScale = 2.55f;
        const short TempOffset = -45;
        static short ToTemperature(byte pixelValue) {
            return (short)(pixelValue / TempScale + TempOffset);
        }

        //pixel value from 0 - 255 => rainfall value in mm from 0 to 200
        const float RainScale = 0.784f; // 200/255 == 0.7843137255
        static byte ToRainfall(byte pixelValue) {
            return (byte)(pixelValue * RainScale);
        }

        static Dictionary<string, bool[,]> InitializeClimateMaps(int width, int height) {
            var maps = new Dictionary<string, bool[,]>();

            foreach (var zone in Zones.Keys) {
                maps[zone] = new bool[width, height];
            }

            return maps;
        }
    }
}