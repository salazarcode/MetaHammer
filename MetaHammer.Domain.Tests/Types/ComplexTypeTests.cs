using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Tests.Types;

public class ComplexTypeTests
{
    [Fact]
    public void AddProperty_WithPrimitiveType_AddsPropertyToCollection()
    {
        var voEmail = new ValueObjectType("Email");
        voEmail.AddProperty("email_account", PrimitiveType.String);
        voEmail.AddProperty("email_provider", PrimitiveType.String);
        
        var voAddress = new ValueObjectType("Address");
        voAddress.AddProperty("street", PrimitiveType.String);
        voAddress.AddProperty("city", PrimitiveType.String);
        voAddress.AddProperty("zip_code", PrimitiveType.String);
        voAddress.AddProperty("email" , voEmail);
        
        var personType = new ComplexType("Person");
        personType.AddProperty("first_name", PrimitiveType.String);
        personType.AddProperty("middle_name", PrimitiveType.String);
        personType.AddProperty("last_name", PrimitiveType.String);
        personType.AddProperty("birth_date", PrimitiveType.DateTime);
        personType.AddProperty("address", voAddress);
        personType.AddProperty("email", voEmail, isArray:false);
        
        var companyType = new ComplexType("Company");
        companyType.AddProperty("name", PrimitiveType.String);
        companyType.AddProperty("city", PrimitiveType.String);
        companyType.AddProperty("address", voAddress);

        //INSTANCES
        
        var companyInstance = new ComplexInstance(Guid.NewGuid(), companyType);
        companyInstance.SetPropertyValue("name", "MetaHammer Inc.");
        companyInstance.SetPropertyValue("city", "New York");

        var address = new ValueObjectInstance(voAddress);
        address.SetPropertyValue("street", "123 Main St");
        address.SetPropertyValue("city", "New York");
        address.SetPropertyValue("zip_code", "10001");
        
        companyInstance.SetPropertyValue("address", address);
        
        var person = new ComplexInstance(Guid.NewGuid(), personType);
        person.SetPropertyValue("first_name", "John");
        person.SetPropertyValue("middle_name", "A.");
        person.SetPropertyValue("last_name", "Doe");
        person.SetPropertyValue("birth_date", new DateTime(1989, 5, day: 17));
        person.SetPropertyValue("address", address);
        
        var email1 = new ValueObjectInstance(voEmail);
        email1.SetPropertyValue("email_account", "salazarcode");
        email1.SetPropertyValue("email_provider", "gmail.com");
        
        var email2 = new ValueObjectInstance(voEmail);
        email2.SetPropertyValue("email_account", "andresalteclado");
        email2.SetPropertyValue("email_provider", "gmail.com");
        
        var email3 = new ValueObjectInstance(voEmail);
        email3.SetPropertyValue("email_account", "jeronimo");
        email3.SetPropertyValue("email_provider", "gmail.com");
        
        person.SetPropertyValue("email", email1);
        person.SetPropertyValue("email", email2);
        person.SetPropertyValue("email", email3);
        
        
        var email_auxiliar = new ValueObjectInstance(voEmail);
        // Assert
        //Assert.Equal("Name", personType.Properties.First().Name);
        //Assert.Equal(PrimitiveType.String, personType.Properties.First().Type);
        //Assert.False(personType.Properties.First().IsArray);
    }
}
