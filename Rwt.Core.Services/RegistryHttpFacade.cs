using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Models;

namespace Rwt.Core.Services
{
    public class RegistryHttpFacade : IRegistryHttpFacade
    {
        /// <summary>
        /// Returns an instance of PersonModel
        /// </summary>
        /// <param name="ssn">Social security number (fødselsnummer)</param>
        /// <returns></returns>
        public PersonModel GetPerson(string ssn)
        {
            return new PersonModel
            {
                Ssn = ssn,
                FirstName = "Ola",
                LastName = "Nordmann",
            };
        }
    }
}
