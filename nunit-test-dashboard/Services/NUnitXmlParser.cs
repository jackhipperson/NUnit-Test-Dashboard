using nunit_test_dashboard.Models;
using System.Globalization;
using System.Xml;

namespace nunit_test_dashboard.Services
{
    public class NUnitXmlParser
    {
        public TestRunResult Parse(Stream stream)
        {
            var testRunResult = new TestRunResult
            {
                Tests = new List<TestResult>()
            };

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 2_000_000,
            };

            using (var reader = XmlReader.Create(stream, settings))
            {
                reader.MoveToContent();

                if (reader.NodeType != XmlNodeType.Element || reader.Name != "test-run")
                {
                    throw new InvalidDataException("Expected a NUnit report with a test-run root element.");
                }

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-case")
                    {
                        if (reader.GetAttribute("result") == null)
                        {
                            throw new InvalidDataException("Test case missing a result attribute.");
                        }

                        var rawOutcome = reader.GetAttribute("result");

                        bool isSupported = string.Equals(rawOutcome, "Passed", StringComparison.OrdinalIgnoreCase) ||
                                           string.Equals(rawOutcome, "Failed", StringComparison.OrdinalIgnoreCase) ||
                                           string.Equals(rawOutcome, "Skipped", StringComparison.OrdinalIgnoreCase);

                        if (!isSupported)
                        {
                            throw new InvalidDataException($"Unsupported test outcome: {reader.GetAttribute("name")}: {rawOutcome}");
                        }

                        if (Enum.TryParse<TestOutcome>(rawOutcome, ignoreCase: true, out var outcome))
                        {
                            var testResult = new TestResult
                            {
                                Name = reader.GetAttribute("name") ?? "",
                                Outcome = outcome,
                                Duration = double.TryParse(reader.GetAttribute("duration"), NumberStyles.Float, CultureInfo.InvariantCulture, out var duration) ? duration : 0,
                                FailureMessage = null
                            };
                            if (testResult.Outcome == TestOutcome.Failed)
                            {
                                if (reader.ReadToDescendant("failure"))
                                {
                                    if (reader.ReadToDescendant("message"))
                                    {
                                        testResult.FailureMessage = reader.ReadElementContentAsString();
                                    }
                                }
                            }
                            testRunResult.Tests.Add(testResult);

                        }
                    }
                }
            }

            // Calculate summary statistics
            testRunResult.Passed = testRunResult.Tests.Count(t => t.Outcome == TestOutcome.Passed);
            testRunResult.Failed = testRunResult.Tests.Count(t => t.Outcome == TestOutcome.Failed);
            testRunResult.Skipped = testRunResult.Tests.Count(t => t.Outcome == TestOutcome.Skipped);
            var testsRun = testRunResult.Tests.Count(t => t.Outcome == TestOutcome.Passed || t.Outcome == TestOutcome.Failed);
            testRunResult.PassRate = (testsRun > 0) ? (double)testRunResult.Passed
                / testRunResult.Tests.Count(t => t.Outcome != TestOutcome.Skipped) * 100 : 0;
            return testRunResult;
        }
    }
}
