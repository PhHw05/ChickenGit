using MongoCore.Models.ModelViews;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace MongoCore.Models.Repositoties
{
    public class ProRepository
    {
        private readonly IMongoCollection<ProM> _collection;

        public ProRepository(IMongoCollection<ProM> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public List<ProM> GetAll()
        {
            try
            {
                var result = _collection.Find(_ => true).ToList();
                Console.WriteLine($"GetAll: Found {result.Count} products");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return new List<ProM>();
            }
        }

        public ProM? GetById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                {
                    Console.WriteLine($"GetById: Invalid ID={id}");
                    return null;
                }
                var result = _collection.Find(c => c._id == objectId).FirstOrDefault();
                Console.WriteLine(result != null ? $"GetById: Found _id={id}, namePro={result.namePro}" : $"GetById: Not found _id={id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public int GetNextId()
        {
            try
            {
                var maxId = _collection.Find(_ => true).SortByDescending(p => p._id).FirstOrDefault()?._id ?? 0;
                Console.WriteLine($"GetNextId: Current max idPro={maxId}, Next idPro={maxId + 1}");
                return maxId + 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNextId: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return 1;
            }
        }

        public bool Add(ProM pro)
        {
            try
            {
                if (pro == null)
                {
                    Console.WriteLine("Add: ProM is null");
                    return false;
                }
                pro._id = GetNextId();
                Console.WriteLine($"Adding: _id={pro._id}, namePro={pro.namePro}, idPro={pro._id}");
                _collection.InsertOne(pro);
                Console.WriteLine($"Add successful: _id={pro._id}, namePro={pro.namePro}, idPro={pro._id}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public bool Update(ProM pro)
        {
            try
            {
                if (pro == null)
                {
                    Console.WriteLine("Update: ProM is null");
                    return false;
                }
                Console.WriteLine($"Updating: _id={pro._id}, namePro={pro.namePro}, idPro={pro._id}");
                var result = _collection.ReplaceOne(c => c._id == pro._id, pro);
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
                if (!ObjectId.TryParse(id, out var objectId))
                {
                    Console.WriteLine($"Delete: Invalid ID={id}");
                    return false;
                }
                var result = _collection.DeleteOne(c => c._id == objectId);
                Console.WriteLine($"Delete: _id={id}, DeletedCount={result.DeletedCount}");
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public List<ProM> SearchByName(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    Console.WriteLine("SearchByName: Query is null or empty");
                    return new List<ProM>();
                }
                var filter = Builders<ProM>.Filter.Regex(p => p.namePro, new BsonRegularExpression(query, "i"));
                var result = _collection.Find(filter).Limit(10).ToList();
                Console.WriteLine($"SearchByName: Found {result.Count} products for query={query}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchByName: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return new List<ProM>();
            }
        }
    }
}