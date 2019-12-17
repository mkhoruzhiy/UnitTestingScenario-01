using Rwt.Persistence.ValueObjects;
using System;

namespace Rwt.Persistence.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Ssn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PersonStatusEnum Status { get; set; }
    }
}
