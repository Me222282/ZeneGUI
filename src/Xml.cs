using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Linq;
using Zene.Windowing;
using System.ComponentModel;

namespace Zene.GUI
{
    public delegate object StringParser(string value);

    public class Xml
    {
        public Xml()
        {
            _guiAsm = Assembly.GetExecutingAssembly();

            _types = AppDomain.CurrentDomain.GetAllTypes();
            _elementTypes = _types.Where(ti =>
            {
                Type type = ti.AsType();
                return type.IsSubclassOf(typeof(Element));
            });

            AddParser(str =>
            {
                return str switch
                {
                    "Arrow" => Cursor.Arrow,
                    "CrossHair" => Cursor.CrossHair,
                    "Default" => Cursor.Default,
                    "Hand" => Cursor.Hand,
                    "IBeam" => Cursor.IBeam,
                    "NotAllowed" => Cursor.NotAllowed,
                    "ResizeAll" => Cursor.ResizeAll,
                    "ResizeBottomLeft" => Cursor.ResizeBottomLeft,
                    "ResizeBottomRight" => Cursor.ResizeBottomRight,
                    "ResizeHorizontal" => Cursor.ResizeHorizontal,
                    "ResizeTopLeft" => Cursor.ResizeTopLeft,
                    "ResizeTopRight" => Cursor.ResizeTopRight,
                    "ResizeVertical" => Cursor.ResizeVertical,
                    _ => throw new Exception("Invalid Cursor syntax")
                };
            });
        }

        private readonly IEnumerable<TypeInfo> _types;
        private readonly IEnumerable<TypeInfo> _elementTypes;
        private readonly Assembly _asm = Assembly.GetEntryAssembly();
        private readonly Assembly _guiAsm;
        private readonly Dictionary<Type, StringParser> _stringParses = new Dictionary<Type, StringParser>();

        public void AddParser(StringParser func, Type returnType) => _stringParses.Add(returnType, func);
        public void AddParser<T>(Func<string, T> func) => _stringParses.Add(typeof(T), s => func(s));

        public RootElement LoadGUI(Window window, string src)
        {
            XmlDocument root = new XmlDocument();
            root.LoadXml(src);

            RootElement re = new RootElement(window);

            if (root.ChildNodes.Count == 0)
            {
                throw new Exception($"No elements in {nameof(src)}");
            }

            XmlNodeList xnl = root.ChildNodes[0].ChildNodes;

            if (root.ChildNodes[0].Name.ToLower() == "xml")
            {
                if (root.ChildNodes.Count == 1)
                {
                    throw new Exception($"No elements in {nameof(src)}");
                }

                xnl = root.ChildNodes[1].ChildNodes;
            }

            foreach (XmlNode node in xnl)
            {
                Element e = ParseNode(node, re);
                // Property node
                if (e == null) { continue; }

                re.AddChild(e);
            }

            return re;
        }

        private Element ParseNode(XmlNode node, Element parent)
        {
            if (node.Name == "Property")
            {
                ParseProperty(node, parent);
                return null;
            }

            Type t = _elementTypes.FindType(node.Name);
            if (t == null)
            {
                throw new Exception("Tag name doesn't exist");
            }

            ConstructorInfo constructor = t.GetConstructor(Array.Empty<Type>());
            if (constructor == null)
            {
                throw new Exception("Type doesn't have a constructor with no parameters");
            }

            Element element = constructor.Invoke(null) as Element;

            foreach (XmlAttribute a in node.Attributes)
            {
                ParseAttribute(a, t, element);
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                // Set text
                if (child.NodeType == XmlNodeType.Text)
                {
                    PropertyInfo pi = t.GetProperty("Text");
                    if (pi == null || !pi.CanWrite)
                    {
                        throw new Exception($"{node.Name} doesn't have a Text property");
                    }

                    pi.SetValue(element, child.Value.Trim());
                    continue;
                }

                Element e = ParseNode(child, element);
                // Property node
                if (e == null) { continue; }

                element.AddChild(e);
            }

            return element;
        }

        private void ParseAttribute(XmlAttribute a, Type type, object e)
        {
            PropertyInfo p = type.GetProperty(a.Name);
            if (p == null || !p.CanWrite)
            {
                throw new Exception($"{a.ParentNode.Name} doesn't have attribute {a.Name}");
            }

            object o = ParseString(a.Value, p.PropertyType);
            p.SetValue(e, o);
        }
        /*
        private object ParseString(string value, Type returnType)
        {
            if (!_stringParses.TryGetValue(returnType, out StringParser parser) || parser == null)
            {
                throw new Exception($"Cannot parse string to type {returnType.Name}");
            }

            return parser(value);
        }*/

        public object ParseString(string value, Type returnType)
        {
            value = value.Trim();

