using System;

namespace Koppen
{
    /// <summary>
    /// Holds all the data for a specific sample on the map
    /// </summary>
    public class WeatherSample
    {
        public readonly byte RainfallMax; //mm
        public readonly byte RainfallMin;
        public readonly byte RainfallAvg;
        public readonly byte RainfallSummer;
        public readonly byte RainfallWinter;

        public readonly int TotalRainfallSummer;
        public readonly int TotalRainfallWinter;
        public readonly int TotalAnnualRainfall;

        public readonly short TemperatureMax; //C
        public readonly short TemperatureMin;
        public readonly short TemperatureAvg;
        public readonly short[] Temperatures;

        public readonly Hemisphere Hemisphere;

        public readonly int DesertThreshold;

        public WeatherSample(
            Hemisphere hemisphere,
            byte summerRainfall, byte winterRainfall,
            short summerTemp, short winterTemp,

            //"hottest" and "coldest" are slightly misleading because the values
            //they contain are not necessarily the hottest or coldest of the 4
            //given temperatures. These variables should be understood as "A
            //sample from the [hottest/coldest/summer/winter] global average
            //temperature map"
            short hottestTemp, short coldestTemp) {

            Hemisphere = hemisphere;

            //

            var rainfall = new [] {summerRainfall, winterRainfall};
            Array.Sort(rainfall);

            RainfallMin = rainfall[0];
            RainfallMax = rainfall[1];
            RainfallAvg = (byte)Math.Round(
                (summerRainfall + winterRainfall) / 2f);

            if (Hemisphere == Hemisphere.North) {
                RainfallSummer = summerRainfall;
                RainfallWinter = winterRainfall;
            } else {
                RainfallSummer = winterRainfall;
                RainfallWinter = summerRainfall;
            }

            TotalRainfallSummer = RainfallSummer * 6;
            TotalRainfallWinter = RainfallWinter * 6;
            TotalAnnualRainfall = RainfallAvg * 12;

            //

            var temperatures = new[]
                {summerTemp, winterTemp, hottestTemp, coldestTemp};
            Array.Sort(temperatures);

            Temperatures = temperatures;
            TemperatureMin = temperatures[0];
            TemperatureMax = temperatures[3];
            TemperatureAvg = (short)Math.Round(
                (summerTemp + winterTemp + hottestTemp + coldestTemp) / 4f);

            //

            DesertThreshold = CalculateDesertThreshold();
        }

        //Defined in the Koppen Group B specification
        int CalculateDesertThreshold() {
            var desertThreshold = TemperatureAvg * 20;
            var summerRainRatio = TotalRainfallSummer /
                                  (float) TotalAnnualRainfall;

            if (summerRainRatio >= .66f)
                desertThreshold += 280;
            else if (summerRainRatio >= .33f)
                desertThreshold += 140;
            // else if (summerRainRatio < .3f)
            // add nothing

            return desertThreshold;
        }

        public override string  ToString() {
            return $"Hemisphere: {Hemisphere}\n" +
                   $"Summer Rainfall: {RainfallSummer}\n" +
                   $"Winter Rainfall: {RainfallWinter}\n" +
                   $"Total Rainfall: {TotalAnnualRainfall}\n" +
                   $"Temps: {string.Join(", ", Temperatures)}\n" +
                   $"Temp Avg: {TemperatureAvg}\n" +
                   $"Desert Threshold: {DesertThreshold}";
        }

    }
}