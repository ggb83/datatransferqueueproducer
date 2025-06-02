namespace DataTransferService.Producers;

public class ObjectProducerReaderSettings
{
    public int QueueSize { get; set; } = 30000;
    public int ProducerDelay { get; set; } = 500;
    public int WaitForItemsInQueue { get; set; } = 100;

}