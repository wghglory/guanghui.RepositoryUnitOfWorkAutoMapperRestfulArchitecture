
 
using Guanghui.Repository.GenericRepository;

namespace Guanghui.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
		GenericRepository<Product> ProductRepository { get; }
		GenericRepository<Token> TokenRepository { get; }
		GenericRepository<User> UserRepository { get; }
		void Save();
    }
}

	