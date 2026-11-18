import { Fragment, useEffect, useState, type ChangeEvent } from 'react';
import type { TestResult } from '../types/TestResults';

interface TestResultsTableProps {
    tests: TestResult[];
}

function TestResultTable({tests} : TestResultsTableProps) {

    const [filter,setFilter] = useState<string>("All");
    const [openIds, setOpenIds] = useState<Set<string>>(new Set());

    const handleToggle = (id: string) => {
        setOpenIds((prev) => {
            const next = new Set(prev);
            if (next.has(id)) {
                next.delete(id) 
            } else {
                next.add(id);
            } 
            return next;
        })
    }

    const filteredList = filter === "All" ? tests : tests.filter(x => x.outcome == filter);
    
    const resetFilter = () => {
        setFilter("All");
    }
    
    const resetOpenIds = () => {
        setOpenIds(new Set());
    }

    const handleFilter = (event : ChangeEvent<HTMLSelectElement>) => {
        setFilter(event.currentTarget.value);
        resetOpenIds();
    }

    useEffect(() => {
        resetOpenIds();
        resetFilter();
    },[tests])

    return (
        <>
        <div className='py-4'>
            <label className='pe-4' htmlFor='resultFilter'>Filter results:</label>
            <select className="bg-white" id="resultFilter" value={filter} onChange={handleFilter}>
                <option value="All">All</option>
                <option value="Passed">Passed</option>
                <option value="Failed">Failed</option>
                <option value="Skipped">Skipped</option>
            </select>
        </div>

                    {filteredList.length == 0 && <p>No results found!</p>}
            {filteredList.length > 0 &&
        <table className="table-auto min-w-full">
            <thead>
                <tr className="border-b-2 border-slate-400 font-bold">
                    <th className="text-left py-2">Test Name</th>
                    <th className="text-left py-2">Outcome</th>
                    <th className="text-left py-2">Duration</th>
                    <th className="text-left py-2"></th>
                </tr>
            </thead>
            <tbody>
                { filteredList.map((test, i) => 
                {
                const expandable = test.outcome == "Failed";
                const isOpen = openIds.has(i.toString());

            return (
                <Fragment key={i}>
                <tr className="border-b-2 border-b-slate-400 py-4">
                    <td className="py-2">{test.name}</td>
                    <td className="py-2"><span className={`${test.outcome === "Passed" ? "bg-green-300 text-green-900"
                        : test.outcome === "Failed" ? "bg-red-300 text-red-900"
                        : "bg-gray-300 text-gray-900"} text-sm px-2 rounded-3xl`}>{test.outcome}</span></td>
                    <td className="py-2">{test.duration}s</td>
                    <td className="py-2">{ expandable &&
                        <button aria-expanded={isOpen} onClick={() => handleToggle(i.toString())}>{isOpen ? "Close" : "View"}</button>
            }</td>
                </tr>
                {isOpen &&
                <tr className="border-b-2 border-b-slate-400 py-4">
                    <td className='py-2 whitespace-pre-wrap' colSpan={4}>
                        {test.failureMessage}
                    </td>
                </tr>
                }
                </Fragment>
            )})
        }
        </tbody>
            
        </table>
}
        </>
    )
}

export default TestResultTable;