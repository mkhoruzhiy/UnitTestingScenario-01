using Rwt.Abstractions.ValueObjects;
using Rwt.Persistence.Entities;
using System;


namespace Rwt.Persistence.Abstractions
{
    public interface IPersonRepository
    {
        void UpdateOrCreate(Person person);

        void SetPersonStatus(Guid personId, PersonStatusEnum status);
    }
}
