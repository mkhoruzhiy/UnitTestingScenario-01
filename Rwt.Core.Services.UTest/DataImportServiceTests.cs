using AutoFixture;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Models;
using Rwt.Abstractions.Services;
using Rwt.Persistence.Abstractions;
using Rwt.Persistence.Entities;
using Rwt.Persistence.ValueObjects;
using System;

namespace Rwt.Core.Services.Tests
{
    [TestClass]
    public class DataImportServiceTests
    {
        private readonly DataImportService _sut;
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IRegistryHttpFacade> _registryHttpFacadeMock;
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<IMessageQueueService> _messageQueueServiceMock;

        public DataImportServiceTests()
        {
            this._registryHttpFacadeMock = new Mock<IRegistryHttpFacade>();
            this._personRepositoryMock = new Mock<IPersonRepository>();
            this._messageQueueServiceMock = new Mock<IMessageQueueService>();

            _sut = new DataImportService(
                this._registryHttpFacadeMock.Object,
                this._personRepositoryMock.Object,
                this._messageQueueServiceMock.Object);
        }

        [TestMethod]
        public void ImportPerson_Returns_PersonId()
        {
            // arrange
            var personModel = _fixture.Create<PersonModel>();
            var personId = _fixture.Create<Guid>();

            this._registryHttpFacadeMock.Setup(s => s.GetPerson(personModel.Ssn)).Returns(personModel);
            this._personRepositoryMock
                .Setup(s => s.UpdateOrCreate(It.Is<Person>(p => p.Ssn == personModel.Ssn && p.FirstName == personModel.FirstName && p.LastName == personModel.LastName)))
                .Returns(personId);

            // act
            var result = _sut.ImportPerson(personModel.Ssn);
            
            // assert
            Assert.AreEqual(result, personId);
            // _messageQueue.Put(MSQ_QUEUE_NAME, new PersonUpdateMessage { PersonId = id });
            this._messageQueueServiceMock
                .Verify(s => s.Put(ValueObjects.Constants.PersonUpdateQueueName, It.Is<PersonUpdateMessage>(p => p.PersonId == personId)), Times.Once);
            this._personRepositoryMock
                .Verify(s => s.SetPersonStatus(personId, PersonStatusEnum.PopulatedForChanges), Times.Once);
        }
    }
}