namespace ELMS.Data.Entities
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Balance {  get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
