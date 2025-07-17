namespace ELMS.Data.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public int StatusId { get; set; }
        public DateTime AppliedOn { get; set; }
        public int NumberofDays { get; set; }
        public int ApproverId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public  Employee? Employee { get; set; }
        public LeaveType? LeaveType { get; set; }
        public LeaveRequestStatus? Status { get; set; }
        public Employee? Approver { get; set; }


    }
}
