namespace ELMS.Models
{
    public class LeaveBudgetViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int RemainingBalance { get; set; }
        public int OpeningBalance { get; set; }
        public int Availed { get; set; } = 0;
        public string LeaveTypeName { get; set; }
    }
}
