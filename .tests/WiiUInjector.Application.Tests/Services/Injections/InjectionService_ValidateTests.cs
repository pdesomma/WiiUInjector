#nullable disable
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Services.Injections
{
    /// <summary>
    /// Tests for InjectionService.Validate synchronous validation method.
    /// </summary>
    [TestClass]
    public class InjectionService_ValidateTests
    {
        private InjectionService sut;
        private FakeInjectionValidator fakeValidator;

        [TestInitialize]
        public void Setup()
        {
            var fakeRomLoader = new FakeRomFileLoader();
            var fakeImageLoader = new FakeGameImageLoader();
            var fakeSoundLoader = new FakeBootSoundLoader();
            var fakeBaseRomCatalog = new FakesB.FakeBaseRomCatalog();
            fakeValidator = new FakeInjectionValidator();
            var fakePipeline = new FakeInjectionPipeline();
            var fakeLogger = new FakeApplicationLogger();

            sut = new InjectionService(
                fakeRomLoader,
                fakeImageLoader,
                fakeSoundLoader,
                fakeBaseRomCatalog,
                fakeValidator,
                fakePipeline,
                fakeLogger);
        }

        /// <summary>
        /// Validate returns failure when injection is null.
        /// </summary>
        [TestMethod]
        public void Validate_NullInjection_ReturnsFailure()
        {
            var result = sut.Validate(null);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(ApplicationErrors.DomainInvariantViolation, result.Errors[0].Code);
        }

        /// <summary>
        /// Validate returns success when domain validator passes.
        /// </summary>
        [TestMethod]
        public void Validate_ValidInjection_ReturnsSuccess()
        {
            fakeValidator.NextResult = ValidationResult.Success();
            var injection = TestData.AnyN64Injection();

            var result = sut.Validate(injection);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Errors.Count);
        }

        /// <summary>
        /// Validate maps domain validation errors to application errors.
        /// </summary>
        [TestMethod]
        public void Validate_InvalidInjection_MapsErrors()
        {
            var errors = new List<ValidationError>
            {
                new ValidationError("ICON_DIMENSIONS", "Icon wrong", "Images[Icon]"),
                new ValidationError("ROM_SIZE", "ROM too big", "Rom.Length")
            };
            fakeValidator.NextResult = new ValidationResult(errors);
            var injection = TestData.AnyN64Injection();

            var result = sut.Validate(injection);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("ICON_DIMENSIONS", result.Errors[0].Code);
            Assert.AreEqual("Icon wrong", result.Errors[0].Message);
            Assert.AreEqual("ROM_SIZE", result.Errors[1].Code);
        }

        /// <summary>
        /// Validate calls the domain validator with the injection.
        /// </summary>
        [TestMethod]
        public void Validate_Success_CallsValidator()
        {
            var injection = TestData.AnyN64Injection();

            sut.Validate(injection);

            Assert.AreEqual(1, fakeValidator.CallCount);
            Assert.AreEqual(injection, fakeValidator.LastInjection);
        }
    }
}
