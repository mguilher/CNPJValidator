using CNPJValidator;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using static CNPJValidator.Errors.Errors;

namespace CNPJValidator.Test
{
    public class CnpjValidatorTests
    {
        private static readonly string[] VALID_CNPJS = 
        [
            "12345678000195",
            "12ABC34501AB77",
            "AB12CD34EF5602",
            "A1B2C3D4E5F668",
            "ZXCVBN1234QW16",
            "00000000000191",
        ];


        [SetUp]
        public void Setup()
        {
        }

        public static IEnumerable<string> GetTestDataFromFile()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "cnpjs.txt");

            string data = File.ReadAllText("cnpjs.txt");
            return data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        [Test]
        [TestCaseSource(nameof(GetTestDataFromFile))]
        public void IsValid_AcceptsValidCNPJ_InFile(string cnpj)
        {
            var result = Cnpj.Create(cnpj);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        [TestCase("12.ABC.345/01AB-77")]
        [TestCase("12.abc.345/01ab-77")]
        [TestCase("12...ABC...345///01AB---77!!!")]
        public void IsValid_AcceptsValidAlphanumericCNPJ(string value)
        {
            var result = Cnpj.Create(value);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        [TestCase("12.ABC.345/01AB-35")]
        [TestCase("00000000000000")]
        [TestCase("AAAAAAAAAAAAAA")]
        public void IsValid_RejectsLegacySampleAndRepeatedSequences(string value)
        {
            var result = Cnpj.Create(value);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(VALID_CNPJS))]
        public void IsValid_AcceptsValidCNPJ(string cnpj)
        {
            var result = Cnpj.Create(cnpj);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(VALID_CNPJS))]
        public void IsValid_RejectsCheckDigitMutations(string cnpj)
        {
            var penultimate = cnpj[12];
            var penultimateValue = char.IsDigit(penultimate) ? int.Parse(penultimate.ToString()) : 0;
            var nextPenultimate = (penultimateValue + 1) % 10;
            var mutatedPenultimate = $"{cnpj.Substring(0, 12)}{nextPenultimate}{(cnpj.Length > 13 ? cnpj[13].ToString() : "0")}";
            Assert.That(Cnpj.Create(mutatedPenultimate).IsFailure, Is.True);

            var last = cnpj[^1];
            var lastValue = char.IsDigit(last) ? int.Parse(last.ToString()) : 0;
            var nextLast = (lastValue + 1) % 10;
            var mutatedLast = $"{cnpj.Substring(0, 13)}{nextLast}";
            Assert.That(Cnpj.Create(mutatedLast).IsFailure, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase("1")]
        [TestCase("1234567890123")]
        [TestCase("123456789012345")]
        [TestCase("11.222.333/0001")]
        [TestCase("11.222.333/0001-811")]
        public void IsValid_RejectsInvalidLength(string value)
        {
            var result1 = Cnpj.Create(value);
            Assert.That(result1.IsFailure, Is.True);
        }

        [Test]
        public void IsValid_RejectsNullAndWhitespace()
        {
            Assert.That(CnpjValidator.IsValidCNPJ(null), Is.False);
            Assert.That(CnpjValidator.IsValidCNPJ(""), Is.False);
            Assert.That(CnpjValidator.IsValidCNPJ("   "), Is.False);
        }
    }
}
