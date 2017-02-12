using Guanghui.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Guanghui.BusinessServices.Interfaces
{
    public interface ITokenService
    {
        #region Interface member methods.
        /// <summary>
        ///  Function to generate unique token with expiry against the provided userId.
        ///  Also add a record in database for generated token.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        TokenEntity GenerateToken(int userId);

        /// <summary>
        /// Function to validate token againt expiry and existance in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        bool ValidateTokenById(string tokenId);

        /// <summary>
        /// Validate auth token
        /// </summary>
        /// <param name="authToken">token value</param>
        /// <returns></returns>
        bool ValidateToken(string authToken);

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId"></param>
        bool Kill(string tokenId);

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool DeleteByUserId(int userId);
        #endregion
    }
}
