using Rwt.Abstractions.ValueObjects;
using System;

namespace Rwt.Abstractions.Models
{
    public class PersonUpdateMessage
    {
        public Guid PersonId { get; set; }

        public PersonStatusEnum Status { get; set; }
    }
}
