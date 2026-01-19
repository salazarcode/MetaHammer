using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Organizations;

namespace MetaHammer.Domain.Tests;

public class MetaClassTests
{
    [Fact]
    public void CreateClasses()
    {
        var organization = new Organization(Guid.NewGuid(), "Empresa X");
        var user = organization.AddUser(Guid.NewGuid(), "salazarcode");
        var intClass = organization.AddMetaClass( "Int", MetaNature.Primitive, user);
        var stringClass = organization.AddMetaClass( "String", MetaNature.Primitive, user);
        var addressClass = organization.AddMetaClass( "Address", MetaNature.ValueObject, user);
        
        addressClass.AddProperty("street", stringClass);
        addressClass.AddProperty("city", stringClass);
        
        var personClass = organization.AddMetaClass( "Person", MetaNature.Entity, user);
        personClass.AddProperty("first_name", stringClass);
        personClass.AddProperty("last_name", stringClass);
        personClass.AddProperty("age", intClass);
        personClass.AddProperty("address", addressClass, isCollection:true);
        
        Assert.NotNull(intClass);
        Assert.NotNull(stringClass);
        Assert.NotNull(addressClass);
        Assert.NotNull(personClass);
    }
}
