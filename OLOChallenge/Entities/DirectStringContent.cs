using System.Net.Http;
using System.Text;

namespace OLOChallenge.Entities
{
    public class DirectStringContent : IEntity
    {
        public string Content { get; set; }
        public Encoding Encoding { get; set; }
        public string MimeType { get; set; }
        public StringContent GetStringContent() => new StringContent(
                    Content,
                    Encoding,
                    MimeType);
    }

}
