using AutoFixture;
using FluentAssertions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Web.Validators;
using Xunit;

namespace VirtoCommerce.ContentModule.Tests.Validators
{
    public class ContentFolderValidatorTests : ValidatorTestsHelper
    {
        private readonly ContentFolderValidator _validator = new ContentFolderValidator();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ContentFolder_Name_IsNull_ShouldFail()
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = null
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name" && x.ErrorCode == NOT_NULL_VALIDATOR_ERROR_CODE);
        }

        [Fact]
        public void ContentFolder_Name_IsEmpty_ShouldFail()
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = string.Empty
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name" && x.ErrorCode == NOT_EMPTY_VALIDATOR_ERROR_CODE);
        }

        [Fact]
        public void ContentFolder_Name_TooShort_ShouldFail()
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = string.Join(string.Empty, _fixture.CreateMany<char>(2))
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name" && x.ErrorCode == MIN_LENGTH_VALIDATOR_ERROR_CODE);
        }

        [Fact]
        public void ContentFolder_Name_TooLarge_ShouldFail()
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = string.Join(string.Empty, _fixture.CreateMany<char>(64))
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name" && x.ErrorCode == MAX_LENGTH_VALIDATOR_ERROR_CODE);
        }

        [Theory]
        [InlineData("-ice")]
        [InlineData("nic-")]
        [InlineData("n--e")]
        public void ContentFolder_Name_DashesPoliticsViolated_ShouldFail(string name)
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = name
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name" && x.ErrorMessage.Contains("dash"));
        }

        [Theory]
        [InlineData("l-i-z-z-a-r-d")]
        [InlineData("fizz-buzz")]
        [InlineData("rice-fields-f0lder")]
        public void ContentFolder_Name_DashesPoliticsTaken_ShouldPass(string name)
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = name
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("cache$$$")]
        [InlineData("dash===>")]
        [InlineData("lash:(")]
        public void ContentFolder_Name_ContainsSpecialСharacters_ShouldFail(string name)
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = name
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name" && x.ErrorMessage.Contains("alphanumeric lower case"));
        }

        [Fact]
        public void ContentFolder_Name_ContainsUpperCaseLetters_ShouldFail()
        {
            // Arrange
            var instance = new ContentFolder
            {
                Name = "BIGLETTERSNOTPASS"
            };

            // Act
            var result = _validator.Validate(instance);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.PropertyName == "Name");
        }
    }
}
