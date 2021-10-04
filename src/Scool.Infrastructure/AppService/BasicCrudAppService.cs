using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Scool.Infrastructure.Linq;
using Scool.Infrastructure.Common;

namespace Scool.Infrastructure.ApplicationServices
{
      public abstract class BasicCrudAppService
      <
          TEntity,
          TKey,
          TGetDto,
          TGetListDto,
          TCreateDto,
          TUpdateDto
      > : CrudAppService
      <
          TEntity,
          TGetDto,
          TGetListDto,
          TKey,
          PagedAndSortedResultRequestDto,
          TCreateDto,
          TUpdateDto
      >
            where TEntity : class, IEntity<TKey>
            where TGetDto : IEntityDto<TKey>
            where TGetListDto : IEntityDto<TKey>
      {
            protected new IRepository<TEntity, TKey> Repository { get; }

            protected BasicCrudAppService(IRepository<TEntity, TKey> repository)
                    : base(repository)
            {
                Repository = repository;
            }

            public virtual async Task<PagingModel<TGetListDto>> PostPagingAsync(PageInfoRequestDto input)
            {

                var pageSize = input.PageSize > 0 ? input.PageSize : 10;
                var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
                var query = Repository.Filter(input.Filter);
                query = string.IsNullOrEmpty(input.SortName) ? query.OrderBy(x => x.Id) : query.OrderBy(input.SortName, input.Ascend);
                query = query.Page(pageIndex, pageSize);

                var items = ObjectMapper.Map<List<TEntity>, List<TGetListDto>>(await query.ToListAsync());
                var totalCount = await Repository.Filter(input.Filter).CountAsync();

                return new PagingModel<TGetListDto>(items, totalCount, pageIndex, pageSize);
            }

            public override Task<PagedResultDto<TGetListDto>> GetListAsync(PagedAndSortedResultRequestDto input)
            {
                return null;
            }
      }
}