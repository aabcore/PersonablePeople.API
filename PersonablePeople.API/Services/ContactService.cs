using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using PersonablePeople.API.Controllers;
using PersonablePeople.API.Models;
using PersonablePeople.API.Models.ApiDtos;
using PersonablePeople.API.Models.Entities;
using PersonablePeople.API.Results;

namespace PersonablePeople.API.Services
{
    public class ContactService: DbService
    {
        private readonly IMongoCollection<ContactEntity> ContactCollection;

        public ContactService(DatabaseSettings dbSettings)
        {
            var database = BuildDatabaseClient(dbSettings);
            ContactCollection = database.GetCollection<ContactEntity>(dbSettings.ContactsCollectionName);
        }
        public async Task<TypedResult<IEnumerable<ContactOutDto>>> GetAllContacts()
        {
            try
            {
                var foundContacts = (await ContactCollection.FindAsync(x => true)).ToList();
                return new SuccessfulTypedResult<IEnumerable<ContactOutDto>>(foundContacts.Select(ContactOutDto.EntityToOutDto));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<IEnumerable<ContactOutDto>>(e);
            }
        }

        public async Task<TypedResult<IEnumerable<ContactOutDto>>> GetContacts(GetContactFilter getContactFilter)
        {
            try
            {
                var filters = new List<FilterDefinition<ContactEntity>>();
                if (getContactFilter?.CreatedTimeAfter != null)
                {
                    filters.Add(Builders<ContactEntity>.Filter.Gt(le => le.CreatedTime, getContactFilter.CreatedTimeAfter.Value));
                }

                if (getContactFilter?.CreatedTimeBefore != null)
                {
                    filters.Add(Builders<ContactEntity>.Filter.Lt(le => le.CreatedTime, getContactFilter.CreatedTimeBefore.Value));
                }

                if (getContactFilter?.ModifiedTimeAfter != null)
                {
                    filters.Add(Builders<ContactEntity>.Filter.Gt(le => le.LastModifiedTime, getContactFilter.ModifiedTimeAfter.Value));
                }

                if (getContactFilter?.ModifiedTimeBefore != null)
                {
                    filters.Add(Builders<ContactEntity>.Filter.Lt(le => le.LastModifiedTime, getContactFilter.ModifiedTimeBefore.Value));
                }

                if (getContactFilter?.Tags != null && getContactFilter.Tags.Any())
                {
                    var tagFilters = new List<FilterDefinition<ContactEntity>>();
                    foreach (var tag in getContactFilter.Tags)
                    {
                        tagFilters.Add(Builders<ContactEntity>.Filter.AnyEq(le => le.Tags, tag));
                    }

                    filters.Add(Builders<ContactEntity>.Filter.Or(tagFilters));
                }


                var foundLeads = filters.Any()
                    ? (await ContactCollection.FindAsync(Builders<ContactEntity>.Filter.And(filters))).ToList().Select(ContactOutDto.EntityToOutDto)
                    : (await ContactCollection.FindAsync(x => true)).ToList().Select(ContactOutDto.EntityToOutDto);

                return new SuccessfulTypedResult<IEnumerable<ContactOutDto>>(foundLeads);
            }
            catch (Exception e)
            {
                return new FailedTypedResult<IEnumerable<ContactOutDto>>(e);
            }
        }

        public async Task<TypedResult<ContactOutDto>> GetContact(Guid recordId)
        {
            try
            {
                var foundContactResult = (await ContactCollection.FindAsync(c => c.RecordId == recordId)).FirstOrDefault();
                if (foundContactResult == null)
                {
                    return new NotFoundTypedResult<ContactOutDto>();
                }

                return new SuccessfulTypedResult<ContactOutDto>(ContactOutDto.EntityToOutDto(foundContactResult));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<ContactOutDto>(e);
            }
        }

