using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace ce.util
{
	public class StructTypeConverter<T> : TypeConverter where T: struct
	{
		Type structType;
		public StructTypeConverter() {
			structType = typeof(T);
		}

		public override bool GetCreateInstanceSupported ( ITypeDescriptorContext context )
		{
			return true;
		}

		public override object CreateInstance (ITypeDescriptorContext context, IDictionary propertyValues )
		{
			object inst = new T();
			PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (PropertyInfo pi in props) {
				if (pi.CanWrite)
					pi.SetValue(inst, propertyValues[pi.Name], null);
			}

			return inst;
		}


		public override bool GetPropertiesSupported (ITypeDescriptorContext context)
		{
			return true ;
		}

		public override PropertyDescriptorCollection GetProperties (ITypeDescriptorContext context , object value , Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);

			List<PropertyDescriptor> r = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor pd in properties)
				if (!pd.IsReadOnly)
					r.Add(pd);

			return new PropertyDescriptorCollection(r.ToArray());
		}
		
		public override bool CanConvertTo (ITypeDescriptorContext context, System.Type destinationType)
		{
			bool canConvert = (destinationType == typeof(InstanceDescriptor));

			if (!canConvert)
				canConvert = base.CanConvertFrom(context, destinationType);

			return canConvert;
		}
	}
}
