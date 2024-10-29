using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Shared;
using UserService.Service;

namespace UserService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessagePublisher _rabbitMQPublisher;

        public UserController(IUserService userService, IMessagePublisher rabbitMQPublisher)
        {
            _userService = userService;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [HttpPost]
        public async Task<ActionResult> Create(User body)
        {
            // Criação do usuário usando o serviço injetado
            var result = await _userService.Create(body);

            // Verifica se a criação foi bem-sucedida antes de enviar a notificação
            if (result == null)
            {
                return BadRequest("Failed to create user");
            }

            var userEvent = new UserNotification
            {
                UserId = result.Id,
                Nome = result.Nome,
                Email = result.Email,
                Saldo = result.Saldo,
                DataCriacao = result.DataCriacao
            };
            _rabbitMQPublisher.PublishUserCreated(userEvent);

            return Ok(result);
        }
    }


}
