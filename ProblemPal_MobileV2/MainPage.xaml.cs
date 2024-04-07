using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

public class Complaint
{
    [Key]
    public int ComplaintId { get; set; }

    [Required]
    public string? ComplaintText { get; set; } = ""; // Initialize with a non-null value

    [Required]
    public string Type { get; set; } = "";

    [Required]
    public DateTime Date { get; set; }
}

namespace ProblemPal_MobileV2
{
    public partial class MainPage : ContentPage
    {
        private readonly string _dbPath;

        public MainPage()
        {
            InitializeComponent();
            _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db");
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            string complaintText = ComplaintEntry.Text;

            if (!string.IsNullOrEmpty(complaintText))
            {
                var newComplaint = new Complaint
                {
                    ComplaintText = complaintText,
                    Date = DateTime.Now
                };

                try
                {
                    using (var context = new AppDbContext(_dbPath))
                    {
                        context.Complaints.Add(newComplaint);
                        await context.SaveChangesAsync();
                    }

                    await DisplayAlert("Success", "Complaint submitted successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter a complaint.", "OK");
            }
        }
    }
}
