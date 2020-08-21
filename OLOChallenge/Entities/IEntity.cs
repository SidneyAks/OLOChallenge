using System.Net.Http;

namespace OLOChallenge.Entities
{
    public interface IEntity
    {
        StringContent GetStringContent();
    }

}
