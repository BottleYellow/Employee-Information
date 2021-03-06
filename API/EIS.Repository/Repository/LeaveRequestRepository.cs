﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EIS.Repositories.Repository
{
    public class LeaveRequestRepository: RepositoryBase<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }

        public void AddPastLeave(PastLeaves pastLeave)
        {
            _dbContext.PastLeaves.Add(pastLeave);
            _dbContext.SaveChanges();
        }

        public IQueryable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId)
        {
            var results = from Requests in _dbContext.LeaveRequests
                          join Person in _dbContext.Person on Requests.PersonId equals Person.Id
                          where Person.ReportingPersonId == PersonId && Requests.TenantId == TenantId
                          select Requests;
            return results;
        }
        public string CheckForScheduledLeave(int PersonId, DateTime FromDate, DateTime ToDate, int? LeaveId)
        {
            string result = "success";
            List<Attendance> attendances = new List<Attendance>();
            IQueryable<LeaveRequest> requests = null;
            if (LeaveId == 0)
            {
                requests = _dbContext.LeaveRequests.Where(x => x.Status!="Cancelled" && x.Status!="Rejected" && ((x.FromDate <= FromDate && FromDate <= x.ToDate) || (x.FromDate <= ToDate && ToDate <= x.ToDate) || (FromDate <= x.FromDate && x.FromDate <= ToDate) || (FromDate <= x.ToDate && x.ToDate <= ToDate)) && (x.PersonId == PersonId));
            }
            else
            {
                requests= _dbContext.LeaveRequests.Where(x => x.Id != LeaveId && ((x.FromDate <= FromDate && FromDate <= x.ToDate) || (x.FromDate <= ToDate && ToDate <= x.ToDate) || (FromDate <= x.FromDate && x.FromDate <= ToDate) || (FromDate <= x.ToDate && x.ToDate <= ToDate)) && (x.PersonId == PersonId));
                for (var d = FromDate; d <= ToDate; d = d.AddDays(1))
                {
                    Attendance record = _dbContext.Attendances.Where(x => x.DateIn == d.Date && x.PersonId == PersonId).FirstOrDefault();
                    if (record != null) attendances.Add(record);
                }
            }
            //requests = LeaveId == 0 ? _dbContext.LeaveRequests.Where(x => ((x.FromDate <= FromDate && FromDate <= x.ToDate) || (x.FromDate <= ToDate && ToDate <= x.ToDate) || (FromDate <= x.FromDate && x.FromDate <= ToDate) || (FromDate <= x.ToDate && x.ToDate <= ToDate)) && (x.PersonId == PersonId))
            //                                                    : _dbContext.LeaveRequests.Where(x => x.Id!=LeaveId && ((x.FromDate <= FromDate && FromDate <= x.ToDate) || (x.FromDate <= ToDate && ToDate <= x.ToDate) || (FromDate <= x.FromDate && x.FromDate <= ToDate) || (FromDate <= x.ToDate && x.ToDate <= ToDate)) && (x.PersonId == PersonId));
            LeaveRequest request = null;
            if (attendances.Count > 0)
            {
                result = "You must be present on requested Dates. Please Select Correct Dates";
            }
            else if (requests != null && requests.Count() > 0)
            {
                result = "Leaves are already scheduled on requested dates";
                request = requests.Where(x => x.Status == "Cancelled").FirstOrDefault();
                if (request != null)
                {
                    result = "success";
                }
            }

            return result;
        }
        public string CheckForScheduledPastLeave(int PersonId, DateTime FromDate, DateTime ToDate)
        {
            List<Attendance> attendances = new List<Attendance>();
            for (var d = FromDate; d <= ToDate; d = d.AddDays(1))
            {
                Attendance record = _dbContext.Attendances.Where(x => x.DateIn == d.Date && x.PersonId==PersonId).FirstOrDefault();
                if (record!=null) attendances.Add(record);
            }
            string result = "success";
            IQueryable<LeaveRequest> requests = _dbContext.LeaveRequests.Where(x => ((x.FromDate <= FromDate && FromDate <= x.ToDate) || (x.FromDate <= ToDate && ToDate <= x.ToDate) || (FromDate <= x.FromDate && x.FromDate <= ToDate) || (FromDate <= x.ToDate && x.ToDate <= ToDate)) && (x.PersonId == PersonId));
            LeaveRequest request = null;
            if (attendances.Count > 0)
            {
                result = "You must be present on requested Dates. Please Select Correct Dates";
            }
            else if (requests != null && requests.Count() > 0)
            {
                result = "Leaves are already exists on requested dates";
                request = requests.Where(x => x.Status == "Cancelled").FirstOrDefault();
                if (request != null)
                {
                    result = "success";
                }
            }

            return result;
        }
        public IQueryable<PastLeaves> GetPastLeaves(int PersonId,int TenantId,int? LocationId)
        {
            IQueryable<PastLeaves> result = null;
            if (PersonId == 0 && LocationId == 0)
            {
                result = _dbContext.PastLeaves.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true);
            }
            else if (PersonId != 0 && LocationId == 0)
            {
                result= _dbContext.PastLeaves.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.PersonId == PersonId && x.Person.Location.IsActive == true);
            }
            else
            {
                result = _dbContext.PastLeaves.Include(x=>x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.LocationId==LocationId && x.Person.Location.IsActive == true);
            }
           
            return result;
        }

        public string UpdateRequestStatus(int RequestId, string Status,int PersonId,float? OldCountForEdit)
        {
            string messege = null;
            var leaveCredit = new LeaveCredit();
            LeaveRequest leaveRequest = _dbContext.LeaveRequests.Include(x=>x.Person).Where(x => x.Id == RequestId).FirstOrDefault();
            leaveRequest.UpdatedDate = DateTime.Now;
            bool isPaid = _dbContext.LeaveRequests.Include(x => x.TypeOfLeave).Where(x => x.Id == RequestId).FirstOrDefault().TypeOfLeave.IsPaid;
            if (Status == "Approve")
            {
                if ((leaveRequest.FromDate <= DateTime.Now.Date || leaveRequest.ToDate <= DateTime.Now.Date)&&leaveRequest.LeaveType.Contains("Past"))
                {
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.RequestedDays;
                }
                leaveRequest.ApprovedBy = PersonId;
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Approved";
                messege = "Request of " + leaveRequest.Person.FirstName + " is approved for " + leaveRequest.RequestedDays + " days";
                //var requests = _dbContext.LeaveRequests.Where(x => x.PersonId == leaveRequest.PersonId);

            }
            else if (Status == "Reject")
            {
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Rejected";
                if (isPaid == true)
                {
                    leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
                }
                messege = "Request of " + leaveRequest.Person.FirstName + " is rejected for " + leaveRequest.RequestedDays + " days";
            }
            else if (Status == "Pending")
            {
                leaveRequest.UpdatedBy = PersonId;
                if (leaveRequest.Status == null || leaveRequest.Status == "Rejected")
                {
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    if(isPaid==true)
                        leaveCredit.Available = leaveCredit.Available - leaveRequest.RequestedDays;
                }
                leaveRequest.Status = "Pending";
                messege = "Request of " + leaveRequest.Person.FirstName + " marked as pending";
            }
            else if (Status == "Cancel")
            {
                leaveRequest.UpdatedBy = PersonId;
                if (leaveRequest.Status == "Pending")
                {
                    if (isPaid == true)
                    {
                        leaveRequest.Status = "Cancelled";
                        leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                        leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                        leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
                    }
                    messege = "Your request has been cancelled";
                }
                else if (leaveRequest.Status == "Approved")
                {
                    leaveRequest.Status = "Requested For Cancel";
                    messege = "Your request for cancelling submitted successully";
                }
            }
            else if (Status == "Accept Cancel")
            {
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Cancelled";
                if (isPaid == true)
                {
                    leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
                }
                messege = "Request is cancelled";
            }
            else if (Status == "Reject Cancel")
            {
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Approved(Rejected Cancel Request)";
                messege = "Request for cancelling rejected successfully";
            }
            else if (Status == null)
            {
                leaveRequest.UpdatedBy = PersonId;
                if (isPaid == true)
                {
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available + Convert.ToInt64(OldCountForEdit) - leaveRequest.RequestedDays;
                    //leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                }
            }
            Save();
            return messege;
        }

        public List<LeaveRequestViewModel> GetLeaveData(int locationId,string employeeId,int month,int year,int TenantId,string leaveType,bool status)
        {
            List<LeaveRequestViewModel> leaveDataView = new List<LeaveRequestViewModel>();
            IEnumerable<LeaveRequest> leaveData= new List<LeaveRequest>();

            if (locationId == 0)
            {
                if (month == 0)
                {
                    if(leaveType=="All")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.IsActive==status).ToList() :
                                                                        FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.IsActive == status).ToList();

                    }else if(leaveType == "Pending")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && (x.Status == leaveType ||x.Status== "Requested For Cancel") && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Approved")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList();
                    }
                    else
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status== leaveType && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status== leaveType && x.Person.IsActive == status).ToList();

                    }
                }
                else
                {
                    if (leaveType == "All")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Pending")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Approved")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList();
                    }
                    else
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status == leaveType && x.Person.IsActive == status).ToList() :
                                                FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Status== leaveType && x.Person.IsActive == status).ToList();
                    }
                }

            }
            else
            {
                if (month == 0)
                {
                    if (leaveType == "All")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Person.IsActive == status).ToList() :
                                                       FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Pending")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList() :
                                                      FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Approved")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList() :
                                                      FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList();
                    }
                    else
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status == leaveType && x.Person.IsActive == status).ToList() :
                                                       FindAllByCondition(x => x.FromDate.Year == year || x.ToDate.Year == year).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status == leaveType && x.Person.IsActive == status).ToList();
                    }
                }
                else
                {
                    if (leaveType == "All")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Person.IsActive == status).ToList() :
                                                  FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Pending")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList() :
                                                  FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && (x.Status == leaveType || x.Status == "Requested For Cancel") && x.Person.IsActive == status).ToList();
                    }
                    else if (leaveType == "Approved")
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList() :
                                                  FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status.StartsWith(leaveType) && x.Person.IsActive == status).ToList();
                    }
                    else
                    {
                        leaveData = employeeId == "0" ? FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId &&x.Status== leaveType && x.Person.IsActive == status).ToList() :
                                                  FindAllByCondition(x => (x.FromDate.Month == month || x.ToDate.Month == month) && (x.FromDate.Year == year || x.ToDate.Year == year)).Include(x => x.Person).Include(x=>x.Person.Role).Include(x => x.Person.Location).Where(x => x.Person.EmployeeCode == employeeId && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == locationId && x.Status== leaveType && x.Person.IsActive == status).ToList();
                    }

                }
            }
            int idData = 0;
            foreach(var data in leaveData)
            {
                idData = data.UpdatedBy.GetValueOrDefault();
                LeaveRequestViewModel leave = new LeaveRequestViewModel
                {
                    Id = data.Id,
                    LocationName = data.Person.Location.LocationName,
                    EmployeeCode = data.Person.EmployeeCode,
                    EmployeeName = data.EmployeeName,
                    EmployeeRole = data.Person.Role.Name,
                    RequestedDays = data.RequestedDays,
                    FromDate = data.FromDate,
                    ToDate = data.ToDate,
                    LeaveType = data.LeaveType,
                    Available = data.Available,
                    AppliedDate = data.AppliedDate,
                    Status = data.Status,
                    Reason = string.IsNullOrEmpty(data.Reason) ? "-" : data.Reason,
                    PersonId = data.Person.Id
                };
                
                if (idData != 0)
                {
                    leave.ApprovedName = _dbContext.Person.Where(x => x.Id == idData).FirstOrDefault().FullName;
                }
                else
                {
                    leave.ApprovedName = "-";
                }
                leaveDataView.Add(leave);
            }
            return leaveDataView;
        }

        public List<SP_EmployeeLeaveRequest> GetEmployeeLeaveData(int PersonId)
        {
            List<SP_EmployeeLeaveRequest> sP_EmployeeLeave = new List<SP_EmployeeLeaveRequest>();
            var SP_PersonId = new SqlParameter("@PersonId", PersonId);
            string usp = "LMS.usp_GetEmployeeLeavesData @PersonId";
            sP_EmployeeLeave = _dbContext._sp_EmployeeLeaveRequest.FromSql(usp, SP_PersonId).ToList();
            return sP_EmployeeLeave;

        }

        public LeaveRequestForEdit GetLeaveRequestForEdit(int id)
        {
            LeaveRequest request = _dbContext.LeaveRequests.Where(x => x.Id == id).FirstOrDefault();
            int CreditId = _dbContext.LeaveCredit.Where(x => x.PersonId == request.PersonId && x.LeaveId == request.TypeId).FirstOrDefault().Id;
            LeaveRequestForEdit NewRequest = new LeaveRequestForEdit()
            {
                Id = request.Id,
                AppliedDate = request.AppliedDate,
                ApprovedBy = request.ApprovedBy,
                ApprovedDays = request.ApprovedDays,
                Available = request.Available,
                CreatedBy = request.CreatedBy,
                CreatedDate = request.CreatedDate,
                EmployeeName = request.EmployeeName,
                FromDate = request.FromDate,
                IsActive = request.IsActive,
                LeaveType = request.LeaveType,
                PersonId = request.PersonId,
                Reason = request.Reason,
                RequestedDays = request.RequestedDays,
                RowVersion = request.RowVersion,
                Status = request.Status,
                TenantId = request.TenantId,
                ToDate = request.ToDate,
                TypeId = request.TypeId,
                UpdatedBy = request.UpdatedBy,
                UpdatedDate = request.UpdatedDate,
                CreditId = CreditId
            };
            return NewRequest;
        }

        public List<SP_LeavePoliciesInDetail> GetLeavePoliciesInDetails(int PersonId)
        {
            List<SP_LeavePoliciesInDetail> LeavePoliciesInDetails = new List<SP_LeavePoliciesInDetail>();
            var SP_PersonId = new SqlParameter("@PersonId", PersonId);
            string usp = "LMS.usp_GetEmployeePoliciesInDetails @PersonId";
            LeavePoliciesInDetails = _dbContext._sp_GetLeavePoliciesInDetail.FromSql(usp, SP_PersonId).ToList();
            return LeavePoliciesInDetails;
        }

        public int GetPendingLeavesCount()
        {
            int result = 0;
            ActualLeaveCount Model = new ActualLeaveCount();

            string usp = "select * from [LMS].[GetPendingLeavesCount]()";
            Model = _dbContext._sp_GetPendingLeaveCount.FromSql(usp).FirstOrDefault();
            if (Model != null) result = Model.LeaveCount;
            return result;
        }
    }
}
