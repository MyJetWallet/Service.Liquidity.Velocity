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
public class MarkupVelocitySettingsClient : IMarkupVelocitySettingsClient, IStartable
{
    private readonly IMyNoSqlServerDataReader<MarkupVelocitySettingsNoSql> _readerAssets;

    public MarkupVelocitySettingsClient(IMyNoSqlServerDataReader<MarkupVelocitySettingsNoSql> readerAssets)
    {
        _readerAssets = readerAssets;
        
        _readerAssets.SubscribeToUpdateEvents(list => Changed(), list => Changed());
    }
    
    public event Action OnChanged;
    
    public MarkupVelocitySettingsNoSql GetVelocityByAsset(string brokerId, string asset)
    {
        var entity = _readerAssets
            .Get(MarkupVelocitySettingsNoSql.GeneratePartitionKey(brokerId), MarkupVelocitySettingsNoSql.GenerateRowKey(asset));
        return entity;
    }

    public IReadOnlyList<MarkupVelocitySettingsNoSql> GetAllAssets()
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