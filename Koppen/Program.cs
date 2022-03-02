using System;
using System.Collections.Generic;
using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Koppen
{
    class Program
    {
        static string _outDir;
        static string _inDir;

        static readonly Dictionary<string, Rgba32> ZoneColors = new Dictionary<string, Rgba32> {
            //Bytes are RGBA values for the koppen color scheme

            { "Af", new Rgba32(1, 33, 244) },
            { "Am", new Rgba32(1, 108, 244) },
            { "As", new Rgba32(36, 145, 221) },
            { "Aw", new Rgba32(92, 169, 222) },

            { "BWh", new Rgba32(255, 1, 26) },
            { "BWk", new Rgba32(255, 138, 139) },
            { "BSh", new Rgba32(248, 150, 42) },
            { "BSk", new Rgba32(228, 189, 91) },

            { "Cfa", new Rgba32(189, 252, 87) },
            { "Cfb", new Rgba32(81, 251, 70) },
            { "Cfc", new Rgba32(38, 189, 41) },
            { "Cwa", new Rgba32(133, 252, 148) },
            { "Cwb", new Rgba32(80, 190, 97) },
            { "Cwc", new Rgba32(43, 138, 53) },
            { "Csa", new Rgba32(253, 251, 62) },
            { "Csb", new Rgba32(201, 195, 49) },
            { "Csc", new Rgba32(133, 131, 34) },

            { "Dfa", new Rgba32(1, 252, 251) },
            { "Dfb", new Rgba32(38, 192, 245) },
            { "Dfc", new Rgba32(1, 115, 114) },
            { "Dfd", new Rgba32(1, 62, 84) },
            { "Dwa", new Rgba32(153, 168, 250) },
            { "Dwb", new Rgba32(59, 112, 217) },
            { "Dwc", new Rgba32(61, 72, 166) },
            { "Dwd", new Rgba32(41, 16, 123) },
            { "Dsa", new Rgba32(255, 30, 244) },
            { "Dsb", new Rgba32(196, 22, 184) },
            { "Dsc", new Rgba32(143, 48, 136) },
            { "Dsd", new Rgba32(132, 84, 133) },

            { "ET", new Rgba32(148, 148, 148) },
            { "EF", new Rgba32(84, 84, 84) },
        };

        static void Main(string[] args) {

            //TODO: Make args
            // var summerTemperatureOffset = 0;
            // var winterTemperatureOffset = 0;
            const short hottestTemperatureOffset = 0;
            const short coldestTemperatureOffset = 0;
            //

            Directory.SetCurrentDirectory(args[0]);

            _inDir = Path.Join(Directory.GetCurrentDirectory(), "input");
            _outDir = Path.Join(Directory.GetCurrentDirectory(), "output");

            Console.WriteLine($"Koppen Classifier reading from: {_inDir}");

            var imageInfo = Image.Identify(Path.Join(_inDir, "alpha.png"));
            var imageWidth = imageInfo.Width;
            var imageHeight = imageInfo.Height;
            var halfHeight = imageHeight / 2;
            var sampleCount = imageWidth * imageHeight;

            Console.WriteLine($"Map Resolution: {imageWidth}x{imageHeight} ({sampleCount} Samples)");

            var climateMaps = InitializeClimateMaps();

            Console.Write("Loading Maps... ");

            var watch = Stopwatch.StartNew();

            using (var landMask = ReadImage("alpha.png"))
            using (var summerRain = ReadImage("summerRain.png"))
            using (var winterRain = ReadImage("winterRain.png"))
            using (var summerTemperature = ReadImage("summerTemperature.png"))
            using (var winterTemperature = ReadImage("winterTemperature.png"))
            using (var hottestTemperature = MaybeReadImage("hottestTemperature.png", summerTemperature))
            using (var coldestTemperature = MaybeReadImage("coldestTemperature.png", winterTemperature))
            {
                //TODO: Check that all image dimensions match

                Console.WriteLine("Done!");
                Console.WriteLine("Classification Progress...");

                byte transparent = (byte)0;

                void ClassifySample(int x, int y)
                {
                    if (landMask[x, y].PackedValue == transparent) return;

                    //TODO: Check bounds given offsets
                    //TODO: use summer and winter offsets
                    var sample = new WeatherSample(
                        ToRainfall(ReadPixel(summerRain, x, y)),
                        ToRainfall(ReadPixel(winterRain, x, y)),
                        ToTemperature(ReadPixel(summerTemperature, x, y)),
                        ToTemperature(ReadPixel(winterTemperature, x, y)),
                        (short)(ToTemperature(ReadPixel(hottestTemperature, x, y)) +
                                 hottestTemperatureOffset),
                        (short)(ToTemperature(ReadPixel(coldestTemperature, x, y)) +
                                 coldestTemperatureOffset)
                    );

                    var zone = KoppenClassifier.Classify(sample);

                    climateMaps[zone].Enqueue(new Vec2Int(x, y));
                }

                Console.WriteLine("0%");
                var completed = 0;
                var cursorPos = Console.CursorTop - 1;

                void UpdateProgress(object _)
                {
                    Console.SetCursorPosition(0, cursorPos);
                    Console.WriteLine($"{Math.Round((completed / (float)sampleCount) * 100, 1)}%      ");
                }

                var updateProgressPeriod = TimeSpan.FromSeconds(3);

                using (new Timer(UpdateProgress, null, updateProgressPeriod, updateProgressPeriod))
                {
                    //for (var x = 0; x < imageWidth; x++) {
                    //   for (var y = 0; y < imageHeight; y++) {
                    Parallel.For(0, imageWidth, x => { 
                        Parallel.For(0, imageHeight, y => { 
                            ClassifySample(x, y);
                            Interlocked.Increment(ref completed); //threadsafe completed++
                        });
                    });
                }

                Console.SetCursorPosition(0, cursorPos);
                Console.WriteLine("100.0%    ");
                Console.WriteLine($"Classification Completed!");
            }

            Console.WriteLine("Writing Climate Maps...");

            SaveClimateZoneMaps(climateMaps, imageWidth, imageHeight);

            watch.Stop();

            Console.WriteLine($"Climate Maps Complete! ({watch.ElapsedMilliseconds / 1000f}s)");
        }

        static Image<L8> MaybeReadImage(string fileName, Image<L8> fallback)
        {
            if (File.Exists(Path.Join(_inDir, fileName))) {
                return ReadImage(fileName);
            }

            return fallback;
        }

        static Image<L8> ReadImage(string fileName)
        {
            return Image.Load<L8>(Path.Join(_inDir, fileName));
        }

        static byte ReadPixel(Image<L8> pixels, int x, int y)
        {
            return pixels[x, y].PackedValue;
        }

        static void SaveClimateZoneMaps(Dictionary<string, ConcurrentQueue<Vec2Int>> climateMaps, int imageWidth, int imageHeight) {
            if (!Directory.Exists(_outDir))
                Directory.CreateDirectory(_outDir);

            foreach (var zone in climateMaps.Keys) 
            {
                Console.Write($"{zone}...");
                if(SaveClimateZoneMap(climateMaps[zone], zone, imageWidth, imageHeight))
                {
                    Console.WriteLine("✓");
                }
                else
                {
                    Console.WriteLine("(None Found)");
                }
            }

        }

        static bool SaveClimateZoneMap(ConcurrentQueue<Vec2Int> climateMap, string zoneName, int w, int h) {

            if (climateMap.IsEmpty) return false;

            using var img = new Image<Rgba32>(w, h); 

            foreach (var zonePixel in climateMap)
            {
                img[zonePixel.x, zonePixel.y] = ZoneColors[zoneName];
            }

            img.Save(Path.Join(_outDir, $"{zoneName}.png"));

            return true;
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

        static Dictionary<string, ConcurrentQueue<Vec2Int>> InitializeClimateMaps() {
            var maps = new Dictionary<string, ConcurrentQueue<Vec2Int>>();

            foreach (var zone in ZoneColors.Keys)
            {
                maps[zone] = new ConcurrentQueue<Vec2Int>();
            }

            return maps;
        }
    }
}