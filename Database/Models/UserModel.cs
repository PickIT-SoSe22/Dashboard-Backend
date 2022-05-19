using LinqToDB.Mapping;
using Microsoft.AspNetCore.Identity;


[Table("users")]
public class UserModel
{
    [PrimaryKey, Identity, NotNull]
    [Column("uId")]
    public int UId { get; set; }

    [NotNull, PersonalData]
    [Column("email")]
    public string Email { get; set; }
    
    [NotNull, PersonalData]
    [Column("username")]
    public string Username { get; set; }

    [NotNull, PersonalData]
    [Column("passwordHash")]
    public byte[] PasswordHash { get; set; }

    [NotNull, PersonalData]
    [Column("salt")]
    public byte[] Salt { get; set; }
}