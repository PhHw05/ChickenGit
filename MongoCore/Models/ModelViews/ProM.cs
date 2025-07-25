using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MongoCore.Models.ModelViews
{
    public class ProM
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        [BsonElement("namePro")]
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string namePro { get; set; }

        [BsonElement("CateId")]
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int CateId { get; set; }

        [BsonElement("image")]
        public string Image { get; set; }

        [BsonElement("variants")]
        public List<Variant> Variants { get; set; } = new List<Variant>();
    }

    public class Variant
    {
        [BsonElement("sku")]
        [Required(ErrorMessage = "Mã SKU là bắt buộc")]
        public string Sku { get; set; }

        [BsonElement("color")]
        [Required(ErrorMessage = "Màu sắc là bắt buộc")]
        public string Color { get; set; }

        [BsonElement("storage")]
        [Required(ErrorMessage = "Dung lượng là bắt buộc")]
        public string Storage { get; set; }

        [BsonElement("price")]
        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [BsonElement("stock")]
        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int Stock { get; set; }

        [BsonElement("images")]
        public List<string> Images { get; set; } = new List<string>();
    }
}