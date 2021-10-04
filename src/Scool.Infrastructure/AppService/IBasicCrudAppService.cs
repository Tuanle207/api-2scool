using System.Threading.Tasks;
using Scool.Infrastructure.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Scool.Infrastructure.ApplicationServices
{

    /**
      Inherit all basic CRUD AppService Interface from this Base Interface
    */
    public interface IBasicCrudAppService<
          TKey, // Type of primary key
          TGetDto, // Type of get DTO
          TGetListDto, // Type of get List DTO
          TCreateDto, // Type of Create DTO
          TUpdateDto // Type of Update DTO
    > : ICrudAppService
    <
        TGetDto,
        TGetListDto,
        TKey,
        PagedAndSortedResultRequestDto,
        TCreateDto,
        TUpdateDto
    >
        where TGetDto : IEntityDto<TKey>
        where TGetListDto : IEntityDto<TKey>
    {
        Task<PagingModel<TGetListDto>> PostPagingAsync(PageInfoRequestDto input);
    }
}