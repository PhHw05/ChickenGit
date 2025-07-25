using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace MongoCore.Models.ModelViews
{
    public class CateM
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("idCate")]
        public int idCate { get; set; }

        [BsonElement("nameCate")]
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        public string nameCate { get; set; }
    }
}