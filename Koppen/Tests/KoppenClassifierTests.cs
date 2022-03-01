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
                                summerRainfall: summerRainfall,
                                winterRainfall: summerRainfall,
                                temperatureDuringSummer: hottestTemp,
                                temperatureDuringWinter: coldestTemp,
                                temperatureDuringGlobalHottest: hottestTemp,
                                temperatureDuringGlobalColdest: coldestTemp
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
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: -1,
                temperatureDuringWinter: -1,
                temperatureDuringGlobalHottest: -1,
                temperatureDuringGlobalColdest: -1
            );

            Assert.AreEqual("EF", Koppen.KoppenClassifier.Classify(sample));

            var cuspSample = new WeatherSample(
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: 0,
                temperatureDuringWinter: 0,
                temperatureDuringGlobalHottest: 0,
                temperatureDuringGlobalColdest: 0
            );

            Assert.AreNotEqual("EF", Koppen.KoppenClassifier.Classify(cuspSample));

        }
        [Test]
        public void ETTest() {

            var sample = new WeatherSample(
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: 5,
                temperatureDuringWinter: -10,
                temperatureDuringGlobalHottest: 6,
                temperatureDuringGlobalColdest: -10
            );

            Assert.AreEqual("ET", Koppen.KoppenClassifier.Classify(sample));

            var lowCuspSample = new WeatherSample(
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: 0,
                temperatureDuringWinter: 0,
                temperatureDuringGlobalHottest: 0,
                temperatureDuringGlobalColdest: 0
            );

            Assert.AreEqual("ET", Koppen.KoppenClassifier.Classify(lowCuspSample));

            var highCuspSample = new WeatherSample(
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: 9,
                temperatureDuringWinter: 9,
                temperatureDuringGlobalHottest: 9,
                temperatureDuringGlobalColdest: 9
            );

            Assert.AreEqual("ET", Koppen.KoppenClassifier.Classify(highCuspSample));

            var overCuspSample = new WeatherSample(
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: 10
            );

            Assert.AreNotEqual("ET", Koppen.KoppenClassifier.Classify(overCuspSample));

        }

        [Test]
        public void GroupBTest() {
            var sample = new WeatherSample(
                summerRainfall: 0,
                winterRainfall: 0,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: 10
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("B"));

            var desertSample = new WeatherSample(
                summerRainfall: 7,
                winterRainfall: 3,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: 10
            );

            Assert.AreEqual("BWh",
                Koppen.KoppenClassifier.Classify(desertSample), desertSample.ToString());

            var steppeSample = new WeatherSample(
                summerRainfall: 23,
                winterRainfall: 10,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: 10
            );

            Assert.AreEqual("BSh",
                Koppen.KoppenClassifier.Classify(steppeSample), steppeSample.ToString());

            var desertColdSample = new WeatherSample(
                summerRainfall: 7,
                winterRainfall: 3,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: -1
            );

            Assert.AreEqual("BWk",
                Koppen.KoppenClassifier.Classify(desertColdSample), desertColdSample.ToString());

            var steppeColdSample = new WeatherSample(
                summerRainfall: 16,
                winterRainfall: 6,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: -1
            );

            Assert.AreEqual("BSk",
                Koppen.KoppenClassifier.Classify(steppeColdSample), steppeColdSample.ToString());

            var steppeCuspSample = new WeatherSample(
                summerRainfall: 23,
                winterRainfall: 10,
                temperatureDuringSummer: 10,
                temperatureDuringWinter: 10,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: 10
            );

            Assert.AreEqual("BSh",
                Koppen.KoppenClassifier.Classify(steppeCuspSample), steppeCuspSample.ToString());

        }

        [Test]
        public void GroupATest() {

            var sample = new WeatherSample(
                summerRainfall: 42,
                winterRainfall: 42,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("A"), sample.ToString());

            var tooColdSample = new WeatherSample(
                summerRainfall: 42,
                winterRainfall: 42,
                temperatureDuringSummer: 17,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooColdSample).StartsWith("A"), tooColdSample.ToString());

            var tooDrySample = new WeatherSample(
                summerRainfall: 22,
                winterRainfall: 22,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooDrySample).StartsWith("A"), tooDrySample.ToString());


        }

        [Test]
        public void GroupAwTest() {

            var sample = new WeatherSample(
                summerRainfall: 43,
                winterRainfall: 42,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.AreEqual("Aw", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupAsTest() {

            var sample = new WeatherSample(
                summerRainfall: 42,
                winterRainfall: 43,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.AreEqual("As", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupAfTest() {

            var sample = new WeatherSample(
                summerRainfall: 60,
                winterRainfall: 60,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.AreEqual("Af", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupAmTest() {

            var sample = new WeatherSample(
                summerRainfall: 120,
                winterRainfall: 58,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 18,
                temperatureDuringGlobalHottest: 18,
                temperatureDuringGlobalColdest: 18
            );

            Assert.AreEqual("Am", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupCTest() {

            var sample = new WeatherSample(
                summerRainfall: 42,
                winterRainfall: 42,
                temperatureDuringSummer: 15,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("C"), sample.ToString());

            var tooColdSample = new WeatherSample(
                summerRainfall: 42,
                winterRainfall: 42,
                temperatureDuringSummer: 12,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 12,
                temperatureDuringGlobalColdest: -1
                );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooColdSample).StartsWith("C"), tooColdSample.ToString());

        }

        [Test]
        public void GroupCsTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 15,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Cs"), sample.ToString());

            var wrongRainRatioSample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 59,
                temperatureDuringSummer: 15,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(wrongRainRatioSample).StartsWith("Cs"), wrongRainRatioSample.ToString());

            var tooMuchSummerRainSample = new WeatherSample(
                summerRainfall: 41,
                winterRainfall: 124,
                temperatureDuringSummer: 15,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(tooMuchSummerRainSample).StartsWith("Cs"), tooMuchSummerRainSample.ToString());


        }

        [Test]
        public void GroupCwTest() {

            var sample = new WeatherSample(
                summerRainfall: 120,
                winterRainfall: 10,
                temperatureDuringSummer: 15,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Cw"), sample.ToString());

            var wrongRainRatioSample = new WeatherSample(
                summerRainfall: 90,
                winterRainfall: 20,
                temperatureDuringSummer: 15,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsFalse(Koppen.KoppenClassifier.Classify(wrongRainRatioSample).StartsWith("Cw"), wrongRainRatioSample.ToString());

        }

        [Test]
        public void GroupCsaTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 22,
                temperatureDuringGlobalColdest: 1
            );

            Assert.AreEqual("Csa", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupCsbTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 17,
                temperatureDuringGlobalColdest: 1
            );

            Assert.AreEqual("Csb", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupCscTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 9,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: 1
            );

            Assert.AreEqual("Csc", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupDTest() {

            var sample = new WeatherSample(
                summerRainfall: 50,
                winterRainfall: 50,
                temperatureDuringSummer: -1,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("D"), sample.ToString());

        }

        [Test]
        public void GroupDsTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: -1,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Ds"), sample.ToString());

        }

        [Test]
        public void GroupDwTest() {

            var sample = new WeatherSample(
                summerRainfall: 61,
                winterRainfall: 6,
                temperatureDuringSummer: -1,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Dw"), sample.ToString());
        }

        [Test]
        public void GroupDfTest() {

            var sample = new WeatherSample(
                summerRainfall: 30,
                winterRainfall: 30,
                temperatureDuringSummer: -1,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 16,
                temperatureDuringGlobalColdest: 1
            );

            Assert.IsTrue(Koppen.KoppenClassifier.Classify(sample).StartsWith("Df"), sample.ToString());
        }

        [Test]
        public void GroupDsaTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: -1,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 22,
                temperatureDuringGlobalColdest: 1
            );

            Assert.AreEqual("Dsa", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupDsbTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 18,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 17,
                temperatureDuringGlobalColdest: -1
            );

            Assert.AreEqual("Dsb", Koppen.KoppenClassifier.Classify(sample), sample.ToString());

        }

        [Test]
        public void GroupDscTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 9,
                temperatureDuringWinter: 2,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: -1
            );

            Assert.AreEqual("Dsc", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

        [Test]
        public void GroupDsdTest() {

            var sample = new WeatherSample(
                summerRainfall: 20,
                winterRainfall: 61,
                temperatureDuringSummer: 9,
                temperatureDuringWinter: -20,
                temperatureDuringGlobalHottest: 10,
                temperatureDuringGlobalColdest: -38
            );

            Assert.AreEqual("Dsd", Koppen.KoppenClassifier.Classify(sample), sample.ToString());
        }

    }
}