﻿using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelsRepository : IBaseStrongEntityRepository<Channel, int>
{
    Task<IEnumerable<Channel>> GetAllForUserAsync(int userId, CancellationToken cancellationToken);
    Task<Channel?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}