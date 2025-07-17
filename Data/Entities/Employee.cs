namespace ELMS.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public int? ManagerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set;}
        public  Role? Role { get; set; }
        public  Employee? Manager { get; set; }
    }
}
