using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Tests.Types;

public class ComplexTypeTests
{
    public PrimitiveType stringType { get; set; }
    public PrimitiveType intType { get; set; }
    public PrimitiveType decimalType { get; set; }
    public PrimitiveType dateTimeType { get; set; }
    public PrimitiveType booleanType { get; set; }
    
    
    public ComplexTypeTests()
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
    public void CreatePrimitiveType()
    {
        var primitiveType = new PrimitiveType(Guid.NewGuid(), "String");
        
        Assert.NotNull(primitiveType);
    }
    [Fact]
    public void CreateValueObjectType_AddStringProperties()
    {
        var valueObjectType = new ValueObjectType(Guid.NewGuid(), "Address");
        
        valueObjectType.AddProperty("street", stringType);
        valueObjectType.AddProperty("city", stringType);
        valueObjectType.AddProperty("zip_code", intType);
        
        Assert.NotNull(valueObjectType);
        Assert.Contains(valueObjectType.Properties, p => p.Name == "street");
        Assert.Contains(valueObjectType.Properties, p => p.Name == "city");
        Assert.Contains(valueObjectType.Properties, p => p.Name == "zip_code");
        
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="street").MetaType.Name, stringType.Name);
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="city").MetaType.Name, stringType.Name);
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="zip_code").MetaType.Name, intType.Name);
        
    }
    
    [Fact]
    public void CreateComplexType_AddStringProperties()
    {
        var complexType = new ComplexType(Guid.NewGuid(), "Person");
        
        complexType.AddProperty("first_name", stringType);
        complexType.AddProperty("last_name", stringType);
        complexType.AddProperty("birth_date", dateTimeType);
        complexType.AddProperty("height", decimalType);
        complexType.AddProperty("is_employed", booleanType);
        
        Assert.NotNull(complexType);
        Assert.Contains(complexType.Properties, p => p.Name == "first_name");
        Assert.Contains(complexType.Properties, p => p.Name == "last_name");
        Assert.Contains(complexType.Properties, p => p.Name == "birth_date");
        Assert.Contains(complexType.Properties, p => p.Name == "height");
        Assert.Contains(complexType.Properties, p => p.Name == "is_employed");
        
        Assert.Equal(complexType.Properties.First(x =>x.Name =="first_name").MetaType.Name, stringType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="last_name").MetaType.Name, stringType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="birth_date").MetaType.Name, dateTimeType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="height").MetaType.Name, decimalType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="is_employed").MetaType.Name, booleanType.Name);
        
    }

    [Fact]
    public void ComplexType_CreateMethod()
    {
        var person = new ComplexType(Guid.NewGuid(), "Person");
        
        person.AddProperty("first_name", stringType);
        person.AddProperty("last_name", stringType);
        person.AddProperty("birth_date", dateTimeType);

        var constructor = person.AddConstructor();
        constructor.AddParameter("first_name", stringType);
        constructor.AddParameter("last_name", stringType);

        var testMethod1 = person.AddMethod("GetFullName", stringType);
    }
}
