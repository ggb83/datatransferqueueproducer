namespace DataTransferService.Producers;

public interface IProducer
{
    Task Produce(CancellationToken cancellationToken);
    
}