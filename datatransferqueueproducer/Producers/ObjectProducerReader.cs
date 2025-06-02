using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace DataTransferService.Producers;

public interface IProducer<T> : IProducer
{
    Task Produce(IEnumerable<T> item, CancellationToken cancellationToken);
}

public class ObjectProducerReader<T> : IProducerDataReader where T : class
{
    private readonly IOptions<ObjectProducerReaderSettings> _options;
    private ObjectProducerReaderSettings settings => _options.Value;
    //readonly IEnumerator<T> _sourceenumerator;
    //readonly ConcurrentQueue<T> _queue=new();
    ProducerQueue<T> producer;
    
    public bool DoneProducing { get; private set; } = false;
    private readonly PropertyInfo[] properies;

    private T? Current => producer.GetCurrent();
    
    public ObjectProducerReader(IEnumerable<T> dataSource, IOptions<ObjectProducerReaderSettings> options)
    {
        _options = options;
        //_sourceenumerator= dataSource.GetEnumerator();
        properies=typeof(T).GetProperties();
        producer = new ProducerQueue<T>(dataSource, settings.QueueSize,settings.ProducerDelay,settings.WaitForItemsInQueue);
    }
    
    public void Dispose()
    {
        //_sourceenumerator.Dispose();
        producer.Dispose();
        //_queue.dispos
    }
    
    
    public Type GetFieldType(int i)
    {
        return properies[i].PropertyType;
    }
    public string GetDataTypeName(int i)
    {
        return properies[i].PropertyType.Name;
        //throw new NotImplementedException();
    }
#region No_Implementation_Needed_For_SqlBulkCopy

    public bool GetBoolean(int i)
    {
        throw new NotImplementedException();
    }

    public byte GetByte(int i)
    {
        throw new NotImplementedException();
    }

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
    {
        throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
        throw new NotImplementedException();
    }

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
    {
        throw new NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
        throw new NotImplementedException();
    }



    public DateTime GetDateTime(int i)
    {
        throw new NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
        throw new NotImplementedException();
    }

    public double GetDouble(int i)
    {
        throw new NotImplementedException();
    }


    public float GetFloat(int i)
    {
        throw new NotImplementedException();
    }

    public Guid GetGuid(int i)
    {
        throw new NotImplementedException();
    }

    public short GetInt16(int i)
    {
        throw new NotImplementedException();
    }

    public int GetInt32(int i)
    {
        throw new NotImplementedException();
    }

    public long GetInt64(int i)
    {
        throw new NotImplementedException();
    }
    public int GetValues(object[] values)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    
    public string GetName(int i)
    {
        return properies[i].Name;
    }

    public int GetOrdinal(string name)
    {
        for (int i = 0; i < properies.Length; i++)
        {
            if (string.Equals(properies[i].Name, name, StringComparison.CurrentCultureIgnoreCase)) return i;
        }
        return -1;
    }

    public string GetString(int i)
    {
        if(Current==null) return null;
        return properies[i].GetValue(Current)?.ToString();
    }

    public object GetValue(int i)
    {
        if(Current==null) return null;  
        return properies[i].GetValue(Current); 
    }


    public bool IsDBNull(int i)
    {
        if(Current==null) return true;
        if(GetValue(i)==null) return true;
        //if()
        return false;

    }

    public int FieldCount => properies.Length;

    public object this[int i] => GetValue(i);

    public object this[string name] => GetValue(GetOrdinal(name));

    public void Close()
    {
        Dispose();
    }

    public DataTable? GetSchemaTable()
    {
        throw new NotImplementedException();
    }

    public bool NextResult()
    {
        throw new NotImplementedException();
    }

    public bool Read()
    {
        if (producer.MoveNext())
        {
            RecordsAffected++;
            return true;
        }

        return false;
    }

    public int Depth => 1;
    public bool IsClosed => producer.DoneProducing;
    public int RecordsAffected { get; private set; }
    
    public async Task Produce(CancellationToken cancellationToken)
    {
        await producer.Produce(cancellationToken);
    }
}