            try
            {
                TypeConverter tc = TypeDescriptor.GetConverter(returnType);
                object o = tc.ConvertFromInvariantString(value);
                // If null - parser not valid
                if (o != null) { return o; }
            }
            catch (Exception) { }

            if (_stringParses.TryGetValue(returnType, out StringParser parser) && parser != null)
            {
                try
                {
                    return parser(value);
                }
                catch (Exception) { }
            }

            return StringByType(value, returnType);
        }

        private object StringByType(string src, Type assign)
        {
            string[] constructor = SplitConstructor(src);
            if (constructor.Length == 0)
            {
                throw new Exception("Invalid constructor syntax");
            }

            // Find type
            Type type = _types.FindType(constructor[0]);
            if (type == null)
            {
                throw new Exception("Constructor type doesn't exist");
            }
            if (!type.IsAssignableTo(assign))
            {
                throw new Exception("Invalid constructor type");
            }

            // Find all constructors
            ConstructorInfo[] cinfos = type.GetConstructors();
            if (cinfos.Length == 0)
            {
                throw new Exception("Constructor doesn't exist");
            }

            // FInd valid constructors
            IEnumerable<ConstructorInfo> validConstructors = cinfos.Where(ci => ci.GetParameters().Length == constructor.Length - 1);
            if (!validConstructors.Any())
            {
                throw new Exception("Invalid number of parameters in constructor");
            }

            // No parameters
            if (constructor.Length == 1)
            {
                return validConstructors.First().Invoke(null);
            }

            object[] parameters = new object[constructor.Length - 1];
            ConstructorInfo constructorMethod = null;

            foreach (ConstructorInfo ci in validConstructors)
            {
                ParameterInfo[] pinfos = ci.GetParameters();

                int i;
                for (i = 0; i < pinfos.Length; i++)
                {
                    Type pt = pinfos[i].ParameterType;

                    try
                    {
                        parameters[i] = ParseString(constructor[i + 1], pt);
                    }
                    catch (Exception) { break; }
                }

                // Not correct parameter
                if (i != pinfos.Length) { continue; }

                constructorMethod = ci;
                break;
            }

            if (constructorMethod == null)
            {
                throw new Exception("Invalid constructor parameters");
            }

            return constructorMethod.Invoke(parameters);
        }
        private static string[] SplitConstructor(string src)
        {
            if (src == null || src.Length == 0)
            {
                throw new Exception("Invalid constructor syntax");
            }

            src = src.Trim().TrimBrackets();
            if (src[^1] != ')')
            {
                throw new Exception("Invalid constructor syntax");
            }

            List<string> strings = new List<string>();

            bool inBrakets = false;
            bool inConstructor = false;

            int strRef = 0;

            for (int i = 0; i < src.Length; i++)
            {
                if (!inConstructor)
                {
                    // Continue until constructor
                    if (src[i] != '(') { continue; }

                    strings.Add(src[0..i].Trim());
                    strRef = i + 1;
                    inConstructor = true;
                    continue;
                }

                if (inBrakets && src[i] == ')')
                {
                    inBrakets = false;
                    continue;
                }

                if (src[i] == '(')
                {
                    inBrakets = true;
                    continue;
                }

                if (src[i] == ',' || src[i] == ')')
                {
                    strings.Add(src[strRef..i].Trim());
                    strRef = i + 1;
                }
            }

            if (strRef != src.Length)
            {
                throw new Exception("Invalid constructor syntax");
            }

            return strings.ToArray();
        }

        private void ParseProperty(XmlNode node, Element parent)
        {
            XmlAttributeCollection xac = node.Attributes;

            if (xac.Count > 2 || xac.Count == 0)
            {
                throw new Exception("Invalid number of attributes on Property node");
            }

            string name;

            if (xac[0].Name == "Name")
            {
                name = xac[0].Value;
            }
            else if (xac.Count == 2 && xac[1].Name == "Name")
            {
                name = xac[1].Value;
            }
            else
            {
                throw new Exception("Property node needs Name attribute");
            }

            string value;

            if (xac[0].Name == "Value")
            {
                value = xac[0].Value;
            }
            else if (xac.Count == 2 && xac[1].Name == "Value")
            {
                value = xac[1].Value;
            }
            else
            {
                value = node.InnerText;
            }

            ParsePropertyAttribute(name, value, parent.GetType(), parent);
        }
        private void ParsePropertyAttribute(string name, string value, Type type, object e)
        {
            PropertyInfo p = type.GetProperty(name);
            if (p == null || !p.CanWrite)
            {
                throw new Exception($"{type.Name} doesn't have attribute {name}");
            }

            p.SetValue(e, ParseString(value, p.PropertyType));
        }
    }
}
