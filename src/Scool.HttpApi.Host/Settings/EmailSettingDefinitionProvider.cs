using Volo.Abp.Emailing;
using Volo.Abp.Settings;

namespace Scool.Settings
{
    public class EmailSettingDefinitionProvider : SettingDefinitionProvider
    {
        private readonly ISettingEncryptionService _encryptionService;

        public EmailSettingDefinitionProvider(ISettingEncryptionService encryptionService)
        {
            this._encryptionService = encryptionService;
        }

        public override void Define(ISettingDefinitionContext context)
        {
            //var passSetting = context.GetOrNull("Abp.Mailing.Smtp.Password");
            //if (passSetting != null)
            //{
            //    string debug = _encryptionService.Encrypt(passSetting, "4ED75BCC2F4024C122FCC7F796C2E6FE1977@");
            //}


            context.Add(
                new SettingDefinition(EmailSettingNames.DefaultFromAddress, "tuanle2x7@gmail.com"),
                new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "2Scool - Hệ thống quản lý nề nếp THPT"),
                new SettingDefinition(EmailSettingNames.Smtp.Host, "smtp.elasticemail.com"),
                new SettingDefinition(EmailSettingNames.Smtp.UserName, "tuanle2x7@gmail.com"),
                new SettingDefinition(EmailSettingNames.Smtp.Port, "587"),
                new SettingDefinition(EmailSettingNames.Smtp.Password, "m7uHT0jgni5/jx9m83F92xPUcB+1kA5k8H/riNbXRksDkqcv8SAtkvmSjF3oUJDo", isEncrypted: true),
                new SettingDefinition(EmailSettingNames.Smtp.EnableSsl, "true"),
                new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials, "true")
            );
        }
    }
}
