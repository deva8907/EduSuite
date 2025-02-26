using System;
using System.Linq;
using System.Threading.Tasks;
using EduSuite.Database.Entities;
using EduSuite.Database.Repositories;
using Xunit;

namespace EduSuite.Database.Tests;

public class StudentRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly IRepository<Student> _repository;

    public StudentRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new Repository<Student>(_fixture.Context);
    }

    [Fact]
    public async Task AddStudent_ShouldSetTenantIdAndAuditFields()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            AdmissionNumber = "A001",
            DateOfBirth = new DateTime(2000, 1, 1),
            Gender = "Male",
            AdmissionDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(student);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(_fixture.TestTenantId, result.TenantId);
        Assert.Equal(_fixture.TestUserId, result.CreatedBy);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.False(result.IsDeleted);
    }

    [Fact]
    public async Task GetAllStudents_ShouldOnlyReturnCurrentTenantStudents()
    {
        // Arrange
        var student1 = new Student
        {
            FirstName = "Jane",
            LastName = "Smith",
            AdmissionNumber = "A002",
            DateOfBirth = new DateTime(2001, 1, 1),
            Gender = "Female",
            AdmissionDate = DateTime.UtcNow
        };

        var student2 = new Student
        {
            FirstName = "Bob",
            LastName = "Johnson",
            AdmissionNumber = "A003",
            DateOfBirth = new DateTime(2002, 1, 1),
            Gender = "Male",
            AdmissionDate = DateTime.UtcNow
        };

        await _repository.AddAsync(student1);
        await _repository.AddAsync(student2);

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.All(results, student => Assert.Equal(_fixture.TestTenantId, student.TenantId));
    }

    [Fact]
    public async Task DeleteStudent_ShouldSoftDelete()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "Alice",
            LastName = "Brown",
            AdmissionNumber = "A004",
            DateOfBirth = new DateTime(2003, 1, 1),
            Gender = "Female",
            AdmissionDate = DateTime.UtcNow
        };

        var added = await _repository.AddAsync(student);

        // Act
        await _repository.DeleteAsync(added.Id);

        // Assert
        var deleted = await _fixture.Context.Students.FindAsync(added.Id);
        Assert.True(deleted?.IsDeleted);
        Assert.Equal(_fixture.TestUserId, deleted?.DeletedBy);
        Assert.NotNull(deleted?.DeletedAt);

        var allStudents = await _repository.GetAllAsync();
        Assert.DoesNotContain(allStudents, s => s.Id == added.Id);
    }
} 