namespace MyHomework.WebApp.DatabaseModels
{
    using Newtonsoft.Json;

    public partial class Attachment
    {
        public long AttachmentId { get; set; }
        public long MessageId { get; set; }
        public string FileName { get; set; }
        public string StorageUrl { get; set; }

        [JsonIgnore]
        public virtual Message Message { get; set; }
    }
}
