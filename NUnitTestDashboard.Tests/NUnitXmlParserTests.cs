using nunit_test_dashboard.Models;
using nunit_test_dashboard.Services;
using System.Text;
using System.Xml;

namespace NUnitTestDashboard.Tests
{
    public class NUnitXmlParserTests
    {
        [Fact]
        public void Parse_ValidXml_ReturnsCorrectTestCount()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite>
                    <test-case name="Test1" result="Passed" duration="0.5" />
                    <test-case name="Test2" result="Failed" duration="0.2" />
                </test-suite>
            </test-run>
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act
            var result = parser.Parse(stream);

            //Assert
            Assert.Equal(2, result.Tests.Count);

        }

        [Fact]
        public void Parse_ValidXml_ReturnsCorrectTestName()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite>
                    <test-case name="Test1" result="Passed" duration="0.5" />
                    <test-case name="Test2" result="Failed" duration="0.2" />
                </test-suite>
            </test-run>
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act
            var result = parser.Parse(stream);

            //Assert
            Assert.Equal("Test1", result.Tests.First().Name);

        }

        [Fact]
        public void Parse_ValidXml_IncludesFailedTests()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite>
                    <test-case name="First" result="Passed" duration="0.1" />
                    <test-case name="Second" result="Failed" duration="0.2">
                        <failure>
                            <message>Something went wrong</message>
                        </failure>
                    </test-case>
                    <test-case name="Third" result="Passed" duration="0.3" />
                </test-suite>
            </test-run>
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act
            var result = parser.Parse(stream);

            //Assert
            Assert.Equal(3, result.Tests.Count);
            Assert.Equal(2, result.Passed);
            Assert.Equal(1, result.Failed);
            Assert.Equal("Something went wrong", result.Tests.First(t => t.Outcome == TestOutcome.Failed).FailureMessage);

        }

        [Fact]
        public void Parse_ValidXml_MultipleTests()
        {
            //Arrange
            string xml = """
            <test-run id="0" testcasecount="5" result="Failed">
                <test-suite type="Assembly" name="Dashboard.Tests">
                    <test-suite type="TestFixture" name="LoginTests">
                        <test-case name="ValidLogin" fullname="LoginTests.ValidLogin"
                                   result="Passed" duration="0.125" />
                        <test-case name="InvalidLogin" fullname="LoginTests.InvalidLogin"
                                   result="Failed" duration="0.230">
                            <failure>
                                <message>Expected login to fail</message>
                            </failure>
                        </test-case>
                    </test-suite>
                    <test-suite type="TestFixture" name="DashboardTests">
                        <test-case name="LoadsDashboard" fullname="DashboardTests.LoadsDashboard"
                                   result="Passed" duration="0.095" />
                        <test-case name="DisplaysResults" fullname="DashboardTests.DisplaysResults"
                                   result="Passed" duration="0.150" />
                        <test-case name="FiltersResults" fullname="DashboardTests.FiltersResults"
                                   result="Skipped" duration="0" />
                    </test-suite>
                </test-suite>
            </test-run>
            
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act
            var result = parser.Parse(stream);

            //Assert
            Assert.Equal(5, result.Tests.Count);
            Assert.Equal(3, result.Passed);
            Assert.Equal(1, result.Failed);
            Assert.Equal(1, result.Skipped);
            Assert.Equal(75, result.PassRate);
            Assert.Equal(0.230, result.Tests.First(t => t.Outcome == TestOutcome.Failed).Duration);
            Assert.Equal("Expected login to fail", result.Tests.First(t => t.Outcome == TestOutcome.Failed).FailureMessage);

        }

        [Fact]
        public void Parse_MalformedXml_ThrowsXmlException()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite name="BrokenTests">
                    <test-case name="TestOne" fullname="BrokenTests.TestOne"
                               result="Passed" duration="0.1">
                </test-suite>
            </test-run>
            
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            Assert.Throws<XmlException>(() => parser.Parse(stream));
        }

        [Fact]
        public void Parse_UnrelatedXml_ThrowsInvalidDataException()
        {
            //Arrange
            string xml = """
            <unrelated-data>
                <test-suite name="BrokenTests">
                    <test-case name="TestOne" fullname="BrokenTests.TestOne"
                               result="Passed" duration="0.1">
                               </test-case>
                </test-suite>
            </unrelated-data>
            
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            Assert.Throws<InvalidDataException>(() => parser.Parse(stream));
        }

        [Fact]
        public void Parse_InvalidNesting_ThrowsInvalidDataException()
        {
            //Arrange
            string xml = """
            <unrelated-data>
                <test-run>
                    <test-suite name="BrokenTests">
                        <test-case name="TestOne" fullname="BrokenTests.TestOne"
                                   result="Passed" duration="0.1">
                        </test-case>
                    </test-suite>
                </test-run>
            </unrelated-data>
            
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            Assert.Throws<InvalidDataException>(() => parser.Parse(stream));
        }

        [Fact]
        public void Parse_XmlWithDeclaration_ParsesFine()
        {
            //Arrange
            string xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <!-- NUnit report generated for a parser test -->
            <test-run>
                <test-suite name="ExampleTests">
                    <test-case name="ExampleTest" result="Passed" duration="0.125" />
                </test-suite>
            </test-run>
            
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            var result = parser.Parse(stream);

            //Assert
            Assert.Equal(1, result.Tests.Count);
            Assert.Equal("ExampleTest", result.Tests.First().Name);
            Assert.Equal(1, result.Passed);
        }

        [Fact]
        public void Parse_InvalidOutcome_ThrowsInvalidDataException()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite>
                    <test-case name="Test1" result="Passed" duration="0.5" />
                    <test-case name="Test2" result="InvalidOutcome" duration="0.2" />
                </test-suite>
            </test-run>
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            InvalidDataException ex = Assert.Throws<InvalidDataException>(() => parser.Parse(stream));
            Assert.Equal("Unsupported test outcome: Test2: InvalidOutcome", ex.Message);
        }

        [Fact]
        public void Parse_InvalidEnum0_ThrowsInvalidDataException()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite>
                    <test-case name="Test1" result="Passed" duration="0.5" />
                    <test-case name="Test2" result="0" duration="0.2" />
                </test-suite>
            </test-run>
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            InvalidDataException ex = Assert.Throws<InvalidDataException>(() => parser.Parse(stream));
            Assert.Equal("Unsupported test outcome: Test2: 0", ex.Message);
        }

        [Fact]
        public void Parse_InvalidEnum99_ThrowsInvalidDataException()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite>
                    <test-case name="Test1" result="Passed" duration="0.5" />
                    <test-case name="Test2" result="99" duration="0.2" />
                </test-suite>
            </test-run>
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act / Assert
            InvalidDataException ex = Assert.Throws<InvalidDataException>(() => parser.Parse(stream));
            Assert.Equal("Unsupported test outcome: Test2: 99", ex.Message);
        }

        [Fact]
        public void Parse_MissingResult_ThrowsInvalidDataException()
        {
            //Arrange
            string xml = """
            <test-run>
                <test-suite name="ExampleTests">
                    <test-case name="ExampleTest1" result="Passed" duration="0.125" />
                    <test-case name="ExampleTest2" duration="0.120" />
                    <test-case name="ExampleTest3" result="Passed" duration="0.130" />
                </test-suite>
            </test-run>
            
            """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var parser = new NUnitXmlParser();

            //Act & Assert
            InvalidDataException ex = Assert.Throws<InvalidDataException>(() => parser.Parse(stream));
            Assert.Equal("Test case missing a result attribute.", ex.Message);
        }
    }
}
