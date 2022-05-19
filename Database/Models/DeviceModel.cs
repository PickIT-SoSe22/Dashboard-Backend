using LinqToDB.Mapping;

namespace Dashboard_Backend.Database.Models;

[Table("devices")]
public class DeviceModel
{
    [PrimaryKey, Identity, NotNull]
    [Column("dId")]
    public int DId { get; set; }
    
    [NotNull]
    [Column("deviceName")]
    public string DeviceName { get; set; }
    
    [NotNull]
    [Column("deviceType")]
    public string DeviceType { get; set; }
    
    [NotNull]
    [Column("uId")]
    public int UId { get; set; }

    [Association(ThisKey = "UId", OtherKey = "UId")]
    public UserModel User;
}