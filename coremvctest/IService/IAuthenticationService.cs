using coremvctest.Models;

namespace coremvctest.IService
{
    public interface IAuthenticationService
    {
		/// <summary>
		/// Validates a given JWT
		/// </summary>
		/// <param name="jwtToken">The JWT to validate</param>
		/// <param name="httpContext">The HTTP context associated with the request</param>
		/// <returns>Returns true if the token is valid; otherwise, false</returns>
		bool validateJwtToken(string jwtToken, HttpContext httpContext);
		/// <summary>
		/// Generates a JWT (JSON Web Token) for the specified user.
		/// </summary>
		/// <param name="user">The user for whom the token is to be generated.</param>
		/// <returns>A JWT string for the given user.</returns>
		public string GenerateTokenForFoodInventory(FoodStoreEntity? foodInventory);
		/// <summary>
		/// Generates a JWT (JSON Web Token) for the specified user.
		/// </summary>
		/// <param name="user">The user for whom the token is to be generated.</param>
		/// <returns>A JWT string for the given user.</returns>
		public string GenerateTokenForNGO(NGOEntity? ngo);
	}
}
