using Rwt.Abstractions.Models;

namespace Rwt.Abstractions.Facades
{
    public interface IRegistryHttpFacade
    {
        /// <summary>
        /// Returns an instance of PersonModel
        /// </summary>
        /// <param name="ssn">Social security number (fødselsnummer)</param>
        /// <returns></returns>
        PersonModel GetPerson(string ssn);
    }
}
