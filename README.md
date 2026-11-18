## NUnit Test Dashboard

A very simple dashboard for NUnit test results.

It is a single page application that displays the results of NUnit tests in a user-friendly way. The dashboard can be used to monitor the status of tests, view test results.

The purpose of me putting this together was to create a .NET Minimal API combined with a React frontend.
C# and Typescript code was written by a human and reveiwed by AI. 
AI was also used for quick bug fixing.


### Features
- Displays NUnit test results in a table format with tiles
- Provides a clear overview of test statuses (passed, failed, skipped)
- Allows filtering of test results

If I got the time, I would like to add the following features:
- More dashboard detail such as pie charts and graphs
- Exporting more user friendly test results to CSV/Excel

### Prereqs

- .NET 10 SDK
- Node.js and npm compatible with Vite 8
- Git

### Getting Started

Clone the repository and enter its directory:

```bash
git clone https://github.com/jackhipperson/nunit-test-dashboard.git
cd nunit-test-dashboard
```

#### Start the backend

Build the solution and start the API using its HTTPS profile:

```bash
dotnet build
dotnet run --project nunit-test-dashboard/NUnitTestDashboard.csproj --launch-profile https
```

The API runs at `https://localhost:7053`. Leave this terminal running.

#### Start the frontend

Open a second terminal in the repository root:

```bash
cd src/nunit-test-dashboard.client
npm ci
npm run dev
```

Open the URL printed by Vite, normally `http://localhost:5173`.

### Try the Sample Report

1. Click **Download a sample file** on the dashboard.
2. Click **Choose file** and select the downloaded XML file.
3. Click **Upload**.

The sample contains five tests: three passed, one failed, and one skipped. The expected pass rate is 75%, excluding skipped tests.

Use the outcome filter to narrow the results, and click **View** beside the failed test to inspect its failure message.

Uploaded files must be smaller than 2,000,000 bytes.

### Run the Tests

From the repository root:

```bash
dotnet test
```