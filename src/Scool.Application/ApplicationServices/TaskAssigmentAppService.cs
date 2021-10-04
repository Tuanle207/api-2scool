using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Application.Permissions;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.Application.ApplicationServices
{
    public class TaskAssigmentAppService : BasicCrudAppService<
            TaskAssignment,
            Guid,
            TaskAssignmentDto,
            TaskAssignmentDto,
            CreateUpdateTaskAssignmentDto,
            CreateUpdateTaskAssignmentDto
        >, ITaskAssigmentAppService
    {
        private readonly IRepository<TaskAssignment, Guid> _taskAssignmentRepo;
        private readonly IRepository<AppUser, Guid> _usersRepo;
        private readonly IRepository<UserProfile, Guid> _userProfilesRepo;

        public TaskAssigmentAppService(IRepository<TaskAssignment, Guid> taskAssignmentRepo,
            IRepository<UserProfile, Guid> userProfilesRepo,
            IRepository<AppUser, Guid> usersRepo)
            : base(taskAssignmentRepo)
        {
            _taskAssignmentRepo = taskAssignmentRepo;
            _userProfilesRepo = userProfilesRepo;
            _usersRepo = usersRepo;
        }
        
        [Authorize(TaskAssignmentPermissions.AssignDcpReport)]
        [HttpPost("api/app/task-assigment/create-update-schedule")]
        public async Task CreateUpdateAsync(CreateUpdateTaskAssignmentDto input)
        {
            // Clean all previous assigments
            // TODO: instead of cleaning all stuff, we will change state of that, so that we can have somthing like "changes history"
            var preItemIds = await _taskAssignmentRepo
                .Where(x => x.TaskType == input.TaskType)
                .Select(x => x.Id)
                .ToListAsync();
            await _taskAssignmentRepo.DeleteManyAsync(preItemIds);

            await CurrentUnitOfWork.SaveChangesAsync();

            var tasksNeedAssign = new List<TaskAssignment>();

            foreach (var item in input.Items)
            {
                tasksNeedAssign.Add(new TaskAssignment
                {
                    AssigneeId = item.AssigneeId,
                    ClassAssignedId = item.ClassId,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    TaskType = input.TaskType
                });
            }

            await _taskAssignmentRepo.InsertManyAsync(tasksNeedAssign);
        }

        [Authorize(TaskAssignmentPermissions.GetScheduleList)]
        [HttpGet("api/app/task-assigment/get-schedules")]
        public async Task<PagingModel<TaskAssignmentDto>> GetAllAsync(TaskAssignmentFilterDto input)
        {
            var query = _taskAssignmentRepo
                     .WhereIf(!String.IsNullOrEmpty(input.TaskType), x => x.TaskType == input.TaskType)
                     .Include(x => x.ClassAssigned)
                     .Include(x => x.AssigneeProfile)
                     .ThenInclude(y => y.Class)
                     .Select(x => ObjectMapper.Map<TaskAssignment, TaskAssignmentDto>(x));

            var items = await query.ToListAsync();

            if (items.Count > 0 && items[0].CreatorId != null)
            {
                var creator = await _usersRepo
                   .Where(x => x.Id == items[0].CreatorId)
                   .Select(x => ObjectMapper.Map<AppUser, UserForSimpleListDto>(x))
                   .FirstOrDefaultAsync();
                foreach (var item in items)
                {
                    item.Creator = creator;
                }
            }

            return new PagingModel<TaskAssignmentDto>(items, items.Count);
        }

        [Authorize(TaskAssignmentPermissions.GetScheduleList)]
        [HttpGet("api/app/task-assigment/get-schedules-for-update")]
        public async Task<PagingModel<TaskAssignmentForUpdateDto>> GetForUpdateAsync(TaskAssignmentFilterDto input)
        {
            var query = _taskAssignmentRepo
                    .WhereIf(String.IsNullOrEmpty(input.TaskType), x => x.TaskType == input.TaskType)
                    .Select(x => ObjectMapper.Map<TaskAssignment, TaskAssignmentForUpdateDto>(x));

            var items = await query.ToListAsync();

            return new PagingModel<TaskAssignmentForUpdateDto>(items, items.Count);
        }

        [Authorize(ReportsPermissions.CreateNewDcpReport)]
        [HttpGet("api/app/task-assigment/assigned-class-for-dcp-report")]
        public async Task<PagingModel<ClassForSimpleListDto>> GetAssignedClassesForDcpReportAsync([FromQuery] string taskType)
        {
            var emptyRes = new PagingModel<ClassForSimpleListDto>(new List<ClassForSimpleListDto>(), 0);
            if (CurrentUser.Id != null)
            {
                var profile = await _userProfilesRepo.Where(x => x.UserId == CurrentUser.Id).FirstOrDefaultAsync();
                if (profile == null)
                {
                    return emptyRes;
                }

                var items = await _taskAssignmentRepo
                    .Where(x => x.AssigneeId == profile.Id)
                    .Where(x => x.TaskType == taskType)
                    .Include(x => x.ClassAssigned)
                    .Select(x => ObjectMapper.Map<Class, ClassForSimpleListDto>(x.ClassAssigned))
                    .ToListAsync();

                return new PagingModel<ClassForSimpleListDto>(items, items.Count);
            }
            else
            {
                return emptyRes;
            }
        }
    }
}
