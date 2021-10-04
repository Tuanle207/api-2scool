using Scool.Application.ObjectExtentions;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Scool
{
    public static class ScoolDtoExtensions
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

        public static void Configure()
        {
            OneTimeRunner.Run(() =>
            {
                /* You can add extension properties to DTOs
                 * defined in the depended modules.
                 *
                 * Example:
                 *
                 * ObjectExtensionManager.Instance
                 *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
                 *
                 * See the documentation for more:
                 * https://docs.abp.io/en/abp/latest/Object-Extensions
                 */

                ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserCreateDto, Guid?>(
                    IdentityUserCreateDtoExt.ClassId, opt => opt.DefaultValue = null);

                ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserCreateDto, DateTime?>(
                   IdentityUserCreateDtoExt.Dob, opt => opt.DefaultValue = null);

                
            });
        }
    }
}