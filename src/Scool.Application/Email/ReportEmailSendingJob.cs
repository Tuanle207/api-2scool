using Scool.Dtos;
using Scool.IApplicationServices;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Scool.Email
{
    public class ReportEmailSendingJob : AsyncBackgroundJob<ReportEmailSendingArgs>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IStatisticsAppService _statisticsAppService;
        private readonly IDataFilter _dataFilter;

        public ReportEmailSendingJob(IEmailSender emailSender, IStatisticsAppService statisticsAppService, IDataFilter dataFilter)
        {
            _emailSender = emailSender;
            _statisticsAppService = statisticsAppService;
            _dataFilter = dataFilter;
        }

        public override async Task ExecuteAsync(ReportEmailSendingArgs args)
        {
            var content = await GetReportStreamContent(args);

            using (var stream = content.GetStream())
            {
                var bytes = await stream.GetAllBytesAsync();
                await _emailSender.SendAsync(args.To, args.Subject, args.Body, new List<EmailAttachment> {
                    new EmailAttachment
                    {
                        Filename = args.ReportName,
                        Content = bytes
                    }
                });
            }
        }

        private async Task<IRemoteStreamContent> GetReportStreamContent(ReportEmailSendingArgs args)
        {
            IRemoteStreamContent content = null;

            if (!args.EntityId.HasValue)
            {
                return content;
            }

            var dateTimeFilter = new TimeFilterDto {
                StartTime = args.StartTime,
                EndTime = args.EndTime
            };

            using (_dataFilter.Disable<IMultiTenant>())
            {
                if (args.ReportType == EmailReportType.Class)
                {
                    content = await _statisticsAppService.GetClassFaultDetailsExcel(args.EntityId.Value, dateTimeFilter);
                } 
                else if (args.ReportType == EmailReportType.Student)
                {
                    content = await _statisticsAppService.GetStudentFaultDetailsExcel(args.EntityId.Value, dateTimeFilter);
                }
            }

            return content;
        }
    }
}
