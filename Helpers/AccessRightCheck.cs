using Dashboard_Backend.Database;
using LinqToDB;

namespace Dashboard_Backend.Helpers;

public class AccessRightCheck
{
    private readonly DatabaseMain _database;

    public AccessRightCheck(DatabaseMain database)
    {
        _database = database;
    }
    
    public async Task<bool> CheckAccessRights(ValidatedToken? validatedToken)
    {
        if (validatedToken is not ValidatedToken token)
        {
            return false;
        }
        
        var user = await _database.Users.FirstOrDefaultAsync(u => u.Username == token.Username);
        return user != null;
    }
}