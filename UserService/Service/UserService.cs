
using Domain.Entities;
using Domain.Repositories;
using Service.Interfaces;
using Shared;

namespace UserService.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _rabbitMQPublisher;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Create(User body)
        {
          var result = await _userRepository.Create(body);

            
            return result;

        }

        public async Task<bool> Delete(int id)
        {
            var user = await _userRepository.FindById(id);
            var result = await _userRepository.Delete(user);
            return result;
        }

        public async Task<ICollection<User>> FindAll()
        {
            var result = await _userRepository.FindAll();
            return result;
        }

        public async Task<User> FindById(int id)
        {
            var result = await _userRepository.FindById(id);
            return result;
        }

        public async Task<User> Update(User body)
        {
            var result = await _userRepository.Update(body);
            return result;



        }
    }
}
