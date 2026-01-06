using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Enums;

namespace MetaHammer.Domain.Tests.Types;

public class ComplexTypeTests
{
    [Fact]
    public void CreatePrimitiveType()
    {
        var primitiveType = new MetaType(Guid.NewGuid(), "String", MetaTypeNature.Primitive, isNative:true);
        
        Assert.NotNull(primitiveType);
    }
    [Fact]
    public void CreateValueObjectType_AddStringProperties()
    {
        var stringTypeName = "String";
        var intTypeName = "Int";
        
        var stringType = new MetaType(Guid.NewGuid(), stringTypeName, MetaTypeNature.Primitive, isNative:true);
        var intType = new MetaType(Guid.NewGuid(), intTypeName, MetaTypeNature.Primitive, isNative:true);
        
        var valueObjectType = new MetaType(Guid.NewGuid(), "Address", MetaTypeNature.ValueObject);
        
        valueObjectType.AddProperty("street", stringType);
        valueObjectType.AddProperty("city", stringType);
        valueObjectType.AddProperty("zip_code", intType);
        
        Assert.NotNull(valueObjectType);
        Assert.Contains(valueObjectType.Properties, p => p.Name == "street");
        Assert.Contains(valueObjectType.Properties, p => p.Name == "city");
        Assert.Contains(valueObjectType.Properties, p => p.Name == "zip_code");
        
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="street").MetaType.Name, stringTypeName);
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="city").MetaType.Name, stringTypeName);
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="zip_code").MetaType.Name, intTypeName);
        
    }
    
    
    [Fact]
    public void CreateComplexType_AddStringProperties()
    {
        var stringTypeName = "String";
        var intTypeName = "Int";
        var decimalTypeName = "Decimal";
        var dateTimeTypeName = "DateTime";
        var booleanTypeName = "Boolean";
        
        var stringType = new MetaType(Guid.NewGuid(), stringTypeName, MetaTypeNature.Primitive, isNative:true);
        var intType = new MetaType(Guid.NewGuid(), intTypeName, MetaTypeNature.Primitive, isNative:true);
        var decimalType = new MetaType(Guid.NewGuid(), decimalTypeName, MetaTypeNature.Primitive, isNative:true);
        var dateTimeType = new MetaType(Guid.NewGuid(), dateTimeTypeName, MetaTypeNature.Primitive, isNative:true);
        var booleanType = new MetaType(Guid.NewGuid(), booleanTypeName, MetaTypeNature.Primitive, isNative:true);
        
        var complexType = new MetaType(Guid.NewGuid(), "Person", MetaTypeNature.Complex);
        
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
        
        Assert.Equal(complexType.Properties.First(x =>x.Name =="first_name").MetaType.Name, stringTypeName);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="last_name").MetaType.Name, stringTypeName);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="birth_date").MetaType.Name, dateTimeTypeName);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="height").MetaType.Name, decimalTypeName);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="is_employed").MetaType.Name, booleanTypeName);
        
    }
}
