using System;
using System.Collections.Generic;

namespace MyHomework.WebApp.DatabaseModels
{
    public partial class Attachment
    {
        public long AttachmentId { get; set; }
        public long MessageId { get; set; }
        public string FileName { get; set; }
        public string StorageUrl { get; set; }

        public virtual Message Message { get; set; }
    }
}
