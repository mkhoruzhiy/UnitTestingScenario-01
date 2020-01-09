﻿using Rwt.Persistence.Abstractions;
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

        public void UpdateOrCreate(Person person)
        {
            person.Id = Guid.NewGuid();
            person.Status = PersonStatusEnum.Updated;
        }
    }
}
