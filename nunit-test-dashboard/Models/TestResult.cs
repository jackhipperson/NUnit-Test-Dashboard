namespace nunit_test_dashboard.Models
{
    public class TestResult
    {
        public string? Name { get; set; }
        public TestOutcome? Outcome { get; set; }
        public double Duration { get; set; }
        public string? FailureMessage { get; set; }
    }
}
