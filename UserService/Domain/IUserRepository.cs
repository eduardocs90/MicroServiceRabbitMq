

using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<ICollection<User>> FindAll();
        Task<User> FindById(int id);
        Task<User> FindByIdForUpdate(int id);
        Task<User> FindByEmail(string email);
       
     
        Task<User> Create(User body);
        Task<User> Update(User body);
      
        Task<bool> Delete(User body);
        Task<User> Login(string email, string password);
        Task<bool> ValidationFields(string email, string document);


    }
}
