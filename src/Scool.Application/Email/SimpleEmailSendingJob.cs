using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Scool.Email
{
    public class SimpleEmailSendingJob : AsyncBackgroundJob<SimpleEmailSendingArgs>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;

        public SimpleEmailSendingJob(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public override async Task ExecuteAsync(SimpleEmailSendingArgs args)
        {
            await _emailSender.SendAsync(args.To, args.Subject, args.Body);
        }

    }
}
