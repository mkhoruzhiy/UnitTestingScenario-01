using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Models;
using Rwt.Abstractions.Services;
using Rwt.Core.Services.Mappers;
using Rwt.Persistence.Abstractions;
using Rwt.Persistence.ValueObjects;
using System;
using System.Linq;

namespace Rwt.Core.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly IRegistryHttpFacade _facade;
        private readonly IPersonRepository _repo;
        private readonly IMessageQueueService _messageQueue;
        private const string MSQ_QUEUE_NAME = "PersonUpdate";

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
                _repo.UpdateOrCreate(entity);

                if (new[] { PersonStatusEnum.New, PersonStatusEnum.Updated }.Contains(entity.Status))
                {
                    // sending a Queue service message that a given person imported/updated
                    var msqId = _messageQueue.Put(MSQ_QUEUE_NAME, new PersonUpdateMessage { PersonId = entity.Id });

                    // updating status of the imported person in the local DB
                    _repo.SetPersonStatus(entity.Id, PersonStatusEnum.Published);
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
