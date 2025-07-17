using AspNetCoreGeneratedDocument;
using ELMS.Data;
using ELMS.Data.Entities;
using ELMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;
using System.Security.Claims;

namespace ELMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ELMSDbContext _dbContext;
        public EmployeeController(ELMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ActionResult Index()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            var result = _dbContext.Employees
                .Include(e => e.Role)
                .Include(e => e.Manager)
                .ToList();

            foreach (var employee in result)
            {
                EmployeeViewModel model = new EmployeeViewModel
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    RoleId = employee.RoleId,
                    RoleName = employee.Role?.Name,
                    ManagerId = employee.ManagerId,
                    ManagerName = employee.Manager?.Name,
                    IsActive = employee.IsActive
                };
                employees.Add(model);
            }
            return View(employees);
        }

        public ActionResult Create()
        {
            EmployeeCreateViewModel model = new EmployeeCreateViewModel();
            model.Roles = _dbContext.Roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            model.Managers = _dbContext.Employees.Where(m => m.IsActive)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeCreateViewModel model)
        {
            bool isValid = true;

            if (ModelState.IsValid)
            {

                var existingEmployee = _dbContext.Employees.FirstOrDefault(e => e.Email == model.Email);
                if (existingEmployee != null)
                {
                    ModelState.AddModelError("Email", "An employee with this email already exists.");
                    isValid = false;
                }

            }
            if (isValid)
            {
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    RoleId = model.RoleId,
                    IsActive = model.IsActive,
                    ManagerId = model.ManagerId,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };
                _dbContext.Employees.Add(newEmployee);
                _dbContext.SaveChanges();

                // GET all leave types and assign them to the new employee

               
                var leavetypes = _dbContext.LeaveTypes.ToList();
                List<LeaveBudget> leavebudgets = new List<LeaveBudget>();
             
                foreach (var type in leavetypes)
                {
                    LeaveBudget listofleaves = new LeaveBudget
                    {
                        EmployeeId = newEmployee.Id,
                        LeaveTypeId = type.Id,
                        RemainingBalance = type.Balance,
                        OpeningBalance = type.Balance,
                        Availed = 0,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now
                    };
                    leavebudgets.Add(listofleaves);
                }
                _dbContext.LeaveBudgets.AddRange(leavebudgets);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            model.Roles = _dbContext.Roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            model.Managers = _dbContext.Employees.Where(m => m.IsActive)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var employee = _dbContext.Employees
                .Include(m => m.Role)
                .Include(m => m.Manager)
                .FirstOrDefault(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeCreateViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Password = employee.Password,
                RoleId = employee.RoleId,
                IsActive = employee.IsActive,
                ManagerId = employee.ManagerId,
                Manager = employee.Manager?.Name,
                Role = employee.Role?.Name,
                Roles = _dbContext.Roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList(),
                Managers = _dbContext.Employees.Where(m => m.IsActive)
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeCreateViewModel model)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(m => m.Id == model.Id);
            bool emailExists = await _dbContext.Employees.AnyAsync(u => u.Email == model.Email && u.Id != employee.Id);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists.");

                model.Roles = _dbContext.Roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

                model.Managers = _dbContext.Employees.Where(m => m.IsActive)
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name
                    }).ToList();

                return View(model);
            }

            employee.Name = model.Name;
            employee.Password = model.Password;
            employee.IsActive = model.IsActive;
            employee.Email = model.Email;
            employee.RoleId = model.RoleId;
            employee.ManagerId = model.ManagerId;

            await _dbContext.SaveChangesAsync();

            ViewBag.SuccessMessage = "The profile updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Employee/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult LeaveDetails()
        {
            var employeeIdStr = User.FindFirst("Id")?.Value;
            if (employeeIdStr == null)
            {
                return Unauthorized();
            }

            var employeeId = Convert.ToInt32(employeeIdStr);

            List<LeaveBudgetViewModel> leaveBudgets = new List<LeaveBudgetViewModel>();
            var result = _dbContext.LeaveBudgets
                .Where(m => m.EmployeeId == employeeId)
                .Include(m => m.LeaveType)
                .Include(m => m.Employee)
                .ToList();

            foreach (var leaveBudgetdetail in result)
            {
                LeaveBudgetViewModel model = new LeaveBudgetViewModel
                {
                    Id = leaveBudgetdetail.Id,
                    EmployeeId = leaveBudgetdetail.EmployeeId,
                    LeaveTypeId = leaveBudgetdetail.LeaveTypeId,
                    RemainingBalance = leaveBudgetdetail.RemainingBalance,
                    OpeningBalance = leaveBudgetdetail.OpeningBalance,
                    Availed = leaveBudgetdetail.Availed,
                    LeaveTypeName = leaveBudgetdetail.LeaveType.Name
                };
                leaveBudgets.Add(model);
            }



            return View(leaveBudgets);
        }
        public ActionResult LeaveApply()
        {
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            model.LeaveTypeNames = _dbContext.LeaveTypes.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            model.ApproverNames = _dbContext.Employees.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult LeaveApply(LeaveRequestViewModel model)
        {

            if (ModelState.IsValid)
            {
                var employeeIdStr = User.FindFirst("Id")?.Value;
                if (employeeIdStr == null)
                {
                    return Unauthorized();
                }

                var employeeId = Convert.ToInt32(employeeIdStr);

                if (model.EndDate < model.StartDate)
                {
                    ModelState.AddModelError("", "End Date cannot be before Start Date.");
                    return View(model);
                }

                var numberOfDays = (model.EndDate - model.StartDate).Days + 1;
                for (int i = 0; i < numberOfDays; i++)
                {
                    DateTime currentdate = model.StartDate.AddDays(i);
                    if (currentdate.DayOfWeek == DayOfWeek.Saturday || currentdate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        numberOfDays--;
                    }
                }
                model.NumberofDays = numberOfDays;


                LeaveRequest leaveRequest = new LeaveRequest
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = model.LeaveTypeId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Reason = model.Reason,
                    AppliedOn = DateTime.Now,
                    NumberofDays = numberOfDays,
                    StatusId = 1,
                    ApproverId = _dbContext.Employees.FirstOrDefault(e => e.Id == employeeId)?.ManagerId ?? 0,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };

                var leaveBudget = _dbContext.LeaveBudgets.FirstOrDefault(m => m.EmployeeId == employeeId && m.LeaveTypeId == model.LeaveTypeId);
                if(leaveBudget != null)
                {
                    if (leaveBudget.RemainingBalance < model.NumberofDays)
                    {
                        ModelState.AddModelError("", "You do not have enough balance to raise this Request.");
                        return View(model);
                    }
                    leaveBudget.RemainingBalance = leaveBudget.RemainingBalance - model.NumberofDays;
                    leaveBudget.Availed = leaveBudget.Availed + model.NumberofDays;

                }
                else{
                    ModelState.AddModelError("", "Leave budget not found for this leave type.");
                    return View(model);
                }
                _dbContext.LeaveRequests.Add(leaveRequest);
                _dbContext.LeaveBudgets.Update(leaveBudget);
                _dbContext.SaveChanges();

                return RedirectToAction("LeaveDetails");
            }

            model.LeaveTypeNames = _dbContext.LeaveTypes.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            model.ApproverNames = _dbContext.Employees.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult LeaveApproval()
        {
            var leaveRequests = new List<LeaveRequestViewModel>();

            var dbRequests = _dbContext.LeaveRequests
                .Include(m => m.Employee)
                .Include(m => m.LeaveType)
                .Include(m => m.Status)
                .Where(r => r.StatusId == 1)
                .ToList();

            foreach (var r in dbRequests)
            {
                var request = new LeaveRequestViewModel();
                request.Id = r.Id;
                request.EmployeeId = r.EmployeeId;
                request.LeaveTypeId = r.LeaveTypeId;
                request.StartDate = r.StartDate;
                request.EndDate = r.EndDate;
                request.Reason = r.Reason;
                request.StatusId = r.StatusId;
                request.NumberofDays = r.NumberofDays;
                request.ApproverId = r.ApproverId;
                request.AppliedOn = r.AppliedOn;
                request.EmployeeName = r.Employee?.Name;
                request.LeaveTypeName = r.LeaveType?.Name;
                request.StatusName = r.Status?.Name;
                request.ApproverName = r.Approver?.Name;

                leaveRequests.Add(request);
            }
            return View(leaveRequests); 
        }
        [HttpPost]
        public ActionResult LeaveApproval(LeaveRequestViewModel model)
        {
            var employeeIdStr = User.FindFirst("Id")?.Value;
          
            if (employeeIdStr == null)
            {
                return Unauthorized();
            }
            var employeeId = Convert.ToInt32(employeeIdStr);

            var leaveRequest = _dbContext.LeaveRequests.FirstOrDefault(m => m.Id == model.Id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (leaveRequest?.ApproverId != employeeId)
            {
                return Forbid(); //User is authenticated but doesn’t have permission to perform this action.
            }

            if (model.StatusId == 2) 
            {
                leaveRequest.StatusId = 2;
            }
            else if (model.StatusId == 3)
            {
                leaveRequest.StatusId = 3;
            }
            else
            {
                ModelState.AddModelError("", "Invalid leave status.");
                return View(model); 
            }
            _dbContext.SaveChanges();
            return RedirectToAction("ApprovedLeaves");
        }
        [HttpGet]
        public ActionResult ApprovedLeaves()
        {
            var approvedRequests = _dbContext.LeaveRequests
                .Include(m => m.Employee)
                .Include(m => m.LeaveType)
                .Include(m => m.Status)
                .ToList();

            List<LeaveRequestViewModel> leaveRequests = new List<LeaveRequestViewModel>();

            foreach (var r in approvedRequests)
            {
                var request = new LeaveRequestViewModel();
                request.Id = r.Id;
                request.EmployeeId = r.EmployeeId;
                request.LeaveTypeId = r.LeaveTypeId;
                request.StartDate = r.StartDate;
                request.EndDate = r.EndDate;
                request.Reason = r.Reason;
                request.StatusId = r.StatusId;
                request.NumberofDays = r.NumberofDays;
                request.ApproverId = r.ApproverId;
                request.AppliedOn = r.AppliedOn;
                request.EmployeeName = r.Employee?.Name;
                request.LeaveTypeName = r.LeaveType?.Name;
                request.StatusName = r.Status?.Name;
                request.ApproverName = r.Approver?.Name;

                leaveRequests.Add(request);
            }

            return View(leaveRequests); 
        }

    }
}
