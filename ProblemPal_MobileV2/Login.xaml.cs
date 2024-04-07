using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ProblemPal_MobileV2.Views;

public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public bool IsActivated { get; set; }
}

namespace ProblemPal_MobileV2
{
    public partial class Login : ContentPage
    {

        private readonly string dbPath;

        public Login()
        {
            InitializeComponent();
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db");
            using (var context = new AppDbContext(dbPath))
            {
                context.Database.EnsureCreated();
            }
        }

        private async void AaccBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new MainPage());
             
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private async void LoginBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                string studentID = StudentIDEntry.Text;
                string email = EmailEntry.Text;
                string password = PasswordEntry.Text;

                // Check if the entered credentials are for an admin account
                if (IsAdminLogin(email, password))
                {
                    // Navigate to the AdminWindow.xaml page if it's an admin login
                    await Navigation.PushAsync(new AdminWindow());
                }
                else
                {
                    // Check if student account is activated
                    bool isActivated = await CheckAccountActivation(studentID);

                    // Perform login if the account is activated
                    if (isActivated)
                    {
                        // Check if the entered credentials are correct
                        bool loginSuccess = await PerformLogin(studentID, email, password);
                        if (loginSuccess)
                        {
                            // Navigate to the MainPage if login is successful
                            await Navigation.PushAsync(new MainPage());
                        }
                        else
                        {
                            await DisplayAlert("Login Failed", "Invalid student ID, email, or password.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Account Not Activated", "Please activate your account first through email.", "OK");
                    }
                }
            }
            catch (FormatException)
            {
                await DisplayAlert("Invalid Input", "Please enter valid input values.", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private bool IsAdminLogin(string email, string password)
        {

            return email == "admin@example.com" && password == "adminpassword";
        }

        private async Task<bool> CheckAccountActivation(string studentID)
        {
            using (var context = new AppDbContext(dbPath))
            {
                var student = await context.Students.FindAsync(int.Parse(studentID));
                return student != null && student.IsActivated;
            }
        }

        private async Task<bool> PerformLogin(string studentID, string email, string password)
        {
            using (var context = new AppDbContext(dbPath))
            {
                var student = await context.Students.FirstOrDefaultAsync(s => s.StudentId == int.Parse(studentID) && s.Email == email && s.Password == password);
                return student != null;
            }
        }
    }
}