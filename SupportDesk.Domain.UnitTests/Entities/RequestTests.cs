using System;
using FluentAssertions;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using Xunit;

namespace SupportDesk.Domain.UnitTests.Entities
{
    public class RequestTests
    {
        [Fact]
        public void Request_Should_Initialize_With_Default_Values()
        {
            // Arrange & Act
            var request = new Request();

            // Assert
            request.Id.Should().Be(0);
            request.ReviewerUserId.Should().BeNull();
            request.RequestTypeId.Should().Be(0);
            request.ZoneId.Should().Be(0);
            request.Comments.Should().BeEmpty();
            request.StartReviewDate.Should().BeNull();
            request.ReviewerUserComments.Should().BeNullOrEmpty();
            request.ApprovalRejectionDate.Should().BeNull();
            request.RequestStatusId.Should().Be(0);
            request.IsActive.Should().BeTrue();
            request.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Request_Can_Assign_ReviewerUser()
        {
            // Arrange
            var reviewer = new User
            {
                Id = Guid.NewGuid(),
                Email = "jane@example.com",
                FirstName = "Jane",
                LastName = "Lopez"
            };

            var request = new Request
            {
                ReviewerUser = reviewer
            };

            // Act
            var assignedReviewer = request.ReviewerUser;

            // Assert
            assignedReviewer.Should().NotBeNull();
            assignedReviewer.Should().Be(reviewer);
            assignedReviewer.FirstName.Should().Be("Jane");
        }

        [Fact]
        public void Request_Can_Update_Status()
        {
            // Arrange
            var request = new Request
            {
                RequestStatusId = (int)RequestStatusesEnum.New
            };

            // Act
            request.RequestStatusId = (int)RequestStatusesEnum.UnderReview;

            // Assert
            request.RequestStatusId.Should().Be((int)RequestStatusesEnum.UnderReview);
        }

        [Fact]
        public void Request_ApprovalDate_Should_Be_After_StartReviewDate()
        {
            // Arrange
            var request = new Request
            {
                StartReviewDate = new DateTime(2024, 08, 1),
                ApprovalRejectionDate = new DateTime(2024, 07, 31)
            };

            // Act
            var isValid = request.IsApprovalDateValid();

            // Assert
            isValid.Should().BeFalse();
        }

    }
}
