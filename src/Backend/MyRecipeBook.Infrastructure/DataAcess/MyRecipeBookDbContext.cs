using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyRecipeBook.Infrastructure.DataAcess;

internal class MyRecipeBookDbContext : DbContext
{
    public MyRecipeBookDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
        
    }

    public DbSet<User> Users { get; set; }
}
