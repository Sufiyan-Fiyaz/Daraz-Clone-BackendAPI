namespace Daraz_CloneAgain.DTOs
{

    // Register DTO
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }


        // 👇 sirf "customer" ya "seller" allow hoga
        public string Role { get; set; } = "customer";
    }

    // Login DTO
    public class LoginRequest
    {
        public string Identifier { get; set; } // email or phone
        public string Password { get; set; }
    }


    // Response DTO (to avoid exposing password)
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }

        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
