using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Models;
using Rwt.Abstractions.Services;
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
        private const string MSQ_QUEUE_NAME = Constants.PersonUpdateQueueName;

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
                var model = _facade.GetPerson(personId);
                var entity = model.ToEntity();
                var id = _repo.UpdateOrCreate(entity);

                var msqId = _messageQueue.Put(MSQ_QUEUE_NAME, new PersonUpdateMessage { PersonId = id });

                _repo.SetPersonStatus(id, PersonStatusEnum.PopulatedForChanges);

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
