import { useState, useRef, type FormEvent } from 'react';
import { Button, Label, Textarea, Alert } from 'flowbite-react';
import { processTextStream } from '../services/sseService';
import { isEmpty } from '../utils/validation';
import { useProgressBar } from '../hooks/useProgressBar';

const TextCapture = () => {
  const [text, setText] = useState('');
  const [processing, setProcessing] = useState(false);
  const [result, setResult] = useState('');
  const [error, setError] = useState('');
  const { progress, updateProgress, resetProgress, ProgressBar } = useProgressBar();
  const cancelFnRef = useRef<(() => void) | null>(null);
  const requestIdRef = useRef<string | null>(null);

  const handleProcess = (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setResult('');
    resetProgress();

    if (isEmpty(text)) {
      setError('Please enter text');
      return;
    }

    setProcessing(true);

    // Generate a new requestId for this process
    requestIdRef.current = crypto.randomUUID();

    cancelFnRef.current = processTextStream(text, requestIdRef.current, {
      onCharacter: (char, advance) => {
        setResult(prev => prev + char);
        updateProgress(advance);
      },
      onComplete: () => {
        setProcessing(false);
        updateProgress(100);
        cancelFnRef.current = null;
        requestIdRef.current = null;
      },
      onError: (errorMsg) => {
        setError(errorMsg);
        setProcessing(false);
        resetProgress();
        cancelFnRef.current = null;
        requestIdRef.current = null;
      }
    });
  };

  const handleCancel = (e?: React.MouseEvent) => {
    e?.preventDefault();
    e?.stopPropagation();
    
    if (cancelFnRef.current) {
      cancelFnRef.current();
      cancelFnRef.current = null;
    }
    
    setProcessing(false);
    resetProgress();
    setError('Process cancelled by user');
    requestIdRef.current = null;
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <div className="bg-white border-b">
        <div className="max-w-4xl mx-auto px-4 py-3">
          <h1 className="text-xl font-bold">Text Processing</h1>
        </div>
      </div>

      <div className="max-w-4xl mx-auto px-4 py-6">
        <div>
          <h2 className="text-lg font-bold mb-4">
            Job Processor
          </h2>

          <form onSubmit={handleProcess} className="flex flex-col gap-4">
            <div>
              <Label htmlFor="inputText">Input Text</Label>
              <Textarea
                id="inputText"
                value={text}
                onChange={(e) => setText(e.target.value)}
                rows={4}
                placeholder="Enter text to process..."
                disabled={processing}
                required
                className="border border-gray-300"
              />
            </div>

            <div className="flex gap-2">
              {!processing ? (
                <Button
                  type="submit"
                  className="w-full bg-blue-600 text-white py-3"
                >
                  Process
                </Button>
              ) : (
                <Button
                  type="button"
                  onClick={handleCancel}
                  color="failure"
                  className="w-full bg-red-600 text-white py-3"
                >
                  Cancel
                </Button>
              )}
            </div>
          </form>

          {processing && (
            <div className="mt-4 border border-yellow-500 p-4">
              <div className="flex justify-between items-center mb-2">
                <Label>Processing...</Label>
                <span className="text-sm font-semibold text-blue-600">{progress}%</span>
              </div>
              <ProgressBar value={progress} />
            </div>
          )}

          {error && (
            <Alert color="failure" className="mt-4">
              <span className="font-bold">Error:</span> {error}
            </Alert>
          )}

          {(result || processing) && (
            <div className="mt-4">
              <Label htmlFor="resultText">Result:</Label>
              <Textarea
                id="resultText"
                value={result}
                readOnly
                rows={6}
                placeholder="Processed result will appear here..."
                className="border border-gray-300"
              />
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TextCapture;
