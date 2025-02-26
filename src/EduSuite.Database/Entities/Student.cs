using System;

namespace EduSuite.Database.Entities;

public class Student : BaseEntity
{
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public string CurrentClass { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
} 