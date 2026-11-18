namespace nunit_test_dashboard.Models
{
    public class TestRunResult
    {
        public List<TestResult> Tests { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public double PassRate { get; set; }
    }
}
