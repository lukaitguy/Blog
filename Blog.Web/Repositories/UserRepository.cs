using Blog.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            this.authDbContext = authDbContext;
        }
        public async Task<IEnumerable<IdentityUser>> GetAll()
        {
            var users = await authDbContext.Users.ToListAsync();

            var superAdmin = await authDbContext.Users.FirstOrDefaultAsync(u => u.Email == "superadmin@blog.com");

            if(superAdmin is not null)
            {
                users.Remove(superAdmin);
            }
            return users;
        }
    }
}
