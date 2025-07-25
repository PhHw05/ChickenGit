using MongoDB.Driver;
using MongoDB.Bson;
using MongoCore.Models.ModelViews;
using System;

namespace MongoCore.Models.Repositoties
{
    public class CateRepository
    {
        private readonly IMongoCollection<CateM> _collection;

        public CateRepository(IMongoCollection<CateM> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public List<CateM> GetAll()
        {
            try
            {
                var result = _collection.Find(_ => true).ToList();
                Console.WriteLine($"GetAll: Found {result.Count} records");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return new List<CateM>();
            }
        }

        public CateM? GetById(string id)
        {
            try
            {
                var result = _collection.Find(c => c._id == ObjectId.Parse(id)).FirstOrDefault();
                Console.WriteLine(result != null ? $"GetById: Found _id={id}, nameCate={result.nameCate}" : $"GetById: Not found _id={id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public CateM? GetByName(string name)
        {
            try
            {
                var result = _collection.Find(c => c.nameCate == name).FirstOrDefault();
                Console.WriteLine(result != null ? $"GetByName: Found nameCate={name}" : $"GetByName: Not found nameCate={name}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByName: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public int GetNextId()
        {
            try
            {
                var maxId = _collection.Find(_ => true).SortByDescending(c => c.idCate).FirstOrDefault()?.idCate ?? 0;
                Console.WriteLine($"GetNextId: Current max idCate={maxId}, Next idCate={maxId + 1}");
                return maxId + 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNextId: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return 1;
            }
        }

        public bool Add(CateM cate)
        {
            try
            {
                if (cate == null)
                {
                    Console.WriteLine("Add: CateM is null");
                    return false;
                }
                cate.idCate = GetNextId();
                Console.WriteLine($"Adding: _id={cate._id}, nameCate={cate.nameCate}, idCate={cate.idCate}");
                _collection.InsertOne(cate);
                Console.WriteLine($"Add successful: _id={cate._id}, nameCate={cate.nameCate}, idCate={cate.idCate}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public bool Update(CateM cate)
        {
            try
            {
                if (cate == null)
                {
                    Console.WriteLine("Update: CateM is null");
                    return false;
                }
                Console.WriteLine($"Updating: _id={cate._id}, nameCate={cate.nameCate}, idCate={cate.idCate}");
                var result = _collection.ReplaceOne(c => c._id == cate._id, cate);
                Console.WriteLine($"Update result: Matched={result.MatchedCount}, Modified={result.ModifiedCount}");
                return result.MatchedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                var result = _collection.DeleteOne(c => c._id == ObjectId.Parse(id));
                Console.WriteLine($"Delete: _id={id}, DeletedCount={result.DeletedCount}");
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }
    }
}