using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventDressRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IModelService _modelService;
        private readonly IUserService _userService;

        public OrdersController(IOrderService orderService, IModelService modelService, IUserService userService)
        {
            _modelService = modelService;
            _orderService = orderService;
            _userService = userService;
        }
        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> Get()
        {
            List<OrderDTO> list = await _orderService.GetAllOrders();
            if(list == null || list.Count == 0)
                return NoContent();
            return Ok(list);
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            OrderDTO order = await _orderService.GetOrderById(id);
            return order != null ? Ok(order) : NotFound();
        }

        // GET api/<OrdersController>/unpacked
        [HttpGet("unpacked")]
        public async Task<ActionResult<List<OrderDTO>>> GetUnpackedOrdersUntilDate(DateOnly date) 
        {
            if (!_orderService.checkDate(date))
                return BadRequest("date cant be in the past");
            List<OrderDTO> list = await _orderService.GetOrdersByDate(date);
            if (list.Count == 0)
                return NoContent();
            return Ok(list);
        }

        // GET api/<OrdersController>/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrderByUserId(int userId)
        {
            if (await _userService.GetUserById(userId) == null)
                return NotFound(userId);
            List<OrderDTO> list = await _orderService.GetOrderByUserId(userId);
            if (list.Count == 0)
                return NoContent();
            return Ok(list);
        }
        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> AddOrder(NewOrderDTO newOrder)
        {
            bool isValidOrder = await _orderService.checkOrderItems(newOrder);
            if (!isValidOrder)
                return BadRequest("is not valid order");
            if (!await _orderService.checkPrice(newOrder))
                return BadRequest("not match price");
            if(!_orderService.checkDate(newOrder.OrderDate ,newOrder.EventDate))
                return BadRequest("cant match dates");

            OrderDTO orderr = await _orderService.AddOrder(newOrder);
            return CreatedAtAction(nameof(Get), new { Id = orderr.Id }, orderr);
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] NewOrderDTO updateOrder)
        {
            if(!await _orderService.IsExistsOrderById(id))
                return NotFound();
            bool isValidOrder = await _orderService.checkOrderItems(updateOrder);
            if (!isValidOrder)
                return BadRequest("is not valid order");
            if (!await _orderService.checkPrice(updateOrder))
                return BadRequest("not match price");
            if (!_orderService.checkDate(updateOrder.OrderDate, updateOrder.EventDate))
                return BadRequest("cant match dates");

            await _orderService.UpdateOrder(updateOrder, id);
            return Ok();
        }
        // PUT api/<OrdersController>/status/5
        [HttpPut("status/{statusId}")]
        public async Task<IActionResult> UpdateStatusOrder([FromBody] OrderDTO orderDto, int statusId)
        {
            if (!_orderService.checkStatus(statusId))
                return BadRequest("not status match");

            if (!await _orderService.IsExistsOrderById(orderDto.Id))
                return NotFound();

            await _orderService.UpdateStatusOrder(orderDto, statusId);
            return Ok();
        }

    }
}
