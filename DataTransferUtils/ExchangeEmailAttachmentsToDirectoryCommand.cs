using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;
using System.IO;
using CommandLine;

namespace DataTransferUtils
{
    public class ExchangeEmailAttachmentsToDirectoryCommand : Command
    {
        [Option('f', "from-regex", Required = false, HelpText = "Regex pattern the email's from address must match.")]
        public string EmailFromMatchRegex { get; set; }

        [Option('s', "subject-regex", Required = false, HelpText = "Regex pattern the email's subject must match.")]
        public string EmailSubjectMatchRegex { get; set; }

        [Option('b', "body-regex", Required = false, HelpText = "Regex pattern the email's body must match.")]
        public string EmailBodyMatchRegex { get; set; }

        [Option('a', "attachment-name-regex", Required = false, HelpText = "Regex pattern the attachment's name must match.")]
        public string AttachmentNameMatchRegex { get; set; }

        [Option('t', "target-directory-path", Required = true, HelpText = "The path to the directory where the attachment will be saved.")]
        public string TargetDirectoryPath { get; set; }

        public override void Execute(Configuration config)
        {
            var service = GetExchangeService(config.ExchangeServiceUrl, config.ExchangeUsername, config.ExchangePassword);
            var collectedFolderId = GetExchangeFolderId(service, "Collected");
            var emailIds = GetExchangeEmailIds(service, WellKnownFolderName.Inbox);

            foreach (var id in emailIds)
            {
                var ps = new PropertySet(
                    EmailMessageSchema.From,
                    EmailMessageSchema.Subject,
                    EmailMessageSchema.Body,
                    EmailMessageSchema.HasAttachments,
                    EmailMessageSchema.Attachments
                    );

                var email = EmailMessage.Bind(service, id);

                if (!email.HasAttachments) // No need to process this email any further
                    continue;

                if (IsMatch(email.From.Address, this.EmailFromMatchRegex)
                    && IsMatch(email.Subject, this.EmailSubjectMatchRegex)
                    && IsMatch(email.Body, this.EmailBodyMatchRegex))
                {
                    var atLeasOneAttachmentSaved = false;

                    foreach (var attachment in email.Attachments)
                    {
                        if (attachment is FileAttachment && IsMatch(attachment.Name, this.AttachmentNameMatchRegex))
                        {
                            var filePath = Path.Combine(this.TargetDirectoryPath, attachment.Name);
                            (attachment as FileAttachment).Load(filePath);
                            atLeasOneAttachmentSaved = true;
                        }
                    }

                    if (atLeasOneAttachmentSaved)
                    {
                        email.Move(collectedFolderId);
                    }
                }
            }
        }

        private static ExchangeService GetExchangeService(string url, string username, string password)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2010);
            service.Url = new Uri(url);
            service.Credentials = new WebCredentials(username, password);
            return service;
        }

        private static FolderId GetExchangeFolderId(ExchangeService service, string displayName)
        {
            var view = new FolderView(Int32.MaxValue);
            view.PropertySet = new PropertySet(FolderSchema.Id, FolderSchema.DisplayName);
            var results = service.FindFolders(WellKnownFolderName.MsgFolderRoot, view);

            foreach (var folder in results.Folders)
            {
                if (folder.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                {
                    return folder.Id;
                }
            }

            throw new Exception($"Folder with display name '{displayName}' could not be found.");
        }

        private static IEnumerable<ItemId> GetExchangeEmailIds(ExchangeService service, WellKnownFolderName folderName)
        {
            var view = new ItemView(Int32.MaxValue);
            return service.FindItems(folderName, view).Select(i => i.Id);
        }

        private static bool IsMatch(string input, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return true;

            return Regex.IsMatch(input, pattern);
        }
    }
}
