using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace OLOChallenge.Entities
{
    public abstract class EntityBase : IEntity
    {
        public StringContent GetStringContent() => new StringContent(
                    JsonConvert.SerializeObject(this),
                    Encoding.UTF8,
                    "application/json");
    }

}
