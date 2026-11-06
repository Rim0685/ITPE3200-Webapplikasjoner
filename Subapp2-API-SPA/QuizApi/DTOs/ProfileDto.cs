// The DTOs folder contains the data sent between the API and the client.
// This class is the profile returned to the logged-in user.
// It leaves out sensitive fields such as the password hash.

namespace QuizApi.DTOs
{
    public class ProfileDto
    {
        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}
