using Rwt.Abstractions.Models;
using Rwt.Abstractions.Services;
using System;

namespace Rwt.Core.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        /// <summary>
        /// Sets a message into a queue
        /// </summary>
        /// <param name="queueName">name of the queue</param>
        /// <param name="message">Message for sending</param>
        /// <returns>Id of the message in the queue</returns>
        public Guid Put(string queueName, PersonUpdateMessage message)
        {
            return Guid.NewGuid();
        }
    }
}
