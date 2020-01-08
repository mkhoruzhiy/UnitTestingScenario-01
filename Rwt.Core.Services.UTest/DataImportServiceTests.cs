using AutoFixture;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rwt.Abstractions.Facades;
using Rwt.Abstractions.Models;
using Rwt.Abstractions.Services;
using Rwt.Core.Services.Exceptions;
using Rwt.Core.Services.ValueObjects;
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
        public void ImportPerson_Ok_ReturnsPersonId()
        {
            // arrange
            var personModel = _fixture.Create<PersonModel>();
            var personId = _fixture.Create<Guid>();

            this._registryHttpFacadeMock
                .Setup(s => s.GetPerson(personModel.Ssn))
                .Returns(personModel);

            this._personRepositoryMock
                .Setup(s => s.UpdateOrCreate(It.Is<Person>(p => p.Ssn == personModel.Ssn && p.FirstName == personModel.FirstName && p.LastName == personModel.LastName)))
                .Returns(personId);

            // act
            var result = _sut.ImportPerson(personModel.Ssn);
            
            // assert
            Assert.AreEqual(result, personId);

            this._messageQueueServiceMock
                .Verify(s => s.Put(RwtConstants.PersonUpdateQueueName, It.Is<PersonUpdateMessage>(p => p.PersonId == personId)), Times.Once);

            this._messageQueueServiceMock
                .Verify(s => s.Put(It.IsAny<string>(), It.IsAny<PersonUpdateMessage>()), Times.Once);

            this._personRepositoryMock
                .Verify(s => s.SetPersonStatus(personId, PersonStatusEnum.PopulatedForChanges), Times.Once);
        }

        [TestMethod]
        public void ImportPerson_GetPerson_ThrowsException()
        {
            // arrange
            var personModel = _fixture.Create<PersonModel>();
            var errMessage = _fixture.Create<string>();

            this._registryHttpFacadeMock.Setup(s => s.GetPerson(personModel.Ssn))
                .Throws(new Exception(errMessage));

            // act
            var exception = Assert.ThrowsException<ImportException>(
                () => _sut.ImportPerson(personModel.Ssn));
            
            // assert
            Assert.AreEqual(exception.Message, errMessage);

            this._personRepositoryMock
                .Verify(s => s.UpdateOrCreate(It.IsAny<Person>()), Times.Never);

            this._messageQueueServiceMock
                .Verify(s => s.Put(ValueObjects.RwtConstants.PersonUpdateQueueName, It.IsAny<PersonUpdateMessage>()), Times.Never);

            this._personRepositoryMock
                .Verify(s => s.SetPersonStatus(It.IsAny<Guid>(), It.IsAny<PersonStatusEnum>()), Times.Never);
        }

        [TestMethod]
        public void ImportPerson_UpdateOrCreate_ThrowsException()
        {
            // arrange
            var personModel = _fixture.Create<PersonModel>();
            var personId = _fixture.Create<Guid>();
            var errMessage = _fixture.Create<string>();

            this._registryHttpFacadeMock
                .Setup(s => s.GetPerson(personModel.Ssn))
                .Returns(personModel);

            this._personRepositoryMock
                .Setup(s => s.UpdateOrCreate(It.Is<Person>(p => p.Ssn == personModel.Ssn)))
                .Throws(new Exception(errMessage));

            // act
            var exception = Assert.ThrowsException<ImportException>(() => _sut.ImportPerson(personModel.Ssn));
            
            // assert
            Assert.AreEqual(exception.Message, errMessage);

            this._personRepositoryMock
                .Verify(s => s.UpdateOrCreate(It.IsAny<Person>()), Times.Once);

            this._messageQueueServiceMock
                .Verify(s => s.Put(ValueObjects.RwtConstants.PersonUpdateQueueName, It.IsAny<PersonUpdateMessage>()), Times.Never);

            this._personRepositoryMock
                .Verify(s => s.SetPersonStatus(It.IsAny<Guid>(), It.IsAny<PersonStatusEnum>()), Times.Never);
        }

        [TestMethod]
        public void ImportPerson_MessagePut_ThrowsException()
        {
            // arrange
            var personModel = _fixture.Create<PersonModel>();
            var personId = _fixture.Create<Guid>();
            var errMessage = _fixture.Create<string>();

            this._registryHttpFacadeMock
                .Setup(s => s.GetPerson(personModel.Ssn))
                .Returns(personModel);

            this._personRepositoryMock
                .Setup(s => s.UpdateOrCreate(It.Is<Person>(p => p.Ssn == personModel.Ssn)))
                .Returns(personId);

            this._messageQueueServiceMock
                .Setup(s => s.Put(RwtConstants.PersonUpdateQueueName, It.Is<PersonUpdateMessage>(p => p.PersonId == personId)))
                .Throws(new Exception(errMessage));

            // act
            var exception = Assert.ThrowsException<ImportException>(() => _sut.ImportPerson(personModel.Ssn));
            
            // assert
            Assert.AreEqual(exception.Message, errMessage);

            this._personRepositoryMock
                .Verify(s => s.UpdateOrCreate(It.IsAny<Person>()), Times.Once);

            this._messageQueueServiceMock
                .Verify(s => s.Put(ValueObjects.RwtConstants.PersonUpdateQueueName, It.IsAny<PersonUpdateMessage>()), Times.Once);

            this._personRepositoryMock
                .Verify(s => s.SetPersonStatus(It.IsAny<Guid>(), It.IsAny<PersonStatusEnum>()), Times.Never);
        }

        [TestMethod]
        public void ImportPerson_SetPersonStatus_ThrowsException()
        {
            // arrange
            var personModel = _fixture.Create<PersonModel>();
            var personId = _fixture.Create<Guid>();
            var errMessage = _fixture.Create<string>();

            this._registryHttpFacadeMock
                .Setup(s => s.GetPerson(personModel.Ssn))
                .Returns(personModel);

            this._personRepositoryMock
                .Setup(s => s.UpdateOrCreate(It.Is<Person>(p => p.Ssn == personModel.Ssn)))
                .Returns(personId);

            this._personRepositoryMock
                .Setup(s => s.SetPersonStatus(personId, PersonStatusEnum.PopulatedForChanges))
                .Throws(new Exception(errMessage));

            // act
            var exception = Assert.ThrowsException<ImportException>(() => _sut.ImportPerson(personModel.Ssn));
            
            // assert
            Assert.AreEqual(exception.Message, errMessage);

            this._personRepositoryMock
                .Verify(s => s.UpdateOrCreate(It.IsAny<Person>()), Times.Once);

            this._messageQueueServiceMock
                .Verify(s => s.Put(RwtConstants.PersonUpdateQueueName, It.IsAny<PersonUpdateMessage>()), Times.Once);

            this._messageQueueServiceMock
                .Verify(s => s.Put(It.IsAny<string>(), It.IsAny<PersonUpdateMessage>()), Times.Once);

            this._personRepositoryMock
                .Verify(s => s.SetPersonStatus(It.IsAny<Guid>(), It.IsAny<PersonStatusEnum>()), Times.Once);
        }
    }
}