using Rwt.Abstractions.Models;
using System;

namespace Rwt.Abstractions.Services
{
    public interface IMessageQueueService
    {
        Guid Put(string queueName, PersonUpdateMessage message);
    }
}
