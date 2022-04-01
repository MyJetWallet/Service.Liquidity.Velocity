using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Autofac;
using JetBrains.Annotations;
using MyNoSqlServer.Abstractions;
using Service.Liquidity.Velocity.Domain.Models.NoSql;

namespace Service.Liquidity.Velocity.Client;


[UsedImplicitly]
public class MarkupVelocityClient : IMarkupVelocityClient, IStartable
{
    private readonly IMyNoSqlServerDataReader<MarkupVelocityNoSql> _readerAssets;

    public MarkupVelocityClient(IMyNoSqlServerDataReader<MarkupVelocityNoSql> readerAssets)
    {
        _readerAssets = readerAssets;
        
        _readerAssets.SubscribeToUpdateEvents(list => Changed(), list => Changed());
    }
    
    public event Action OnChanged;
    
    public MarkupVelocityNoSql GetVelocityByAsset(string brokerId, string asset)
    {
        var entity = _readerAssets.Get(MarkupVelocityNoSql.GeneratePartitionKey(brokerId), VelocityNoSql.GenerateRowKey(asset));
        return entity;
    }

    public IReadOnlyList<MarkupVelocityNoSql> GetAllAssets()
    {
        var list = _readerAssets.Get();
        return list;
    }
    
    public void Start()
    {
        var sw = new Stopwatch();
        sw.Start();
        var iteration = 0;
        while (iteration < 100)
        {
            iteration++;
            if (GetAllAssets().Count > 0)
                break;

            Thread.Sleep(100);
        }
        sw.Stop();
        Console.WriteLine($"AssetNoSqlEntity client is started. Wait time: {sw.ElapsedMilliseconds} ms. Counts: {GetAllAssets().Count}");
    }

    private void Changed()
    {
        OnChanged?.Invoke();
    }
}