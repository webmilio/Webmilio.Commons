using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Webmilio.Commons.Extensions;
using Webmilio.Commons.Serialization;

namespace Tests.Webmilio.Commons.Serialization;

[TestFixture]
public class SerializationTests
{
    private const string WindowXml = 
@"<Window x:Class=""XAMLTest.MainWindow""
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
        xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
        xmlns:local=""clr-namespace:XAMLTest""
        mc:Ignorable=""d""
        Title=""MainWindow"" Height=""450"" Width=""800"">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width=""159*""/>
            <ColumnDefinition/>
        </Grid.ColumnDefinitions>
        <Button Content=""Test"" Margin=""90,10,0,0"" VerticalAlignment=""Top"" HorizontalAlignment=""Left"" Width=""250"" Height=""58""/>
        <Button Content=""Button"" Margin=""453,10,0,0"" VerticalAlignment=""Top"" HorizontalAlignment=""Left"" Width=""250"" Height=""58""/>

    </Grid>
</Window>
";

    private const string ButtonXml =
        "<Button Content=\"Test\" Margin=\"90,10,0,0\" VerticalAlignment=\"Top\" HorizontalAlignment=\"Left\" Width=\"250\" Height=\"58\"/>";

    [Test]
    public void TestWindowSerialization()
    {
        XmlDocument document = new();
        document.LoadXml(WindowXml);

        XmlSerializer serializer = new(typeof(Window));
        var window = (Window) serializer.Serialize(document.ChildNodes[0]);

        Assert.IsNotNull(window);
    }

    [Test]
    public void TestButtonSerialization()
    {
        XmlDocument document = new();
        document.LoadXml(ButtonXml);

        XmlSerializer serializer = new(typeof(Button));
        var button = (Button) serializer.Serialize(document.ChildNodes[0]);

        Assert.IsNotNull(button);
        Assert.AreEqual("Test", button.Content);
    }

    public class Element
    {
        private readonly List<Element> _children = new();

        public Element()
        {
            Children = _children.AsReadOnly();
        }

        public void Add(Element element)
        {
            element.Parent = this;
            _children.Add(element);
        }

        public void Add(params Element[] elements) => elements.Do(Add);

        public void Remove(Element element)
        {
            _children.Remove(element);
        }

        public Element Parent { get; private set; }

        public IReadOnlyList<Element> Children { get; }
    }

    public class Window : Element { }

    public class Grid : Element { }

    public class Button : Element
    {
        public string Content { get; set; }
    }
}