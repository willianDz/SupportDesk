using System;
using FluentAssertions;
using SupportDesk.Domain.Entities;
using Xunit;

namespace SupportDesk.Domain.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_Should_Initialize_With_Default_Values()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Id.Should().BeEmpty();
            user.Email.Should().BeEmpty();
            user.FirstName.Should().BeEmpty();
            user.LastName.Should().BeEmpty();
            user.BirthDate.Should().BeNull();
            user.GenderId.Should().BeNull();
            user.PhotoUrl.Should().BeNull();
            user.IsActive.Should().BeTrue();
            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void User_Should_Assign_Gender_Correctly()
        {
            // Arrange
            var gender = new Gender
            {
                GenderId = 1,
                Description = "Masculino",
                Abbreviation = "M"
            };

            var user = new User
            {
                Email = "juan@example.com",
                FirstName = "Juan",
                LastName = "Perez",
                Gender = gender
            };

            // Act
            var assignedGender = user.Gender;

            // Assert
            assignedGender.Should().NotBeNull();
            assignedGender.Should().Be(gender);
            assignedGender.Description.Should().Be("Masculino");
        }

        [Fact]
        public void User_Should_Have_Empty_Collections_By_Default()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.UserZones.Should().BeNull();
            user.UserRequestTypes.Should().BeNull();
            user.TwoFactorAuthTokens.Should().BeNull();
            user.ReviewedRequests.Should().BeNull();
        }
    }
}
