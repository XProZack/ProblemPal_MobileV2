using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

public class AppDbContext : DbContext
{
    private readonly string _dbPath;

    public AppDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public DbSet<Student> Students { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Complaint> Complaints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Configure SQLite connection
        options.UseSqlite($"Data Source={_dbPath}");
    }

    public async Task AddStudent(Student student)
    {
        Students.Add(student);
        await SaveChangesAsync();
    }

    class Program
    {
        static void Main(string[] args)
        {
            string dbPath = "app.db"; // Path to your database file

            using (var context = new AppDbContext(dbPath))
            {
                context.Database.EnsureCreated(); // Ensure the database is created
            }

            Console.WriteLine("SQLite database created successfully!");
        }
    }
}