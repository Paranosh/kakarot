using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace kakarot
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(rssMsxOrg));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (rssMsxOrg)serializer.Deserialize(reader);
    // }

    internal class rssMsxOrg
    {
        [XmlRoot(ElementName = "category")]
        public class Category
        {

            [XmlAttribute(AttributeName = "domain")]
            public string Domain { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "guid")]
        public class Guid
        {

            [XmlAttribute(AttributeName = "isPermaLink")]
            public bool IsPermaLink { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "item")]
        public class Item
        {

            [XmlElement(ElementName = "title")]
            public string Title { get; set; }

            [XmlElement(ElementName = "link")]
            public string Link { get; set; }

            [XmlElement(ElementName = "description")]
            public string Description { get; set; }

            [XmlElement(ElementName = "comments")]
            public string Comments { get; set; }

            [XmlElement(ElementName = "category")]
            public List<Category> Category { get; set; }

            [XmlElement(ElementName = "pubDate")]
            public DateTime PubDate { get; set; }

            [XmlElement(ElementName = "creator")]
            public string Creator { get; set; }

            [XmlElement(ElementName = "guid")]
            public Guid Guid { get; set; }
        }

        [XmlRoot(ElementName = "channel")]
        public class Channel
        {

            [XmlElement(ElementName = "title")]
            public string Title { get; set; }

            [XmlElement(ElementName = "link")]
            public string Link { get; set; }

            [XmlElement(ElementName = "description")]
            public string Description { get; set; }

            [XmlElement(ElementName = "language")]
            public string Language { get; set; }

            [XmlElement(ElementName = "item")]
            public List<Item> Item { get; set; }
        }

        [XmlRoot(ElementName = "rss")]
        public class Rss
        {

            [XmlElement(ElementName = "channel")]
            public Channel Channel { get; set; }

            [XmlAttribute(AttributeName = "version")]
            public double Version { get; set; }

            [XmlAttribute(AttributeName = "base")]
            public string Base { get; set; }

            [XmlAttribute(AttributeName = "dc")]
            public string Dc { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

    }
}
