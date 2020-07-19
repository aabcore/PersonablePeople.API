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
    public class LeadService : DbService
    {
        private readonly IMongoCollection<LeadEntity> LeadCollection;
        private readonly IMongoCollection<ContactEntity> ContactCollection;

        public LeadService(DatabaseSettings dbSettings)
        {
            var database = BuildDatabaseClient(dbSettings);
            LeadCollection = database.GetCollection<LeadEntity>(dbSettings.LeadsCollectionName);
            ContactCollection = database.GetCollection<ContactEntity>(dbSettings.ContactsCollectionName);
        }


        public async Task<TypedResult<IEnumerable<LeadOutDto>>> GetLeads(GetLeadFilter getLeadFilter)
        {
            try
            {
                var filters = new List<FilterDefinition<LeadEntity>>();
                if (getLeadFilter?.CreatedTimeAfter != null)
                {
                    filters.Add(Builders<LeadEntity>.Filter.Gt(le => le.CreatedTime, getLeadFilter.CreatedTimeAfter.Value));
                }

                if (getLeadFilter?.CreatedTimeBefore != null)
                {
                    filters.Add(Builders<LeadEntity>.Filter.Lt(le => le.CreatedTime, getLeadFilter.CreatedTimeBefore.Value));
                }

                if (getLeadFilter?.ModifiedTimeAfter != null)
                {
                    filters.Add(Builders<LeadEntity>.Filter.Gt(le => le.LastModifiedTime, getLeadFilter.ModifiedTimeAfter.Value));
                }

                if (getLeadFilter?.ModifiedTimeBefore != null)
                {
                    filters.Add(Builders<LeadEntity>.Filter.Lt(le => le.LastModifiedTime, getLeadFilter.ModifiedTimeBefore.Value));
                }

                if (getLeadFilter?.Tags != null && getLeadFilter.Tags.Any())
                {
                    var tagFilters = new List<FilterDefinition<LeadEntity>>();
                    foreach (var tag in getLeadFilter.Tags)
                    {
                        tagFilters.Add(Builders<LeadEntity>.Filter.AnyEq(le => le.Tags, tag));
                    }

                    filters.Add(Builders<LeadEntity>.Filter.Or(tagFilters));
                }

                if (getLeadFilter?.Converted != null)
                {
                    filters.Add(Builders<LeadEntity>.Filter.Eq(le => le.Converted, getLeadFilter.Converted.Value));
                }


                IEnumerable<LeadOutDto> foundLeads;
                foundLeads = filters.Any()
                    ? (await LeadCollection.FindAsync(Builders<LeadEntity>.Filter.And(filters))).ToList().Select(LeadOutDto.EntityToOutDto)
                    : (await LeadCollection.FindAsync(x => true)).ToList().Select(LeadOutDto.EntityToOutDto);

                return new SuccessfulTypedResult<IEnumerable<LeadOutDto>>(foundLeads);
            }
            catch (Exception e)
            {
                return new FailedTypedResult<IEnumerable<LeadOutDto>>(e);
            }
        }

        public async Task<TypedResult<IEnumerable<LeadOutDto>>> GetAllLeads()
        {
            try
            {
                var foundLeads = (await LeadCollection.FindAsync(l => true)).ToList().Select(LeadOutDto.EntityToOutDto);
                return new SuccessfulTypedResult<IEnumerable<LeadOutDto>>(foundLeads);
            }
            catch (Exception e)
            {
                return new FailedTypedResult<IEnumerable<LeadOutDto>>(e);
            }
        }

        public async Task<TypedResult<LeadOutDto>> CreateLead(NewLeadDtoIn newLeadDtoIn)
        {
            try
            {
                var dateTimeNow = DateTimeOffset.UtcNow;
                var newLeadEntity = new LeadEntity
                {
                    RecordId = Guid.NewGuid(),
                    CreatedTime = dateTimeNow,
                    CreatedByUserId = newLeadDtoIn.ModifiedBy,
                    ModifiedBy = newLeadDtoIn.ModifiedBy,
                    LastModifiedTime = dateTimeNow,
                    AnnualSalary = newLeadDtoIn.AnnualSalary,
                    LeadSource = newLeadDtoIn.LeadSource,
                    Tags = newLeadDtoIn.Tags.Select(x => x),
                    Converted = false,
                    Address = new AddressEntity
                    {
                        StreetLineOne = newLeadDtoIn.Address.StreetLineOne,
                        StreetLineTwo = newLeadDtoIn.Address.StreetLineTwo,
                        AptSuite = newLeadDtoIn.Address.AptSuite,
                        City = newLeadDtoIn.Address.City,
                        State = newLeadDtoIn.Address.State,
                        County = newLeadDtoIn.Address.County,
                        Zip = newLeadDtoIn.Address.Zip
                    },
                    Name = new NameEntity
                    {
                        FirstName = newLeadDtoIn.Name.FirstName,
                        LastName = newLeadDtoIn.Name.LastName,
                        MiddleName = newLeadDtoIn.Name.MiddleName,
                        Salutation = newLeadDtoIn.Name.Salutation,
                        PreferredFirstName = newLeadDtoIn.Name.PreferredFirstName
                    },
                    ContactInfo = new ContactInfoEntity
                    {
                        Mobile = newLeadDtoIn.ContactInfo.Mobile,
                        Fax = newLeadDtoIn.ContactInfo.Fax,
                        Email = newLeadDtoIn.ContactInfo.Email,
                        Home = newLeadDtoIn.ContactInfo.Home,
                        PreferredContactMethod = newLeadDtoIn.ContactInfo.PreferredContactMethod
                    }
                };

                await LeadCollection.InsertOneAsync(newLeadEntity);
                return new SuccessfulTypedResult<LeadOutDto>(LeadOutDto.EntityToOutDto(newLeadEntity));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<LeadOutDto>(e);
            }
        }

        public async Task<TypedResult<LeadOutDto>> GetLead(Guid recordId)
        {
            try
            {
                var foundLead = (await LeadCollection.FindAsync(l => l.RecordId == recordId)).FirstOrDefault();
                if (foundLead == null)
                {
                    return new NotFoundTypedResult<LeadOutDto>();
                }

                return new SuccessfulTypedResult<LeadOutDto>(LeadOutDto.EntityToOutDto(foundLead));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<LeadOutDto>(e);
            }
        }

        public async Task<TypedResult<LeadOutDto>> UpdateLead(Guid recordId, NewLeadDtoIn newLeadDtoIn)
        {
            try
            {
                var foundLead = (await LeadCollection.FindAsync(l => l.RecordId == recordId)).FirstOrDefault();
                if (foundLead == null)
                {
                    return new NotFoundTypedResult<LeadOutDto>();
                }

                foundLead.Address = new AddressEntity
                {
                    StreetLineOne = newLeadDtoIn.Address.StreetLineOne,
                    StreetLineTwo = newLeadDtoIn.Address.StreetLineTwo,
                    AptSuite = newLeadDtoIn.Address.AptSuite,
                    City = newLeadDtoIn.Address.City,
                    State = newLeadDtoIn.Address.State,
                    County = newLeadDtoIn.Address.County,
                    Zip = newLeadDtoIn.Address.Zip
                };
                foundLead.Name = new NameEntity
                {
                    FirstName = newLeadDtoIn.Name.FirstName,
                    LastName = newLeadDtoIn.Name.LastName,
                    MiddleName = newLeadDtoIn.Name.MiddleName,
                    Salutation = newLeadDtoIn.Name.Salutation,
                    PreferredFirstName = newLeadDtoIn.Name.PreferredFirstName
                };
                foundLead.ModifiedBy = newLeadDtoIn.ModifiedBy;
                foundLead.LastModifiedTime = DateTimeOffset.UtcNow;
                foundLead.AnnualSalary = newLeadDtoIn.AnnualSalary;
                foundLead.LeadSource = newLeadDtoIn.LeadSource;
                foundLead.Tags = newLeadDtoIn.Tags.Select(x => x);
                foundLead.ContactInfo = new ContactInfoEntity
                {
                    Mobile = newLeadDtoIn.ContactInfo.Mobile,
                    Fax = newLeadDtoIn.ContactInfo.Fax,
                    Email = newLeadDtoIn.ContactInfo.Email,
                    Home = newLeadDtoIn.ContactInfo.Home,
                    PreferredContactMethod = newLeadDtoIn.ContactInfo.PreferredContactMethod
                };

                var updatedLead =
                    await LeadCollection.ReplaceOneAsync(l => l.RecordId == recordId, foundLead, new ReplaceOptions() {IsUpsert = false});
                return new SuccessfulTypedResult<LeadOutDto>(LeadOutDto.EntityToOutDto(foundLead));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<LeadOutDto>(e);
            }
        }

        public async Task<TypedResult<ContactOutDto>> ConvertLead(Guid recordId, ConvertLeadDtoIn convertLeadDtoIn)
        {
            try
            {
                var foundLead = await LeadCollection.FindOneAndUpdateAsync<LeadEntity>(l => l.RecordId == recordId,
                    Builders<LeadEntity>.Update.Combine(
                        Builders<LeadEntity>.Update.Set(le => le.Converted, true),
                        Builders<LeadEntity>.Update.Set(le => le.ModifiedBy, convertLeadDtoIn.ModifiedBy),
                        Builders<LeadEntity>.Update.Set(le => le.LastModifiedTime, DateTimeOffset.UtcNow)
                    ), GetEntityAfterUpdateOption<LeadEntity>());
                if (foundLead == null)
                {
                    return new NotFoundTypedResult<ContactOutDto>();
                }

                var dateTimeNow = DateTimeOffset.UtcNow;
                
                var newContact = new ContactEntity
                {
                    RecordId = Guid.NewGuid(),
                    CreatedTime = dateTimeNow,
                    CreatedByUserId = convertLeadDtoIn.ModifiedBy,
                    ModifiedBy = convertLeadDtoIn.ModifiedBy,
                    LastModifiedTime = dateTimeNow,
                    AnnualSalary = foundLead.AnnualSalary,
                    LeadSource = foundLead.LeadSource,
                    Tags = foundLead.Tags.Select(x => x),
                    MailingAddress = new AddressEntity
                    {
                        StreetLineOne = foundLead.Address.StreetLineOne,
                        StreetLineTwo = foundLead.Address.StreetLineTwo,
                        AptSuite = foundLead.Address.AptSuite,
                        City = foundLead.Address.City,
                        State = foundLead.Address.State,
                        County = foundLead.Address.County,
                        Zip = foundLead.Address.Zip
                    },
                    OtherAddress = null,
                    PrimaryName = new NameEntity
                    {
                        FirstName = foundLead.Name.FirstName,
                        LastName = foundLead.Name.LastName,
                        MiddleName = foundLead.Name.MiddleName,
                        Salutation = foundLead.Name.Salutation,
                        PreferredFirstName = foundLead.Name.PreferredFirstName
                    },
                    SecondaryName = null,
                    PrimaryContactInfo = new ContactInfoEntity
                    {
                        Mobile = foundLead.ContactInfo.Mobile,
                        Fax = foundLead.ContactInfo.Fax,
                        Email = foundLead.ContactInfo.Email,
                        Home = foundLead.ContactInfo.Home,
                        PreferredContactMethod = foundLead.ContactInfo.PreferredContactMethod
                    },
                    SecondaryContactInfo = null,
                    AssociatedLeadId = foundLead.RecordId
                };

                await ContactCollection.InsertOneAsync(newContact);


                return new SuccessfulTypedResult<ContactOutDto>(ContactOutDto.EntityToOutDto(newContact));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<ContactOutDto>(e);
            }
        }
    }
}