        public async Task<TypedResult<ContactOutDto>> UpdateContact(Guid recordId, UpdateContactDtoIn updateContactIn)
        {
            try
            {
                var foundContactResult = (await ContactCollection.FindAsync(c => c.RecordId == recordId)).FirstOrDefault();
                if (foundContactResult == null)
                {
                    return new NotFoundTypedResult<ContactOutDto>();
                }

                foundContactResult.MailingAddress = new AddressEntity
                {
                    StreetLineOne = updateContactIn.MailingAddress.StreetLineOne,
                    StreetLineTwo = updateContactIn.MailingAddress.StreetLineTwo,
                    AptSuite = updateContactIn.MailingAddress.AptSuite,
                    City = updateContactIn.MailingAddress.City,
                    State = updateContactIn.MailingAddress.State,
                    County = updateContactIn.MailingAddress.County,
                    Zip = updateContactIn.MailingAddress.Zip
                };
                foundContactResult.OtherAddress = new AddressEntity
                {
                    StreetLineOne = updateContactIn.OtherAddress.StreetLineOne,
                    StreetLineTwo = updateContactIn.OtherAddress.StreetLineTwo,
                    AptSuite = updateContactIn.OtherAddress.AptSuite,
                    City = updateContactIn.OtherAddress.City,
                    State = updateContactIn.OtherAddress.State,
                    County = updateContactIn.OtherAddress.County,
                    Zip = updateContactIn.OtherAddress.Zip
                };

                foundContactResult.PrimaryName = new NameEntity
                {
                    FirstName = updateContactIn.PrimaryName.FirstName,
                    LastName = updateContactIn.PrimaryName.LastName,
                    MiddleName = updateContactIn.PrimaryName.MiddleName,
                    Salutation = updateContactIn.PrimaryName.Salutation,
                    PreferredFirstName = updateContactIn.PrimaryName.PreferredFirstName
                };
                foundContactResult.SecondaryName = new NameEntity
                {
                    FirstName = updateContactIn.SecondaryName.FirstName,
                    LastName = updateContactIn.SecondaryName.LastName,
                    MiddleName = updateContactIn.SecondaryName.MiddleName,
                    Salutation = updateContactIn.SecondaryName.Salutation,
                    PreferredFirstName = updateContactIn.SecondaryName.PreferredFirstName
                };
                foundContactResult.ModifiedBy = updateContactIn.ModifiedBy;
                foundContactResult.LastModifiedTime = DateTimeOffset.UtcNow;
                foundContactResult.AnnualSalary = updateContactIn.AnnualSalary;
                foundContactResult.Tags = updateContactIn.Tags.Select(x => x);
                foundContactResult.PrimaryContactInfo = new ContactInfoEntity
                {
                    Mobile = updateContactIn.PrimaryContactInfo.Mobile,
                    Fax = updateContactIn.PrimaryContactInfo.Fax,
                    Email = updateContactIn.PrimaryContactInfo.Email,
                    Home = updateContactIn.PrimaryContactInfo.Home,
                    PreferredContactMethod = updateContactIn.PrimaryContactInfo.PreferredContactMethod
                };
                foundContactResult.SecondaryContactInfo = new ContactInfoEntity
                {
                    Mobile = updateContactIn.SecondaryContactInfo.Mobile,
                    Fax = updateContactIn.SecondaryContactInfo.Fax,
                    Email = updateContactIn.SecondaryContactInfo.Email,
                    Home = updateContactIn.SecondaryContactInfo.Home,
                    PreferredContactMethod = updateContactIn.SecondaryContactInfo.PreferredContactMethod
                };

                var updatedContact = await ContactCollection.ReplaceOneAsync(c => c.RecordId == recordId, foundContactResult,
                    new ReplaceOptions() {IsUpsert = false});
                return new SuccessfulTypedResult<ContactOutDto>(ContactOutDto.EntityToOutDto(foundContactResult));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<ContactOutDto>(e);
            }
        }
    }
}
