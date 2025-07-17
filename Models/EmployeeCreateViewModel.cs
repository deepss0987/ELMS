using ELMS.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ELMS.Models
{
    public class EmployeeCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role is Required")]
        public int RoleId { get; set; }
        public bool IsActive { get; set; } 
        public int? ManagerId { get; set; }
        public string? Manager { get; set; }
        public string? Role { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
        public IEnumerable<SelectListItem> Managers { get; set; }

    }
}
