using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace MongoCore.Models.ModelViews
{
    public class UserM
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("idUser")]
        public int idUser { get; set; }

        [BsonElement("nameUser")]
        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        public string nameUser { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }

        [BsonElement("Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [BsonElement("Password")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }

        [BsonElement("Role")]
        [Required(ErrorMessage = "Quyền là bắt buộc")]
        public string Role { get; set; }

        [BsonElement("Address")]
        public string Address { get; set; }

        [BsonElement("Phone")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
    }
}
