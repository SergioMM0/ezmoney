namespace AuthService;

public class AuthServiceConfiguration {
    //This could be split up into multiple configuration classes in the future
    public string Key { get; set; }
    public string CreateUserUrl { get; set; }
    public string GetUserByPhoneNumberUrl { get; set; }
}
