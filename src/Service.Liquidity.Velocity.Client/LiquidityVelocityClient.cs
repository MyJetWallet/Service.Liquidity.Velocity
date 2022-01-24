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
public class LiquidityVelocityClient : ILiquidityVelocityClient, IStartable
{
    private readonly IMyNoSqlServerDataReader<VelocityNoSql> _readerAssets;

    public LiquidityVelocityClient(IMyNoSqlServerDataReader<VelocityNoSql> readerAssets)
    {
        _readerAssets = readerAssets;
        
        _readerAssets.SubscribeToUpdateEvents(list => Changed(), list => Changed());
    }
    
    public event Action OnChanged;
    
    public VelocityNoSql GetVelocityByAsset(string brokerId, string asset)
    {
        var entity = _readerAssets.Get(VelocityNoSql.GeneratePartitionKey(brokerId), VelocityNoSql.GenerateRowKey(asset));
        return entity;
    }

    public IReadOnlyList<VelocityNoSql> GetAllAssets()
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