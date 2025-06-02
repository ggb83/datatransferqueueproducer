using System.Collections;
using System.Collections.Concurrent;

namespace DataTransferService.Producers;

public class ProducerQueue<T> :  IProducer<T>,IEnumerator<T>
{
    ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
    
    private T _current;
    public bool DoneProducing { get; private set; } = false;
    public int QueueSize { get; set; } 
    public int ProducerDelay { get; set; } 
    public int WaitForItemsInQueue { get; set; } 
    private IEnumerable<T> _source;
    private CancellationToken _cancellationToken;

    public ProducerQueue(int queueSize, int delay, int waitForItemsInQueue)
    {
        
        QueueSize = queueSize;
        ProducerDelay = delay;
        WaitForItemsInQueue = waitForItemsInQueue;
    }
    public ProducerQueue(IEnumerable<T> source,int queueSize, int delay, int waitForItemsInQueue):this(queueSize, delay, waitForItemsInQueue)
    {
        _source = source;
    }
    

    public Task Produce(IEnumerable<T> items, CancellationToken cancellationToken)
    {
        _source = items;
        return Produce(cancellationToken);
    }

    public async Task Produce(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        if(DoneProducing)
            throw new ArgumentException("DoneProducing ist erledigt! Neue instanz wird benötigt! Abbruch ....");
        bool done = false;
        using var _sourceenumerator = _source.GetEnumerator();
        while (!done)
        {
            if (cancellationToken.IsCancellationRequested) break;
            if (_queue.Count >= QueueSize)
            {
                await Task.Delay(ProducerDelay, cancellationToken);
                continue;
            }

            if (!_sourceenumerator.MoveNext())
            {
                done=true;
                continue;
            }
            _queue.Enqueue(_sourceenumerator.Current);
            
        }
        DoneProducing=true;
        
        await Task.CompletedTask;
    }

    public bool MoveNext()
    {
        if(_cancellationToken.IsCancellationRequested)return false;
        while ((!DoneProducing) || _queue.Any() )
        {
            if(_cancellationToken.IsCancellationRequested)return false; 
            if (!_queue.Any())
            {
                Thread.Sleep(100);
                continue;
            }
            
            if (_queue.TryDequeue(out var item))
            {
                _current = item;
                return true;
            }
            
            
        }
        return false;
    }

    public void Reset()
    {
        //throw new NotImplementedException();
    }

    public T GetCurrent()=> _current;
    T IEnumerator<T>.Current => _current;

    object? IEnumerator.Current => _current;

    public void Dispose()
    {
        _queue = [];
    }
}