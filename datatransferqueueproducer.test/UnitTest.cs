using System.IO.Compression;
using System.Text;
using DataTransferService;
using DataTransferService.Producers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace DataTransferTest;

public class Tests
{
#pragma warning disable NUnit1032
    IServiceProvider serviceProvider;
#pragma warning restore NUnit1032
    [SetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        var coll = new ServiceCollection();
        //coll.AddDataTransferService().AddSingleton<IConfiguration>(config).AddLogging();
        serviceProvider=coll.BuildServiceProvider();
        

    }



    

    [Test]
    public async Task ExecuteSimple()
    {
        var sqlConnectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("mssql");
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        await sqlConnection.OpenAsync();

        
        
        using var service = new SqlBulkCopy(sqlConnection);
        service.DestinationTableName = "inserttest";
        service.ColumnMappings.Add(nameof(Test.Artikelnummer),"anr");
        service.ColumnMappings.Add(nameof(Test.Hersteller),"hnr");
        service.ColumnMappings.Add(nameof(Test.Text),"text");
        
        using var rdr = new ObjectProducerReader<Test>(
            
            GetItems.GetMockData(2000000),
            Options.Create(new ObjectProducerReaderSettings())
        );
        await service.WriteToServerAsync(rdr);

    }
    

    [TearDown]
    public void Dispose()
    {
        serviceProvider = null;
    }
}