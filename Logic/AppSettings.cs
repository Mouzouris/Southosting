
namespace southosting.Logic
{
    public class AppSettings
    {
        public string Title { get; set; }

        public string Website { get; set; }

        public string DefaultPassword { get; set; }
    }

    public class BlobStorage
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string QueueName { get; set; }
        public string Url { get; set; }
        public string ImageContainer { get; set; }
        public string ThumbnailContainer { get; set; }
    }
}