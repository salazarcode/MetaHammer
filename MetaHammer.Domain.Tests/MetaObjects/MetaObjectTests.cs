using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Tests.MetaObjects;

public class MetaObjectTests
{
    public PrimitiveType stringType { get; set; }
    public PrimitiveType intType { get; set; }
    public PrimitiveType decimalType { get; set; }
    public PrimitiveType dateTimeType { get; set; }
    public PrimitiveType booleanType { get; set; }
    
    
    public MetaObjectTests()
    {
        var stringTypeName = "String";
        var intTypeName = "Int";
        var decimalTypeName = "Decimal";
        var dateTimeTypeName = "DateTime";
        var booleanTypeName = "Boolean";
        
        stringType = new PrimitiveType(Guid.NewGuid(), stringTypeName);
        intType = new PrimitiveType(Guid.NewGuid(), intTypeName);
        decimalType = new PrimitiveType(Guid.NewGuid(), decimalTypeName);
        dateTimeType = new PrimitiveType(Guid.NewGuid(), dateTimeTypeName);
        booleanType = new PrimitiveType(Guid.NewGuid(), booleanTypeName);
    }

    [Fact]
    public void MetaObject_Creation_Tests()
    {
        var addressVOType = new ValueObjectType(Guid.NewGuid(), "Address");

        addressVOType.AddProperty("street", stringType);
        addressVOType.AddProperty("city", stringType);
        addressVOType.AddProperty("zip_code", intType);

        var personComplexType = new ComplexType(Guid.NewGuid(), "Person");

        personComplexType.AddProperty("first_name", stringType);
        personComplexType.AddProperty("last_name", stringType);
        personComplexType.AddProperty("birth_date", dateTimeType);
        personComplexType.AddProperty("height", decimalType);
        personComplexType.AddProperty("is_employed", booleanType);
        personComplexType.AddProperty("address", addressVOType, isArray:true);
        
        var person = new MetaObject(personComplexType, Guid.NewGuid());
        person.Property("first_name").Set(new MetaObject(stringType, "Andrés"));
        person.Property("last_name").Set(new MetaObject(stringType, "Guzmán"));
        person.Property("birth_date").Set(new MetaObject(dateTimeType, new DateTime(1989, 5, 17)));
        
        Assert.NotNull(person);
        Assert.Equal("Andrés", person.Property("first_name").GetValue().Value);
        Assert.Equal("Guzmán", person.Property("last_name").GetValue().Value);
        Assert.Equal(new DateTime(1989, 5, 17), person.Property("birth_date").GetValue().Value);
        
        var address1 = new MetaObject(addressVOType);
        address1.Property("street").Set(new MetaObject(stringType, "123 Main St"));
        address1.Property("city").Set(new MetaObject(stringType, "Springfield"));
        address1.Property("zip_code").Set(new MetaObject(intType, 12345));
        
        var address2 = new MetaObject(addressVOType);
        address2.Property("street").Set(new MetaObject(stringType, "456 Elm St"));
        address2.Property("city").Set(new MetaObject(stringType, "Shelbyville"));
        address2.Property("zip_code").Set(new MetaObject(intType, 67890));
        
        person.Property("address").Add(address1);
        person.Property("address").Add(address2);
        
        var expectedCount = 2;
        var actualCount = person.Property("address").GetList().Count();
        var isSeemType = person.Property("address").GetList() is IReadOnlyList<MetaObject>;
        
        Assert.Equal(isSeemType, true);
        Assert.Equal(actualCount, expectedCount);
        Assert.Equal(person.Property("address").GetList()[0].Property("street").GetValue().Value, "123 Main St");
        Assert.Equal(person.Property("address").GetList()[0].Property("zip_code").GetValue().Value , 12345);

        Assert.Equal(person.Property("address").GetList()[1].Property("street").GetValue().Value , "456 Elm St");
        Assert.Equal(person.Property("address").GetList()[1].Property("city").GetValue().Value , "Shelbyville");
        Assert.Equal(person.Property("address").GetList()[1].Property("zip_code").GetValue().Value , 67890);
        
    }
}