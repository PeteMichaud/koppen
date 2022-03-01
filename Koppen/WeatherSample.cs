using System;

namespace Koppen
{
    /// <summary>
    /// Holds all the data for a specific sample on the map
    /// </summary>
    public readonly struct WeatherSample
    {
        public readonly byte RainfallMax; //mm
        public readonly byte RainfallMin;
        public readonly byte RainfallAvg;
        public readonly byte RainfallSummer;
        public readonly byte RainfallWinter;
        public readonly byte RainfallHotSeason;
        public readonly byte RainfallColdSeason;

        public readonly int TotalRainfallSummer;
        public readonly int TotalRainfallWinter;
        public readonly int TotalRainfallHotSeason;
        public readonly int TotalRainfallColdSeason;

        public readonly int TotalAnnualRainfall;

        public readonly short TemperatureMax; //C
        public readonly short TemperatureMin;
        public readonly short TemperatureAvg;
        public readonly short[] Temperatures;

        // Nominal Summer is colder than nominal Winter
        public readonly bool InvertedSeason;

        // Annual Rainfall threshold below which the region is defined as Arid, per Koppen classification rules
        public readonly int DesertThreshold;

        public WeatherSample(
            byte summerRainfall, byte winterRainfall,
            short temperatureDuringSummer, short temperatureDuringWinter,
            short temperatureDuringGlobalHottest, short temperatureDuringGlobalColdest) {

            RainfallSummer = summerRainfall;
            RainfallWinter = winterRainfall;

            InvertedSeason = temperatureDuringWinter > temperatureDuringSummer;
            if (InvertedSeason) {
                RainfallHotSeason = winterRainfall;
                RainfallColdSeason = summerRainfall;
            } else {
                RainfallHotSeason = summerRainfall;
                RainfallColdSeason = winterRainfall;
            }

            var rainfall = new[] { 
                summerRainfall, 
                winterRainfall 
            };
            Array.Sort(rainfall);

            RainfallMin = rainfall[0];
            RainfallMax = rainfall[1];
            RainfallAvg = (byte)Math.Round(
                (summerRainfall + winterRainfall) / 2f);

            TotalRainfallSummer = RainfallSummer * 6;
            TotalRainfallWinter = RainfallWinter * 6;
            TotalRainfallHotSeason = RainfallHotSeason * 6;
            TotalRainfallColdSeason = RainfallColdSeason * 6;
            TotalAnnualRainfall = RainfallAvg * 12;

            //

            var temperatures = new[] {
                temperatureDuringSummer, 
                temperatureDuringWinter, 
                temperatureDuringGlobalHottest, 
                temperatureDuringGlobalColdest
            };
            Array.Sort(temperatures);

            Temperatures = temperatures;
            TemperatureMin = temperatures[0];
            TemperatureMax = temperatures[^1];
            TemperatureAvg = (short)Math.Round(
                (temperatureDuringSummer + 
                temperatureDuringWinter + 
                temperatureDuringGlobalHottest + 
                temperatureDuringGlobalColdest) / 4f);

            //

            DesertThreshold = -1;
            DesertThreshold = CalculateDesertThreshold();
        }

        //Defined in the Koppen Group B specification
        int CalculateDesertThreshold() {
            var desertThreshold = TemperatureAvg * 20;
            var summerRainRatio = TotalRainfallHotSeason /
                                  (float) TotalAnnualRainfall;

            if (summerRainRatio >= .66f)
                desertThreshold += 280;
            else if (summerRainRatio >= .33f)
                desertThreshold += 140;
            // else if (summerRainRatio < .33f)
            // add nothing

            return desertThreshold;
        }

        public override string  ToString() {
            return $"Inverted Season: {InvertedSeason}\n" +
                   $"Summer Rainfall: {RainfallSummer}\n" +
                   $"Winter Rainfall: {RainfallWinter}\n" +
                   $"Total Rainfall: {TotalAnnualRainfall}\n" +
                   $"Temps: {string.Join(", ", Temperatures)}\n" +
                   $"Temp Avg: {TemperatureAvg}\n" +
                   $"Desert Threshold: {DesertThreshold}";
        }

    }
}