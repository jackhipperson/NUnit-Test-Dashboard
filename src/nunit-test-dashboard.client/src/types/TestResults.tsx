export type TestOutcome = 'Passed' | 'Failed' | 'Skipped';

export interface TestResult {
    name: string;
    outcome: TestOutcome;
    duration: number;
    failureMessage: string | null;
}

export interface TestRunResult {
    tests: TestResult[];
    passed: number;
    failed: number;
    skipped: number;
    passRate: number;
}