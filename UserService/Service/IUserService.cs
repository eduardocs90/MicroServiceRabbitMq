using Domain.Entities;

namespace Service.Interfaces
{
    public interface IUserService
    {

        Task<ICollection<User>> FindAll();
        Task<User> FindById(int id);
        Task<User> Create(User body);
        Task<User> Update(User body);
        Task<bool> Delete(int id);



    }
}
