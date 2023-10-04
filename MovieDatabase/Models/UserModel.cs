using System.ComponentModel.DataAnnotations;

namespace MovieDatabase.Models
{
    public class UserModel
    {
        [Display(Name = "Users Id")]
        public int UserId { get; set; }
    }
}
