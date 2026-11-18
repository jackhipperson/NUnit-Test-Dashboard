import { useRef, useState, type ChangeEvent, type FormEvent } from "react";
import type { TestRunResult } from "../types/TestResults";

interface TestUploadProps {
  onUploadComplete: (results: TestRunResult) => void;
}

function FileUpload({ onUploadComplete }: TestUploadProps) {
  const [selectedFile, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  function handleChange(e: ChangeEvent<HTMLInputElement>) {
    const selected = e.target.files?.[0] ?? null;
    setError(null);

    if (selected && selected.size >= 2_000_000) {
      setError("File must be smaller than 2MB.");
      setFile(null);
      e.target.value = "";
      return;
    }

    setFile(selected);
  }

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!selectedFile) return;

    setUploading(true);
    setError(null);

    try {
      const formData = new FormData();
      formData.append("request", selectedFile);

      const response = await fetch("api/results/upload", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        var problem = await response.json().catch(() => null);
        throw new Error(problem?.detail ?? `Upload failed: ${response.status}`);
      }

      const data: TestRunResult = await response.json();
      onUploadComplete(data);

      setFile(null);
      if (inputRef.current) inputRef.current.value = "";
    } catch (err) {
      setError(err instanceof Error ? err.message : "Upload failed");
    } finally {
      setUploading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col md:items-end mt-2 gap-2">
      <input
        ref={inputRef}
        type="file"
        accept=".xml"
        onChange={handleChange}
        className="hidden"
      />

      <div className="flex items-center gap-3">
        <button
          type="button"
          onClick={() => inputRef.current?.click()}
          disabled={uploading}
          className="rounded bg-blue-700 hover:bg-blue-900 p-2 text-white">
          <FileIcon />
          Choose file
        </button>

        <button type="submit" disabled={!selectedFile || uploading}
        className="rounded bg-blue-700 hover:bg-blue-900 p-2 text-white">
          <UploadIcon />
          {uploading ? "Uploading..." : "Upload"}
        </button>
      </div>

      {selectedFile && (
        <p className="text-sm text-slate-500">
          Selected: {selectedFile.name} ({(selectedFile.size / 1024).toFixed(1)} KB)
        </p>
      )}

      {error && (
        <p role="alert" className="text-sm text-red-600">
          {error}
        </p>
      )}
    </form>
  );
}

function UploadIcon() {
  return (
      <i className="fa-solid fa-upload mr-2"></i>
  );
}

function FileIcon() {
  return (
<i className="fa-regular fa-folder-open mr-2"></i>
  );
}

export default FileUpload;