using NUnit.Framework;

namespace Koppen.Tests
{
    [TestFixture]
    public class KoppenClassifierTests
    {
        [Test]
        [Ignore("Ignored for performance")]
        public void LegalRangeTest() {
            const byte RainMin = 0;
            const byte RainMax = 201;

            const short TempMin = -45;
            const short TempMax = 46;

            for (var summerRainfall = RainMin;
                summerRainfall < RainMax;
                summerRainfall++) {
                for (var winterRainfall = RainMin;
                    winterRainfall < RainMax;
                    winterRainfall++) {
                    for (var hottestTemp = TempMin;
                        hottestTemp < TempMax;
                        hottestTemp++) {
                        for (var coldestTemp = TempMin;
                            coldestTemp < TempMax;
                            coldestTemp++) {
                            var sample = new WeatherSample(
                                hemisphere: Hemisphere.North,
                                summerRainfall: summerRainfall,
                                winterRainfall: summerRainfall,
                                summerTemp: hottestTemp,
                                winterTemp: coldestTemp,
                                hottestTemp: hottestTemp,
                                coldestTemp: coldestTemp
                            );

                            Assert.DoesNotThrow(
                                () => KoppenClassifier.Classify(sample));
                        }

                    }
                }
            }
        }

        [Test]
        public void EFTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: -1,
                winterTemp: -1,
                hottestTemp: -1,
                coldestTemp: -1
            );

            Assert.AreEqual("EF", Koppen.KoppenClassifier.Classify(sample));

