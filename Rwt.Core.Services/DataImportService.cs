using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Models;
using Rwt.Abstractions.Services;
using Rwt.Core.Services.Exceptions;
using Rwt.Core.Services.Mappers;
using Rwt.Core.Services.ValueObjects;
using Rwt.Persistence.Abstractions;
using Rwt.Persistence.ValueObjects;
using System;

namespace Rwt.Core.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly IRegistryHttpFacade _facade;
        private readonly IPersonRepository _repo;
        private readonly IMessageQueueService _messageQueue;
        private const string MSQ_QUEUE_NAME = RwtConstants.PersonUpdateQueueName;

        public DataImportService(IRegistryHttpFacade facade, IPersonRepository repository, IMessageQueueService messageQueue)
        {
            this._facade = facade;
            this._repo = repository;
            this._messageQueue = messageQueue;
        }

        public Guid ImportPerson(string personId)
        {
            try
            {
                // getting person data from external data source
                var model = _facade.GetPerson(personId);

                // mapping received data to DB entity
                var entity = model.ToEntity(PersonStatusEnum.New);

                // persisting person data in database
                var id = _repo.UpdateOrCreate(entity);

                // sending a Queue service message that a given person imported/updated
                var msqId = _messageQueue.Put(MSQ_QUEUE_NAME, new PersonUpdateMessage { PersonId = id });

                // updating status of the imported person in the local DB
                _repo.SetPersonStatus(id, PersonStatusEnum.Published);

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new ImportException(ex.Message, ex);
            }
        }
    }
}
