using brain_back_domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace brain_back_infrastructure.Data
{
    public class BrainContext : DbContext
    {

        public BrainContext(DbContextOptions<BrainContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }

    }
}