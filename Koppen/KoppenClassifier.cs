using System;
using System.Linq;

namespace Koppen
{
    public static class KoppenClassifier
    {
        // Based on the wikipedia koppen entry, and Table 1 from this paper:
        // https://opus.bibliothek.uni-augsburg.de/opus4/frontdoor/deliver/index/docId/40083/file/metz_Vol_15_No_3_p259-263_World_Map_of_the_Koppen_Geiger_climate_classification_updated_55034.pdf
        public static string Classify(WeatherSample sample) {

            if(sample.TemperatureMax < 0)
                return "EF";

            if (sample.TemperatureMax >= 0 && sample.TemperatureMax < 10)
                return "ET";

            var group = string.Empty;
            var precipitation = string.Empty;
            var temperature = string.Empty;

            // have to check before Group A because Aw is defined basically as
            // "Hot, but not rainy, but also not B"
            //Group B
            if (sample.TotalAnnualRainfall <= sample.DesertThreshold) {

                group = "B";

                precipitation =
                    (sample.TotalAnnualRainfall <= sample.DesertThreshold * .5f)
                    ? "W" //desert
                    : "S"; //steppe

                temperature = sample.TemperatureMin > 0 ? "h" : "k";

                return $"{group}{precipitation}{temperature}";
            } // else rainfall is above the desert threshold, this is not a Group B climate

            //Group A
            if (sample.TemperatureMin >= 18) {

                group = "A";

                var monsoonRainfallCutoff = 25 * (100 - sample.RainfallMin);

                if (sample.RainfallMin >= 60)
                    precipitation = "f";
                else if (sample.TotalAnnualRainfall >= monsoonRainfallCutoff)
                    precipitation = "m";
                else if (sample.RainfallWinter < 60
                      && sample.RainfallWinter < sample.RainfallSummer)
                    precipitation = "w";
                else
                    precipitation = "s";

                return $"{group}{precipitation}";

            }

            string GetPrecipitationForCorD (WeatherSample s) {
                if (s.RainfallSummer < s.RainfallWinter
                 && s.RainfallWinter > 3 * s.RainfallSummer
                 && s.RainfallSummer < 40)
                    return "s";
                else if (s.RainfallSummer > 10 * s.RainfallWinter)
                    return "w";
                else
                    return "f";
            }

            //Group C
            if (sample.TemperatureMin > 0) {
                group = "C";
                precipitation = GetPrecipitationForCorD(sample);
            }

            //Group D
            if (sample.TemperatureMin <= 0) {
                group = "D";
                precipitation = GetPrecipitationForCorD(sample);
            }

            if (sample.TemperatureMax >= 22)
                temperature = "a";
            else if (sample.Temperatures.Count(t => t > 10) >= 2)
                temperature = "b";
            else if (sample.TemperatureMin > -38)
                temperature = "c";
            else
                temperature = "d";

            if(group == string.Empty)
                throw new Exception("Could not determine Koppen Climate Group for Sample: " + sample.ToString());

            return $"{group}{precipitation}{temperature}";

        }
    }
}