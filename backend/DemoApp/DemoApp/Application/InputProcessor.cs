using DemoApp.Application.Entities;
using DemoApp.Domain;
using System.Collections.Concurrent;
using System.Text;

namespace DemoApp.Application;

public class InputProcessor : IInputProcessor
{
    private readonly ConcurrentDictionary<string, ProcessingContext> tasks = new();

    public bool CancelProcessing(string requestId)
    {
        if (this.tasks.TryGetValue(requestId, out var context))
        {
            context.CancellationTokenSource.Cancel();
            return true;
        }
        return false;
    }

    public CancellationToken GetToken(string requestId)
    {
        if (this.tasks.TryGetValue(requestId, out var context))
        {
            return context.CancellationTokenSource.Token;
        }
        return CancellationToken.None;
    }

    public CancellationToken StartProcessingInput(string requestId, string input)
    {
        var queue = GetQueue(input);
        var context = new ProcessingContext(new CancellationTokenSource(), queue, queue.Count);
        this.tasks[requestId] = context;
        return context.CancellationTokenSource.Token;
    }

    public (char nextChar, int percentageCompleted) GetNextProcessedData(string requestId)
    {
        if (this.tasks.TryGetValue(requestId, out var context)
                && context.characters.TryDequeue(out var c))
        {
            var percentageComplete = (int)(((context.inputLength - context.characters.Count) / (double)context.inputLength) * 100);
            return (nextChar: c, percentageComplete);
        }
        return ((char)0, 0);
    }

    private ConcurrentQueue<char> GetQueue(string input)
    {
        var chars = input
            .GroupBy(c => c)
            .Select(c => $"{c.Key}{c.Count()}")
            .OrderBy(c => c[0]);
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        return new ConcurrentQueue<char>($"{string.Join("", chars)}/{base64}");
    }
}
