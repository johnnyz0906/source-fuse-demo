using AutoFixture;
using JohnnyDemo.Model.Requests;
using JohnnyDemo.Repository;
using JohnnyDemo.Repository.Context;
using JohnnyDemo.Repository.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace JohnnyDemo.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class CustomerRepositoryTest
    {
        private readonly Mock<ILogger<CustomerRepository>> _loggerMock;
        private readonly Mock<JohnnyDemoContext> _contextMock;
        private readonly Mock<DbSet<Customer>> _customerDbSetMock;
        private readonly ICustomerRepository _repository;
        private readonly IFixture _fixture;
        public CustomerRepositoryTest()
        {
            _loggerMock = new Mock<ILogger<CustomerRepository>>();
            _contextMock = new Mock<JohnnyDemoContext>();
            _customerDbSetMock = new Mock<DbSet<Customer>>();
            _repository = new CustomerRepository(_loggerMock.Object, _contextMock.Object);
            _fixture = new Fixture() { RepeatCount = 0 };

            _contextMock.Setup(x => x.Customers).Returns(_customerDbSetMock.Object);
        }
        [Fact]
        public async Task GetCustomer()
        {
            var customerId = _fixture.Create<int>();
            await _repository.GetAsync(customerId);

            _customerDbSetMock.Verify(x => x.FindAsync(customerId, CancellationToken.None), Times.Once());
        }
        [Fact]
        public async Task CreateCustomer_Success()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            await _repository.CreateAsync(request);

            _customerDbSetMock.Verify(x => x.Add(It.Is<Customer>(x => x.FirstName == request.FirstName
                && x.LastName == request.LastName
                && x.PhoneNumber == request.PhoneNumber
                && x.Email == request.Email
                && x.Address == request.Address)), Times.Once());

            _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
        }
        [Fact]
        public async Task UpdateCustomer_Success()
        {
            var request = _fixture.Create<CustomerUpdateRequest>();

            _customerDbSetMock.Setup(x => x.FindAsync(request.Id, CancellationToken.None))
                .ReturnsAsync(_fixture.Create<Customer>());

            await _repository.UpdateAsync(request.Id, request);

            _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
        }
        [Fact]
        public async Task UpdateCustomer_NotExist()
        {
            var request = _fixture.Create<CustomerUpdateRequest>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.UpdateAsync(request.Id, request));

            _customerDbSetMock.Verify(x => x.FindAsync(request.Id, CancellationToken.None), Times.Once());
            _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never());
        }
        [Fact]
        public async Task UpdateCustomer_ConflictId()
        {
            var request = _fixture.Create<CustomerUpdateRequest>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.UpdateAsync(_fixture.Create<int>(), request));

            _customerDbSetMock.Verify(x => x.FindAsync(request.Id, CancellationToken.None), Times.Never());
            _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never());
        }
    }
}