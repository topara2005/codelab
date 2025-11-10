import { API_URL, API_ENDPOINT } from '../constants';
import type { SSECallbacks } from '../types';

export const processTextStream = (
  text: string,
   requestId: string,
  callbacks: SSECallbacks,
 
): (() => void) => {
  // Generate a unique requestId if not provided
  const id = requestId || crypto.randomUUID();
  const url = `${API_URL}${API_ENDPOINT}/${id}?input=${encodeURIComponent(text)}`;

  // Using native EventSource - Basic Auth is handled automatically by the browser
  const eventSource = new EventSource(url);

  eventSource.onmessage = (event) => {
    try {
      const data = event.data.trim();

      // Check for completion signals
      if (data === 'done' || data === 'cancelled') {
        callbacks.onComplete();
        eventSource.close();
        return;
      }

      // Check for error
      if (data.startsWith('error -')) {
        callbacks.onError(data.substring(8));
        eventSource.close();
        return;
      }

      // Parse character and advance: "X, 50" format
      const parts = data.split(',').map((s: string) => s.trim());
      if (parts.length === 2) {
        const character = parts[0];
        const advance = parseInt(parts[1], 10);
        callbacks.onCharacter(character, advance);
      }
    } catch (error) {
      callbacks.onError(error instanceof Error ? error.message : 'Failed to parse message');
    }
  };

  eventSource.onerror = () => {
    callbacks.onError('Connection error');
    eventSource.close();
  };

  return () => {
    console.log(`Closing EventSource for request ${id}`);
    eventSource.close();
  };
};
