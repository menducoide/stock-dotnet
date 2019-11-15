using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using stock_dotnet.security;
using stock_dotnet.stock;
using stock_dotnet.stock.vo;
using stock_dotnet.utils.Exceptions;
using stock_dotnet.utils.Extensions;
using stock_dotnet.utils.rabbitmq;

namespace stock_dotnet.Controllers
{
    [Route("v1/stock")]
    [ApiController]
    [EnableCors]
    public class RestController : ControllerBase
    {
        public readonly StockService service;
        public readonly Security _security;
        public readonly EmiterRabbit _emiterRabbit;
        public RestController(StockService service, Security security, EmiterRabbit emiterRabbit)
        {
            this.service = service;
            _security = security;
            _emiterRabbit = emiterRabbit;
        }

        /**
         * @api {post} article/:articleId Create stock of an article
         * @apiName Update
         * @apiGroup Stock
         *
         * @apiParam {String} articleId id of article.
         *
          * @apiSuccessExample Success-Response:
        *     HTTP/1.1 200 OK
         * @apiErrorExample 400:
         * HTTP/1.1 400 Bad Request
         *{
         *   "code": 400        
         *   "message": [{"key": "value"}]
         * }
         * @apiErrorExample 401:
         * HTTP/1.1 401 Unauthorized
         *{
         *   "code": 401        
         *   "message": "Unauthorized"
         * }
         *
         */
        [HttpPost("article/{articleId}")]
        [Authorize(Policy = "admin")]
        public IActionResult Create(VoStock data)
        {
            if (ModelState.IsValid)
            {
                service.Create(data);
                return Ok();
            }
            else
            {
                throw new BusinessException(ModelState.ToDictionary());
            }
        }
        /**
    * @api {get} article/:articleId GET stock of an article
    * @apiName GET
    * @apiGroup Stock
    *
    * @apiParam {String} articleId id of article.
    *
    * @apiErrorExample 400:
    * HTTP/1.1 400 Bad Request
    *{
    *   "code": 400        
    *   "message": "stock not found"
    * }
    * @apiErrorExample 401:
    * HTTP/1.1 401 Unauthorized
    *{
    *   "code": 401        
    *   "message": "Unauthorized"
    * }
    *
    */
        [HttpGet("article/{articleId}")]
        [Authorize(Policy = "user-loged")]
        public IActionResult Get(string articleId)
        {
            return Ok(service.Get(articleId));
        }

        /**
  * @api {POST}  /article Reserve stock of an article
  * @apiName POST
  * @apiGroup Stock
  * @apiParam {
    "quantity":"",
    "articleId":""
}  
 
  *
  * @apiSuccessExample Success-Response:
*     HTTP/1.1 200 OK
*     {
*       "reservationId": "5dac7e4bb8b02e0e08505db0"
*     } 
  * @apiErrorExample 400:
  * HTTP/1.1 400 Bad Request
  *{
  *   "code": 400        
  *   "message": "The quantity to reserve isn't avaliable"
  * }
  * @apiErrorExample 401:
  * HTTP/1.1 401 Unauthorized
  *{
  *   "code": 401        
  *   "message": "Unauthorized"
  * }
  *
  */
        [HttpPost("article")]
        [Authorize(Policy = "user-loged")]
        public IActionResult Reserve(VoReservation vo)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(ModelState.ToDictionary());
            string reserveId = service.Reserve(vo);

            return Ok(new VoReserveConfirmation{ReservationId = reserveId});
        }

        /**
* @api {post}   Confirm  stock reserve of an article
* @apiName POST
* @apiGroup Stock
*
* @apiParam {
    "reservationId":""
}  
*
* @apiSuccessExample Success-Response:
*     HTTP/1.1 200 OK
*    {
        "articleId" = "",
        "quantity" = ""
     }
* @apiErrorExample 400:
* HTTP/1.1 400 Bad Request
*{
*   "code": 400        
*   "message": "The reserve isn't avaliable"
* }
* @apiErrorExample 401:
* HTTP/1.1 401 Unauthorized
*{
*   "code": 401        
*   "message": "Unauthorized"
* }
*
*/
        [HttpPost()]
        [Authorize(Policy = "user-loged")]
        public IActionResult Confirm(VoReserveConfirmation vo)
        {
           if (!ModelState.IsValid)
                throw new BusinessException(ModelState.ToDictionary());
            VoStock voStock = service.Confirm(vo);
            var result = new
            {
                articleId = voStock.ArticleId,
                quantity = voStock.Quantity
            };
            var config = new RabbitConfiguration
            {
                Exchange = "stock-updated",
                ExchangeType = "fanout",
                Queue = ""
            };
            _emiterRabbit.Emit(config, JsonConvert.SerializeObject(result));

            return Ok(result);
        }
    }
}
