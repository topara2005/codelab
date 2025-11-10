import { useState } from 'react';

export const useProgressBar = () => {
  const [progress, setProgress] = useState(0);

  const updateProgress = (value: number) => {
    setProgress(Math.min(Math.max(value, 0), 100));
  };

  const resetProgress = () => {
    setProgress(0);
  };

//https://flowbite.com/docs/components/progress/
  const ProgressBar = ({ value }: { value: number }) => (
    <div className="w-full bg-gray-200 rounded-full h-4">
      <div
        className="bg-blue-600 h-4 rounded-full transition-all duration-500 ease-in-out"
        style={{ width: `${value}%` }}
      ></div>
    </div>
  );

  return {
    progress,
    updateProgress,
    resetProgress,
    ProgressBar
  };
};