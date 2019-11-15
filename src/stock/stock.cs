using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;
using System;
using stock_dotnet.utils.Exceptions;
using stock_dotnet.utils.env;

namespace stock_dotnet.stock
{
    public class Stock
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime LastModification { get; set; }
        public StockDetail[] Details { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ArticleId { get; set; }

        public bool Enabled { get; set; }



    }

    public class StockService
    {
        private readonly StockRepository _repository;

        public StockService(StockRepository repository)
        {
            this._repository = repository;
        }

        public void Update(vo.VoStock vo) => _repository.Update(vo.toEntity());


        public void Create(vo.VoStock vo)
        {
            Stock stock = _repository.GetLastStock(vo.ArticleId);
            if (stock != null) throw new BusinessException(Const.BusinessException_CANT_CREATE);
            stock = vo.toEntity();
            _repository.Create(stock);
        }

        public void Delete(string articleId)
        {
            Stock stock = _repository.GetLastStock(articleId);
            if (stock != null)
            {
                stock.Enabled = false;
                _repository.Update(stock);
            }

        }


        public vo.VoStock Get(string articleId)
        {
            Stock stock = _repository.GetLastStock(articleId);
            return stock != null ? stock.toVo() : throw new NotFoundException(string.Format(Const.NotFoundException_Message, articleId));
        }

        public string Reserve(vo.VoReservation vo)
        {
            Stock stock = _repository.GetLastStock(vo.ArticleId);
            DateTime now = DateTime.Now;
            if (stock == null) throw new NotFoundException(string.Format(Const.NotFoundException_Message, vo.ArticleId));
            var details = stock.Details.Where(s => (s.ReserveDate == null
            ||(int)(now -s.ReserveDate.Value).TotalMinutes>=5)
            && s.Confirmed != true).ToList();
            if (details.Count() < vo.Quantity) throw new BusinessException(Const.BusinessException_Stock_Not_Available);
            string reservartionId = ObjectId.GenerateNewId().ToString();
            details.Take(vo.Quantity).ToList().ForEach(d =>
            {
                d.ReserveDate = DateTime.Now;
                d.ReservationId = reservartionId;
            });
            _repository.Update(stock);
            return reservartionId;
        }
        public vo.VoStock Confirm(vo.VoReserveConfirmation vo)
        {
            var stock = _repository.GetStocks()
                                .Where(s => s.Details.Select(k => k.ReservationId).Contains(vo.ReservationId)).FirstOrDefault();
            if (stock == null) throw new BusinessException(Const.BusinessException_Reservation_Not_Available);
            var details = stock.Details.Where(d => d.ReservationId == vo.ReservationId &&
                                d.ReserveDate.Value.AddMinutes(Const.Reservation_Expiration_Time) >= DateTime.Now &&
                                d.Confirmed != true).ToList();


            if (details.Count == 0) throw new BusinessException(Const.BusinessException_Reservation_Not_Available);
            details.ToList().ForEach(s =>
            {
                s.Confirmed = true;
            });
            _repository.Update(stock);
            return stock.toVo();
        }
    }

    public static class StockFactory
    {

        public static vo.VoStock toVo(this Stock stock)
         => new vo.VoStock
         {
             Id = stock.Id,
             ArticleId = stock.ArticleId,
             LastModification = stock.LastModification,
             Quantity = stock.Details?.Count(s =>
             (s.ReserveDate == null || s.ReserveDate.Value.AddMinutes(Const.Reservation_Expiration_Time) <= DateTime.Now)
             && s.Confirmed != true) ?? 0
         };
        public static Stock toEntity(this vo.VoStock vo)
  => new Stock
  {
      Id = vo.Id,
      ArticleId = vo.ArticleId,
      LastModification = DateTime.Now,
      Details = CreateDetails(vo.Quantity),
      Enabled = true
  };
        private static StockDetail[] CreateDetails(int quantity)
        {
            var result = new StockDetail[quantity];
            for (int i = 0; i < quantity; i++)
            {
                result[i] = new StockDetail();
            }
            return result;
        }

    }
}