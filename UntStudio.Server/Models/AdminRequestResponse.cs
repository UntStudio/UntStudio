namespace UntStudio.Server.Models;

public class AdminRequestResponse
{
    public AdminCodeResponse Code;



    public AdminRequestResponse(AdminCodeResponse code)
    {
        Code = code;
    }



    public enum AdminCodeResponse
    {
        None,
        KeyValidationFailed,
        NameValidationFailed,
        SpecifiedAdminCredentialsNotExsist,
        SpecifiedPluginKeyOrNameNotFound,
        NoOnePluginWithSpecifiedKeyNotFound,
        SpecifiedPluginAlreadyBanned,
        AllowedAddressesNotSpecified,
    }
}
