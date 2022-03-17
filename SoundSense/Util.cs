using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace SoundSense;

// TODO move to Foxite.Common
public static class Util {
	public static Type? GetEnumerableItemType(Type type) {
		if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
			return type.GenericTypeArguments[0];
		} else {
			foreach (Type iInterface in type.GetInterfaces()) {
				Type? result = GetEnumerableItemType(iInterface);
				if (result != null) {
					return result;
				}
			}

			return null;
		}
	}

	public static MethodInfo GetInterfaceImplementationMethod(Type implementingType, Type interfaceType, string methodName) {
		InterfaceMapping interfaceMapping = implementingType.GetInterfaceMap(interfaceType);
		for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; i++) {
			if (interfaceMapping.InterfaceMethods[i].Name == methodName) {
				return interfaceMapping.TargetMethods[i];
			}
		}

		throw new InvalidOperationException($"Method {methodName} not found on interface {interfaceType.FullName}");
	}
}

public static class XmlUtil {
	public static T Deserialize<T>(XmlElement xml) => (T) Deserialize(xml, typeof(T));

	public static object Deserialize(XmlElement xml, Type type) {
		foreach (ConstructorInfo ctor in type.GetConstructors()) {
			object? result = TryConstructor(xml, ctor);
			if (result != null) {
				return result;
			}
		}

		throw new InvalidOperationException("No suitable constructor found for type: " + type.FullName);
	}

	private static object? ConvertValue(object? input, Type type) {
		if (input == null) {
			return null;
		} else if (type == typeof(Regex) && input is string inputString) {
			return new Regex(inputString);
		} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
			return type.GetConstructor(new[] {type.GetGenericArguments()[0]}).Invoke(new[] {ConvertValue(input, type.GetGenericArguments()[0])});
		} else if (type.IsEnum) {
			if (input is string inputString2 && Enum.TryParse(type, inputString2, true, out object? result)) {
				return result;
			} else {
				try {
					return Convert.ChangeType(Convert.ToInt32(input), type);
				} catch (Exception e) when (e is FormatException or InvalidCastException) {
					return Convert.ChangeType(input, type);
				}
			}
		} else {
			return Convert.ChangeType(input, type);
		}
	}

	private static object? TryConstructor(XmlElement xml, ConstructorInfo ctor) {
		ParameterInfo[] parameters = ctor.GetParameters();
		var parameterValues = new object?[parameters.Length];
		for (int i = 0; i < parameters.Length; i++) {
			var param = parameters[i];
			XmlAttributeAttribute? xmlAttributeAttr = param.GetCustomAttributes<XmlAttributeAttribute>().FirstOrDefault();
			XmlElementAttribute? xmlElementAttr = param.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault();
			if (xmlAttributeAttr != null) {
				object? attributeValue = null;
				if (xml.Attributes != null && xml.Attributes.Count != 0) {
					string lowercaseName = (string.IsNullOrEmpty(xmlAttributeAttr.AttributeName) ? param.Name! : xmlAttributeAttr.AttributeName).ToLower();
					attributeValue = xml.Attributes.Cast<XmlAttribute>().FirstOrDefault(attr => attr.Name.ToLower() == lowercaseName)?.Value;

					attributeValue = ConvertValue(attributeValue, param.ParameterType);
				}

				if (attributeValue == null) {
					if (param.HasDefaultValue) {
						attributeValue = param.DefaultValue;
					} else if ((param.ParameterType.IsGenericType && param.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)) || param.GetCustomAttributesData().Any(cad => cad.AttributeType.Name is "AllowNullAttribute" or "NullableAttribute")) {
						attributeValue = null;
					} else {
						throw new InvalidOperationException($"Missing xml attribute for {ctor.DeclaringType.FullName} constructor parameter {param.Name}");
					}
				}

				parameterValues[i] = attributeValue;
			} else if (xmlElementAttr != null) {
				if (param.ParameterType.IsAssignableTo(typeof(IEnumerable))) {
					Type? itemType = Util.GetEnumerableItemType(param.ParameterType);
					if (itemType == null) {
						return null;
					} else {
						IEnumerable<object> items = xml.ChildNodes.OfType<XmlElement>().Select(node => Deserialize(node, itemType));
						var list = (IList) typeof(List<>).MakeGenericType(itemType).GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>())!;
						foreach (object item in items) {
							list.Add(item);
						}
						parameterValues[i] = list;
					}
				} else {
					return ConvertValue(xml.Value, param.ParameterType);
				}
			} else {
				return null;
			}
		}

		return ctor.Invoke(parameterValues);
	}
}
