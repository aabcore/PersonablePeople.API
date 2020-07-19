using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonablePeople.API.Models.Entities;

namespace PersonablePeople.API.Models.ApiDtos
{
    public class UserOutDto
    {
        public Guid UserId { get; set; }

        public NameEntity Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserRoles> Roles { get; set; }
        public UserStatus Status { get; set; }

        public Guid? ReportingTo { get; set; }

        public static UserOutDto EntityToOutDto(UserEntity userEntity)
        {
            return new UserOutDto()
            {
                Name = userEntity.Name,
                Email = userEntity.Email,
                UserId = userEntity.UserId,
                ReportingTo = userEntity.ReportingTo,
                Status = userEntity.Status,
                Roles = userEntity.Roles.Select(x =>
                {
                    Enum.TryParse(x, true, out UserRoles ret);
                    return ret;
                })
            };
        }
    }
    public class ContactOutDto : RecordOutDto
    {
        public AddressDtoOut MailingAddress { get; set; }
        public AddressDtoOut OtherAddress { get; set; }
        public NameDtoOut PrimaryName { get; set; }
        public NameDtoOut SecondaryName { get; set; }
        public Guid? AssociatedLeadId { get; set; }
        public ContactInfoDtoOut PrimaryContactInfo { get; set; }
        public ContactInfoDtoOut SecondaryContactInfo { get; set; }
        public static ContactOutDto EntityToOutDto(ContactEntity contactEntity)
        {
            var contactOut = RecordOutDto.EntityToOutDto<ContactOutDto>(contactEntity);
            contactOut.MailingAddress = AddressDtoOut.EntityToOutDto(contactEntity.MailingAddress);
            contactOut.OtherAddress = AddressDtoOut.EntityToOutDto(contactEntity.OtherAddress);
            contactOut.PrimaryName = NameDtoOut.EntityToOutDto(contactEntity.PrimaryName);
            contactOut.SecondaryName = NameDtoOut.EntityToOutDto(contactEntity.SecondaryName);
            contactOut.AssociatedLeadId = contactEntity.AssociatedLeadId;
            contactOut.PrimaryContactInfo = ContactInfoDtoOut.EntityToOutDto(contactEntity.PrimaryContactInfo);
            contactOut.SecondaryContactInfo = ContactInfoDtoOut.EntityToOutDto(contactEntity.SecondaryContactInfo);
            return contactOut;
        }
    }

    public class ContactInfoDtoOut
    {
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Home { get; set; }
        public ContactTypes PreferredContactMethod { get; set; }

        public static ContactInfoDtoOut EntityToOutDto(ContactInfoEntity contactInfoEntity)
        {
            if (contactInfoEntity == null)
            {
                return null;
            }
            return new ContactInfoDtoOut
            {
                Mobile = contactInfoEntity.Mobile,
                Fax = contactInfoEntity.Fax,
                Email = contactInfoEntity.Email,
                Home = contactInfoEntity.Home,
                PreferredContactMethod = contactInfoEntity.PreferredContactMethod
            };
        }
    }

    public class LeadOutDto: RecordOutDto
    {
        public bool Converted { get; set; }
        public AddressDtoOut Address { get; set; }
        public NameDtoOut Name { get; set; }
        public double? PredictionScore { get; set; }
        public ContactInfoDtoOut ContactInfo { get; set; }

        public static LeadOutDto EntityToOutDto(LeadEntity leadEntity)
        {
            var leadOut = RecordOutDto.EntityToOutDto<LeadOutDto>(leadEntity);
            leadOut.Converted = leadEntity.Converted;
            leadOut.Address = AddressDtoOut.EntityToOutDto(leadEntity.Address);
            leadOut.Name = NameDtoOut.EntityToOutDto(leadEntity.Name);
            leadOut.PredictionScore = leadEntity.PredictionScore;
            leadOut.ContactInfo = ContactInfoDtoOut.EntityToOutDto(leadEntity.ContactInfo);
            return leadOut;
        }

    }

    public class AddressDtoOut
    {
        public string StreetLineOne { get; set; }
        public string StreetLineTwo { get; set; }
        public string AptSuite { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }

        public static AddressDtoOut EntityToOutDto(AddressEntity addressEntity)
        {
            if (addressEntity == null)
            {
                return null;
            }
            return new AddressDtoOut()
            {
                StreetLineOne = addressEntity.StreetLineOne,
                StreetLineTwo = addressEntity.StreetLineTwo,
                AptSuite = addressEntity.AptSuite,
                City = addressEntity.City,
                State = addressEntity.State,
                County = addressEntity.County,
                Zip = addressEntity.Zip
            };
        }
    }

    public class NameDtoOut
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Salutation { get; set; }
        public string PreferredFirstName { get; set; }

        public static NameDtoOut EntityToOutDto(NameEntity nameEntity)
        {
            if (nameEntity== null)
            {
                return null;
            }
            return new NameDtoOut
            {
                FirstName = nameEntity.FirstName,
                LastName = nameEntity.LastName,
                MiddleName = nameEntity.MiddleName,
                Salutation = nameEntity.Salutation,
                PreferredFirstName = nameEntity.PreferredFirstName
            };
        }
    }

    public class RecordOutDto
    {

        public Guid RecordId { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid ModifiedBy { get; set; }
        public DateTimeOffset LastModifiedTime { get; set; }

        public decimal? AnnualSalary { get; set; }
        public string LeadSource { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public static T EntityToOutDto<T>(RecordEntity recordEntity) where T: RecordOutDto, new()
        {
            return new T()
            {
                RecordId = recordEntity.RecordId,
                CreatedTime = recordEntity.CreatedTime,
                CreatedByUserId = recordEntity.CreatedByUserId,
                ModifiedBy = recordEntity.ModifiedBy,
                LastModifiedTime = recordEntity.LastModifiedTime,
                AnnualSalary = recordEntity.AnnualSalary,
                LeadSource = recordEntity.LeadSource,
                Tags = recordEntity.Tags.Select(x => x)
            };
        }
    }
}
