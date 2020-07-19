using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonablePeople.API.Models.Entities
{
    public class LeadEntity : RecordEntity
    {
        public bool Converted { get; set; }
        public AddressEntity Address { get; set; }
        public NameEntity Name { get; set; }
        public double? PredictionScore { get; set; }
        public ContactInfoEntity ContactInfo { get; set; }
    }

    public class ContactEntity : RecordEntity
    {
        public AddressEntity MailingAddress { get; set; }
        public AddressEntity OtherAddress { get; set; }
        public NameEntity PrimaryName { get; set; }
        public NameEntity SecondaryName { get; set; }
        public ContactInfoEntity PrimaryContactInfo { get; set; }
        public ContactInfoEntity SecondaryContactInfo { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? AssociatedLeadId { get; set; }
    }

    public class RecordEntity
    {
        [BsonId, BsonRepresentation(BsonType.String)]
        public Guid RecordId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset CreatedTime { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CreatedByUserId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ModifiedBy { get; set; }


        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset LastModifiedTime { get; set; }

        public decimal? AnnualSalary { get; set; }
        public string LeadSource { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }

    public class ContactInfoEntity
    {
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Home { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ContactTypes PreferredContactMethod { get; set; }
    }

    public enum ContactTypes
    {
        Mobile,
        Fax,
        Email,
        Phone,
        Mail
    }

    public class NameEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Salutation { get; set; }
        public string PreferredFirstName { get; set; }
    }

    public class AddressEntity
    {
        public string StreetLineOne { get; set; }
        public string StreetLineTwo { get; set; }
        public string AptSuite { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }
    }

    public class UserEntity
    {
        [BsonId, BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public NameEntity Name { get; set; }
        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; }

        [BsonRepresentation(BsonType.String)]
        public UserStatus Status { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? ReportingTo { get; set; }
    }

    public enum UserStatus
    {
        active,
        suspended,
        inactive,
        training
    }

    public enum UserRoles
    {
        Admin,
        Developer,
        IT,
        Support,
        Teller,
        Manager,
        Accountant,
        Driver,
        Counter,
        FinancialAdvisor,
        AccountManager,
        LoanOfficer,
        InvestmentAdvisor,
        Enforcement,
        Legal,
        HR
    }
}