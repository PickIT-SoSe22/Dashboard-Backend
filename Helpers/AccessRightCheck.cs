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
    
    public async Task<KeyValuePair<bool, UserModel?>> CheckAccessRights(ValidatedToken? validatedToken)
    {
        KeyValuePair<bool, UserModel?> kvp;
        
        if (validatedToken is not ValidatedToken token)
        {
            kvp = new KeyValuePair<bool, UserModel?>(false, null);
            return kvp;
        }
        
        var user = await _database.Users.FirstOrDefaultAsync(u => u.Username == token.Username);
        kvp = new KeyValuePair<bool, UserModel?>(true, user);
        return kvp;
    }
}