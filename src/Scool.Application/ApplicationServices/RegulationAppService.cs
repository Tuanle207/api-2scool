using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class RegulationAppService : BasicCrudAppService<
        Regulation,
        Guid,
        RegulationDto,
        RegulationDto,
        CreateUpdateRegulationDto,
        CreateUpdateRegulationDto
    >, IRegulationsAppService
    {
        private readonly IRepository<Regulation, Guid> _regulationsRepo;

        public RegulationAppService(IRepository<Regulation, Guid> regulationsRepo) : base(regulationsRepo)
        {
            _regulationsRepo = regulationsRepo;
        }

        public async Task<PagingModel<RegulationForSimpleListDto>> GetSimpleListAsync()
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<RegulationForSimpleListDto>();
            }

            var regulations = await _regulationsRepo
                .Where(x => x.IsActive == true && x.CourseId == ActiveCourse.Id.Value)
                .Include(x => x.Criteria)
                .OrderBy(x => x.DisplayName)
                .Select(x => ObjectMapper.Map<Regulation, RegulationForSimpleListDto>(x))
                .ToListAsync();

            var result = new PagingModel<RegulationForSimpleListDto>
            (
                items: regulations,
                totalCount: regulations.Count
            );

            return result;
        }

        public override async Task<PagingModel<RegulationDto>> PostPagingAsync(PageInfoRequestDto input)
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<RegulationDto>();
            }

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            
            var query = _regulationsRepo.AsNoTracking()
                .Filter(input.Filter)
                .Where(x => x.IsActive == true && x.CourseId == ActiveCourse.Id.Value);
            
            var totalCount = await query.CountAsync();
            
            query = string.IsNullOrEmpty(input.SortName) ? query.OrderBy(x => x.Id) : query.OrderBy(input.SortName, input.Ascend);
            query = query.Page(pageIndex, pageSize);
            query = query.Include(e => e.Criteria);

            var items = await query.Select(x => ObjectMapper.Map<Regulation, RegulationDto>(x))
                .ToListAsync();

            return new PagingModel<RegulationDto>(items, totalCount, pageIndex, pageSize);
        }

        public async override Task<RegulationDto> UpdateAsync(Guid id, CreateUpdateRegulationDto input)
        {
            // Change state of this regulation
            var oldRegulation = await _regulationsRepo.FirstOrDefaultAsync(x => x.Id == id);
            oldRegulation.IsActive = false;

            // Create new regulation as new updation
            var newRegulation = ObjectMapper.Map<CreateUpdateRegulationDto, Regulation>(input);
            newRegulation.TenantId = CurrentTenant.Id;
            await _regulationsRepo.InsertAsync(newRegulation);
            await CurrentUnitOfWork.SaveChangesAsync();
            
            var regulation = await _regulationsRepo.Include(e => e.Criteria)
                .FirstOrDefaultAsync(x => x.Id == newRegulation.Id);
            var result = ObjectMapper.Map<Regulation, RegulationDto>(regulation);
            return result;
        }

        public async override Task<RegulationDto> CreateAsync(CreateUpdateRegulationDto input)
        {
            var newRegulation = ObjectMapper.Map<CreateUpdateRegulationDto, Regulation>(input);
            newRegulation.CourseId = ActiveCourse.Id.Value;
            newRegulation.TenantId = CurrentTenant.Id;
            await _regulationsRepo.InsertAsync(newRegulation);
            await CurrentUnitOfWork.SaveChangesAsync();
            
            var regulation = await _regulationsRepo.Include(e => e.Criteria)
                .FirstOrDefaultAsync(x => x.Id == newRegulation.Id);
            var result = ObjectMapper.Map<Regulation, RegulationDto>(regulation);
            return result;
        }

        public async override Task<RegulationDto> GetAsync(Guid id)
        {
            var regulation = await _regulationsRepo.Include(e => e.Criteria)
                .FirstOrDefaultAsync(x => x.Id == id);
            var result = ObjectMapper.Map<Regulation, RegulationDto>(regulation);
            return result;
        }
    }
}
