using System;
using System.Collections.Generic;

namespace MyHomework.WebApp.DatabaseModels
{
    public partial class Message
    {
        public Message()
        {
            Attachment = new HashSet<Attachment>();
        }

        public long MessageId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string Content { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Attachment> Attachment { get; set; }
    }
}
