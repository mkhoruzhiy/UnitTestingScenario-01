using Rwt.Abstractions.Models;
using Rwt.Persistence.Entities;
using Rwt.Persistence.ValueObjects;

namespace Rwt.Core.Services.Mappers
{
    public static class PersonModelMapper
    {
        public static Person ToEntity(this PersonModel @this, PersonStatusEnum status)
        {
            return new Person
            { 
                Ssn = @this.Ssn,
                FirstName = @this.FirstName,
                LastName = @this.LastName,
                Status = status,
            };
        }
    }
}
