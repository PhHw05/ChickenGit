using MongoCore.Models.ModelViews;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace MongoCore.Models.Repositoties
{
    public class UserRepository
    {
        private readonly IMongoCollection<UserM> _collection;

        public UserRepository(IMongoCollection<UserM> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public List<UserM> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        public UserM? GetById(string id)
        {
            try
            {
                return _collection.Find(c => c._id == ObjectId.Parse(id)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById: {ex.Message}");
                return null;
            }
        }

        public int GetMaxId()
        {
            try
            {
                var maxId = _collection.Find(_ => true).SortByDescending(u => u.idUser).FirstOrDefault()?.idUser ?? 0;
                return maxId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMaxId: {ex.Message}");
                return 0;
            }
        }

        public void Add(UserM user)
        {
            try
            {
                _collection.InsertOne(user);
                Console.WriteLine($"Add successfully: {user._id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add: {ex.Message}");
            }
        }

        public void Update(UserM user)
        {
            try
            {
                _collection.ReplaceOne(c => c._id == user._id, user);
                Console.WriteLine($"Update successfully: {user._id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update: {ex.Message}");
            }
        }

        public void Delete(string id)
        {
            try
            {
                _collection.DeleteOne(c => c._id == ObjectId.Parse(id));
                Console.WriteLine($"Delete successfully: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}");
            }
        }
    }
}