#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Common
{
    /// <summary>
    /// Tests for DomainErrorMapper static utility methods.
    /// </summary>
    [TestClass]
    public class DomainErrorMapperTests
    {
        /// <summary>
        /// ToApplicationError throws ArgumentNullException when domainError is null.
        /// </summary>
        [TestMethod]
        public void ToApplicationError_NullError_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => DomainErrorMapper.ToApplicationError(null));
        }

        /// <summary>
        /// ToApplicationError preserves code, message, and field from ValidationError.
        /// </summary>
        [TestMethod]
        public void ToApplicationError_ValidError_PreservesFields()
        {
            var domainError = new ValidationError("TEST_CODE", "Test message", "TestField");

            var appError = DomainErrorMapper.ToApplicationError(domainError);

            Assert.AreEqual("TEST_CODE", appError.Code);
            Assert.AreEqual("Test message", appError.Message);
            Assert.AreEqual("TestField", appError.Field);
        }

        /// <summary>
        /// ToApplicationError preserves null field when not provided.
        /// </summary>
        [TestMethod]
        public void ToApplicationError_NullField_PreservesNull()
        {
            var domainError = new ValidationError("CODE", "Message");

            var appError = DomainErrorMapper.ToApplicationError(domainError);

            Assert.IsNull(appError.Field);
        }

        /// <summary>
        /// ToResult throws ArgumentNullException when validationResult is null.
        /// </summary>
        [TestMethod]
        public void ToResult_NullValidationResult_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => DomainErrorMapper.ToResult(null));
        }

        /// <summary>
        /// ToResult returns Result.Success when validation result is valid.
        /// </summary>
        [TestMethod]
        public void ToResult_ValidResult_ReturnsSuccess()
        {
            var validationResult = ValidationResult.Success();

            var result = DomainErrorMapper.ToResult(validationResult);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Errors.Count);
        }

        /// <summary>
        /// ToResult returns Result.Failure with mapped errors when validation fails.
        /// </summary>
        [TestMethod]
        public void ToResult_InvalidResult_MapsErrors()
        {
            var errors = new List<ValidationError>
            {
                new ValidationError("ERR1", "Error 1", "Field1"),
                new ValidationError("ERR2", "Error 2", "Field2")
            };
            var validationResult = new ValidationResult(errors);

            var result = DomainErrorMapper.ToResult(validationResult);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("ERR1", result.Errors[0].Code);
            Assert.AreEqual("Error 1", result.Errors[0].Message);
            Assert.AreEqual("Field1", result.Errors[0].Field);
            Assert.AreEqual("ERR2", result.Errors[1].Code);
        }

        /// <summary>
        /// ToResult generic throws ArgumentNullException when validationResult is null.
        /// </summary>
        [TestMethod]
        public void ToResultGeneric_NullValidationResult_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => DomainErrorMapper.ToResult<string>(null, "value"));
        }

        /// <summary>
        /// ToResult generic returns Result.Success with value when validation passes.
        /// </summary>
        [TestMethod]
        public void ToResultGeneric_ValidResult_ReturnsSuccessWithValue()
        {
            var validationResult = ValidationResult.Success();

            var result = DomainErrorMapper.ToResult<string>(validationResult, "TestValue");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("TestValue", result.Value);
            Assert.AreEqual(0, result.Errors.Count);
        }

        /// <summary>
        /// ToResult generic returns Result.Failure with mapped errors when validation fails.
        /// </summary>
        [TestMethod]
        public void ToResultGeneric_InvalidResult_MapsErrorsAndReturnsDefault()
        {
            var errors = new List<ValidationError>
            {
                new ValidationError("CODE", "Message", "Field")
            };
            var validationResult = new ValidationResult(errors);

            var result = DomainErrorMapper.ToResult<int>(validationResult, 42);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(default(int), result.Value);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("CODE", result.Errors[0].Code);
        }

        /// <summary>
        /// ToResult generic preserves null values on success.
        /// </summary>
        [TestMethod]
        public void ToResultGeneric_ValidResultWithNullValue_ReturnsSuccessWithNull()
        {
            var validationResult = ValidationResult.Success();

            var result = DomainErrorMapper.ToResult<string>(validationResult, null);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Value);
        }
    }
}
