using Rwt.Persistence.Abstractions;
using Rwt.Persistence.Entities;
using Rwt.Persistence.ValueObjects;
using System;

namespace Rwt.Persistence.Repo
{
    public class PersonRepository : IPersonRepository
    {
        public void SetPersonStatus(Guid personId, PersonStatusEnum status)
        {
        }

        public Guid UpdateOrCreate(Person person)
        {
            person.Id = Guid.NewGuid();
            return person.Id;
        }
    }
}
