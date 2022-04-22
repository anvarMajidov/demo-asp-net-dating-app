using System.ComponentModel.DataAnnotations;

namespace api.Dtos.UserDtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
