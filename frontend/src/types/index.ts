


export interface SSECallbacks {
  onCharacter: (char: string, advance: number) => void;
  onComplete: () => void;
  onError: (error: string) => void;
}
