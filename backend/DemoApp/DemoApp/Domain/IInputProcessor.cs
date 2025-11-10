namespace DemoApp.Domain;

public interface IInputProcessor
{
    CancellationToken StartProcessingInput(string inputrequestId, string input);
    bool CancelProcessing(string requestId);
    CancellationToken GetToken (string requestId);
    public (char nextChar, int percentageCompleted) GetNextProcessedData(string requestId);

}
