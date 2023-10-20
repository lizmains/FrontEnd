using MauiApp2.ViewModel;
using SQLite;
namespace MauiApp2.Data;

public class UserRepository
{
    string _dbPath;
    private SQLiteConnection conn;
    
    public UserRepository(string dbPath)
    {
        _dbPath = dbPath;
        
    }
    public void Init()
    {
        conn = new SQLiteConnection(_dbPath);
        conn.CreateTable<User>();
    }
    
    public void Add(User user)
    {
        conn = new SQLiteConnection(_dbPath);
        conn.Insert(user);
    }
    
    public List<User> GetAllUsers()
    {
        Init();
        return conn.Table<User>().ToList();
    }
}