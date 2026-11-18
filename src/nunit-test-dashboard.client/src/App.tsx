import { useState } from 'react';
import TestSummary from './components/TestSummary';
import TestUpload from './components/TestUpload';
import type { TestRunResult } from './types/TestResults';
import TestResultTable from './components/TestResultsTable';

function App() {

const [results, setResults] = useState<TestRunResult | null>(null);

function handleUploadComplete(results : TestRunResult) {
  setResults(results)
}

  return (
    <div className="min-h-screen bg-slate-400 p-6 font-sans">
      <div className="mx-auto max-w-2xl rounded-3xl bg-slate-100/60 p-6">
        <div className="md:flex md:items-start md:justify-between gap-4">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-slate-900">
              NUnit Test Dashboard
            </h1>
            <p className="mt-1 text-sm text-slate-500">Automated test results overview</p>
          </div>
          <TestUpload onUploadComplete={handleUploadComplete} />
        </div>
      <section className="mt-4">
        <TestSummary results={results} />
      </section>
      <section className="mt-4">
        {results ? <TestResultTable tests={results?.tests || []} /> : <p>Upload a file to get started! 
            <a className='btn mx-2 p-1 rounded text-white bg-blue-600 hover:bg-blue-800' href='/test.xml' download>
              Download a sample file.
            </a>
          </p>}
      </section>

        </div>
      </div>)};

      export default App;
