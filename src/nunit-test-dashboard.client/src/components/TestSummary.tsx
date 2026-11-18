import type { TestRunResult } from "../types/TestResults";
import Tile from "./Tile";

interface TestSummaryProps {
    results : TestRunResult | null
}

function TestSummary({results} : TestSummaryProps) {

    return(
        <>
        {
            results &&
        <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
            <Tile title="Total" number={results?.tests.length} />
            <Tile title="Passed" number={results?.passed} />
            <Tile title="Failed" number={results?.failed} />
            <Tile title="Skipped" number={results?.skipped} />
            <Tile title="Pass Rate" number={results?.passRate} pct={true} />
        </div>
        }
        </>
    )
}

export default TestSummary;
