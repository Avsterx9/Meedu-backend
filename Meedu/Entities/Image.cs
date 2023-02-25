namespace Meedu.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
