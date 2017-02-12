using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Guanghui.Repository;
using Guanghui.Repository.UnitOfWork;
using Guanghui.BusinessServices.Interfaces;
using Guanghui.Repository.GenericRepository;
using Guanghui.BusinessServices.Implementations;
using Guanghui.TestHelper;

namespace Guanghui.BusinessServices.Tests
{
    public class TokenServiceTest
    {
        #region Variables

        private ITokenService _tokenService;
        private IUnitOfWork _unitOfWork;
        private List<Token> _tokens;
        private GenericRepository<Token> _tokenRepository;
        private WebApiDbContext _dbContext;
        private const string SampleAuthToken = "9f907bdf-f6de-425d-be5b-b4852eb77761";

        #endregion

        #region Test fixture setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _tokens = SetUpTokens();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Re-initializes test.
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _dbContext = new Mock<WebApiDbContext>().Object;
            _tokenRepository = SetUpTokenRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.TokenRepository).Returns(_tokenRepository);
            _unitOfWork = unitOfWork.Object;
            _tokenService = new TokenService(_unitOfWork);
        }

        #endregion

        #region Private member methods

        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Token> SetUpTokenRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Token>>(MockBehavior.Default, _dbContext);

            // Setup mocking behavior
            mockRepo.Setup(p => p.Get()).Returns(_tokens);

            mockRepo.Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(new Func<int, Token>(
                             id => _tokens.Find(p => p.TokenId.Equals(id))));

            mockRepo.Setup(p => p.GetById(It.IsAny<string>()))
               .Returns(new Func<string, Token>(
                            authToken => _tokens.Find(p => p.AuthToken.Equals(authToken))));

            mockRepo.Setup(p => p.Add((It.IsAny<Token>())))
                .Callback(new Action<Token>(newToken =>
                {
                    dynamic maxTokenId = _tokens.Last().TokenId;
                    dynamic nextTokenId = maxTokenId + 1;
                    newToken.TokenId = nextTokenId;
                    _tokens.Add(newToken);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Token>()))
                .Callback(new Action<Token>(token =>
                {
                    var oldToken = _tokens.Find(a => a.TokenId == token.TokenId);
                    oldToken = token;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Token>()))
                .Callback(new Action<Token>(prod =>
                {
                    var tokenToRemove =
                        _tokens.Find(a => a.TokenId == prod.TokenId);

                    if (tokenToRemove != null)
                        _tokens.Remove(tokenToRemove);
                }));
            //Create setup for other methods too. note non virtauls methods can not be set up

            // Return mock implementation object
            return mockRepo.Object;
        }

        /// <summary>
        /// Setup dummy tokens data
        /// </summary>
        /// <returns></returns>
        private static List<Token> SetUpTokens()
        {
            var tokId = 0;
            var tokens = DataInitializer.GetAllTokens();
            foreach (Token tok in tokens)
                tok.TokenId = ++tokId;
            return tokens;
        }

        #endregion

        #region Unit Tests

        /// <summary>
        /// Generate token test
        /// </summary>
        [Test]
        public void GenerateTokenByUserIdTest()
        {
            const int userId = 1;
            var maxTokenIdBeforeAdd = _tokens.Max(a => a.TokenId);
            var tokenEntity = _tokenService.GenerateToken(userId);
            var newTokenDataModel = new Token()
            {
                AuthToken = tokenEntity.AuthToken,
                TokenId = maxTokenIdBeforeAdd + 1,
                ExpiresOn = tokenEntity.ExpiresOn,
                IssuedOn = tokenEntity.IssuedOn,
                UserId = tokenEntity.UserId
            };
            AssertObjects.PropertyValuesAreEquals(newTokenDataModel, _tokens.Last());

        }

        /// <summary>
        /// Validate token test
        /// </summary>
        [Test]
        public void ValidateTokenWithRightAuthToken()
        {
            var authToken = Convert.ToString(SampleAuthToken);
            var validationResult = _tokenService.ValidateTokenById(authToken);  //inside will call GetById method, which should use SetUpTokenRepository() mockRepo.Setup GetById()
            Assert.That(validationResult, Is.EqualTo(true));
        }

        /// <summary>
        /// Generate token with wrong auth token test
        /// </summary>
        [Test]
        public void ValidateTokenWithWrongAuthToken()
        {
            var authToken = Convert.ToString("xyz");
            var validationResult = _tokenService.ValidateTokenById(authToken);
            Assert.That(validationResult, Is.EqualTo(false));
        }

        #endregion

        #region Tear Down

        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _tokenService = null;
            _unitOfWork = null;
            _tokenRepository = null;
            if (_dbContext != null)
                _dbContext.Dispose();
        }

        #endregion

        #region TestFixture TearDown.

        /// <summary>
        /// TestFixture teardown
        /// </summary>
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _tokens = null;
        }

        #endregion

    }
}
