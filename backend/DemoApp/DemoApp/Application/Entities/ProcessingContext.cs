using System.Collections.Concurrent;

namespace DemoApp.Application.Entities;

public record ProcessingContext(CancellationTokenSource CancellationTokenSource, ConcurrentQueue<char> characters, int inputLength);

