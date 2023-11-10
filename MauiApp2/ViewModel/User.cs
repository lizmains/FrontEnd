using SQLite;
namespace MauiApp2.ViewModel;

[Table("user")]
public class User
{
    [PrimaryKey, AutoIncrement, Column("UserId")]
    public int UserId { get; set; }
    
    public string UserName { get; set; }
    public string Password { get; set; }
    public DateTime LastLogin { get; set; }
    public string BallList { get; set; }
}