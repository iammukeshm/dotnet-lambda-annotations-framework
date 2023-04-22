using Amazon.DynamoDBv2.DataModel;

namespace HelloWorld;

[DynamoDBTable("developers")]
public class Developer
{
    [DynamoDBHashKey("id")]
    public int Id { get; set; }
    [DynamoDBProperty("name")]
    public string Name { get; set; }
    [DynamoDBProperty("email")]
    public string Email { get; set; }
}
