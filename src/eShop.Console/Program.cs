using eShop.Domain.Models;
using eShop.EF;

Console.WriteLine("Hello, World!");


using eShopDbContext context = new eShopDbContextFactory().CreateDbContext();

User newUser = new User {
    Name = "Mo Salah",
};

context.Users.Add(newUser);

context.SaveChanges();


ICollection<User> users = context.Users.ToList();

foreach (var u in users)
{
    Console.WriteLine($"Id: {u.Id}, Name: {u.Name}");
}