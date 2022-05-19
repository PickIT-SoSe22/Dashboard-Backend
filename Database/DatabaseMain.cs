using Dashboard_Backend.Database.Models;
using LinqToDB.Data;
using LinqToDB;
using LinqToDB.Configuration;

namespace Dashboard_Backend.Database;

public class DatabaseMain : DataConnection
{
    public DatabaseMain(LinqToDBConnectionOptions<DatabaseMain> options) : base(options) { }

    public ITable<UserModel> Users => this.GetTable<UserModel>();

    public ITable<DeviceModel> Devices => this.GetTable<DeviceModel>();
}