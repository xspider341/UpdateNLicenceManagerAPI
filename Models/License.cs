using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdateNLicenceManagerAPI.Models
{
    public class License
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string LicenseKey { get; set; }
        public bool IsActive { get;  set; }
        // Diğer lisans özellikleri...

    }
}
