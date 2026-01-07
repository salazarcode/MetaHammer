using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Tests.MetaObjects;

public class MetaObjectTests
{
    public PrimitiveMetaType stringType { get; set; }
    public PrimitiveMetaType intType { get; set; }
    public PrimitiveMetaType decimalType { get; set; }
    public PrimitiveMetaType dateTimeType { get; set; }
    public PrimitiveMetaType booleanType { get; set; }
    
    
    public MetaObjectTests()
    {
        var stringTypeName = "String";
        var intTypeName = "Int";
        var decimalTypeName = "Decimal";
        var dateTimeTypeName = "DateTime";
        var booleanTypeName = "Boolean";
        
        stringType = new PrimitiveMetaType(Guid.NewGuid(), stringTypeName);
        intType = new PrimitiveMetaType(Guid.NewGuid(), intTypeName);
        decimalType = new PrimitiveMetaType(Guid.NewGuid(), decimalTypeName);
        dateTimeType = new PrimitiveMetaType(Guid.NewGuid(), dateTimeTypeName);
        booleanType = new PrimitiveMetaType(Guid.NewGuid(), booleanTypeName);
    }

    [Fact]
    public void MetaObject_Creation_Tests()
    {
        var address = new ValueObjectMetaType(Guid.NewGuid(), "Address");

        address.AddProperty("street", stringType);
        address.AddProperty("city", stringType);
        address.AddProperty("zip_code", intType);

        var complexType = new ComplexMetaType(Guid.NewGuid(), "Person");

        complexType.AddProperty("first_name", stringType);
        complexType.AddProperty("last_name", stringType);
        complexType.AddProperty("birth_date", dateTimeType);
        complexType.AddProperty("height", decimalType);
        complexType.AddProperty("is_employed", booleanType);
        complexType.AddProperty("address", address, isArray:true);
        
        var person = new MetaObject(complexType, Guid.NewGuid());
        person.SetPropertyValue("first_name", new MetaObject(stringType, "Andrés"));
        person.SetPropertyValue("last_name", new MetaObject(stringType, "Guzmán"));
        person.SetPropertyValue("birth_date", new MetaObject(dateTimeType, new DateTime(1989, 5, 17)));
        
        Assert.NotNull(person);
        Assert.Equal("Andrés", person.Property("first_name").Value);
        Assert.Equal("Guzmán", person.Property("last_name").Value);
        Assert.Equal(new DateTime(1989, 5, 17), person.Property("birth_date").Value);
        
        var address1 = new MetaObject(address);
        address1.SetPropertyValue("street", new MetaObject(stringType, "123 Main St"));
        address1.SetPropertyValue("city", new MetaObject(stringType, "Springfield"));
        address1.SetPropertyValue("zip_code", new MetaObject(intType, 12345));
        
        var address2 = new MetaObject(address);
        address2.SetPropertyValue("street", new MetaObject(stringType, "456 Elm St"));
        address2.SetPropertyValue("city", new MetaObject(stringType, "Shelbyville"));
        address2.SetPropertyValue("zip_code", new MetaObject(intType, 67890));
        
        person.AddItemToProperty("address", address1);
        person.AddItemToProperty("address", address2);
        
        var expectedCount = 2;
        var actualCount = person.GetPropertyList("address").Count;
        var isSeemType = person.GetPropertyList("address") is List<MetaObject>;
        
        Assert.Equal(isSeemType, true);
        Assert.Equal(actualCount, expectedCount);
        Assert.Equal(person.GetPropertyList("address")[0].Property("street").Value , "123 Main St");
        Assert.Equal(person.GetPropertyList("address")[0].Property("city").Value , "Springfield");
        Assert.Equal(person.GetPropertyList("address")[0].Property("zip_code").Value , 12345);
        
        Assert.Equal(person.GetPropertyList("address")[1].Property("street").Value , "456 Elm St");
        Assert.Equal(person.GetPropertyList("address")[1].Property("city").Value , "Shelbyville");
        Assert.Equal(person.GetPropertyList("address")[1].Property("zip_code").Value , 67890);
        
    }
}