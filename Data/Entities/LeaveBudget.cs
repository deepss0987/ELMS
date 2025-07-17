using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;

namespace ELMS.Data.Entities
{
    public class LeaveBudget
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int RemainingBalance { get; set; }
        public int OpeningBalance { get; set; }
        public int Availed {  get; set; }= 0;
    
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Employee? Employee { get; set; }
        public LeaveType? LeaveType { get; set; }

        }
}
