using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using stock_dotnet.utils.db;

namespace stock_dotnet.stock
{

    public class StockRepository
    {
        private readonly IMongoCollection<Stock> _stocks;

        public StockRepository(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _stocks = database.GetCollection<Stock>(settings.CollectionName);
        }
        public List<Stock> GetStocks() => _stocks.Find(s => true).ToList();
        public Stock GetLastStock(string articleId) => _stocks.Find(s => s.ArticleId == articleId && s.Enabled).FirstOrDefault();
        public Stock Create(Stock stock)
        {
            _stocks.InsertOne(stock);
            return stock;
        }
        public void Update(Stock stock)
        {
            var update = Builders<Stock>.Update
            .Set(a => a.Details, stock.Details)
            .Set(a => a.Enabled, stock.Enabled)
            .Set(s => s.LastModification, DateTime.Now);
            _stocks.UpdateOne(model => model.Id == stock.Id, update);
        }
        public void Delete(Stock stock) =>
            Delete(stock.ArticleId);

        public void Delete(string articleId) =>
             _stocks.DeleteMany(s => s.ArticleId == articleId);

    }

}