            var cuspSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: 0,
                winterTemp: 0,
                hottestTemp: 0,
                coldestTemp: 0
            );

            Assert.AreNotEqual("EF", Koppen.KoppenClassifier.Classify(cuspSample));

        }
        [Test]
        public void ETTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: 5,
                winterTemp: -10,
                hottestTemp: 6,
                coldestTemp: -10
            );

            Assert.AreEqual("ET", Koppen.KoppenClassifier.Classify(sample));

            var lowCuspSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: 0,
                winterTemp: 0,
                hottestTemp: 0,
                coldestTemp: 0
            );

            Assert.AreEqual("ET", Koppen.KoppenClassifier.Classify(lowCuspSample));

            var highCuspSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: 9,
                winterTemp: 9,
                hottestTemp: 9,
                coldestTemp: 9
            );

            Assert.AreEqual("ET", Koppen.KoppenClassifier.Classify(highCuspSample));

            var overCuspSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: 10
            );

            Assert.AreNotEqual("ET", Koppen.KoppenClassifier.Classify(overCuspSample));

        }

        [Test]
        public void GroupBTest() {
            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 0,
                winterRainfall: 0,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: 10
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("B"));

            var desertSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 7,
                winterRainfall: 3,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: 10
            );

            Assert.AreEqual("BWh",
                Koppen.KoppenClassifier.Classify(desertSample), desertSample.ToString());

            var steppeSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 29,
                winterRainfall: 13,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: 10
            );

            Assert.AreEqual("BSh",
                Koppen.KoppenClassifier.Classify(steppeSample), steppeSample.ToString());

            var desertColdSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 7,
                winterRainfall: 3,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: -1
            );

            Assert.AreEqual("BWk",
                Koppen.KoppenClassifier.Classify(desertColdSample), desertColdSample.ToString());

            var steppeColdSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 29,
                winterRainfall: 13,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: -1
            );

            Assert.AreEqual("BSk",
                Koppen.KoppenClassifier.Classify(steppeColdSample), steppeColdSample.ToString());

            var steppeCuspSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 29,
                winterRainfall: 13,
                summerTemp: 10,
                winterTemp: 10,
                hottestTemp: 10,
                coldestTemp: 10
            );

            Assert.AreEqual("BSh",
                Koppen.KoppenClassifier.Classify(steppeCuspSample), steppeCuspSample.ToString());

        }

        [Test]
        public void GroupATest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 42,
                winterRainfall: 42,
                summerTemp: 18,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("A"), sample.ToString());

            var tooColdSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 42,
                winterRainfall: 42,
                summerTemp: 17,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooColdSample).StartsWith("A"), tooColdSample.ToString());

            var tooDrySample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 22,
                winterRainfall: 22,
                summerTemp: 18,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooDrySample).StartsWith("A"), tooDrySample.ToString());


        }

        [Test]
        public void GroupAwTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 43,
                winterRainfall: 42,
                summerTemp: 18,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.AreEqual("Aw", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupAsTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 42,
                winterRainfall: 43,
                summerTemp: 18,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.AreEqual("As", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupAfTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 60,
                winterRainfall: 60,
                summerTemp: 18,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.AreEqual("Af", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupAmTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 120,
                winterRainfall: 58,
                summerTemp: 18,
                winterTemp: 18,
                hottestTemp: 18,
                coldestTemp: 18
            );

            Assert.AreEqual("Am", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupCTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 42,
                winterRainfall: 42,
                summerTemp: 15,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("C"), sample.ToString());

            var tooColdSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 42,
                winterRainfall: 42,
                summerTemp: 12,
                winterTemp: 2,
                hottestTemp: 12,
                coldestTemp: -1
                );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooColdSample).StartsWith("C"), tooColdSample.ToString());

        }

        [Test]
        public void GroupCsTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 15,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Cs"), sample.ToString());

            var wrongRainRatioSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 59,
                summerTemp: 15,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(wrongRainRatioSample).StartsWith("Cs"), wrongRainRatioSample.ToString());

            var tooMuchSummerRainSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 41,
                winterRainfall: 124,
                summerTemp: 15,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooMuchSummerRainSample).StartsWith("Cs"), tooMuchSummerRainSample.ToString());


        }

        [Test]
        public void GroupCwTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 120,
                winterRainfall: 10,
                summerTemp: 15,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Cw"), sample.ToString());

            var wrongRainRatioSample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 90,
                winterRainfall: 20,
                summerTemp: 15,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(wrongRainRatioSample).StartsWith("Cw"), wrongRainRatioSample.ToString());

        }

        [Test]
        public void GroupCsaTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 18,
                winterTemp: 2,
                hottestTemp: 22,
                coldestTemp: 1
            );

            Assert.AreEqual("Csa", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupCsbTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 18,
                winterTemp: 2,
                hottestTemp: 17,
                coldestTemp: 1
            );

            Assert.AreEqual("Csb", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupCscTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 9,
                winterTemp: 2,
                hottestTemp: 10,
                coldestTemp: 1
            );

            Assert.AreEqual("Csc", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupDTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 50,
                winterRainfall: 50,
                summerTemp: -1,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("D"), sample.ToString());

        }

        [Test]
        public void GroupDsTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: -1,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Ds"), sample.ToString());

        }

        [Test]
        public void GroupDwTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 61,
                winterRainfall: 6,
                summerTemp: -1,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Dw"), sample.ToString());
        }

        [Test]
        public void GroupDfTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 30,
                winterRainfall: 30,
                summerTemp: -1,
                winterTemp: 2,
                hottestTemp: 16,
                coldestTemp: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Df"), sample.ToString());
        }

        [Test]
        public void GroupDsaTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: -1,
                winterTemp: 2,
                hottestTemp: 22,
                coldestTemp: 1
            );

            Assert.AreEqual("Dsa", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupDsbTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 18,
                winterTemp: 2,
                hottestTemp: 17,
                coldestTemp: -1
            );

            Assert.AreEqual("Dsb", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupDscTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 9,
                winterTemp: 2,
                hottestTemp: 10,
                coldestTemp: -1
            );

            Assert.AreEqual("Dsc", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupDsdTest() {

            var sample = new WeatherSample(
                hemisphere: Hemisphere.North,
                summerRainfall: 20,
                winterRainfall: 61,
                summerTemp: 9,
                winterTemp: -20,
                hottestTemp: 10,
                coldestTemp: -38
            );

            Assert.AreEqual("Dsd", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

    }
}