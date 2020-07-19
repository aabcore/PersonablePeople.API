using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PersonablePeople.API.Controllers;
using PersonablePeople.API.Models;
using PersonablePeople.API.Models.ApiDtos;
using PersonablePeople.API.Models.Entities;
using PersonablePeople.API.Results;

namespace PersonablePeople.API.Services
{
    public class UserService: DbService
    {
        private readonly IMongoCollection<UserEntity> UserCollection;

        public UserService(DatabaseSettings dbSettings)
        {
            var database = BuildDatabaseClient(dbSettings);
            UserCollection = database.GetCollection<UserEntity>(dbSettings.UsersCollectionName);
        }

        public async Task<TypedResult<IEnumerable<UserOutDto>>> GetAllUsers()
        {
            try
            {
                var foundUsers = (await UserCollection.FindAsync(x => true)).ToList();
                return new SuccessfulTypedResult<IEnumerable<UserOutDto>>(foundUsers.Select(UserOutDto.EntityToOutDto));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<IEnumerable<UserOutDto>>(e);
            }
        }



        public async Task<TypedResult<IEnumerable<UserOutDto>>> GetUsers(GetUserFilter getUserFilter)
        {
            try
            {
                var filters = new List<FilterDefinition<UserEntity>>();
                if (!string.IsNullOrWhiteSpace(getUserFilter?.FirstNameLike))
                {
                    filters.Add(Builders<UserEntity>.Filter.Regex(u => u.Name.FirstName, BsonRegularExpression.Create(new Regex($"{getUserFilter.FirstNameLike}"))));
                }
                if (!string.IsNullOrWhiteSpace(getUserFilter?.LastNameLike))
                {
                    filters.Add(Builders<UserEntity>.Filter.Regex(u => u.Name.LastName, BsonRegularExpression.Create(new Regex($"{getUserFilter.LastNameLike}"))));
                }

                if (getUserFilter?.ReportsTo != null)
                {
                    filters.Add(Builders<UserEntity>.Filter.Eq(u => u.ReportingTo, getUserFilter.ReportsTo));
                }



                if (getUserFilter?.Roles != null && getUserFilter.Roles.Any())
                {
                    var tagFilters = new List<FilterDefinition<UserEntity>>();
                    foreach (var role in getUserFilter.Roles)
                    {
                        tagFilters.Add(Builders<UserEntity>.Filter.AnyEq(le => le.Roles, role.ToString()));
                    }

                    filters.Add(Builders<UserEntity>.Filter.Or(tagFilters));
                }


                var foundUsers = filters.Any()
                    ? (await UserCollection.FindAsync(Builders<UserEntity>.Filter.And(filters))).ToList().Select(UserOutDto.EntityToOutDto)
                    : (await UserCollection.FindAsync(x => true)).ToList().Select(UserOutDto.EntityToOutDto);

                return new SuccessfulTypedResult<IEnumerable<UserOutDto>>(foundUsers);
            }
            catch (Exception e)
            {
                return new FailedTypedResult<IEnumerable<UserOutDto>>(e);
            }
        }

        public async Task<TypedResult<UserOutDto>> GetUser(Guid userId)
        {
            try
            {
                var foundUser = (await UserCollection.FindAsync(c => c.UserId == userId)).FirstOrDefault();
                if (foundUser == null)
                {
                    return new NotFoundTypedResult<UserOutDto>();
                }

                return new SuccessfulTypedResult<UserOutDto>(UserOutDto.EntityToOutDto(foundUser));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<UserOutDto>(e);
            }
        }

        public async Task<TypedResult<UserOutDto>> UpdateUser(Guid userId, UpdateUserDtoIn newUserId)
        {
            try
            {
                var foundUser = (await UserCollection.FindAsync(c => c.UserId == userId)).FirstOrDefault();
                if (foundUser == null)
                {
                    return new NotFoundTypedResult<UserOutDto>();
                }

                foundUser.Name = new NameEntity()
                {
                    FirstName = newUserId.Name.FirstName,
                    LastName = newUserId.Name.LastName,
                    MiddleName = newUserId.Name.MiddleName,
                    Salutation = newUserId.Name.Salutation,
                    PreferredFirstName = newUserId.Name.PreferredFirstName
                };
                foundUser.Email = newUserId.Email;
                foundUser.ReportingTo = newUserId.ReportingTo;
                foundUser.Status = newUserId.Status;
                foundUser.Roles = newUserId.Roles.Select(x => x.ToString());

                await UserCollection.ReplaceOneAsync(u => u.UserId == userId, foundUser, new ReplaceOptions() {IsUpsert = false});
                return new SuccessfulTypedResult<UserOutDto>(UserOutDto.EntityToOutDto(foundUser));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<UserOutDto>(e);
            }
        }

        public async Task<TypedResult<UserOutDto>> CreateUser(UpdateUserDtoIn updateUserIn)
        {
            try
            {
                var newUser = new UserEntity()
                {
                    UserId = Guid.NewGuid(),
                    Email = updateUserIn.Email,
                    ReportingTo = updateUserIn.ReportingTo,
                    Name = new NameEntity()
                    {
                        FirstName = updateUserIn.Name.FirstName,
                        LastName = updateUserIn.Name.LastName,
                        MiddleName = updateUserIn.Name.MiddleName,
                        Salutation = updateUserIn.Name.Salutation,
                        PreferredFirstName = updateUserIn.Name.PreferredFirstName
                    },
                    Roles = updateUserIn.Roles.Select(x => x.ToString()),
                    Status = updateUserIn.Status
                };

                await UserCollection.InsertOneAsync(newUser);
                return new SuccessfulTypedResult<UserOutDto>(UserOutDto.EntityToOutDto(newUser));
            }
            catch (Exception e)
            {
                return new FailedTypedResult<UserOutDto>(e);
            }
        }

        public async Task<TypedResult<bool>> DeleteUser(Guid userId)
        {
            try
            {
                var foundUser = (await UserCollection.FindAsync(c => c.UserId == userId)).FirstOrDefault();
                if (foundUser == null)
                {
                    return new NotFoundTypedResult<bool>();
                }

                var deleteResult = await UserCollection.DeleteOneAsync(u => u.UserId == userId);
                if (deleteResult.IsAcknowledged)
                {
                    return new SuccessfulTypedResult<bool>(true);
                }
                else
                {
                    return new FailedTypedResult<bool>(new Exception("Didn't get acknowledgement from DB."));
                }
            }
            catch (Exception e)
            {
                return new FailedTypedResult<bool>(e);
            }
        }
    }
}
