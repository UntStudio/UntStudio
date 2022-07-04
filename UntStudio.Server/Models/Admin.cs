using System.ComponentModel.DataAnnotations;

namespace UntStudio.Server.Models;

public class Admin
{
    public Admin(string login, string password)
    {
        Login = login;
        Password = password;
    }



    public int Id { get; set; }

    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}
