using Microsoft.AspNetCore.Mvc.Rendering;

namespace ELMS.Models
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public int StatusId { get; set; }
        public int NumberofDays { get; set; }
        public int ApproverId { get; set; }
        public DateTime AppliedOn { get; set; }
        public string? EmployeeName { get; set; }
        public string? LeaveTypeName { get; set; }
        public IEnumerable<SelectListItem>? LeaveTypeNames { get; set; }
        public string? ApproverName { get; set; }
        public IEnumerable<SelectListItem>? ApproverNames{ get; set; }
        public string? StatusName { get; set; }
        public IEnumerable<SelectListItem>? StatusNames { get; set; }
    }
}
