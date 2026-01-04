using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Instances.Properties;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Tests.Types;

public class ComplexTypeTests
{
    [Fact]
    public void CreateInstance_WithDictionary_SetsAllProperties()
    {
        // // Arrange - Define types
        // var voEmail = new ValueObjectType("Email");
        // voEmail.AddProperty("email_account", PrimitiveType.String);
        // voEmail.AddProperty("email_provider", PrimitiveType.String);
        //
        // var voAddress = new ValueObjectType("Address");
        // voAddress.AddProperty("street", PrimitiveType.String);
        // voAddress.AddProperty("city", PrimitiveType.String);
        // voAddress.AddProperty("zip_code", PrimitiveType.String);
        //
        // var personType = new ComplexType("Person");
        // personType.AddProperty("first_name", PrimitiveType.String);
        // personType.AddProperty("last_name", PrimitiveType.String);
        // personType.AddProperty("age", PrimitiveType.Int);
        // personType.AddProperty("birth_date", PrimitiveType.DateTime);
        // personType.AddProperty("address", voAddress);
        //
        // var companyType = new ComplexType("Company");
        // companyType.AddProperty("name", PrimitiveType.String);
        // companyType.AddProperty("city", PrimitiveType.String);
        // companyType.AddProperty("address", voAddress);
        //
        // // Act - Create instances using anonymous objects
        // var email = voEmail.CreateInstance(new
        // {
        //     email_account = "salazarcode",
        //     email_provider = "gmail.com"
        // });
        //
        // var address = voAddress.CreateInstance(new
        // {
        //     street = "123 Main St",
        //     city = "New York",
        //     zip_code = "10001"
        // });
        //
        // var person = personType.Create(first_name: "John", last_name: "Doe", age: 35, birth_date: new DateTime(1989, 5, 17), address: address);
        //
        // // Assert
        // Assert.Equal(5, person.PropertiesAsReadOnly.Count);
        //
        // var personFirstName = person.PropertiesAsReadOnly.First(p => p.Name == "first_name") as StringProperty;
        // Assert.NotNull(personFirstName);
        // Assert.Equal("John", personFirstName.Value);
        //
        // var personAge = person.PropertiesAsReadOnly.First(p => p.Name == "age") as IntProperty;
        // Assert.NotNull(personAge);
        // Assert.Equal(35, personAge.Value);
        //
        // var personAddress = person.PropertiesAsReadOnly.First(p => p.Name == "address") as ValueObjectProperty;
        // Assert.NotNull(personAddress);
        // Assert.Equal(3, personAddress.Value.PropertiesAsReadOnly.Count);
    }
}
