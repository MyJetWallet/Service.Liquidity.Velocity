using System;
using System.Collections.Generic;
using Service.Liquidity.Velocity.Domain.Models.NoSql;

namespace Service.Liquidity.Velocity.Client;

public interface IMarkupVelocitySettingsClient
{
    MarkupVelocitySettingsNoSql GetVelocityByAsset(string brokerId, string asset);
    IReadOnlyList<MarkupVelocitySettingsNoSql> GetAllAssets();
}